using Microsoft.EnterpriseManagement.Configuration;
using System;
using System.Collections.Generic;

namespace OpsLogix.IMP.Url.Shared.Helpers
{
    public static class InstanceValidator
    {
        public static InstanceValidationResult Validate(this ScomMonitoringInstance instance, ManagementPackClass seedClass, ManagementPackClass actionPointClass = null)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (seedClass == null) throw new ArgumentNullException(nameof(seedClass));

            var errors = new Dictionary<string, string>();

            foreach (var classProperty in seedClass.PropertyCollection)
            {
                object instancePropertyValue = instance[classProperty.Id];
                if ((classProperty.Key || classProperty.Required) && instancePropertyValue == null)
                {
                    errors.Add(classProperty.Name, "Missing required property");
                    continue;
                }

                if (instancePropertyValue == null)
                    continue;

                //Validate constrictions
            }

            if ((seedClass.Hosted || actionPointClass != null) && instance.ActionPoint == null)
                errors.Add("ActionPoint", "Missing ActionPoint value");

            return new InstanceValidationResult(errors);
        }
    }

    public class InstanceValidationResult
    {
        public InstanceValidationResult(Dictionary<string,string> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));

            IsValid = Errors.Count > 0 ? false : true;
        }

        public bool IsValid { get; }
        public Dictionary<string, string> Errors { get; }
    }
}
