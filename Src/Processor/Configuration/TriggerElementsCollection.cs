// ----------------------------------------------------------------------------
// <copyright file="TriggerElementsCollection.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Configuration {
    using System.Configuration;

    [ConfigurationCollection(typeof(TriggerElement))]
    internal class TriggerElementsCollection : ConfigurationElementCollection {
        public override ConfigurationElementCollectionType CollectionType {
            get {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName {
            get {
                return "trigger";
            }
        }

        public new TriggerElement this[string name] {
            get {
                return (TriggerElement)this.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new TriggerElement(false);  
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((TriggerElement)element).Name;
        }
    }
}
