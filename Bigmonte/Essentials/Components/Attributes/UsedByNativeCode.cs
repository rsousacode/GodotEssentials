using System;

namespace CodexEngine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Enum |
                    AttributeTargets.Field |
                    AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property |
                    AttributeTargets.Struct, Inherited = false)]
    internal class UsedByNativeCodeAttribute : Attribute
    {
        //
        // Properties
        //

        //
        // Constructors
        //
        public UsedByNativeCodeAttribute()
        {
        }

        public UsedByNativeCodeAttribute(string name)
        {
        }
    }
}