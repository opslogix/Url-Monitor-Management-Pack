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
        [ScomClassProperty("1e274158-73c3-27ff-3ccd-4d73150124a0")]
        private string _description { get; set; }
        [ScomClassProperty("c385c626-b034-43a4-4cc5-8b84a617658b")]
        private string _actingAgent { get; set; }
        [ScomClassProperty("fc50a8e2-6c9f-9f21-53bf-80448f06006c")]
        private string _dnsServers { get; set; }
        [ScomClassProperty("391bc527-7c78-3dca-4c2e-61605e4c3073")]
        private int _dnsServersTimeout { get; set; } = 15;
        [ScomClassProperty("b0707798-7df1-612f-9303-6e90ad9b0272")]
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
