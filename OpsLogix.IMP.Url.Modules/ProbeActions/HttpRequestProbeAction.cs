using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;
using OpsLogix.IMP.Url.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace OpsLogix.IMP.Url.Modules.ProbeActions
{
    [MonitoringModule(ModuleType.ReadAction), ModuleOutput(true)]
    public class HttpRequestProbeAction : ModuleBaseExtension<PropertyBagDataItem>
    {
        private Uri _testUri;
        private string _method, _pageExpression, _path;
        private NameValueCollection _headers = null;
        private string _userAgent;
        private string _authenticationUserName, _authenticationPassword, _authenticationDomain = string.Empty;
        private string _authenticationScheme;
        private bool _allowRedirection;
        private int _dnsRequestTimeout;
        private List<IPAddress> _customDnsServers;

        public HttpRequestProbeAction(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
        {
        }

        protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
        {
            var result = new List<PropertyBagDataItem>();
            var bagItem = new Dictionary<string, object>();

            try
            {
                var requestUri = new UriBuilder(_testUri)
                {
                    Path = _path
                };

                string hostHeader = null;

                if (_customDnsServers.Count > 0)
                {
                    // is custom DNS used, then URI is like https://192.168.1.1/path, but Host header is set to the actual host name
                    hostHeader = requestUri.Host;
                    requestUri.Host = TcpHelper.ResolveHostName(hostHeader, _customDnsServers.ToArray())[0].ToString(); // IP address
                }

                // prepare request
                var request = WebRequest.CreateHttp(requestUri.Uri);
                request.Method = _method;

                if (hostHeader != null)
                    request.Host = hostHeader;

                TcpHelper.SetHeaders(request, _headers);

                if (!string.IsNullOrEmpty(_userAgent))
                    request.UserAgent = _userAgent;

                if (!string.IsNullOrWhiteSpace(_authenticationScheme))
                {
                    var cachedCreds = new CredentialCache();

                    if (string.IsNullOrWhiteSpace(_authenticationDomain))
                        cachedCreds.Add(_testUri, _authenticationScheme, new NetworkCredential(_authenticationUserName, _authenticationPassword));
                    else
                        cachedCreds.Add(_testUri, _authenticationScheme, new NetworkCredential(_authenticationUserName, _authenticationPassword, _authenticationDomain));
                    request.Credentials = cachedCreds;
                }

                request.AllowAutoRedirect = _allowRedirection;
                // Execute the request
                using (var response = request.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string fullResponse = reader.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(_pageExpression) && !Regex.IsMatch(fullResponse, _pageExpression))
                        {
                            bagItem.Add("State", "WARNING");
                            bagItem.Add("Error", "Page regular expression does not match.");
                            bagItem.Add("Response", fullResponse.Substring(0, fullResponse.Length >= 255 ? 255 : fullResponse.Length));
                            bagItem.Add("ResponseLength", fullResponse.Length);
                        }
                        else
                        {
                            bagItem.Add("State", "OK");
                            bagItem.Add("Error", "");
                            bagItem.Add("Response", fullResponse.Substring(0, fullResponse.Length >= 255 ? 255 : fullResponse.Length));
                            bagItem.Add("ResponseLength", fullResponse.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                bagItem.Add("State", "ERROR");
                bagItem.Add("Error", e.Message);
                bagItem.Add("Response", string.Empty);
                bagItem.Add("ResponseLength", 0);
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
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configuration);
                LoadConfigurationElement(xmlDoc, "URL", out string strURL);
                LoadConfigurationElement(xmlDoc, "Path", out _path, string.Empty, false);
                LoadConfigurationElement(xmlDoc, "AllowRedirection", out _allowRedirection, false, false);
                LoadConfigurationElement(xmlDoc, "Method", out _method);
                LoadConfigurationElement(xmlDoc, "PageExpression", out _pageExpression, string.Empty, false);
                LoadConfigurationElement(xmlDoc, "Headers", out string strHeaders, string.Empty, false);
                LoadConfigurationElement(xmlDoc, "AuthenticationScheme", out _authenticationScheme);
                LoadConfigurationElement(xmlDoc, "AuthenticationUserName", out _authenticationUserName);
                LoadConfigurationElement(xmlDoc, "AuthenticationPassword", out _authenticationPassword);
                LoadConfigurationElement(xmlDoc, "UserAgent", out _userAgent);
                LoadConfigurationElement(xmlDoc, "DNSServerList", out string strDNSServerList, null, false);
                LoadConfigurationElement(xmlDoc, "DNSRequestTimeoutSec", out _dnsRequestTimeout, 20, false);
                // parse URL and other fields
                // URL
                _testUri = new Uri(strURL);
                // Headers
                char[] headerSeparator = new char[] { '|' };
                char[] headerNameSeparator = new char[] { ':' };
                _headers = new NameValueCollection();
                foreach (string headerNVPair in strHeaders.Split(headerSeparator, StringSplitOptions.RemoveEmptyEntries))
                    _headers.Add(headerNVPair.Split(headerNameSeparator)[0], headerNVPair.Split(headerNameSeparator)[1]);
                // UserName
                if (_authenticationUserName.IndexOf('\\') > 0) // strict condition, because domain name must have at least 1 char
                {
                    char[] domainSeparator = new char[] { '\\' };
                    _authenticationDomain = _authenticationUserName.Split(domainSeparator)[0];
                    _authenticationUserName = _authenticationUserName.Split(domainSeparator)[1];
                }
                // parse DNS
                _dnsRequestTimeout = _dnsRequestTimeout * 1000;
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
    }
}
