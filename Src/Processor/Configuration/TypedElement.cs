// ----------------------------------------------------------------------------
// <copyright file="TypedElement.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Configuration {
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;

    internal class TypedElement : ConfigurationElement {
        protected static readonly ConfigurationProperty _propTypeName = new ConfigurationProperty("type", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);
        protected ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        protected object _runtimeObject;
        private Type _baseType;

        public TypedElement(Type baseType) {
            this._baseType = baseType;
            this._properties.Add(_propTypeName);
        }

        [ConfigurationProperty("type", IsRequired = true, DefaultValue = "")]
        public virtual string TypeName {
            get { return (string)base[_propTypeName]; }
            set { base[_propTypeName] = value; }
        }

        protected override ConfigurationPropertyCollection Properties {
            get {
                return this._properties;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception rethrowed")]
        internal static object GetRuntimeObject(string className, Type baseType, string initializeData) {
            if (className.Length == 0) {
                throw new ConfigurationErrorsException(SR.EmptyTypeNameNotAllowed);
            }

            Type c = Type.GetType(className);
            if (c == null) {
                throw new ConfigurationErrorsException(SR.CouldNotFindTypeFormat(className));
            }

            if (!baseType.IsAssignableFrom(c)) {
                throw new ConfigurationErrorsException(SR.IncorrectBaseTypeFormat(className, baseType.FullName));
            }

            Exception innerException = null;
            try {
                if (string.IsNullOrEmpty(initializeData)) {
                    /*
                    //if (IsOwnedTextWriterTL(c)) {
                    //    throw new ConfigurationErrorsException(SR.GetString("TextWriterTL_DefaultConstructor_NotSupported"));
                    //}
                     */

                    ConstructorInfo constructor = c.GetConstructor(new Type[0]);
                    if (constructor == null) {
                        throw new ConfigurationErrorsException(SR.CouldNotGetConstructorFormat(className));
                    }

                    return constructor.Invoke(new object[0]);
                }
                else {
                    ConstructorInfo constructor = c.GetConstructor(new Type[] { typeof(string) });
                    if (constructor != null) {
                        /*
                        //if ((IsOwnedTextWriterTL(c) && (initializeData[0] != Path.DirectorySeparatorChar)) && ((initializeData[0] != Path.AltDirectorySeparatorChar) && !Path.IsPathRooted(initializeData))) {
                        //    string configFilePath = DiagnosticsConfiguration.ConfigFilePath;
                        //    if (!string.IsNullOrEmpty(configFilePath)) {
                        //        string directoryName = Path.GetDirectoryName(configFilePath);
                        //        if (directoryName != null) {
                        //            initializeData = Path.Combine(directoryName, initializeData);
                        //        }
                        //    }
                        //}
                         */

                        return constructor.Invoke(new object[] { initializeData });
                    }
                    else {
                        ConstructorInfo[] constructors = c.GetConstructors();
                        if (constructors == null) {
                            throw new ConfigurationErrorsException(SR.CouldNotGetConstructorFormat(className));
                        }

                        for (int i = 0; i < constructors.Length; i++) {
                            ParameterInfo[] parameters = constructors[i].GetParameters();
                            if (parameters.Length == 1) {
                                Type parameterType = parameters[0].ParameterType;
                                try {
                                    object parameter = ConvertToBaseTypeOrEnum(initializeData, parameterType);
                                    return constructors[i].Invoke(new object[] { parameter });
                                }
                                catch (TargetInvocationException targetInvocationException) {
                                    innerException = targetInvocationException.InnerException;
                                }
                                catch (Exception exception) {
                                    innerException = exception;
                                }
                            }
                        }
                    }
                }
            }
            catch (TargetInvocationException targetInvocationException) {
                innerException = targetInvocationException.InnerException;
            }

            if (innerException != null) {
                throw new ConfigurationErrorsException(SR.CouldNotGetConstructorFormat(className), innerException);
            }

            throw new ConfigurationErrorsException(SR.CouldNotGetConstructorFormat(className));
        }

        internal virtual void ResetProperties() {
            this._properties.Clear();
            this._properties.Add(_propTypeName);
        }

        protected object BaseGetRuntimeObject() {
            if (this._runtimeObject == null) {
                this._runtimeObject = GetRuntimeObject(this.TypeName, this._baseType, null);
            }

            return this._runtimeObject;
        }

        private static object ConvertToBaseTypeOrEnum(string value, Type type) {
            if (type.IsEnum) {
                return Enum.Parse(type, value, false);
            }

            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }
    }
}
