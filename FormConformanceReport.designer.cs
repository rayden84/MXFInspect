namespace Myriadbits.MXFInspect
{
    partial class FormConformanceReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConformanceReport));
            this.btnClose = new System.Windows.Forms.Button();
            this.tlvConformanceResult = new BrightIdeasSoftware.TreeListView();
            this.colState = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colRule = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colActualValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colExpectedValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageListResult = new System.Windows.Forms.ImageList(this.components);
            this.txtGeneralInfo = new System.Windows.Forms.TextBox();
            this.txtSum = new System.Windows.Forms.TextBox();
            this.btnExecuteAllTests = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.prbProcessing = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.tlvConformanceResult)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(764, 458);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tlvConformanceResult
            // 
            this.tlvConformanceResult.AllColumns.Add(this.colState);
            this.tlvConformanceResult.AllColumns.Add(this.colRule);
            this.tlvConformanceResult.AllColumns.Add(this.colActualValue);
            this.tlvConformanceResult.AllColumns.Add(this.colExpectedValue);
            this.tlvConformanceResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlvConformanceResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colState,
            this.colRule,
            this.colActualValue,
            this.colExpectedValue});
            this.tlvConformanceResult.FullRowSelect = true;
            this.tlvConformanceResult.GridLines = true;
            this.tlvConformanceResult.HideSelection = false;
            this.tlvConformanceResult.Location = new System.Drawing.Point(12, 58);
            this.tlvConformanceResult.Name = "tlvConformanceResult";
            this.tlvConformanceResult.OwnerDraw = true;
            this.tlvConformanceResult.RowHeight = 19;
            this.tlvConformanceResult.ShowGroups = false;
            this.tlvConformanceResult.ShowItemToolTips = true;
            this.tlvConformanceResult.Size = new System.Drawing.Size(827, 334);
            this.tlvConformanceResult.SmallImageList = this.imageListResult;
            this.tlvConformanceResult.TabIndex = 19;
            this.tlvConformanceResult.UseCompatibleStateImageBehavior = false;
            this.tlvConformanceResult.View = System.Windows.Forms.View.Details;
            this.tlvConformanceResult.VirtualMode = true;
            // 
            // colState
            // 
            this.colState.AspectName = "";
            this.colState.Text = "";
            // 
            // colRule
            // 
            this.colRule.AspectName = "";
            this.colRule.Text = "Rule";
            this.colRule.Width = 280;
            // 
            // colActualValue
            // 
            this.colActualValue.AspectName = "";
            this.colActualValue.Text = "Actual Value";
            this.colActualValue.Width = 150;
            // 
            // colExpectedValue
            // 
            this.colExpectedValue.AspectName = "";
            this.colExpectedValue.FillsFreeSpace = true;
            this.colExpectedValue.Text = "Expected Value";
            this.colExpectedValue.Width = 150;
            // 
            // imageListResult
            // 
            this.imageListResult.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListResult.ImageStream")));
            this.imageListResult.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListResult.Images.SetKeyName(0, "Error");
            this.imageListResult.Images.SetKeyName(1, "Success");
            this.imageListResult.Images.SetKeyName(2, "Warning");
            this.imageListResult.Images.SetKeyName(3, "Info");
            // 
            // txtGeneralInfo
            // 
            this.txtGeneralInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGeneralInfo.Location = new System.Drawing.Point(12, 12);
            this.txtGeneralInfo.Multiline = true;
            this.txtGeneralInfo.Name = "txtGeneralInfo";
            this.txtGeneralInfo.ReadOnly = true;
            this.txtGeneralInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtGeneralInfo.Size = new System.Drawing.Size(827, 40);
            this.txtGeneralInfo.TabIndex = 20;
            // 
            // txtSum
            // 
            this.txtSum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSum.Location = new System.Drawing.Point(12, 422);
            this.txtSum.Multiline = true;
            this.txtSum.Name = "txtSum";
            this.txtSum.ReadOnly = true;
            this.txtSum.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSum.Size = new System.Drawing.Size(827, 30);
            this.txtSum.TabIndex = 21;
            // 
            // btnExecuteAllTests
            // 
            this.btnExecuteAllTests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExecuteAllTests.Location = new System.Drawing.Point(12, 458);
            this.btnExecuteAllTests.Name = "btnExecuteAllTests";
            this.btnExecuteAllTests.Size = new System.Drawing.Size(133, 23);
            this.btnExecuteAllTests.TabIndex = 22;
            this.btnExecuteAllTests.Text = "Execute All Tests";
            this.btnExecuteAllTests.UseVisualStyleBackColor = true;
            this.btnExecuteAllTests.Click += new System.EventHandler(this.btnExecuteAllTests_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // prbProcessing
            // 
            this.prbProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prbProcessing.Location = new System.Drawing.Point(12, 398);
            this.prbProcessing.Name = "prbProcessing";
            this.prbProcessing.Size = new System.Drawing.Size(827, 18);
            this.prbProcessing.TabIndex = 23;
            this.prbProcessing.Visible = false;
            // 
            // FormConformanceReport
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(851, 493);
            this.Controls.Add(this.prbProcessing);
            this.Controls.Add(this.btnExecuteAllTests);
            this.Controls.Add(this.txtSum);
            this.Controls.Add(this.txtGeneralInfo);
            this.Controls.Add(this.tlvConformanceResult);
            this.Controls.Add(this.btnClose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConformanceReport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Conformance Report";
            this.Load += new System.EventHandler(this.ReportForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tlvConformanceResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private BrightIdeasSoftware.TreeListView tlvConformanceResult;
        private BrightIdeasSoftware.OLVColumn colState;
        private BrightIdeasSoftware.OLVColumn colRule;
        private BrightIdeasSoftware.OLVColumn colExpectedValue;
        private System.Windows.Forms.TextBox txtGeneralInfo;
        private System.Windows.Forms.ImageList imageListResult;
        private System.Windows.Forms.TextBox txtSum;
        private System.Windows.Forms.Button btnExecuteAllTests;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ProgressBar prbProcessing;
        private BrightIdeasSoftware.OLVColumn colActualValue;
    }
}