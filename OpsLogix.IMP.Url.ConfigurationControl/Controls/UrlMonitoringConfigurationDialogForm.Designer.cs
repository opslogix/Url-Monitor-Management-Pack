namespace OpsLogix.IMP.Url.ConfigurationControl.Controls
{
  partial class UrlMonitoringConfigurationDialogForm
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.lblDnsServerList = new System.Windows.Forms.Label();
            this.lblQueryTimeout = new System.Windows.Forms.Label();
            this.lblProxy = new System.Windows.Forms.Label();
            this.tbCustomDNSServers = new System.Windows.Forms.TextBox();
            this.tbProxy = new System.Windows.Forms.TextBox();
            this.nudDNSTimeout = new System.Windows.Forms.NumericUpDown();
            this.configurationPanel = new System.Windows.Forms.Panel();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.lblActionPoint = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.tbDisplayName = new System.Windows.Forms.TextBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.cmbActionPoint = new System.Windows.Forms.ComboBox();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.gbExtraSettings = new System.Windows.Forms.GroupBox();
            this.btOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudDNSTimeout)).BeginInit();
            this.configurationPanel.SuspendLayout();
            this.gbSettings.SuspendLayout();
            this.gbExtraSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDnsServerList
            // 
            this.lblDnsServerList.AutoSize = true;
            this.lblDnsServerList.Location = new System.Drawing.Point(35, 71);
            this.lblDnsServerList.Name = "lblDnsServerList";
            this.lblDnsServerList.Size = new System.Drawing.Size(102, 13);
            this.lblDnsServerList.TabIndex = 1;
            this.lblDnsServerList.Text = "Custom DNS Server";
            // 
            // lblQueryTimeout
            // 
            this.lblQueryTimeout.AutoSize = true;
            this.lblQueryTimeout.Location = new System.Drawing.Point(6, 19);
            this.lblQueryTimeout.Name = "lblQueryTimeout";
            this.lblQueryTimeout.Size = new System.Drawing.Size(131, 13);
            this.lblQueryTimeout.TabIndex = 1;
            this.lblQueryTimeout.Text = "DNS Query Timeout (sec):";
            // 
            // lblProxy
            // 
            this.lblProxy.AutoSize = true;
            this.lblProxy.Location = new System.Drawing.Point(50, 45);
            this.lblProxy.Name = "lblProxy";
            this.lblProxy.Size = new System.Drawing.Size(87, 13);
            this.lblProxy.TabIndex = 0;
            this.lblProxy.Text = "Proxy (fqdn:port):";
            // 
            // tbCustomDNSServers
            // 
            this.tbCustomDNSServers.Location = new System.Drawing.Point(143, 68);
            this.tbCustomDNSServers.Name = "tbCustomDNSServers";
            this.tbCustomDNSServers.Size = new System.Drawing.Size(323, 20);
            this.tbCustomDNSServers.TabIndex = 5;
            // 
            // tbProxy
            // 
            this.tbProxy.Location = new System.Drawing.Point(143, 42);
            this.tbProxy.Name = "tbProxy";
            this.tbProxy.Size = new System.Drawing.Size(323, 20);
            this.tbProxy.TabIndex = 5;
            // 
            // nudDNSTimeout
            // 
            this.nudDNSTimeout.Location = new System.Drawing.Point(143, 16);
            this.nudDNSTimeout.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.nudDNSTimeout.Name = "nudDNSTimeout";
            this.nudDNSTimeout.Size = new System.Drawing.Size(54, 20);
            this.nudDNSTimeout.TabIndex = 4;
            this.nudDNSTimeout.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // configurationPanel
            // 
            this.configurationPanel.Controls.Add(this.gbSettings);
            this.configurationPanel.Controls.Add(this.gbExtraSettings);
            this.configurationPanel.Controls.Add(this.btOk);
            this.configurationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationPanel.Location = new System.Drawing.Point(0, 0);
            this.configurationPanel.Name = "configurationPanel";
            this.configurationPanel.Size = new System.Drawing.Size(495, 256);
            this.configurationPanel.TabIndex = 0;
            // 
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.lblDisplayName);
            this.gbSettings.Controls.Add(this.lblActionPoint);
            this.gbSettings.Controls.Add(this.lblDescription);
            this.gbSettings.Controls.Add(this.lblAddress);
            this.gbSettings.Controls.Add(this.tbDisplayName);
            this.gbSettings.Controls.Add(this.tbDescription);
            this.gbSettings.Controls.Add(this.cmbActionPoint);
            this.gbSettings.Controls.Add(this.tbAddress);
            this.gbSettings.Location = new System.Drawing.Point(12, 6);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Size = new System.Drawing.Size(472, 103);
            this.gbSettings.TabIndex = 1;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Configuration";
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.Location = new System.Drawing.Point(18, 25);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(75, 13);
            this.lblDisplayName.TabIndex = 0;
            this.lblDisplayName.Text = "Display Name:";
            // 
            // lblActionPoint
            // 
            this.lblActionPoint.AutoSize = true;
            this.lblActionPoint.Location = new System.Drawing.Point(3, 79);
            this.lblActionPoint.Name = "lblActionPoint";
            this.lblActionPoint.Size = new System.Drawing.Size(90, 13);
            this.lblActionPoint.TabIndex = 1;
            this.lblActionPoint.Text = "Monitoring Agent:";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(257, 33);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description:";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(45, 52);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(48, 13);
            this.lblAddress.TabIndex = 2;
            this.lblAddress.Text = "Address:";
            // 
            // tbDisplayName
            // 
            this.tbDisplayName.Location = new System.Drawing.Point(99, 21);
            this.tbDisplayName.Name = "tbDisplayName";
            this.tbDisplayName.Size = new System.Drawing.Size(152, 20);
            this.tbDisplayName.TabIndex = 0;
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(257, 49);
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(209, 48);
            this.tbDescription.TabIndex = 3;
            // 
            // cmbActionPoint
            // 
            this.cmbActionPoint.DisplayMember = "DisplayName";
            this.cmbActionPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActionPoint.FormattingEnabled = true;
            this.cmbActionPoint.Location = new System.Drawing.Point(99, 76);
            this.cmbActionPoint.Name = "cmbActionPoint";
            this.cmbActionPoint.Size = new System.Drawing.Size(152, 21);
            this.cmbActionPoint.TabIndex = 2;
            this.cmbActionPoint.ValueMember = "DisplayName";
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(99, 49);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(152, 20);
            this.tbAddress.TabIndex = 1;
            // 
            // gbExtraSettings
            // 
            this.gbExtraSettings.Controls.Add(this.lblProxy);
            this.gbExtraSettings.Controls.Add(this.lblQueryTimeout);
            this.gbExtraSettings.Controls.Add(this.nudDNSTimeout);
            this.gbExtraSettings.Controls.Add(this.tbCustomDNSServers);
            this.gbExtraSettings.Controls.Add(this.lblDnsServerList);
            this.gbExtraSettings.Controls.Add(this.tbProxy);
            this.gbExtraSettings.Location = new System.Drawing.Point(12, 125);
            this.gbExtraSettings.Name = "gbExtraSettings";
            this.gbExtraSettings.Size = new System.Drawing.Size(472, 96);
            this.gbExtraSettings.TabIndex = 1;
            this.gbExtraSettings.TabStop = false;
            this.gbExtraSettings.Text = "Advanced configuration";
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(384, 227);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(99, 23);
            this.btOk.TabIndex = 2;
            this.btOk.Text = "Ok";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // UrlMonitoringConfigurationDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 256);
            this.Controls.Add(this.configurationPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "UrlMonitoringConfigurationDialogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.nudDNSTimeout)).EndInit();
            this.configurationPanel.ResumeLayout(false);
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            this.gbExtraSettings.ResumeLayout(false);
            this.gbExtraSettings.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblDnsServerList;
    private System.Windows.Forms.Label lblQueryTimeout;
    private System.Windows.Forms.Label lblProxy;
    private System.Windows.Forms.TextBox tbCustomDNSServers;
    private System.Windows.Forms.TextBox tbProxy;
    private System.Windows.Forms.NumericUpDown nudDNSTimeout;
        private System.Windows.Forms.Panel configurationPanel;
        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.ComboBox cmbActionPoint;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.GroupBox gbExtraSettings;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox tbDisplayName;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.Label lblActionPoint;
    }
}