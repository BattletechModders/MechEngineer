using System;

namespace CustomComponents
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAttribute : Attribute
    {
        public string CustomType { get; set; }

        public CustomAttribute(string customType)
        {
            CustomType = customType;
        }
    }
}
