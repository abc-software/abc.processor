// ----------------------------------------------------------------------------
// <copyright file="ProcessorElementsCollection.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Configuration {
    using System.Configuration;

    /// <summary>
    /// Processor elements collection.
    /// </summary>
    [ConfigurationCollection(typeof(ProcessorElement))]
    internal class ProcessorElementsCollection : ConfigurationElementCollection {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorElementsCollection"/> class.
        /// </summary>
        public ProcessorElementsCollection() {
        }

        /// <summary>
        /// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Configuration.ConfigurationElementCollectionType"/> of this collection.
        /// </returns>
        public override ConfigurationElementCollectionType CollectionType {
            get {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// Gets the <see cref="IVIS.Infrastructure.Request.Processor.Configuration.ProcessorElement"/> with the specified name.
        /// </summary>
        /// <param name="name">The processor name.</param>
        /// <value></value>
        public new ProcessorElement this[string name] {
            get {
                return (ProcessorElement)this.BaseGet(name);
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement() {
            return new ProcessorElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element) {
            return ((ProcessorElement)element).Name;
        }

        /*
        //protected override void BaseAdd(ConfigurationElement element) {
        //    ProcessorElement processorElement = element as ProcessorElement;
        //    if (processorElement.Name.Equals("Default") && processorElement.TypeName.Equals(typeof(RedirectProcessor).FullName)) {
        //        base.BaseAdd(processorElement, false);
        //    }
        //    else {
        //        base.BaseAdd(processorElement, this.ThrowOnDuplicate);
        //    }
        //}

        //protected override void InitializeDefault() {
        //    this.InitializeDefaultInternal();
        //}

        //internal void InitializeDefaultInternal() {
        //    ProcessorElement processorElement = new ProcessorElement();
        //    processorElement.Name = "Default";
        //    processorElement.TypeName = typeof(RedirectProcessor).FullName;
        //    processorElement._isAddedByDefault = true;
        //    this.BaseAdd(processorElement);
        //}
        */
    }
}
