namespace Abc.Processor.UnitTests.Design {
    partial class TestService {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.testProcessor1 = new Abc.Processor.UnitTests.Design.TestProcessor();
            this.filterTrigger1 = new Abc.Processor.Triggers.FilterTrigger();
            this.processorServer1 = new Abc.Processor.ProcessorServer();
            // 
            // testProcessor1
            // 
            this.testProcessor1.Name = "";
            this.testProcessor1.Trigger = this.filterTrigger1;
            // 
            // filterTrigger1
            // 
            this.filterTrigger1.Name = "";
            // 
            // processorServer1
            // 
            this.processorServer1.ProcessorManager.Processors.Add(this.testProcessor1);
            // 
            // TestService
            // 
            this.ServiceName = "TestService";

        }

        #endregion

        private TestProcessor testProcessor1;
        private Triggers.FilterTrigger filterTrigger1;
        private ProcessorServer processorServer1;
    }
}
