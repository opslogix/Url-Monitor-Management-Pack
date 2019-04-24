using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpsLogix.IMP.Url.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScomClassInstanceEditor<T> where T : ScomMonitoringInstance
    {
        private readonly Guid _managementActionPointClassId = new Guid("414bd649-ccf2-26a7-4171-e20694c802a4");
        private readonly Guid _managementActionPointShouldManageEntityRelationshipId = new Guid("cdb09107-2411-d9e2-d718-e574983d304d");
        private readonly Guid _managementActionPointManagesEntityRelationshipId = new Guid("cb72a458-d56e-3be8-950b-955b16f2f6a2");
        private readonly Guid _managementServicePoolClassId = new Guid("f5d4c6af-f7ff-57d6-011f-82713e64100d");
        private readonly Guid _healthServicePropertyId = new Guid("cac04922-bb71-8a38-2a97-d4f9f51d46c5");
        private readonly Guid _displayNamePropertyId = new Guid("afb4f9e6-bf48-1737-76ad-c9b3ec325b97");

        private readonly ManagementGroup _managementGroup;
        private readonly MonitoringConnector _monitoringConnector;
        private readonly ManagementPackClass _seedClass, _actionPointClass;
        private readonly List<MonitoringObject> _actionPoints = new List<MonitoringObject>();

        protected ManagementPackRelationship RelMapShouldManageEntity, RelMapManagesEntity, relHosting, relHsShouldManageEntity;
        protected ManagementPackRelationship RelToClearMap, RelToClearHs;

        /// <summary>
        /// This constructor is for unhosted objects managed by All Management Server resource pool.
        /// </summary>
        /// <param name="managementGroup"></param>
        /// <param name="seedClass"></param>
        /// <param name="insertConnector"></param>
        public ScomClassInstanceEditor(ManagementGroup managementGroup, ManagementPackClass seedClass, MonitoringConnector monitoringConnector = null)
        {
            _managementGroup = managementGroup ?? throw new ArgumentNullException(nameof(managementGroup));
            _seedClass = seedClass ?? throw new ArgumentNullException(nameof(seedClass));
            _monitoringConnector = monitoringConnector;

            RelToClearHs = _managementGroup.EntityTypes.GetRelationshipClass(SystemMonitoringRelationship.HealthServiceShouldManageEntity);
            RelToClearMap = _managementGroup.EntityTypes.GetRelationshipClass(_managementActionPointShouldManageEntityRelationshipId);
        }

        public ScomClassInstanceEditor(ManagementGroup managementGroup, Guid seedClassId,  MonitoringConnector monitoringConnector = null)
            : this(managementGroup, managementGroup.EntityTypes.GetClass(seedClassId), monitoringConnector)
        {
            if (seedClassId == null) throw new ArgumentNullException(nameof(seedClassId));
        }

        /// <summary>
        /// This constructor is for unhosted objects managed by specific instance of defined Action Point class OR for hosted objects, 
        /// which are hosted on defined Action Point class.
        /// </summary>
        /// <param name="managementGroup"></param>
        /// <param name="seedClass"></param>
        /// <param name="actionPointClassName"></param>
        /// <param name="monitoringConnector"></param>
        public ScomClassInstanceEditor(ManagementGroup managementGroup, ManagementPackClass seedClass, ManagementPackClass actionPointClass, MonitoringConnector monitoringConnector = null, ManagementPackRelationship hostingRelationship = null) 
            : this(managementGroup, seedClass, monitoringConnector)
        {
            _seedClass = seedClass ?? throw new ArgumentNullException(nameof(seedClass));
            _actionPointClass = actionPointClass ?? throw new ArgumentNullException(nameof(actionPointClass));

            if (seedClass.Hosted)
            {
                RelMapShouldManageEntity = null;
                RelMapManagesEntity = null;

                relHosting = hostingRelationship ?? managementGroup.EntityTypes.GetRelationshipClass(SystemRelationship.Hosting);
            }
            else
            {
                if (actionPointClass.IsSubtypeOf(managementGroup.EntityTypes.GetClass(_managementActionPointClassId)) || actionPointClass.Id == _managementActionPointClassId)
                    RelMapShouldManageEntity = managementGroup.EntityTypes.GetRelationshipClass(_managementActionPointShouldManageEntityRelationshipId);
                else if (actionPointClass.IsSubtypeOf(managementGroup.EntityTypes.GetClass(SystemMonitoringClass.HealthService)) || actionPointClass.Id == SystemMonitoringClass.HealthService.Id)
                    relHsShouldManageEntity = managementGroup.EntityTypes.GetRelationshipClass(SystemMonitoringRelationship.HealthServiceShouldManageEntity);
                else
                    throw new NotSupportedException("For unhosted scenario, action point class should be either Microsoft.SystemCenter.ManagementActionPoint or Microsoft.SystemCenter.HealthService, or inherited from these classes.");

                RelMapManagesEntity = managementGroup.EntityTypes.GetRelationshipClass(_managementActionPointManagesEntityRelationshipId);
            }

            _actionPoints.AddRange(managementGroup.EntityObjects.GetObjectReader<MonitoringObject>(_actionPointClass, ObjectQueryOptions.Default));
        }

        public ScomClassInstanceEditor(ManagementGroup managementGroup, Guid seedClassId, Guid actionPointClassId, MonitoringConnector monitoringConnector = null, ManagementPackRelationship hostingRelationship = null) 
            : this(managementGroup, managementGroup.EntityTypes.GetClass(seedClassId), managementGroup.EntityTypes.GetClass(actionPointClassId), monitoringConnector)
        {
        }

        public IEnumerable<T> GetInstances()
        {
            return _managementGroup.EntityObjects.GetObjectReader<MonitoringObject>(_seedClass, ObjectQueryOptions.Default).Select(x => (T)Activator.CreateInstance(typeof(T), new object[] { x, FindActionPoint(x) }));
        }

        public void ExecuteDiscovery(ScomDiscoveryType direction, IEnumerable<T> objects)
        {
            var discoveryDataIncremental = PrepareScomDiscoveryData(direction, objects);

            switch (direction)
            {
                case ScomDiscoveryType.Insert:
                    discoveryDataIncremental.Commit(_managementGroup);
                break;

                case ScomDiscoveryType.Update:
                    discoveryDataIncremental.Overwrite(_managementGroup);
                break;

                case ScomDiscoveryType.Delete:
                    discoveryDataIncremental.Commit(_managementGroup);
                break;
            }
        }

        protected virtual IncrementalDiscoveryData PrepareScomDiscoveryData(ScomDiscoveryType direction, IEnumerable<T> objects)
        {
            // create object for both incremental and full discovery
            var result = new IncrementalDiscoveryData();

            foreach (var @object in objects)
            {
                //Ignore
                if (direction != ScomDiscoveryType.Delete && !HasAllRequiredProperties(@object))
                    throw new Exception($"Invalid instance, missing required properties");

                var newSeedInstance = new CreatableEnterpriseManagementObject(_managementGroup, _seedClass);
                CreatableEnterpriseManagementRelationshipObject newSomethingShouldManageInstance = null;

                foreach (var classProperty in _seedClass.PropertyCollection)
                {
                    try
                    {
                        newSeedInstance[classProperty].Value = @object[classProperty.Id];
                    }
                    catch (KeyNotFoundException) // ignore situations, when non-key or non-required fields are not available
                    {
                        if (classProperty.Required || classProperty.Key)
                            throw;
                    }
                }

                // Set displayname
                try
                {
                    newSeedInstance[_displayNamePropertyId].Value = @object[_displayNamePropertyId];
                }
                catch (Exception ex)
                {

                }

                //Unhosted, with specific action point
                if (!_seedClass.Hosted && _actionPointClass != null && @object.ActionPoint != null)
                {
                    if (RelMapShouldManageEntity != null)
                    {
                        newSomethingShouldManageInstance = new CreatableEnterpriseManagementRelationshipObject(_managementGroup, RelMapShouldManageEntity);
                        newSomethingShouldManageInstance.SetTarget(newSeedInstance);
                        newSomethingShouldManageInstance.SetSource(@object.ActionPoint);
                    }

                    // sometimes when specific Health Service is deleted, relationship may revert to All Management Server Pool
                    if (relHsShouldManageEntity != null && !@object.ActionPoint.IsInstanceOf(_managementGroup.EntityTypes.GetClass(_managementServicePoolClassId)))
                    {
                        newSomethingShouldManageInstance = new CreatableEnterpriseManagementRelationshipObject(_managementGroup, relHsShouldManageEntity);
                        newSomethingShouldManageInstance.SetTarget(newSeedInstance);
                        newSomethingShouldManageInstance.SetSource(@object.ActionPoint);
                    }
                    if (RelMapShouldManageEntity == null && relHsShouldManageEntity == null)
                        throw new NotSupportedException("Scenario not supported.");
                }
                // Hosted, in this case myActionPointClass is the hosing class
                if (_seedClass.Hosted && _actionPointClass != null && @object.ActionPoint != null)
                {
                    //Doesn't break reference most likely, needs to be tested
                    var hostClass = _actionPointClass;

                    while (hostClass != null)
                    {
                        foreach (var hostProperty in hostClass.PropertyCollection)
                            if (hostProperty.Key)
                                newSeedInstance[hostProperty].Value = @object.ActionPoint[hostProperty].Value;

                        hostClass = hostClass.FindHostClass();
                    }
                }

                switch (direction)
                {
                    case ScomDiscoveryType.Insert:
                    case ScomDiscoveryType.Update:
                        result.Add(newSeedInstance);
                        if (newSomethingShouldManageInstance != null)
                            result.Add(newSomethingShouldManageInstance);
                        // don't need Hosted==True check, newSomethingShouldManageInstance will be null for hosted classes
                        if (newSomethingShouldManageInstance != null && direction == ScomDiscoveryType.Update)
                            PerformRelationshipCleanup(_seedClass, newSeedInstance, @object.ActionPoint);
                        //if (mySeedClass.Hosted) { }
                        break;
                    case ScomDiscoveryType.Delete:
                        result.Remove(newSeedInstance);
                        if (newSomethingShouldManageInstance != null)
                            result.Remove(newSomethingShouldManageInstance);
                    break;
                    default:
                        throw new Exception("Unknown direction, unable to prepare discovery");
                }
            }

            return result;
        }

        private void PerformRelationshipCleanup(ManagementPackClass seedClass, CreatableEnterpriseManagementObject seedInstance, MonitoringObject monitoringObject)
        {
            // it's so complex, because the new instance may not exist
            EnterpriseManagementObject realSeedInstance;

            string criteriaString = string.Empty;

            foreach (var property in seedInstance.GetProperties())
                if (property.Key)
                    if (criteriaString == string.Empty)
                        criteriaString = property.Name + "='" + seedInstance[property] + "'";
                    else
                        criteriaString += " AND " + property.Name + "='" + seedInstance[property] + "'";

            var searchQuery = new EnterpriseManagementObjectCriteria(criteriaString, seedClass);
            var realSeedInstanceList = _managementGroup.EntityObjects.GetObjectReader<EnterpriseManagementObject>(searchQuery, ObjectQueryOptions.Default);

            realSeedInstance = realSeedInstanceList.FirstOrDefault();

            if (realSeedInstance == null)
                return;

            var removalDiscovery = new IncrementalDiscoveryData();

            // Management Point
            bool commitOverwrite = false;
            var allMAPRelations = _managementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(realSeedInstance.Id, RelToClearMap, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted);
            var allHSRelations = _managementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(realSeedInstance.Id, RelToClearHs, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted);

            foreach (var rel in allMAPRelations)
            {
                if (rel.SourceObject.Id != monitoringObject.Id)
                {
                    // remove this relationship
                    removalDiscovery.Remove(rel);
                    commitOverwrite = true;
                }
            }

            foreach (var rel in allHSRelations)
            {
                if (rel.SourceObject.Id != monitoringObject.Id)
                {
                    // remove this relationship
                    removalDiscovery.Remove(rel);
                    commitOverwrite = true;
                }
            }

            if (commitOverwrite)
                removalDiscovery.Overwrite(_monitoringConnector);
        }

        private bool HasAllRequiredProperties(T record)
        {
            // test if all key fields have values
            bool allKeys = true;
            bool allRequired = true;

            foreach (var classProperty in _seedClass.PropertyCollection)
            {
                //This wont work correctly, don't use the monitoringobject
                object objectProperty = record[classProperty.Id];

                if (classProperty.Key && objectProperty == null)
                    allKeys = false;
                else if (classProperty.Required && objectProperty == null)
                    allRequired = false;
            }

            // in some cases Action Point must have value too
            if ((_seedClass.Hosted || _actionPointClass != null) && record.ActionPoint == null)
                allRequired = false;

            return allKeys & allRequired;
        }

        private MonitoringObject FindActionPoint(MonitoringObject instance)
        {
            if (_seedClass.Hosted)
            {
                // if it is hosted, then myActionPoints contains lists of all hosts, so instead of querying SCOM, let's search in this list
                int lastPathPortionIndex = instance.Path.LastIndexOf(';');
                MonitoringObject result = null;
                if (lastPathPortionIndex > 0) // ??
                {
                    string hostName = instance.Path.Substring(lastPathPortionIndex + 1);
                    string hostPath = instance.Path.Substring(0, lastPathPortionIndex);
                    result = _actionPoints.Where(x => x.Name == hostName && x.Path == hostPath).FirstOrDefault();
                }
                else
                    result = _actionPoints.Where(x => x.Name == instance.Path).FirstOrDefault();
                if (result != null)
                    return result;
                else if (relHosting != null)
                    // backup way if myActionPointClass defined incorrectly
                    return _managementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<MonitoringObject>(instance.Id, relHosting, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted).FirstOrDefault().SourceObject;
                else
                    return result;
            }
            else
            {
                if (RelMapManagesEntity == null && relHsShouldManageEntity == null)
                    return null; // don't try to find Action Point for truly unhosted class
                var allMAPs = instance.ManagementGroup.EntityObjects.GetRelationshipObjectsWhereTarget<MonitoringObject>(instance.Id,
                RelMapManagesEntity, DerivedClassTraversalDepth.Recursive, TraversalDepth.OneLevel, ObjectQueryOptions.Default).Where(x => !x.IsDeleted);
                if (allMAPs.Count() == 1)
                {
                    var draftResult = allMAPs.First().SourceObject;
                    if (draftResult.LeastDerivedNonAbstractManagementPackClassId == SystemMonitoringClass.ManagementService.Id)
                        return _managementGroup.EntityObjects.GetObject<MonitoringObject>((Guid)draftResult[_healthServicePropertyId].Value, ObjectQueryOptions.Default);
                    else
                        return draftResult;
                }
                else if (allMAPs.Count() == 0)
                    return null;
                else
                    throw new Exception("Unexpected result from GetRelationshipObjectsWhereTarget");
            }
        }
    }

    public enum ScomDiscoveryType { Insert, Update, Delete }
}
