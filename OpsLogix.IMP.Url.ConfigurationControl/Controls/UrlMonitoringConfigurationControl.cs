using BrightIdeasSoftware;
using Microsoft.EnterpriseManagement.Mom.Internal.UI;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using OpsLogix.IMP.Url.ConfigurationControl.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpsLogix.IMP.Url.ConfigurationControl.Controls
{
    /// <summary>
    /// Control for managing and viewing your collection of URL monitoring instances
    /// </summary>
    public partial class UrlMonitoringConfigurationControl : MomViewBase, IUrlMonitoringConfigurationView
    {
        public event EventHandler<UrlAddressInstanceEventArgs> AddUrl;
        public event EventHandler<UrlAddressInstanceEventArgs> EditUrl;
        public event EventHandler<UrlAddressInstanceEventArgs> DeleteUrl;

        public UrlMonitoringConfigurationControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Double click row to edit url instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DlvUrls_DoubleClick(object sender, EventArgs e)
        {
            if (dlvUrls.SelectedItem != null)
                EditUrl?.Invoke(this, new UrlAddressInstanceEventArgs((UrlAddressMonitoringInstance)dlvUrls.SelectedItem.RowObject));
        }

        /// <summary>
        /// Cell button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dlvUrls_ButtonClick(object sender, CellClickEventArgs e)
        {
            EditUrl?.Invoke(sender, new UrlAddressInstanceEventArgs((UrlAddressMonitoringInstance)e.Model));
        }

        /// <summary>
        /// Add button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddUrl_Click(object sender, EventArgs e)
        {
            AddUrl?.Invoke(this, new UrlAddressInstanceEventArgs());
        }

        /// <summary>
        /// Remove button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteInstances_Click(object sender, EventArgs e)
        {
            DeleteUrl?.Invoke(this, new UrlAddressInstanceEventArgs(dlvUrls.SelectedObjects.Cast<UrlAddressMonitoringInstance>()));
        }

        /// <summary>
        /// Set and get data list view datasource
        /// </summary>
        public IEnumerable<UrlAddressMonitoringInstance> Urls
        {
            get { return (IEnumerable<UrlAddressMonitoringInstance>)dlvUrls.DataSource; }
            set
            {
                dlvUrls.DataSource = value;
            }
        }

        private void dlvUrls_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //Enable delete button if one or more items were checked.
            if (dlvUrls.SelectedItems.Count > 0)
                btnDeleteInstances.Enabled = true;
            else
                btnDeleteInstances.Enabled = false;
        }
    }

    public class UrlAddressInstanceEventArgs : EventArgs
    {
        public readonly IEnumerable<UrlAddressMonitoringInstance> Instances;
        public UrlAddressInstanceEventArgs(UrlAddressMonitoringInstance instance = null)
        {
            Instances = new UrlAddressMonitoringInstance[] { instance };
        }

        public UrlAddressInstanceEventArgs(IEnumerable<UrlAddressMonitoringInstance> instances)
        {
            Instances = instances;
        }
    }
}