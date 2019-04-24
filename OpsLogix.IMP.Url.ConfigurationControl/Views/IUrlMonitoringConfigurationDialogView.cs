using Microsoft.EnterpriseManagement.Monitoring;
using OpsLogix.IMP.Url.ConfigurationControl.Controls;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpsLogix.IMP.Url.ConfigurationControl.Views
{
    public interface IUrlMonitoringConfigurationDialogView
    {
        event EventHandler<UrlAddressInstanceEventArgs> ConfiguredUrl;

        DialogResult ShowDialog();
        void Hide();

        IEnumerable<MonitoringObject> ActionPoints { get; set; }

        UrlAddressMonitoringInstance Instance { get; set; }
        string Address { get; set; }
        string Description { get; set; }
        string DisplayName { get; set; }
        string Proxy { get; set; }
        string CustomDnsServers { get; set; }
        int DnsTimeout { get; set; }
        MonitoringObject ActionPoint { get; set; }
    }
}
