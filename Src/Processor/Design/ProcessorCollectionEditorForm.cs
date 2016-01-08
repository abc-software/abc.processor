// ----------------------------------------------------------------------------
// <copyright file="ProcessorCollectionEditorForm.cs" company="ABC Software Ltd">
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
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    internal partial class ProcessorsCollectionEditor {
        /// <summary>
        /// Processor Colletion Editor Form
        /// </summary>
        private partial class ProcessorsCollectionEditorForm : CollectionEditor.CollectionForm {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProcessorsCollectionEditorForm"/> class.
            /// </summary>
            /// <param name="available">The available.</param>
            /// <param name="selected">The selected.</param>
            public ProcessorsCollectionEditorForm(CollectionEditor editor)
                : base(editor) {
                InitializeComponent();
            }

            /// <summary>
            /// Shows the dialog box for the collection editor using the specified <see cref="T:System.Windows.Forms.Design.IWindowsFormsEditorService"/> object.
            /// </summary>
            /// <param name="edSvc">An <see cref="T:System.Windows.Forms.Design.IWindowsFormsEditorService"/> that can be used to show the dialog box.</param>
            /// <returns>
            /// A <see cref="T:System.Windows.Forms.DialogResult"/> that indicates the result code returned from the dialog box.
            /// </returns>
            protected override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc) {
                var selected = new List<object>(this.Items);
                var host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                foreach (Component item in host.Container.Components) {
                    IProcessor processor = item as IProcessor;
                    if (processor != null) {
                        this.checkedListBox.Items.Add(new ListBoxItem(processor), selected.Contains(processor));
                    }
                }  

                return base.ShowEditorDialog(edSvc);
            }

            private void OKButton_click(object sender, EventArgs e) {
                List<IProcessor> selected = new List<IProcessor>();
                foreach (ListBoxItem item in this.checkedListBox.CheckedItems) {
                    selected.Add(item.Processor);
                }

                this.Items = selected.ToArray();
            }

            /// <summary>
            /// Provides an opportunity to perform processing when a collection value has changed.
            /// </summary>
            protected override void OnEditValueChanged() {
                var selected = new List<object>(this.Items);
                for (int i = 0; i < this.checkedListBox.Items.Count; i++) {
                    ListBoxItem item = (ListBoxItem)this.checkedListBox.Items[i];
                    this.checkedListBox.SetItemChecked(i, selected.Contains(item.Processor));
                }
            }
        }

        private class ListBoxItem {
            /// <summary>
            /// Initializes a new instance of the <see cref="ListBoxItem"/> class.
            /// </summary>
            /// <param name="processor">The processor.</param>
            public ListBoxItem(IProcessor processor) {
                this.Processor = processor;
            }

            /// <summary>
            /// Gets or sets the processor.
            /// </summary>
            /// <value>
            /// The processor.
            /// </value>
            public IProcessor Processor { get; set; }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString() {
                return this.Processor.GetType().Name + ": " + this.Processor.Name;
            }
        }
    }
}