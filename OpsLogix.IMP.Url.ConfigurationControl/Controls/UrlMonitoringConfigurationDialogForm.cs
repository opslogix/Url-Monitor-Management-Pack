using Microsoft.EnterpriseManagement.Monitoring;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using OpsLogix.IMP.Url.ConfigurationControl.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpsLogix.IMP.Url.ConfigurationControl.Controls
{
    public partial class UrlMonitoringConfigurationDialogForm : Form, IUrlMonitoringConfigurationDialogView
    {
        public event EventHandler<UrlAddressInstanceEventArgs> ConfiguredUrl;

        public string Address { get => tbAddress.Text; set => tbAddress.Text = value; }
        public string Description { get => tbDescription.Text; set => tbDescription.Text = value; }
        public string DisplayName { get => tbDisplayName.Text; set => tbDisplayName.Text = value; }
        public string Proxy { get => tbProxy.Text; set => tbProxy.Text = value; }
        public string CustomDnsServers { get => tbCustomDNSServers.Text; set => tbCustomDNSServers.Text = value; }
        public int DnsTimeout { get => (int)nudDNSTimeout.Value; set => nudDNSTimeout.Value = value; }
        public MonitoringObject ActionPoint { get => (MonitoringObject)cmbActionPoint.SelectedItem; set => cmbActionPoint.SelectedItem = value; }
        public IEnumerable<MonitoringObject> ActionPoints { get => (IEnumerable<MonitoringObject>)cmbActionPoint.DataSource; set => cmbActionPoint.DataSource = value; }
        private UrlAddressMonitoringInstance _instance;
        public UrlAddressMonitoringInstance Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                if (!string.IsNullOrEmpty(value.Address))
                    tbAddress.Enabled = false;
                else
                    tbAddress.Enabled = true;

                Address = value.Address;
                Description = value.Description;
                DisplayName = value.DisplayName;
                Proxy = value.ProxyAddress;
                CustomDnsServers = value.DnsServers;
                DnsTimeout = value.DnsServersTimeout;
                ActionPoint = ActionPoints.FirstOrDefault(x => x.Id == value.ActionPoint?.Id);

                _instance = value;
            }
        }

        public UrlMonitoringConfigurationDialogForm()
        {
            InitializeComponent();
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            ConfiguredUrl?.Invoke(this, new UrlAddressInstanceEventArgs());
        }
    }
}
