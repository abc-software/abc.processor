// ----------------------------------------------------------------------------
// <copyright file="ReflectorPropertyAttribute.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ReflectorPropertyAttribute : Attribute {
        private string _name;
        private bool _required;

        public ReflectorPropertyAttribute(string name) {
            _name = name;
        }

        public string Name {
            get { return _name; }
        }

        public bool Required {
            get { return _required; }
            set { _required = value; }
        }

        public static ReflectorPropertyAttribute GetAttribute(MemberInfo member) {
            if (member == null) {
                throw new ArgumentNullException("member"); 
            }

            object[] customAttributes = member.GetCustomAttributes(typeof(ReflectorPropertyAttribute), false);
            if (customAttributes.Length != 0) {
                return (ReflectorPropertyAttribute)customAttributes[0];
            }

            return null;
        }
    }
}
