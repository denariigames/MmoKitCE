using System;

namespace Insthync.DevExtension
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DevExtMethodsAttribute : Attribute
    {
        public string BaseMethodName { get; private set; }
        public DevExtMethodsAttribute(string baseMethodName)
        {
            BaseMethodName = baseMethodName;
        }
    }
}
