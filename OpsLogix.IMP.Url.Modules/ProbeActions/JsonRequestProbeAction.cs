using Microsoft.EnterpriseManagement.HealthService;
using Microsoft.EnterpriseManagement.Mom.Modules.DataItems;
using Newtonsoft.Json.Linq;
using OpsLogix.IMP.Url.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace OpsLogix.IMP.Url.Modules.ProbeActions
{
    [MonitoringModule(ModuleType.ReadAction), ModuleOutput(true)]
    public class JsonRequestProbeAction : ModuleBaseExtension<PropertyBagDataItem>
    {
        private Uri _testUri;
        private string _method, _requestPath;
        private NameValueCollection _headers;
        private string _userAgent, _jsonBody;
        private string _authenticationUserName, _authenticationPassword, _authenticationDomain = string.Empty;
        private string _authenticationScheme;
        private string _okFieldValue, _okFieldName;
        private string _errorFieldValue, _errorFieldName;
        private int _dnsRequestTimeout;
        private List<IPAddress> _customDnsServers;

        public JsonRequestProbeAction(ModuleHost<PropertyBagDataItem> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost, configuration, previousState)
        {
        }

        /// <summary>
        /// TODO: Needs some more work
        /// </summary>
        /// <param name="inputDataItems"></param>
        /// <returns></returns>
        protected override PropertyBagDataItem[] GetOutputData(DataItemBase[] inputDataItems)
        {
            var result = new List<PropertyBagDataItem>();

            var requestURI = new UriBuilder(_testUri)
            {
                Path = _requestPath
            };

            string hostHeader = null;
            if (_customDnsServers.Count > 0)
            {
                // is custom DNS used, then URI is like https://192.168.1.1/path, but Host header is set to the actual host name
                hostHeader = requestURI.Host;
                requestURI.Host = TcpHelper.ResolveHostName(hostHeader, _customDnsServers.ToArray())[0].ToString(); // IP address
            }

            var request = WebRequest.CreateHttp(requestURI.Uri);

            // Set request parameters
            request.Method = _method;
            if (hostHeader != null)
                request.Host = hostHeader;

            TcpHelper.SetHeaders(request, _headers);

            if (!string.IsNullOrWhiteSpace(_jsonBody))
            {
                var encoding = new UTF8Encoding();

                byte[] byteBody = encoding.GetBytes(_jsonBody);
                if (string.IsNullOrWhiteSpace(request.ContentType))
                    request.ContentType = "application/json";

                request.ContentLength = byteBody.Length;

                using (var dataStream = request.GetRequestStream())
                    dataStream.Write(byteBody, 0, byteBody.Length);
            }

            // Create CredentialCache if authentication is required
            if (!string.IsNullOrWhiteSpace(_authenticationScheme) && !string.IsNullOrWhiteSpace(_authenticationUserName) && !string.IsNullOrWhiteSpace(_authenticationPassword))
            {
                var cachedCreds = new CredentialCache();

                if (string.IsNullOrWhiteSpace(_authenticationDomain))
                    cachedCreds.Add(_testUri, _authenticationScheme, new NetworkCredential(_authenticationUserName, _authenticationPassword));
                else
                    cachedCreds.Add(_testUri, _authenticationScheme, new NetworkCredential(_authenticationUserName, _authenticationPassword, _authenticationDomain));

                request.Credentials = cachedCreds;
            }

            var bagItem = new Dictionary<string, object>();

            try
            {
                // Execute the request
                using (var response = request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string fullResponse = reader.ReadToEnd();
                    var responseParsed = JObject.Parse(fullResponse);

                    //QUESTION: Why? use fake cycle to get advantage of break statement when an error state is detected
                    do
                    {
                        // lookup for a specifically defined error in JSON response, even if everything looks OK
                        string responseErrorValue = null;
                        if (!string.IsNullOrWhiteSpace(_errorFieldName))
                        {
                            try
                            {
                                responseErrorValue = GetJsonValue(responseParsed, _errorFieldName);
                            }
                            catch { };
                            // ignore errors 
                            // if error field value parameter is defined, then use it for comparison, otherwise, just "has value"
                        }

                        if (!string.IsNullOrEmpty(responseErrorValue))
                        {
                            bagItem.Add("State", "ERROR");
                            bagItem.Add("ErrorKind", "APPLICATION");
                            bagItem.Add("Error", responseErrorValue);
                            bagItem.Add("Response", fullResponse);
                            break;

                            ////There seems to be no difference between bagitem values
                            //if (!string.IsNullOrEmpty(_errorFieldValue))
                            //{
                            //    if (responseErrorValue == _errorFieldValue)
                            //    {
                            //        bagItem.Add("State", "ERROR");
                            //        bagItem.Add("ErrorKind", "APPLICATION");
                            //        bagItem.Add("Error", responseErrorValue);
                            //        bagItem.Add("Response", fullResponse);
                            //        break;
                            //    }
                            //}
                            //else
                            //{
                            //    if (responseErrorValue != null) // buttery butter
                            //    {
                                    
                            //    }
                            //}
                        }

                        // now check the OK field
                        string responseOkValue = string.Empty;

                        try
                        {
                            responseOkValue = GetJsonValue(responseParsed, _okFieldName);
                        }
                        catch
                        {
                            bagItem.Add("State", "ERROR");
                            bagItem.Add("ErrorKind", "UNEXPECTED RESULT");
                            bagItem.Add("Error", $"Cannot find {_okFieldName} in the JSON response.");
                            bagItem.Add("Response", fullResponse);
                            break;
                        }
                        if ((responseOkValue != null) && (responseOkValue == _okFieldValue))
                        {
                            bagItem.Add("State", "OK");
                            bagItem.Add("ErrorKind", string.Empty);
                            bagItem.Add("Error", string.Empty);
                            bagItem.Add("Response", fullResponse);
                            break; // not really needed, but if future there might be more conditions, so to keep it's compatible
                        }
                        else
                        {
                            if (responseOkValue == null)
                                responseOkValue = "NULL"; // display purposes only

                            bagItem.Add("State", "ERROR");
                            bagItem.Add("ErrorKind", "UNEXPECTED RESULT");
                            bagItem.Add("Error", $"[{responseOkValue}] is not equal to [{_okFieldValue}]");
                            bagItem.Add("Response", fullResponse);
                            break; // not really needed, but if future there might be more conditions, so to keep it's compatible
                        }
                    } while (false);
                }
            }
            catch (Exception e)
            {
                bagItem.Add("State", "ERROR");
                bagItem.Add("ErrorKind", "SYSTEM");
                bagItem.Add("Error", e.Message);
                bagItem.Add("Response", string.Empty);
            }
            finally
            {
                result.Add(new PropertyBagDataItem(null, new Dictionary<string, Dictionary<string, object>>
                {
                    {string.Empty, bagItem }
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
                LoadConfigurationElement(xmlDoc, "RequestPath", out _requestPath);
                LoadConfigurationElement(xmlDoc, "Method", out _method);
                LoadConfigurationElement(xmlDoc, "JSONBody", out _jsonBody, compulsory: false);
                LoadConfigurationElement(xmlDoc, "Headers", out string strHeaders, compulsory: false);
                LoadConfigurationElement(xmlDoc, "OKFieldName", out _okFieldName);
                LoadConfigurationElement(xmlDoc, "OKFieldValue", out _okFieldValue);
                LoadConfigurationElement(xmlDoc, "ErrorFieldName", out _errorFieldName, null, false);
                LoadConfigurationElement(xmlDoc, "ErrorFieldValue", out _errorFieldValue, null, false);
                LoadConfigurationElement(xmlDoc, "AuthenticationScheme", out _authenticationScheme, null, false);
                LoadConfigurationElement(xmlDoc, "AuthenticationUserName", out _authenticationUserName, string.Empty, false);
                LoadConfigurationElement(xmlDoc, "AuthenticationPassword", out _authenticationPassword, null, false);
                LoadConfigurationElement(xmlDoc, "UserAgent", out _userAgent, null, false);
                LoadConfigurationElement(xmlDoc, "DNSServerList", out string strDNSServerList, null, false);
                LoadConfigurationElement(xmlDoc, "DNSRequestTimeoutSec", out _dnsRequestTimeout, 20, false);

                // parse URL and other
                _testUri = new Uri(strURL);

                // Headers
                char[] headerSeparator = new char[] { '|' };
                char[] headerNameSeparator = new char[] { ':' };

                _headers = new NameValueCollection();
                foreach (string headerNVPair in strHeaders.Split(headerSeparator, StringSplitOptions.RemoveEmptyEntries))
                    _headers.Add(headerNVPair.Split(headerNameSeparator)[0].Trim(), headerNVPair.Split(headerNameSeparator)[1].Trim());

                // UserName, strict condition, because domain name must have at least 1 char
                if (!string.IsNullOrWhiteSpace(_authenticationUserName) && _authenticationUserName.IndexOf('\\') > 0)
                { 
                    char[] domainSeparator = new char[] { '\\' };
                    _authenticationDomain = _authenticationUserName.Split(domainSeparator)[0];
                    _authenticationUserName = _authenticationUserName.Split(domainSeparator)[1];
                }

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

        private string GetJsonValue(JObject response, string valuePath)
        {
            JToken lastResult = null;
            char[] fieldPathSeparator = { '\\' };

            foreach (string fieldName in valuePath.Split(fieldPathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                if (lastResult == null)
                    lastResult = response[fieldName];
                else
                    lastResult = lastResult[fieldName];
            }

            if ((lastResult != null) && !lastResult.HasValues) // has child tokens => non scalar
                return lastResult.Value<string>();
            else
                return null;
        }
    }
}
