// ----------------------------------------------------------------------------
// <copyright file="ProcessorCollection.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Processors collection.
    /// </summary>
    public class ProcessorCollection : Collection<IProcessor> {
        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"></see> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can not be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
        /// <exception cref="T:System.ArgumentNullException">item is null.</exception>
        protected override void InsertItem(int index, IProcessor item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can not be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
        /// <exception cref="T:System.ArgumentNullException">item is null.</exception>
        protected override void SetItem(int index, IProcessor item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            base.SetItem(index, item);
        }
    }
}
