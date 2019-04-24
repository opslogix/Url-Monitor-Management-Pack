using OpsLogix.IMP.Url.ConfigurationControl.Controls;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using System;
using System.Collections.Generic;

namespace OpsLogix.IMP.Url.ConfigurationControl.Views
{
    public interface IUrlMonitoringConfigurationView
    {
        event EventHandler<UrlAddressInstanceEventArgs> AddUrl;
        event EventHandler<UrlAddressInstanceEventArgs> EditUrl;
        event EventHandler<UrlAddressInstanceEventArgs> DeleteUrl;
        bool Enabled { get; set; }
        IEnumerable<UrlAddressMonitoringInstance> Urls { get; set; }
    }
}
