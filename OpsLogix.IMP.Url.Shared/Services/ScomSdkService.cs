using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using System;

namespace OpsLogix.IMP.Url.Shared.Services
{
    public class ScomSdkService : IScomSdkService
    {
        public ManagementGroup ManagementGroup { get; }

        public ScomSdkService(ManagementGroup managementGroup)
        {
            ManagementGroup = managementGroup ?? throw new ArgumentNullException(nameof(managementGroup));
        }

        public ScomClassInstanceEditor<T> GetInstanceEditor<T>(Guid seedClassId, MonitoringConnector monitoringConnector = null) where T : ScomMonitoringInstance
        {
            return new ScomClassInstanceEditor<T>(ManagementGroup, seedClassId, monitoringConnector);
        }

        public ScomClassInstanceEditor<T> GetInstanceEditor<T>(Guid seedClassId, Guid actionPointClassId, MonitoringConnector monitoringConnector = null, ManagementPackRelationship hostingRelationship = null) where T : ScomMonitoringInstance
        {
            return new ScomClassInstanceEditor<T>(ManagementGroup, seedClassId, actionPointClassId, monitoringConnector);
        }
    }
}
