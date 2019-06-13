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
        private readonly Guid _urlMonitoringAddressClassId = new Guid("7072898d-75b3-9a8b-20ed-404021f93c60");

        public UrlMonitoringConfigurationContainer()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            var scomSdkService = new ScomSdkService(base.ManagementGroup);
            var urlConfigurationModel = new UrlMonitoringConfigurationModel(scomSdkService, _urlMonitoringAddressClassId, SystemMonitoringClass.HealthService.Id);

            new UrlMonitoringConfigurationPresenter(urlMonitoringConfigurationControl, urlConfigurationModel);
            new UrlMonitoringConfigurationDialogPresenter(new UrlMonitoringConfigurationDialogForm(), urlConfigurationModel);

            base.OnLoad(e);
        }
    }
}
