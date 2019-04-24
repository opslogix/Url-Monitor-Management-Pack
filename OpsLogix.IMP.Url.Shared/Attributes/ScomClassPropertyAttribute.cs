using System;

namespace OpsLogix.IMP.Url.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ScomClassPropertyAttribute : Attribute
    {
        public ScomClassPropertyAttribute(string propertyId) => PropertyId = Guid.Parse(propertyId);
        public Guid PropertyId { get; }
    }
}
