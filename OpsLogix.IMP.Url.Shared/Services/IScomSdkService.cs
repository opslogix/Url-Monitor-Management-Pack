﻿using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using System;

namespace OpsLogix.IMP.Url.Shared.Services
{
    public interface IScomSdkService
    {
        ManagementGroup ManagementGroup { get; }
        ScomClassInstanceEditor<T> GetInstanceEditor<T>(Guid seedClassId, MonitoringConnector monitoringConnector = null) where T : ScomMonitoringInstance;
        ScomClassInstanceEditor<T> GetInstanceEditor<T>(ManagementPackClass seedClass, MonitoringConnector monitoringConnector = null) where T : ScomMonitoringInstance;

        ScomClassInstanceEditor<T> GetInstanceEditor<T>(Guid seedClassId, Guid actionPointClassId, MonitoringConnector monitoringConnector = null, ManagementPackRelationship hostingRelationship = null) where T : ScomMonitoringInstance;
        ScomClassInstanceEditor<T> GetInstanceEditor<T>(ManagementPackClass seedClass, ManagementPackClass actionPointClass, MonitoringConnector monitoringConnector = null, ManagementPackRelationship hostingRelationship = null) where T : ScomMonitoringInstance;

    }
}
