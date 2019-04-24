using Microsoft.EnterpriseManagement.Monitoring;
using OpsLogix.IMP.Url.Shared.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace OpsLogix.IMP.Url.Shared
{
    public abstract class ScomMonitoringInstance
    {
        public MonitoringObject MonitoringObject { get; set; }
        public MonitoringObject ActionPoint { get; set; }

        public ScomMonitoringInstance(MonitoringObject monitoringObject = null, MonitoringObject actionPoint = null)
        {
            ActionPoint = actionPoint;
            MonitoringObject = monitoringObject;

            if (monitoringObject == null)
                return;
            
            //Map attribute Guid to actual property value
            foreach(var property in GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                var scomAttributeProperty = (ScomClassPropertyAttribute)property.GetCustomAttributes(typeof(ScomClassPropertyAttribute), true).FirstOrDefault();
                if (scomAttributeProperty == null) continue;

                object instanceValue = monitoringObject[scomAttributeProperty.PropertyId]?.Value;

                if (instanceValue == null) continue;

                property.SetValue(this, instanceValue);
            }
        }

        public object this[Guid propertyId]
        {
            get
            {
                object result = null;

                foreach (var property in GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    var scomAttributeProperty = (ScomClassPropertyAttribute)property.GetCustomAttributes(typeof(ScomClassPropertyAttribute), true).FirstOrDefault();
                    if (scomAttributeProperty == null || scomAttributeProperty.PropertyId != propertyId) continue;

                    result = property.GetValue(this);
                }

                return result;
            }
            set
            {
                //Map attribute Guid to actual property
                foreach (var property in GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    var scomAttributeProperty = (ScomClassPropertyAttribute)property.GetCustomAttributes(typeof(ScomClassPropertyAttribute), true).FirstOrDefault();
                    if (scomAttributeProperty == null || scomAttributeProperty.PropertyId != propertyId) continue;

                    property.SetValue(this, value);
                }
            }
        }
    }
}
