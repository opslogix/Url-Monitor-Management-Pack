using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using OpsLogix.IMP.Url.ConfigurationControl.Controls;
using OpsLogix.IMP.Url.Shared;
using OpsLogix.IMP.Url.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpsLogix.IMP.Url.ConfigurationControl.Models
{
    public class UrlMonitoringConfigurationModel : IUrlMonitoringConfigurationModel
    {
        private readonly ScomClassInstanceEditor<UrlAddressMonitoringInstance> _urlInstanceEditor;
        private readonly IScomSdkService _scomSdkService;

        private readonly ManagementPackClass _actionPointClass;

        public event EventHandler<UrlAddressInstanceEventArgs> ConfigureInstance;
        public event EventHandler<UrlAddressInstanceEventArgs> InstanceAdded;
        public event EventHandler<UrlAddressInstanceEventArgs> InstanceDeleted;
        public event EventHandler<UrlAddressInstanceEventArgs> InstanceUpdated;

        private List<UrlAddressMonitoringInstance> _urlAddresses { get; set; }
        private IEnumerable<MonitoringObject> _actionPoints { get; set; }

        public UrlMonitoringConfigurationModel(IScomSdkService scomSdkService, Guid urlMonitoringAddressClassId, Guid actionPointClassId)
        {
            _scomSdkService = scomSdkService ?? throw new ArgumentNullException(nameof(scomSdkService));
            if (actionPointClassId == null) throw new ArgumentNullException(nameof(actionPointClassId));
            if (urlMonitoringAddressClassId == null) throw new ArgumentNullException(nameof(urlMonitoringAddressClassId));

            _urlInstanceEditor = _scomSdkService.GetInstanceEditor<UrlAddressMonitoringInstance>(urlMonitoringAddressClassId, actionPointClassId);
            _actionPointClass = _scomSdkService.ManagementGroup.EntityTypes.GetClass(actionPointClassId);
        }

        public void AddUrlMonitoringInstance(UrlAddressMonitoringInstance instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            //Check for duplicate class key
            if (_urlAddresses.FirstOrDefault(x => x.Address == instance.Address) != null)
                throw new Exception("Unable to add new instance, Existing instance found with same key property");

            _urlInstanceEditor.ExecuteDiscovery(ScomDiscoveryType.Insert, new UrlAddressMonitoringInstance[] { instance });
            _urlAddresses.Add(instance);

            InstanceAdded?.Invoke(this, new UrlAddressInstanceEventArgs(instance));
        }

        public void UpdateUrlMonitoringInstance(UrlAddressMonitoringInstance instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var urlToUpdate = _urlAddresses.FirstOrDefault(x => x.Address == instance.Address);

            //Check for duplicate class key
            if (urlToUpdate == null)
                throw new Exception("Unable to update instance, Could not find existing instance with same key property");

            _urlInstanceEditor.ExecuteDiscovery(ScomDiscoveryType.Update, new UrlAddressMonitoringInstance[] { instance });

            int index = _urlAddresses.IndexOf(urlToUpdate);
            if (index != -1)
                _urlAddresses[index] = instance;

            InstanceUpdated?.Invoke(this, new UrlAddressInstanceEventArgs(instance));
        }

        public void DeleteUrlMonitoringInstances(IEnumerable<UrlAddressMonitoringInstance> instances)
        {
            if (instances == null) throw new ArgumentNullException(nameof(instances));

            _urlInstanceEditor.ExecuteDiscovery(ScomDiscoveryType.Delete, instances);

            foreach(var instance in instances)
                _urlAddresses.Remove(_urlAddresses.First(x => x.Address == instance.Address));

            InstanceDeleted?.Invoke(this, new UrlAddressInstanceEventArgs(instances.FirstOrDefault()));
        }

        public IEnumerable<UrlAddressMonitoringInstance> GetUrlMonitoringInstances()
        {
            if (_urlAddresses != null)
                return _urlAddresses;

            _urlAddresses = _urlInstanceEditor.GetInstances().ToList();

            return _urlAddresses;
        }

        public IEnumerable<MonitoringObject> GetActionPoints()
        {
            if (_actionPoints != null)
                return _actionPoints;

            _actionPoints = _scomSdkService.ManagementGroup.EntityObjects.GetObjectReader<MonitoringObject>(_actionPointClass, ObjectQueryOptions.Default);

            return _actionPoints;
        }

        public void ConfigureUrlMonitoringInstance(UrlAddressMonitoringInstance instance)
        {
            ConfigureInstance?.Invoke(this, new UrlAddressInstanceEventArgs(instance));
        }
    }
}
