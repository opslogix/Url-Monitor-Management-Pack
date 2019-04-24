using OpsLogix.IMP.Url.ConfigurationControl.Controls;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using OpsLogix.IMP.Url.ConfigurationControl.Views;
using System;
using System.ComponentModel;
using System.Linq;

namespace OpsLogix.IMP.Url.ConfigurationControl.Presenters
{
    public class UrlMonitoringConfigurationPresenter
    {
        private readonly IUrlMonitoringConfigurationView _view;
        private readonly IUrlMonitoringConfigurationModel _model;

        public UrlMonitoringConfigurationPresenter(IUrlMonitoringConfigurationView view, IUrlMonitoringConfigurationModel model)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));

            _view.EditUrl += _view_EditUrl;
            _view.AddUrl += _view_AddUrl;
            _view.DeleteUrl += _view_DeleteUrl;

            _view.Urls = new BindingList<UrlAddressMonitoringInstance>(_model.GetUrlMonitoringInstances().ToList());

            _model.InstanceAdded += _model_InstanceAdded;
            _model.InstanceDeleted += _model_InstanceDeleted;
            _model.InstanceUpdated += _model_InstanceUpdated;
        }

        private void _model_InstanceUpdated(object sender, UrlAddressInstanceEventArgs e)
        {
            _view.Urls = new BindingList<UrlAddressMonitoringInstance>(_model.GetUrlMonitoringInstances().ToList());
        }

        private void _model_InstanceDeleted(object sender, UrlAddressInstanceEventArgs e)
        {
            _view.Urls = new BindingList<UrlAddressMonitoringInstance>(_model.GetUrlMonitoringInstances().ToList());
        }

        private void _model_InstanceAdded(object sender, UrlAddressInstanceEventArgs e)
        {
            _view.Urls = new BindingList<UrlAddressMonitoringInstance>(_model.GetUrlMonitoringInstances().ToList());
        }

        /// <summary>
        /// Remove existing url monitoring instances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _view_DeleteUrl(object sender, UrlAddressInstanceEventArgs e)
        {
            _model.DeleteUrlMonitoringInstances(e.Instances);
        }

        /// <summary>
        /// Add a new url monitoring instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _view_AddUrl(object sender, UrlAddressInstanceEventArgs e)
        {
            _model.ConfigureUrlMonitoringInstance(new UrlAddressMonitoringInstance());
        }

        /// <summary>
        /// Edit an existing url monitoring instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _view_EditUrl(object sender, UrlAddressInstanceEventArgs e)
        {
            _model.ConfigureUrlMonitoringInstance(e.Instances.First());
        }
    }
}
