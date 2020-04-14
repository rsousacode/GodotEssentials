using System;

namespace Bigmonte.Entities
{
    [AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Parameter)]
    [Serializable]
    public class DefaultValueAttribute : Attribute
    {
        //
        // Constructors
        //
        public DefaultValueAttribute(string value)
        {
            Value = value;
        }
        //
        // Fields
        //

        //
        // Properties
        //
        private object Value { get; }

        //
        // Methods
        //
        public override bool Equals(object obj)
        {
            var defaultValueAttribute = obj as DefaultValueAttribute;
            bool result;
            if (defaultValueAttribute == null)
                result = false;
            else if (Value == null)
                result = defaultValueAttribute.Value == null;
            else
                result = Value.Equals(defaultValueAttribute.Value);

            return result;
        }

        public override int GetHashCode()
        {
            int hashCode;
            if (Value == null)
                hashCode = base.GetHashCode();
            else
                hashCode = Value.GetHashCode();

            return hashCode;
        }
    }
}