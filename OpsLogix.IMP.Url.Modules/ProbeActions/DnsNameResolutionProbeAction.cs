using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;
using OpsLogix.IMP.Url.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace OpsLogix.IMP.Url.Modules.ProbeActions
{
    /// <summary>
    /// 
    /// </summary>
    [MonitoringModule(ModuleType.ReadAction), ModuleOutput(true)]
    public class DnsNameResolutionProbeAction: ModuleBaseExtension<PropertyBagDataItem>
    {
        private Uri _testUri;
        private int _minValidResponses, _dnsRequestTimeout;
        private List<IPAddress> _customDnsServers;

        public DnsNameResolutionProbeAction(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
        {
            _logger.WriteLog(TraceEventType.Information, 95, $"New probe initialized");
        }

        /// <summary>
        /// TODO: this method needs some work
        /// </summary>
        /// <param name="inputDataItems"></param>
        /// <returns></returns>
        protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
        {
            var result = new List<PropertyBagDataItem>();

            var bagItem = new Dictionary<string, object>();

            string ipAddressesText = string.Empty;

            if (_customDnsServers.Count == 0)
            {
                try
                {
                    var ipAddresses = Dns.GetHostAddresses(_testUri.DnsSafeHost);
                    foreach (var ipAddress in ipAddresses)
                        ipAddressesText += $"{ipAddress}; ";

                    bagItem.Add("State", "OK");
                    bagItem.Add("DNSServers", "<System Defined>");
                    bagItem.Add("Error", "");
                    bagItem.Add("IP", ipAddressesText);
                }
                // catch only name resolution process related exceptions to alert on resolution problem
                catch (SocketException se)
                {
                    bagItem.Add("State", "ERROR");
                    bagItem.Add("DNSServers", "<System Defined>");
                    bagItem.Add("Error", se.Message);
                    bagItem.Add("IP", "");
                }
                catch (ArgumentException se)
                {
                    bagItem.Add("State", "ERROR");
                    bagItem.Add("DNSServers", "<System Defined>");
                    bagItem.Add("Error", se.Message);
                    bagItem.Add("IP", "");
                }
            }
            else
            {
                int successCount = 0;
                string errorList = string.Empty;
                string failedDnsServers = string.Empty, successDnsServers = string.Empty;

                foreach (var dns in _customDnsServers)
                {
                    try
                    {
                        var draftIP = TcpHelper.GetHostEntry(_testUri.DnsSafeHost, dns, timeOut: _dnsRequestTimeout);
                        if (draftIP?.AddressList?.Length > 0)
                        {
                            successCount++;
                            foreach (var ipAddress in draftIP.AddressList)
                                ipAddressesText += $"{ipAddress}; ";

                            successDnsServers += $"{dns}; ";
                        }
                        else
                        {
                            errorList += $"DNS Server [{dns}] returned no addresses for '{_testUri.DnsSafeHost}'. ";
                            failedDnsServers += $"{dns}; ";
                        }
                    }
                    catch (SocketException e)
                    {
                        errorList += $"Error: [{e.Message}]; ";
                        failedDnsServers += $"{dns}; ";
                    }
                }

                string dnsServers = $"Query Success: {successDnsServers} Query empty of failed: {failedDnsServers}";

                if (successCount == 0)
                {
                    bagItem.Add("State", "ERROR");
                    bagItem.Add("DNSServers", dnsServers);
                    bagItem.Add("Error", errorList);
                    bagItem.Add("IP", "");
                }
                else if (successCount < _minValidResponses)
                {
                    bagItem.Add("State", "WARNING");
                    bagItem.Add("DNSServers", dnsServers);
                    bagItem.Add("Error", errorList);
                    bagItem.Add("IP", ipAddressesText);
                }
                else
                {
                    bagItem.Add("State", "OK");
                    bagItem.Add("DNSServers", dnsServers);
                    bagItem.Add("Error", errorList);
                    bagItem.Add("IP", ipAddressesText);
                }
            }

            result.Add(new PropertyBagDataItem(null, new Dictionary<string, Dictionary<string, object>>
            {
                {string.Empty, bagItem }
            }));

            return result.ToArray();
        }

        protected override void LoadConfiguration(XmlReader configuration)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configuration);
                LoadConfigurationElement(xmlDoc, "URL", out string strURL);
                LoadConfigurationElement(xmlDoc, "DNSServerList", out string strDnsServerList, null, false);
                LoadConfigurationElement(xmlDoc, "MinValidResponses", out _minValidResponses, 1, false);
                LoadConfigurationElement(xmlDoc, "DNSRequestTimeoutSec", out _dnsRequestTimeout, 20, false);

                _dnsRequestTimeout *= 1000;

                // parse inputs
                _testUri = new Uri(strURL);
                _customDnsServers = new List<IPAddress>();

                if (!string.IsNullOrWhiteSpace(strDnsServerList))
                {
                    foreach (string dnsIPstr in strDnsServerList.Split(TcpHelper.Separators, StringSplitOptions.RemoveEmptyEntries))
                        _customDnsServers.Add(IPAddress.Parse(dnsIPstr.Trim()));

                    if (_customDnsServers.Count == 0)
                        _minValidResponses = -1;
                    if (_minValidResponses > _customDnsServers.Count)
                        _minValidResponses = _customDnsServers.Count;
                }
                else
                {
                    _minValidResponses = -1;
                }
            }
            catch (Exception ex)
            {
                throw new ModuleException("Error parsing configuration XML", ex);
            }
        }
    }
}
