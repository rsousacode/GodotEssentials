using System;

namespace Bigmonte.Entities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Enum |
                    AttributeTargets.Field |
                    AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property |
                    AttributeTargets.Struct, Inherited = false)]
    internal class RequiredByNativeCodeAttribute : Attribute
    {
        //
        // Constructors
        //
        public RequiredByNativeCodeAttribute()
        {
        }

        public RequiredByNativeCodeAttribute(string name)
        {
            Name = name;
        }

        public RequiredByNativeCodeAttribute(bool optional)
        {
            Optional = optional;
        }

        public RequiredByNativeCodeAttribute(string name, bool optional)
        {
            Name = name;
            Optional = optional;
        }

        //
        // Properties
        //
        public string Name { get; set; }

        public bool Optional { get; set; }
    }
}