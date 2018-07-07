using System;

namespace CustomComponents
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomComponentAttribute : Attribute
    {
        public string Name { get; set; }

        public CustomComponentAttribute(string name)
        {
            Name = name;
        }
    }
}
