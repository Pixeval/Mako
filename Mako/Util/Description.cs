using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Mako.Util
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Delegate | AttributeTargets.Property)]
    [PublicAPI]
    public class Description : Attribute
    {
        public string Name { get; }

        public Description(string name)
        {
            Name = name;
        }
    }

    [PublicAPI]
    public static class DescriptionHelper
    {
        public static Description GetDescription<TEnum>(this TEnum @enum) where TEnum : Enum
        {
            return (Description) (typeof(TEnum).GetField(@enum.ToString())?.GetCustomAttribute(typeof(Description)) ?? throw new InvalidOperationException("Attribute not found"));
        }
    }
}