using Microsoft.EnterpriseManagement;
using OpsLogix.IMP.Url.ConfigurationControl.Models;
using OpsLogix.IMP.Url.Modules.ProbeActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpsLogix.IMP.Url.Debug
{
    public class Program
    {
        static void Main(string[] args)
        {
            new OpsLogix.IMP.Url.ConfigurationControl.UrlMonitoringConfigurationContainer("tstezalert01.contoso.com");
            Console.ReadLine();
        }
    }
}
