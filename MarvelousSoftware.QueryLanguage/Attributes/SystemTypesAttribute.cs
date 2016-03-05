using System;
using System.Collections.Generic;
using System.Linq;

namespace MarvelousSoftware.QueryLanguage.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class SystemTypesAttribute : Attribute
    {
        public List<Type> Types { get; private set; }

        public SystemTypesAttribute(params Type[] types)
        {
            Types = types.ToList();
        }
    }
}