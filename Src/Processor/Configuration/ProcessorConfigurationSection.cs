// ----------------------------------------------------------------------------
// <copyright file="ProcessorConfigurationSection.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Configuration {
    using System.Configuration;

    /// <summary>
    /// Processor configuration section class.
    /// </summary>
    internal class ProcessorConfigurationSection : ConfigurationSection {
        private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propProcessors = new ConfigurationProperty("processors", typeof(ProcessorElementsCollection), new ProcessorElementsCollection(), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSharedTriggers = new ConfigurationProperty("sharedTriggers", typeof(TriggerElementsCollection), new TriggerElementsCollection(), ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes static members of the <see cref="ProcessorConfigurationSection"/> class.
        /// </summary>
        static ProcessorConfigurationSection() {
            _properties.Add(_propProcessors);
            _properties.Add(_propSharedTriggers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorConfigurationSection"/> class.
        /// </summary>
        public ProcessorConfigurationSection() {
        }

        /// <summary>
        /// Gets the processor elements collection.
        /// </summary>
        /// <value>The processor elements collection.</value>
        [ConfigurationProperty("processors")]
        public ProcessorElementsCollection Processors {
            get {
                return (ProcessorElementsCollection)base[_propProcessors];
            }
        }

        /// <summary>
        /// Gets the shared triggers.
        /// </summary>
        /// <value>The shared triggers.</value>
        [ConfigurationProperty("sharedTriggers")]
        public TriggerElementsCollection SharedTriggers {
            get {
                return (TriggerElementsCollection)base[_propSharedTriggers];
            }
        }

        /// <summary>
        /// Gets the runtime object.
        /// </summary>
        /// <returns>The processor collection runtime object <see cref="ProcessorCollection"/>.</returns>
        public new ProcessorCollection GetRuntimeObject() {
            ProcessorCollection processors = new ProcessorCollection();
            
            // bool flag = false;
            foreach (ProcessorElement element in this.Processors) {
                /*
                //if (!flag && !element._isAddedByDefault) {
                //    new System.Security.Permissions.SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                //    flag = true;
                //}
                 */
 
                processors.Add(element.GetRuntimeObject());
            }

            return processors;
        }

        /// <summary>
        /// Used to initialize a default set of values for the <see cref="T:System.Configuration.ConfigurationElement"/> object.
        /// </summary>
        protected override void InitializeDefault() {
            // this.Processors.InitializeDefaultInternal();  
        }
    }
}
