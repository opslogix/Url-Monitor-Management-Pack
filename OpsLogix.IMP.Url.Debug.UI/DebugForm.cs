using OpsLogix.IMP.Url.ConfigurationControl;
using System.Windows.Forms;

namespace OpsLogix.IMP.Url.Debug.UI
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();

            var urlConfiguration = new UrlMonitoringConfigurationContainer()
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(urlConfiguration);
        }
    }
}
