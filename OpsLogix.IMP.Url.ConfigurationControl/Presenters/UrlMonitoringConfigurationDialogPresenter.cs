using OpsLogix.IMP.Url.ConfigurationControl.Controls;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using OpsLogix.IMP.Url.ConfigurationControl.Views;
using System;
using System.Linq;
using System.Windows.Forms;

namespace OpsLogix.IMP.Url.ConfigurationControl.Presenters
{
    public class UrlMonitoringConfigurationDialogPresenter
    {
        private readonly IUrlMonitoringConfigurationModel _model;
        private readonly IUrlMonitoringConfigurationDialogView _view;
        public UrlMonitoringConfigurationDialogPresenter(IUrlMonitoringConfigurationDialogView view, IUrlMonitoringConfigurationModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));

            _model.ConfigureInstance += _model_ConfigureInstance;
            _view.ActionPoints = _model.GetActionPoints().ToList();
            _view.ConfiguredUrl += _view_ConfiguredUrl;
        }

        /// <summary>
        /// Validates the configured url and adds or updates the monitoring instance 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _view_ConfiguredUrl(object sender, UrlAddressInstanceEventArgs e)
        {
            var instance = new UrlAddressMonitoringInstance
            {
                Address = _view.Address,
                Description = _view.Description,
                DisplayName = _view.DisplayName,
                DnsServers = _view.CustomDnsServers,
                DnsServersTimeout = _view.DnsTimeout,
                ProxyAddress = _view.Proxy,
                ActingAgent = _view.ActionPoint?.DisplayName,
                ActionPoint = _view.ActionPoint
            };

            try
            {
                //Update or create instance
                if (string.IsNullOrEmpty(_view.Instance.Address))
                    _model.AddUrlMonitoringInstance(instance);
                else
                    _model.UpdateUrlMonitoringInstance(instance);

                _view.Hide();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Invalid instance", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Shows view for user to configure url monitoring instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _model_ConfigureInstance(object sender, UrlAddressInstanceEventArgs e)
        {
            _view.Instance = e.Instances.First();

            _view.ShowDialog();
        }
    }
}
