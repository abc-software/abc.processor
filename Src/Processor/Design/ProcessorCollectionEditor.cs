// ----------------------------------------------------------------------------
// <copyright file="ProcessorCollectionEditor.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Design {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Windows.Forms;
    using System.Security.Permissions;

    /// <summary>
    /// Processors Collection Editor.
    /// </summary>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    internal partial class ProcessorsCollectionEditor : CollectionEditor {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorsCollectionEditor"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public ProcessorsCollectionEditor(Type type)
            : base(type) {
        }

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.CollectionEditor.CollectionForm"/> to provide as the user interface for editing the collection.
        /// </returns>
        protected override CollectionEditor.CollectionForm CreateCollectionForm() {
            return new ProcessorsCollectionEditorForm(this);
        }
    }
}
