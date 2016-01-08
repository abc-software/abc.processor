// ----------------------------------------------------------------------------
// <copyright file="DayOfWeekUIEditor.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Design {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Collections.ObjectModel;
    using System.Security.Permissions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    internal class DayOfWeekUIEditor : UITypeEditor {
        private CheckedListBox listBox;

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            if (provider != null) {
                var service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                var collection = value as Collection<DayOfWeek>;
                if (service != null && collection != null) {
                    if (this.listBox == null) {
                        this.listBox = new CheckedListBox() {
                            BorderStyle = BorderStyle.None,
                            CheckOnClick = true,
                        };
                    }

                    foreach (string item in Enum.GetNames(typeof(DayOfWeek))) {
                        this.listBox.Items.Add(item, collection.Contains((DayOfWeek)Enum.Parse(typeof(DayOfWeek), item))); 
                    }

                    if (this.listBox.Height > (this.listBox.Items.Count * this.listBox.ItemHeight)) {
                        this.listBox.Height = this.listBox.Items.Count * this.listBox.ItemHeight;
                    }

                    service.DropDownControl(this.listBox);

                    collection.Clear();
                    foreach (string item in this.listBox.CheckedItems) {
                        collection.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), item));  
                    }

                    value = collection;
                }
            }

            return value;
        }
    }
}
