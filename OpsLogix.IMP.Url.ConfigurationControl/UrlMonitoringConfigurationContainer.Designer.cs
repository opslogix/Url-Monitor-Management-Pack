namespace OpsLogix.IMP.Url.ConfigurationControl
{
    partial class UrlMonitoringConfigurationContainer
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
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
            this.urlMonitoringConfigurationControl = new OpsLogix.IMP.Url.ConfigurationControl.Controls.UrlMonitoringConfigurationControl();
            this.tblLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.urlMonitoringConfigurationControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // tblLayout
            // 
            this.tblLayout.ColumnCount = 2;
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayout.Controls.Add(this.urlMonitoringConfigurationControl, 0, 1);
            this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayout.Location = new System.Drawing.Point(0, 0);
            this.tblLayout.Name = "tblLayout";
            this.tblLayout.RowCount = 2;
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.556962F));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95.44304F));
            this.tblLayout.Size = new System.Drawing.Size(801, 411);
            this.tblLayout.TabIndex = 0;
            // 
            // urlMonitoringConfigurationControl
            // 
            this.tblLayout.SetColumnSpan(this.urlMonitoringConfigurationControl, 2);
            this.urlMonitoringConfigurationControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlMonitoringConfigurationControl.HelpKey = "";
            this.urlMonitoringConfigurationControl.Location = new System.Drawing.Point(3, 21);
            this.urlMonitoringConfigurationControl.ManagementGroupSession = null;
            this.urlMonitoringConfigurationControl.Name = "urlMonitoringConfigurationControl";
            this.urlMonitoringConfigurationControl.Personalization = null;
            this.urlMonitoringConfigurationControl.PersonalizationKey = "";
            this.urlMonitoringConfigurationControl.Size = new System.Drawing.Size(795, 387);
            this.urlMonitoringConfigurationControl.TabIndex = 0;
            this.urlMonitoringConfigurationControl.Target = null;
            this.urlMonitoringConfigurationControl.Urls = null;
            // 
            // UrlMonitoringConfigurationContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblLayout);
            this.Name = "UrlMonitoringConfigurationContainer";
            this.Size = new System.Drawing.Size(801, 411);
            this.tblLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.urlMonitoringConfigurationControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLayout;
        private OpsLogix.IMP.Url.ConfigurationControl.Controls.UrlMonitoringConfigurationControl urlMonitoringConfigurationControl;
    }
}
