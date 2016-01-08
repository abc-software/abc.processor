// ----------------------------------------------------------------------------
// <copyright file="ReflectorBase.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;
    using Abc.Processor.Triggers;

    /// <summary>
    /// Base class for clases with reflected properties.
    /// </summary>
    public abstract class ReflectorBase : Component {
        private IDictionary<string, string> attr;

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        protected IDictionary<string, string> Attributes {
            get {
                if (this.attr == null) {
                    this.attr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                return this.attr;
            }
        }

        internal void SetAttributes(IDictionary<string, string> attributes) {
            this.attr = attributes;
            this.VerifyAttributes(attributes);
        }

        internal void VerifyAttributes(IDictionary<string, string> attributes) {
            MemberInfo[] members = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.SetField | BindingFlags.Instance);
            if (members == null || members.Length == 0) {
                /*throw new ConfigurationErrorsException(ProcessorSR.Attribute_not_supportedFormat(str, this.GetType().FullName));*/
            }

            foreach (string attributeName in attributes.Keys) {
                bool flag = false;
                foreach (MemberInfo member in members) {
                    ReflectorPropertyAttribute reflectorAttribute = ReflectorPropertyAttribute.GetAttribute(member);
                    if (reflectorAttribute != null) {
                        if (string.Equals(reflectorAttribute.Name, attributeName, StringComparison.Ordinal)) {
                            flag = true;
                            this.SetMember(member, (string)attributes[attributeName]);
                        }
                    }
                }

                if (!flag) {
                    if (!this.OnReflectUnrecognizedAttribute(attributeName)) {
                        throw new ConfigurationErrorsException(SR.AttributeNotSupportedFormat(attributeName, this.GetType().FullName));
                    }
                }
            }
        }

        /// <summary>
        /// Called when reflect unrecognized attribute.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <returns><c>true</c> if unrecognized attribute reflected, otherwise <c>false</c>.</returns>
        protected virtual bool OnReflectUnrecognizedAttribute(string name) {
            return false;
        }

        private void SetMember(MemberInfo member, string value) {
            Type memberType = null;
            bool isProperty = false;
            object obj = null;

            PropertyInfo property = member as PropertyInfo;
            if (property != null) {
                memberType = property.PropertyType;
                isProperty = true;
            }

            FieldInfo field = member as FieldInfo;
            if (field != null) {
                memberType = field.FieldType;
            }

            if (memberType == null) {
                throw new InvalidOperationException(SR.NotSupportedMemberType);
            }

            if (memberType.IsAssignableFrom(typeof(Trigger))) {
                // sharedTrigger
                if (Abc.Processor.Configuration.ProcessorConfiguration.SharedTriggers == null) {
                    throw new ConfigurationErrorsException(SR.ReferenceToNonexistentTriggerFormat(value));
                }

                Abc.Processor.Configuration.TriggerElement element = Abc.Processor.Configuration.ProcessorConfiguration.SharedTriggers[value];
                if (element == null) {
                    throw new ConfigurationErrorsException(SR.ReferenceToNonexistentTriggerFormat(/*this.Name*/"name"));
                }

                obj = element.GetRuntimeObject();
            }
            else {
                TypeConverter converter = TypeDescriptor.GetConverter(memberType);
                if (!converter.CanConvertFrom(typeof(string))) {
                    throw new ConfigurationErrorsException(SR.ErrorConvertingAttributeFormat(memberType.Name, this.GetType().FullName));
                }

                try {
                    obj = converter.ConvertFromString(value);
                }
                catch (Exception innerException) {
                    throw new ConfigurationErrorsException(SR.ErrorConvertingAttributeFormat(memberType.Name, this.GetType().FullName), innerException);
                }
            }

            // Uzstādam vērtības
            if (isProperty) {
                property.SetValue(this, obj, new object[0]);
            }
            else {
                field.SetValue(this, obj);
            }
        }
    }
}
