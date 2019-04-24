using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Mom.Internal.UI;
using OpsLogix.IMP.Url.ConfigurationControl.Controls;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using OpsLogix.IMP.Url.ConfigurationControl.Presenters;
using OpsLogix.IMP.Url.Shared.Services;
using System;

namespace OpsLogix.IMP.Url.ConfigurationControl
{
    public partial class UrlMonitoringConfigurationContainer : MomViewBase
    {
        private readonly Guid _urlMonitoringAddressClassId = new Guid("4146aafa-4d54-17cc-30cc-8b6d5ce6d392");

        public UrlMonitoringConfigurationContainer(string managementGroupName = null)
        {
            InitializeComponent();

            ManagementGroup managementGroup = null;

            if (!string.IsNullOrEmpty(managementGroupName))
                managementGroup = new ManagementGroup(managementGroupName);
            else
                managementGroup = new ManagementGroup("localhost");

            var scomSdkService = new ScomSdkService(managementGroup);
            var urlConfigurationModel = new UrlMonitoringConfigurationModel(scomSdkService, _urlMonitoringAddressClassId, SystemMonitoringClass.HealthService.Id);

            new UrlMonitoringConfigurationPresenter(urlMonitoringConfigurationControl, urlConfigurationModel);
            new UrlMonitoringConfigurationDialogPresenter(new UrlMonitoringConfigurationDialogForm(), urlConfigurationModel);
        }
    }
}
