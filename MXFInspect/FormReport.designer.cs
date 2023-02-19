﻿namespace Myriadbits.MXFInspect
{
	partial class FormReport
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReport));
			this.bntClose = new System.Windows.Forms.Button();
			this.tlvResults = new BrightIdeasSoftware.TreeListView();
			this.chState = new BrightIdeasSoftware.OLVColumn();
			this.chCategory = new BrightIdeasSoftware.OLVColumn();
			this.chResult = new BrightIdeasSoftware.OLVColumn();
			this.imageListResult = new System.Windows.Forms.ImageList(this.components);
			this.txtGeneralInfo = new System.Windows.Forms.TextBox();
			this.txtSum = new System.Windows.Forms.TextBox();
			this.btnExecuteAllTests = new System.Windows.Forms.Button();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.prbProcessing = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.tlvResults)).BeginInit();
			this.SuspendLayout();
			// 
			// bntClose
			// 
			this.bntClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bntClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bntClose.Location = new System.Drawing.Point(816, 625);
			this.bntClose.Margin = new System.Windows.Forms.Padding(7);
			this.bntClose.Name = "bntClose";
			this.bntClose.Size = new System.Drawing.Size(163, 58);
			this.bntClose.TabIndex = 7;
			this.bntClose.Text = "Close";
			this.bntClose.UseVisualStyleBackColor = true;
			this.bntClose.Click += new System.EventHandler(this.bntClose_Click);
			// 
			// tlvResults
			// 
			this.tlvResults.AllColumns.Add(this.chState);
			this.tlvResults.AllColumns.Add(this.chCategory);
			this.tlvResults.AllColumns.Add(this.chResult);
			this.tlvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tlvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chState,
            this.chCategory,
            this.chResult});
			this.tlvResults.FullRowSelect = true;
			this.tlvResults.GridLines = true;
			this.tlvResults.Location = new System.Drawing.Point(15, 180);
			this.tlvResults.Margin = new System.Windows.Forms.Padding(7);
			this.tlvResults.Name = "tlvResults";
			this.tlvResults.RowHeight = 19;
			this.tlvResults.ShowGroups = false;
			this.tlvResults.ShowItemToolTips = true;
			this.tlvResults.Size = new System.Drawing.Size(964, 297);
			this.tlvResults.SmallImageList = this.imageListResult;
			this.tlvResults.TabIndex = 19;
			this.tlvResults.UseCompatibleStateImageBehavior = false;
			this.tlvResults.View = System.Windows.Forms.View.Details;
			this.tlvResults.VirtualMode = true;
			this.tlvResults.SelectionChanged += new System.EventHandler(this.tlvResults_SelectionChanged);
			// 
			// chState
			// 
			this.chState.AspectName = "State";
			this.chState.Text = "";
			this.chState.Width = 26;
			// 
			// chCategory
			// 
			this.chCategory.AspectName = "Category";
			this.chCategory.Text = "Test";
			this.chCategory.Width = 114;
			// 
			// chResult
			// 
			this.chResult.AspectName = "Result";
			this.chResult.FillsFreeSpace = true;
			this.chResult.Text = "Result";
			this.chResult.WordWrap = true;
			// 
			// imageListResult
			// 
			this.imageListResult.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListResult.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListResult.ImageStream")));
			this.imageListResult.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListResult.Images.SetKeyName(0, "Error");
			this.imageListResult.Images.SetKeyName(1, "Success");
			this.imageListResult.Images.SetKeyName(2, "Warning");
			this.imageListResult.Images.SetKeyName(3, "Info");
			this.imageListResult.Images.SetKeyName(4, "Question");
			// 
			// txtGeneralInfo
			// 
			this.txtGeneralInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtGeneralInfo.Location = new System.Drawing.Point(16, 15);
			this.txtGeneralInfo.Margin = new System.Windows.Forms.Padding(7);
			this.txtGeneralInfo.Multiline = true;
			this.txtGeneralInfo.Name = "txtGeneralInfo";
			this.txtGeneralInfo.ReadOnly = true;
			this.txtGeneralInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtGeneralInfo.Size = new System.Drawing.Size(963, 151);
			this.txtGeneralInfo.TabIndex = 20;
			// 
			// txtSum
			// 
			this.txtSum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSum.Location = new System.Drawing.Point(16, 523);
			this.txtSum.Margin = new System.Windows.Forms.Padding(8);
			this.txtSum.MinimumSize = new System.Drawing.Size(0, 32);
			this.txtSum.Multiline = true;
			this.txtSum.Name = "txtSum";
			this.txtSum.ReadOnly = true;
			this.txtSum.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtSum.Size = new System.Drawing.Size(963, 87);
			this.txtSum.TabIndex = 21;
			// 
			// btnExecuteAllTests
			// 
			this.btnExecuteAllTests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExecuteAllTests.Location = new System.Drawing.Point(569, 625);
			this.btnExecuteAllTests.Margin = new System.Windows.Forms.Padding(7);
			this.btnExecuteAllTests.Name = "btnExecuteAllTests";
			this.btnExecuteAllTests.Size = new System.Drawing.Size(233, 58);
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
			this.prbProcessing.Location = new System.Drawing.Point(15, 492);
			this.prbProcessing.Margin = new System.Windows.Forms.Padding(8);
			this.prbProcessing.Name = "prbProcessing";
			this.prbProcessing.Size = new System.Drawing.Size(963, 19);
			this.prbProcessing.TabIndex = 23;
			this.prbProcessing.Visible = false;
			// 
			// FormReport
			// 
			this.AcceptButton = this.bntClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bntClose;
			this.ClientSize = new System.Drawing.Size(994, 698);
			this.Controls.Add(this.prbProcessing);
			this.Controls.Add(this.btnExecuteAllTests);
			this.Controls.Add(this.txtSum);
			this.Controls.Add(this.txtGeneralInfo);
			this.Controls.Add(this.tlvResults);
			this.Controls.Add(this.bntClose);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReport";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Report";
			this.Load += new System.EventHandler(this.ReportForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.tlvResults)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bntClose;
		private BrightIdeasSoftware.TreeListView tlvResults;
		private BrightIdeasSoftware.OLVColumn chState;
		private BrightIdeasSoftware.OLVColumn chCategory;
		private BrightIdeasSoftware.OLVColumn chResult;
		private System.Windows.Forms.TextBox txtGeneralInfo;
		private System.Windows.Forms.ImageList imageListResult;
		private System.Windows.Forms.TextBox txtSum;
		private System.Windows.Forms.Button btnExecuteAllTests;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.ProgressBar prbProcessing;
	}
}