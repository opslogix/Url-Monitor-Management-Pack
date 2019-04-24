using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;
using OpsLogix.IMP.Url.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpsLogix.IMP.Url.Modules.ProbeActions
{
    [MonitoringModule(ModuleType.ReadAction), ModuleOutput(true)]
    public class ServerCertificateProbeAction: ModuleBaseExtension<PropertyBagDataItem>
    {
        private Uri _testUri;
        private bool _validateCertificate;
        private List<Oid> _disabledHashes;
        private SslPolicyErrors _policyErrors; // not really need a lock
        private int _dnsRequestTimeout;
        private List<IPAddress> _customDnsServers;

        public ServerCertificateProbeAction(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
        {

        }

        /// <summary>
        /// TODO: needs a big rewrite
        /// </summary>
        /// <param name="inputDataItems"></param>
        /// <returns></returns>
        protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
        {
            var result = new List<PropertyBagDataItem>();

            TcpClient tcpClient;

            if (Array.IndexOf(TcpHelper.SecureSchemes, _testUri.Scheme.ToLowerInvariant()) == -1)
                return result.ToArray(); // skip checks if scheme is not secure (nothing to check)

            bool authSuccess = false;

            try
            {
                if (_customDnsServers.Count > 0)
                    tcpClient = new TcpClient(TcpHelper.ResolveHostName(_testUri.DnsSafeHost, _customDnsServers.ToArray())[0].ToString(), _testUri.Port); // it always return and address OR exception
                else
                    tcpClient = new TcpClient(_testUri.DnsSafeHost, _testUri.Port);
            }
            catch (Exception ex)
            {
                _logger.WriteLog(TraceEventType.Warning, 175, $"Unable to test certificate. Can't connect to {_testUri.Host}. \n\n {ex}");
                return result.ToArray();
            }

            X509Certificate2 remoteCert = null;

            using (var sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(RecordCertificateProperties)))
            {
                foreach (SslProtocols protocolType in Enum.GetValues(typeof(SslProtocols)))
                {
                    if (protocolType == SslProtocols.None)
                        continue;


                        sslStream.ReadTimeout = 15000;
                        sslStream.WriteTimeout = 15000;

                        try
                        {
                            sslStream.AuthenticateAsClient(_testUri.Host, null, protocolType, false);
                        }
                        catch
                        {
                            continue;
                        };

                    authSuccess = true;

                    break;
                }

                if (!authSuccess)
                    return result.ToArray();

                remoteCert = new X509Certificate2(sslStream.RemoteCertificate.GetRawCertData());
            }

            try
            {
                // dump some cert properties in a property bag
                var bagItem = new Dictionary<string, object>
                {
                    { "DaysToExpire", Math.Round(remoteCert.NotAfter.Subtract(DateTime.UtcNow).TotalDays, 2) },
                    { "StartDate", remoteCert.NotBefore.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "EndDate", remoteCert.NotAfter.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "Issuer", remoteCert.Issuer },
                    { "Subject", remoteCert.Subject },
                    { "SerialNumber", remoteCert.SerialNumber },
                    { "SignatureAlgorithm", remoteCert.SignatureAlgorithm.FriendlyName }
                };

                if (_disabledHashes.TrueForAll(x => x.Value != remoteCert.SignatureAlgorithm.Value))
                    bagItem.Add("SignatureAlgorithmDisabled", "false");
                else
                    bagItem.Add("SignatureAlgorithmDisabled", "true");

                bagItem.Add("Thumbprint", remoteCert.Thumbprint);

                if (_policyErrors == SslPolicyErrors.None)
                    bagItem.Add("PolicyErrors", "NONE");
                else
                    bagItem.Add("PolicyErrors", _policyErrors.ToString());

                // Verify the cert chain
                if (_validateCertificate)
                {
                    var certificateChain = new X509Chain(true); // Use Machine Context
                    certificateChain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    certificateChain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    certificateChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    certificateChain.Build(remoteCert);

                    if (certificateChain.ChainStatus.Length == 0)
                    {
                        bagItem.Add("CertificateIsValid", "true");
                        bagItem.Add("CertificateStatus", "");
                    }
                    else
                    {
                        bagItem.Add("CertificateIsValid", "false");

                        string strStatusMessage = string.Empty;
                        foreach (var status in certificateChain.ChainStatus)
                            strStatusMessage += $"{status.StatusInformation}; ";

                        bagItem.Add("CertificateStatus", strStatusMessage);
                    }
                }
                else
                {
                    bagItem.Add("CertificateIsValid", "true");
                    bagItem.Add("CertificateStatus", "");
                }

                result.Add(new PropertyBagDataItem(null, new Dictionary<string, Dictionary<string, object>>
                {
                    {string.Empty, bagItem }
                }));
            }
            catch(Exception ex)
            {
                //_logger.TraceEvent(TraceEventType.Warning)
            }

            return result.ToArray();
        }

        protected override void LoadConfiguration(XmlReader configuration)
        {
            /*
            * <xsd:element minOccurs="1" name="URL" type="xsd:string" />
            * <xsd:element minOccurs="1" name="ValidateCertificate" type="xsd:boolean" />
            * <xsd:element minOccurs="1" name="DisabledHash" type="xsd:string" />
            * <xsd:element minOccurs="0" name="DNSServerList" type="xsd:string" />
            * <xsd:element minOccurs="0" name="DNSRequestTimeoutSec" type="xsd:integer" />
            */
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configuration);
                LoadConfigurationElement(xmlDoc, "URL", out string strURL);
                LoadConfigurationElement(xmlDoc, "DisabledHash", out string strDisabledHash);
                LoadConfigurationElement(xmlDoc, "ValidateCertificate", out _validateCertificate);
                LoadConfigurationElement(xmlDoc, "DNSServerList", out string strDNSServerList, null, false);
                LoadConfigurationElement(xmlDoc, "DNSRequestTimeoutSec", out _dnsRequestTimeout, 20, false);

                // Convert to hash list
                _disabledHashes = new List<Oid>();

                foreach (string hashName in strDisabledHash.Split(TcpHelper.Separators, StringSplitOptions.RemoveEmptyEntries))
                    _disabledHashes.Add(Oid.FromFriendlyName(hashName, OidGroup.SignatureAlgorithm));
                
                // parse URL
                _testUri = new Uri(strURL);
                // parse DNS
                _dnsRequestTimeout *= 1000;
                _customDnsServers = new List<IPAddress>();
                if (!string.IsNullOrWhiteSpace(strDNSServerList))
                    foreach (string dnsIPstr in strDNSServerList.Split(TcpHelper.Separators, StringSplitOptions.RemoveEmptyEntries))
                        _customDnsServers.Add(IPAddress.Parse(dnsIPstr.Trim()));
            }
            catch (Exception ex)
            {
                throw new ModuleException("Error parsing configuration XML", ex);
            }
        }

        private bool RecordCertificateProperties(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            _policyErrors = sslPolicyErrors;
            return true;
        }
    }
}
