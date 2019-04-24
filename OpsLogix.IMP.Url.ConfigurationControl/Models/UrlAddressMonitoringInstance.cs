using Microsoft.EnterpriseManagement.Monitoring;
using OpsLogix.IMP.Url.Shared;
using OpsLogix.IMP.Url.Shared.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpsLogix.IMP.Url.ConfigurationControl.Models
{
    public class UrlAddressMonitoringInstance : ScomMonitoringInstance, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [ScomClassProperty("afb4f9e6-bf48-1737-76ad-c9b3ec325b97")]
        private string _displayName { get; set; }
        [ScomClassProperty("97847f46-5997-9716-22c1-69b38c5ec2c9")]
        private string _address { get; set; }
        [ScomClassProperty("2bb68b9e-7472-efa9-9729-3f9270092b68")]
        private string _description { get; set; }
        [ScomClassProperty("4b8e04d3-d6c3-7590-cab3-2beb96948671")]
        private string _actingAgent { get; set; }
        [ScomClassProperty("d824d222-3199-d368-8251-fad79082238f")]
        private string _dnsServers { get; set; }
        [ScomClassProperty("7a7f0211-9138-582b-9325-731a47103dd3")]
        private int _dnsServersTimeout { get; set; } = 15;
        [ScomClassProperty("4677b553-210c-b3d0-ce1d-fd3a9b72d4dd")]
        private string _proxyAddress { get; set; }

        public UrlAddressMonitoringInstance(MonitoringObject monitoringObject = null, MonitoringObject actionPoint = null) : base(monitoringObject, actionPoint) { }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (value != _displayName)
                {
                    _displayName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                if (value != _address)
                {
                    _address = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ActingAgent
        {
            get
            {
                return _actingAgent;
            }

            set
            {
                if (value != _actingAgent)
                {
                    _actingAgent = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string DnsServers
        {
            get
            {
                return _dnsServers;
            }

            set
            {
                if (value != _dnsServers)
                {
                    _dnsServers = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int DnsServersTimeout
        {
            get
            {
                return _dnsServersTimeout;
            }

            set
            {
                if (value != _dnsServersTimeout)
                {
                    _dnsServersTimeout = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ProxyAddress
        {
            get
            {
                return _proxyAddress;
            }

            set
            {
                if (value != _proxyAddress)
                {
                    _proxyAddress = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
