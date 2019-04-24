using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;
using OpsLogix.IMP.Url.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;

namespace OpsLogix.IMP.Url.Modules.ProbeActions
{
    [MonitoringModule(ModuleType.ReadAction), ModuleOutput(true)]
    public class PingProbeAction : ModuleBaseExtension<PropertyBagDataItem>
    {
        private Uri _testUri;
        private int _bufferSize, _maxRtt, _maxTtl;
        private bool _dontFragment;
        private int _dnsRequestTimeout;
        private List<IPAddress> _customDnsServers;

        public PingProbeAction(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
        {
        }

        protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
        {
            var result = new List<PropertyBagDataItem>();
            var bagItem = new Dictionary<string, object>();

            try
            {
                var pingOptions = new PingOptions
                {
                    DontFragment = _dontFragment,
                    Ttl = _maxTtl
                };

                string bufferData = new string('A', _bufferSize);
                byte[] buffer = Encoding.ASCII.GetBytes(bufferData);

                var pingTest = new Ping();
                string hostAddress = _testUri.DnsSafeHost;

                if (_customDnsServers.Count > 0)
                    hostAddress = TcpHelper.ResolveHostName(_testUri.DnsSafeHost, _customDnsServers.ToArray())[0].ToString(); // IP address

                var pingReply = pingTest.Send(hostAddress, _maxRtt, buffer, pingOptions);

                if (pingReply.Status == IPStatus.Success)
                {
                    bagItem.Add("State", "OK");
                    bagItem.Add("Error", string.Empty);
                    bagItem.Add("Address", pingReply.Address.ToString());
                    bagItem.Add("RoundTripTime", pingReply.RoundtripTime);
                    bagItem.Add("TimeToLive", pingReply.Options.Ttl);
                }
                else
                {
                    bagItem.Add("State", "ERROR");
                    bagItem.Add("Error", pingReply.Status.ToString());
                    bagItem.Add("Address", pingReply.Address.ToString());
                    bagItem.Add("RoundTripTime", -1);
                    bagItem.Add("TimeToLive", -1);
                }
            }
            catch (Exception ex)
            {
                bagItem.Add("State", "ERROR");
                bagItem.Add("Error", ex.Message);
                bagItem.Add("Address", string.Empty);
                bagItem.Add("RoundTripTime", -1);
                bagItem.Add("TimeToLive", -1);
            }
            finally
            {
                result.Add(new PropertyBagDataItem(null, new Dictionary<string, Dictionary<string, object>>
                {
                    { string.Empty, bagItem }
                }));
            }

            return result.ToArray();
        }

        protected override void LoadConfiguration(XmlReader configuration)
        {
            /*
            * <xsd:element minOccurs="1" name="URL" type="xsd:string" />
            * <xsd:element minOccurs="0" name="BufferSize" type="xsd:integer" />
            * <xsd:element minOccurs="0" name="DontFragment" type="xsd:bool" />
            * <xsd:element minOccurs="0" name="MaxRTT" type="xsd:integer" />
            * <xsd:element minOccurs="0" name="MaxTTL" type="xsd:integer" />
            * <xsd:element minOccurs="0" name="DNSServerList" type="xsd:string" />
            * <xsd:element minOccurs="0" name="DNSRequestTimeoutSec" type="xsd:integer" />
            */
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configuration);
                LoadConfigurationElement(xmlDoc, "URL", out string strURL);
                LoadConfigurationElement(xmlDoc, "BufferSize", out _bufferSize, 32, false);
                LoadConfigurationElement(xmlDoc, "DontFragment", out _dontFragment, false, false);
                LoadConfigurationElement(xmlDoc, "MaxRTT", out _maxRtt, 5000, false);
                LoadConfigurationElement(xmlDoc, "MaxTTL", out _maxTtl, 128, false);
                LoadConfigurationElement(xmlDoc, "DNSServerList", out string strDnsServerList, null, false);
                LoadConfigurationElement(xmlDoc, "DNSRequestTimeoutSec", out _dnsRequestTimeout, 20, false);

                // URL
                _testUri = new Uri(strURL);

                // parse DNS
                _dnsRequestTimeout *= 1000;

                _customDnsServers = new List<IPAddress>();
                if (!string.IsNullOrWhiteSpace(strDnsServerList))
                    foreach (string dnsIPstr in strDnsServerList.Split(TcpHelper.Separators, StringSplitOptions.RemoveEmptyEntries))
                        _customDnsServers.Add(IPAddress.Parse(dnsIPstr.Trim()));
            }
            catch (Exception ex)
            {
                throw new ModuleException("Error parsing configuration XML", ex);
            }
        }
    }
}
