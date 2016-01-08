// ----------------------------------------------------------------------------
// <copyright file="ProcessorElement.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Configuration {
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Processor element class.
    /// </summary>
    internal class ProcessorElement : TypedElement {
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, null, new StringValidator(1), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propTrigger = new ConfigurationProperty("trigger", typeof(TriggerElement), null, ConfigurationPropertyOptions.None);
        private IDictionary<string, string> _propertyNameCollection = null;

        /* internal bool _isAddedByDefault; */

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorElement"/> class.
        /// </summary>
        public ProcessorElement()
            : base(typeof(IProcessor)) {
            this._properties.Add(_propName);
            this._properties.Add(_propTrigger);
        }

        /// <summary>
        /// Gets or sets the processor name.
        /// </summary>
        /// <value>The processor name.</value>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name {
            get {
                return (string)base[_propName];
            }

            set {
                base[_propName] = value;
            }
        }

        /// <summary>
        /// Gets the trigger element.
        /// </summary>
        /// <value>The trigger element.</value>
        [ConfigurationProperty("trigger")]
        public TriggerElement Trigger {
            get {
                return (TriggerElement)base[_propTrigger];
            }
        }

        /// <summary>
        /// Gets a collection of user-defined parameters for the processor.
        /// </summary>
        /// <returns>A <see cref="T:IDictionary"></see> of parameters for the provider.</returns>
        public IDictionary<string, string> Parameters {
            get {
                if (this._propertyNameCollection == null) {
                    lock (this) {
                        if (this._propertyNameCollection == null) {
                            this._propertyNameCollection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }

                return this._propertyNameCollection;
            }
        }
 
        /// <summary>
        /// Gets the runtime object.
        /// </summary>
        /// <returns>The processor object <see cref="IProcessor"/>.</returns>
        public IProcessor GetRuntimeObject() {
            if (this._runtimeObject != null) {
                return (IProcessor)this._runtimeObject;
            }

            IProcessor processor = (IProcessor)this.BaseGetRuntimeObject();
            processor.Name = this.Name;
            if (this.Trigger != null && this.Trigger.Name != null) {
                processor.Trigger = this.Trigger.GetRuntimeObject();
            }

            // Only for internal processors
            var intrenalProcessor = processor as Processor;
            if (intrenalProcessor != null) {
                intrenalProcessor.SetAttributes(this.Parameters);
            }

            this._runtimeObject = processor;

            return processor;
        }

        /// <summary>
        /// Resets the properties.
        /// </summary>
        internal override void ResetProperties() {
            base.ResetProperties();
            this._properties.Add(_propName);
            this._properties.Add(_propTrigger);
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value) {
            // ConfigurationProperty property = new ConfigurationProperty(name, typeof(string), value);
            this.Parameters.Add(name, value);
            return true;
        }
    }
}
