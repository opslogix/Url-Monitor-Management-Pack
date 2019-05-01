using Microsoft.EnterpriseManagement.Monitoring;
using OpsLogix.IMP.Url.ConfigurationControl.Controls;
using OpsLogix.IMP.Url.Shared.Helpers;
using System;
using System.Collections.Generic;

namespace OpsLogix.IMP.Url.ConfigurationControl.Models
{
    public interface IUrlMonitoringConfigurationModel
    {
        event EventHandler<UrlAddressInstanceEventArgs> ConfigureInstance;
        event EventHandler<UrlAddressInstanceEventArgs> InstanceAdded;
        event EventHandler<UrlAddressInstanceEventArgs> InstanceDeleted;
        event EventHandler<UrlAddressInstanceEventArgs> InstanceUpdated;

        void ConfigureUrlMonitoringInstance(UrlAddressMonitoringInstance instance = null);

        void AddUrlMonitoringInstance(UrlAddressMonitoringInstance instance);
        void UpdateUrlMonitoringInstance(UrlAddressMonitoringInstance instances);

        IEnumerable<UrlAddressMonitoringInstance> GetUrlMonitoringInstances();
        void DeleteUrlMonitoringInstances(IEnumerable<UrlAddressMonitoringInstance> instances);
        IEnumerable<MonitoringObject> GetActionPoints();
        InstanceValidationResult ValidateInstance(UrlAddressMonitoringInstance instance);
    }
}
