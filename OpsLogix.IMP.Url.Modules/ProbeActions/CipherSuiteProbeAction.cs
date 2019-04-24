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
using System.Xml;

namespace OpsLogix.IMP.Url.Modules.ProbeActions
{
    /// <summary>
    /// Probe action for 
    /// </summary>
    [MonitoringModule(ModuleType.ReadAction), ModuleOutput(true)]
    public class CipherSuiteProbeAction: ModuleBaseExtension<PropertyBagDataItem>
    {
        private IEnumerable<SslProtocols> _cipherSuitesToTest, _allowedCipherSuites, _requiredCipherSuites;
        private string _strCipherSuitesToTest, _strAllowedCipherSuites, _strRequiredCipherSuites;

        private Uri _testUri;
        private int _dnsRequestTimeout;
        private List<IPAddress> _customDnsServers;

        public CipherSuiteProbeAction(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
        {
        }

        protected override void LoadConfiguration(XmlReader configuration)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configuration);
                LoadConfigurationElement(xmlDoc, "URL", out string strUrl);
                LoadConfigurationElement(xmlDoc, "CipherSuitesToTest", out _strCipherSuitesToTest);
                LoadConfigurationElement(xmlDoc, "AllowedCipherSuites", out _strAllowedCipherSuites, string.Empty, false);
                LoadConfigurationElement(xmlDoc, "RequiredCipherSuites", out _strRequiredCipherSuites, string.Empty, false);
                LoadConfigurationElement(xmlDoc, "DNSServerList", out string strDNSServerList, null, false);
                LoadConfigurationElement(xmlDoc, "DNSRequestTimeoutSec", out _dnsRequestTimeout, 20, false);

                if (string.IsNullOrWhiteSpace(_strAllowedCipherSuites) && string.IsNullOrWhiteSpace(_strRequiredCipherSuites))
                    throw new ArgumentOutOfRangeException("Either AllowedCipherSuites or RequiredCipherSuites must have a value.");
               
                // Convert to protocol list
                _cipherSuitesToTest = DecodeSslProtocolType(_strCipherSuitesToTest);
                _allowedCipherSuites = DecodeSslProtocolType(_strAllowedCipherSuites);
                _requiredCipherSuites = DecodeSslProtocolType(_strRequiredCipherSuites);

                // parse URL
                _testUri = new Uri(strUrl);

                // parse DNS
                _dnsRequestTimeout *= 1000;
                _customDnsServers = new List<IPAddress>();

                if (!string.IsNullOrWhiteSpace(strDNSServerList))
                    foreach (string dnsIPstr in strDNSServerList.Split(TcpHelper.Separators, StringSplitOptions.RemoveEmptyEntries))
                        _customDnsServers.Add(IPAddress.Parse(dnsIPstr.Trim()));
            }
            catch (XmlException xex)
            {
                throw new ModuleException("Error parsing configuration XML", xex);
            }
        }

        /// <summary>
        /// CipherSuiteProbe implementation of retrieving output data
        /// </summary>
        /// <param name="inputDataItems"></param>
        /// <returns></returns>
        protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
        {
            var result = new List<PropertyBagDataItem>();

            // true == OK, false = Security Risk
            bool aStatus = true, rStatus = true;

            var supportedCipherSuites = new List<SslProtocols>();

            //skip checks if scheme is not secure (nothing to check)
            if (Array.IndexOf(TcpHelper.SecureSchemes, _testUri.Scheme.ToLowerInvariant()) == -1)
                return result.ToArray(); 
                
            // list all supported CS
            foreach (var cipherSuite in _cipherSuitesToTest)
            {
                if (TryConnect(_testUri.Host, _testUri.Port, cipherSuite))
                    supportedCipherSuites.Add(cipherSuite);
            }

            // Test results
            string currentCipherSuiteList = string.Empty;
            foreach (var cipherSuite in supportedCipherSuites)
            {
                aStatus = aStatus & _allowedCipherSuites.Contains(cipherSuite);
                currentCipherSuiteList += $"{cipherSuite}; ";
            }

            foreach (var cipherSuite in _requiredCipherSuites)
                rStatus = rStatus & supportedCipherSuites.Contains(cipherSuite);
                
            // create output
            var bagItem = new Dictionary<string, object>();
            if (aStatus)
                bagItem.Add("AState", "OK");
            else
                bagItem.Add("AState", "SecurityRisk");
            if (rStatus)
                bagItem.Add("RState", "OK");
            else
                bagItem.Add("RState", "SecurityRisk");

            bagItem.Add("SupportedProtocols", currentCipherSuiteList);

            // return configuration items
            bagItem.Add("ListAllowed", _strAllowedCipherSuites);
            bagItem.Add("ListRequired", _strRequiredCipherSuites);

            //Add propertybag
            result.Add(new PropertyBagDataItem(null, new Dictionary<string, Dictionary<string, object>>
            {
                { string.Empty, bagItem }
            }));

            return result.ToArray();
        }

        private bool TryConnect(string hostName, int port, SslProtocols cipherName)
        {
            if (string.IsNullOrEmpty(hostName)) throw new ArgumentNullException(nameof(hostName));

            //QUESTION: Why do we always take the first one?
            hostName = _customDnsServers.Count() > 0 ? TcpHelper.ResolveHostName(hostName, _customDnsServers.ToArray()).FirstOrDefault().ToString() : hostName;

            using (var tcpClient = new TcpClient(hostName, port))
            using (var sslStream = new SslStream(tcpClient.GetStream()) { ReadTimeout = 15000, WriteTimeout = 15000 })
            {
                try
                {
                    sslStream.AuthenticateAsClient(hostName, null, cipherName, false);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private IEnumerable<SslProtocols> DecodeSslProtocolType(string inputValue)
        {
            var result = new List<SslProtocols>();

            foreach (string protocolName in inputValue.Split(TcpHelper.Separators, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse(protocolName, true, out SslProtocols parsedProtocol))
                    result.Add(parsedProtocol);
                else
                    throw new ArgumentException($"Cannot parse {protocolName}");
            }

            return result;
        }
    }
}
