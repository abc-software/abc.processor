// ----------------------------------------------------------------------------
// <copyright file="ProcessorConfiguration.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Configuration {
    using System.Configuration;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Processor configuration class.
    /// </summary>
    internal static class ProcessorConfiguration {
        private const string ProcessorSectionName = "abc.processor";
        private static ProcessorConfigurationSection _configSection;
        private static bool _inicialized;

        /// <summary>
        /// Gets the processors.
        /// </summary>
        /// <value>The processors.</value>
        public static ProcessorElementsCollection Processors {
            get {
                Initialize();
                if (_configSection != null) {
                    return _configSection.Processors;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the shared triggers.
        /// </summary>
        /// <value>The shared triggers.</value>
        public static TriggerElementsCollection SharedTriggers {
            get {
                Initialize();
                if (_configSection != null) {
                    return _configSection.SharedTriggers;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the processor configuration section.
        /// </summary>
        /// <value>The processor configuration section.</value>
        public static ProcessorConfigurationSection ProcessorConfigurationSection {
            get {
                Initialize();
                return _configSection;
            }
        }

        internal static void Initialize() {
            try {
                if (!_inicialized) {
                    _configSection = (ProcessorConfigurationSection)GetSection(ProcessorSectionName, typeof(ProcessorConfigurationSection), true);
                }
            }
            finally {
                _inicialized = true;
            }
        }

        internal static void Refresh() {
            ConfigurationManager.RefreshSection(ProcessorSectionName);

            if (_configSection != null) {
                if (_configSection.Processors != null) {
                    foreach (ProcessorElement element in _configSection.Processors) {
                        element.ResetProperties();
                    }
                }

                if (_configSection.SharedTriggers != null) {
                    foreach (TriggerElement element in _configSection.SharedTriggers) {
                        element.ResetProperties();
                    }
                }
            }

            _configSection = null;
            _inicialized = false;
            Initialize();
        }

        private static object GetSection(string sectionName, System.Type type, bool permitNull) {
            object sectionObject = null;

            // Check to see if we are running on the server without loading system.web.dll
            if (Thread.GetDomain().GetData(".appDomain") != null) {
                HttpContext context = HttpContext.Current;
                if (context != null) {
                    sectionObject = context.GetSection(sectionName);
                }
            }

            if (sectionObject == null) {
                sectionObject = ConfigurationManager.GetSection(sectionName);
            }

            if (sectionObject == null) {
                if (!permitNull) {
                    throw new ConfigurationErrorsException(SR.ConfigurationSectionErrorFormat(sectionName));
                }

                if (type != null) {
                    sectionObject = type.GetConstructor(new System.Type[0]).Invoke(new object[0]);
                }
            }
            else if (type != null && !type.IsAssignableFrom(sectionObject.GetType())) {
                throw new ConfigurationErrorsException(SR.ConfigurationSectionErrorFormat(sectionName));
            }

            return sectionObject;
        }
    }
}