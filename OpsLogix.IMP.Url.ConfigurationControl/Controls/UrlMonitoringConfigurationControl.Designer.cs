namespace OpsLogix.IMP.Url.ConfigurationControl.Controls
{
    partial class UrlMonitoringConfigurationControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tblLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dlvUrls = new BrightIdeasSoftware.DataListView();
            this.olvcDisplayName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcDescription = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcAddress = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcMonitoringAgent = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcProxyAddress = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcDnsServers = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcDnsServersTimeout = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvcConfigure = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pControls = new System.Windows.Forms.Panel();
            this.btnDeleteInstances = new System.Windows.Forms.Button();
            this.btnAddUrl = new System.Windows.Forms.Button();
            this.tblLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dlvUrls)).BeginInit();
            this.pControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // tblLayoutPanel
            // 
            this.tblLayoutPanel.ColumnCount = 2;
            this.tblLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutPanel.Controls.Add(this.dlvUrls, 0, 0);
            this.tblLayoutPanel.Controls.Add(this.pControls, 0, 1);
            this.tblLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tblLayoutPanel.Name = "tblLayoutPanel";
            this.tblLayoutPanel.RowCount = 2;
            this.tblLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tblLayoutPanel.Size = new System.Drawing.Size(1024, 434);
            this.tblLayoutPanel.TabIndex = 4;
            // 
            // dlvUrls
            // 
            this.dlvUrls.AllColumns.Add(this.olvcDisplayName);
            this.dlvUrls.AllColumns.Add(this.olvcDescription);
            this.dlvUrls.AllColumns.Add(this.olvcAddress);
            this.dlvUrls.AllColumns.Add(this.olvcMonitoringAgent);
            this.dlvUrls.AllColumns.Add(this.olvcProxyAddress);
            this.dlvUrls.AllColumns.Add(this.olvcDnsServers);
            this.dlvUrls.AllColumns.Add(this.olvcDnsServersTimeout);
            this.dlvUrls.AllColumns.Add(this.olvcConfigure);
            this.dlvUrls.AutoGenerateColumns = false;
            this.dlvUrls.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dlvUrls.CellEditUseWholeCell = false;
            this.dlvUrls.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvcDisplayName,
            this.olvcDescription,
            this.olvcAddress,
            this.olvcMonitoringAgent,
            this.olvcProxyAddress,
            this.olvcDnsServers,
            this.olvcDnsServersTimeout,
            this.olvcConfigure});
            this.tblLayoutPanel.SetColumnSpan(this.dlvUrls, 2);
            this.dlvUrls.Cursor = System.Windows.Forms.Cursors.Default;
            this.dlvUrls.DataSource = null;
            this.dlvUrls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dlvUrls.FullRowSelect = true;
            this.dlvUrls.GridLines = true;
            this.dlvUrls.HideSelection = false;
            this.dlvUrls.Location = new System.Drawing.Point(3, 3);
            this.dlvUrls.Name = "dlvUrls";
            this.dlvUrls.ShowGroups = false;
            this.dlvUrls.Size = new System.Drawing.Size(1018, 393);
            this.dlvUrls.TabIndex = 4;
            this.dlvUrls.UseCompatibleStateImageBehavior = false;
            this.dlvUrls.View = System.Windows.Forms.View.Details;
            this.dlvUrls.ButtonClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.dlvUrls_ButtonClick);
            this.dlvUrls.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.dlvUrls_ItemSelectionChanged);
            this.dlvUrls.DoubleClick += new System.EventHandler(this.DlvUrls_DoubleClick);
            // 
            // olvcDisplayName
            // 
            this.olvcDisplayName.AspectName = "DisplayName";
            this.olvcDisplayName.Text = "Display name";
            this.olvcDisplayName.Width = 132;
            // 
            // olvcDescription
            // 
            this.olvcDescription.AspectName = "Description";
            this.olvcDescription.Text = "Description";
            this.olvcDescription.Width = 159;
            // 
            // olvcAddress
            // 
            this.olvcAddress.AspectName = "Address";
            this.olvcAddress.Text = "Address";
            this.olvcAddress.Width = 122;
            // 
            // olvcMonitoringAgent
            // 
            this.olvcMonitoringAgent.AspectName = "ActingAgent";
            this.olvcMonitoringAgent.IsEditable = false;
            this.olvcMonitoringAgent.Text = "Monitoring agent";
            this.olvcMonitoringAgent.Width = 269;
            // 
            // olvcProxyAddress
            // 
            this.olvcProxyAddress.AspectName = "ProxyAddress";
            this.olvcProxyAddress.Text = "Proxy";
            // 
            // olvcDnsServers
            // 
            this.olvcDnsServers.AspectName = "DnsServers";
            this.olvcDnsServers.Text = "DNS servers";
            this.olvcDnsServers.Width = 133;
            // 
            // olvcDnsServersTimeout
            // 
            this.olvcDnsServersTimeout.AspectName = "DnsServersTimeout";
            this.olvcDnsServersTimeout.Text = "DNS Timeout";
            this.olvcDnsServersTimeout.Width = 93;
            // 
            // olvcConfigure
            // 
            this.olvcConfigure.AspectName = "Address";
            this.olvcConfigure.AspectToStringFormat = "...";
            this.olvcConfigure.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.olvcConfigure.IsButton = true;
            this.olvcConfigure.IsEditable = false;
            this.olvcConfigure.MaximumWidth = 35;
            this.olvcConfigure.MinimumWidth = 35;
            this.olvcConfigure.Sortable = false;
            this.olvcConfigure.Text = "...";
            this.olvcConfigure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvcConfigure.Width = 35;
            // 
            // pControls
            // 
            this.tblLayoutPanel.SetColumnSpan(this.pControls, 2);
            this.pControls.Controls.Add(this.btnDeleteInstances);
            this.pControls.Controls.Add(this.btnAddUrl);
            this.pControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pControls.Location = new System.Drawing.Point(3, 402);
            this.pControls.Name = "pControls";
            this.pControls.Size = new System.Drawing.Size(1018, 29);
            this.pControls.TabIndex = 6;
            // 
            // btnDeleteInstances
            // 
            this.btnDeleteInstances.Enabled = false;
            this.btnDeleteInstances.Location = new System.Drawing.Point(84, 3);
            this.btnDeleteInstances.Name = "btnDeleteInstances";
            this.btnDeleteInstances.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteInstances.TabIndex = 8;
            this.btnDeleteInstances.Text = "Remove";
            this.btnDeleteInstances.UseVisualStyleBackColor = true;
            this.btnDeleteInstances.Click += new System.EventHandler(this.btnDeleteInstances_Click);
            // 
            // btnAddUrl
            // 
            this.btnAddUrl.Location = new System.Drawing.Point(3, 3);
            this.btnAddUrl.Name = "btnAddUrl";
            this.btnAddUrl.Size = new System.Drawing.Size(75, 23);
            this.btnAddUrl.TabIndex = 6;
            this.btnAddUrl.Text = "Add";
            this.btnAddUrl.UseVisualStyleBackColor = true;
            this.btnAddUrl.Click += new System.EventHandler(this.btnAddUrl_Click);
            // 
            // UrlMonitoringConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblLayoutPanel);
            this.Name = "UrlMonitoringConfigurationControl";
            this.Size = new System.Drawing.Size(1024, 434);
            this.tblLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dlvUrls)).EndInit();
            this.pControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLayoutPanel;
        private BrightIdeasSoftware.DataListView dlvUrls;
        private BrightIdeasSoftware.OLVColumn olvcDisplayName;
        private BrightIdeasSoftware.OLVColumn olvcAddress;
        private BrightIdeasSoftware.OLVColumn olvcDescription;
        private BrightIdeasSoftware.OLVColumn olvcMonitoringAgent;
        private BrightIdeasSoftware.OLVColumn olvcConfigure;
        private System.Windows.Forms.Panel pControls;
        private System.Windows.Forms.Button btnAddUrl;
        private System.Windows.Forms.Button btnDeleteInstances;
        private BrightIdeasSoftware.OLVColumn olvcProxyAddress;
        private BrightIdeasSoftware.OLVColumn olvcDnsServers;
        private BrightIdeasSoftware.OLVColumn olvcDnsServersTimeout;
    }
}
