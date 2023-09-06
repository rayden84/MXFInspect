namespace Myriadbits.MXFInspect
{
    partial class FormProgress
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
            components = new System.ComponentModel.Container();
            btnCancel = new System.Windows.Forms.Button();
            prgOverall = new System.Windows.Forms.ProgressBar();
            prgSingle = new System.Windows.Forms.ProgressBar();
            lblOverall = new System.Windows.Forms.Label();
            lblSingle = new System.Windows.Forms.Label();
            lblOverallDesc = new System.Windows.Forms.Label();
            lblSingleDesc = new System.Windows.Forms.Label();
            lblTime = new System.Windows.Forms.Label();
            tmrStopwatch = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.Location = new System.Drawing.Point(319, 140);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(113, 27);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // prgOverall
            // 
            prgOverall.Location = new System.Drawing.Point(14, 16);
            prgOverall.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            prgOverall.Name = "prgOverall";
            prgOverall.Size = new System.Drawing.Size(418, 27);
            prgOverall.TabIndex = 1;
            // 
            // prgSingle
            // 
            prgSingle.Location = new System.Drawing.Point(14, 64);
            prgSingle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            prgSingle.Name = "prgSingle";
            prgSingle.Size = new System.Drawing.Size(418, 27);
            prgSingle.TabIndex = 2;
            // 
            // lblOverall
            // 
            lblOverall.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblOverall.BackColor = System.Drawing.Color.Transparent;
            lblOverall.Location = new System.Drawing.Point(381, 46);
            lblOverall.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblOverall.Name = "lblOverall";
            lblOverall.Size = new System.Drawing.Size(51, 15);
            lblOverall.TabIndex = 6;
            lblOverall.Text = "20%";
            lblOverall.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblSingle
            // 
            lblSingle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblSingle.BackColor = System.Drawing.Color.Transparent;
            lblSingle.Location = new System.Drawing.Point(368, 94);
            lblSingle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSingle.Name = "lblSingle";
            lblSingle.Size = new System.Drawing.Size(64, 15);
            lblSingle.TabIndex = 7;
            lblSingle.Text = "30%";
            lblSingle.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblOverallDesc
            // 
            lblOverallDesc.BackColor = System.Drawing.Color.Transparent;
            lblOverallDesc.Location = new System.Drawing.Point(14, 46);
            lblOverallDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblOverallDesc.Name = "lblOverallDesc";
            lblOverallDesc.Size = new System.Drawing.Size(382, 15);
            lblOverallDesc.TabIndex = 8;
            // 
            // lblSingleDesc
            // 
            lblSingleDesc.BackColor = System.Drawing.Color.Transparent;
            lblSingleDesc.Location = new System.Drawing.Point(14, 94);
            lblSingleDesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSingleDesc.Name = "lblSingleDesc";
            lblSingleDesc.Size = new System.Drawing.Size(382, 18);
            lblSingleDesc.TabIndex = 9;
            // 
            // lblTime
            // 
            lblTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblTime.BackColor = System.Drawing.Color.Transparent;
            lblTime.Location = new System.Drawing.Point(14, 120);
            lblTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(418, 17);
            lblTime.TabIndex = 10;
            lblTime.Text = "Time Elapsed: 00:00:00";
            lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrStopwatch
            // 
            tmrStopwatch.Tick += tmrStopwatch_Tick;
            // 
            // FormProgress
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new System.Drawing.Size(457, 205);
            ControlBox = false;
            Controls.Add(lblTime);
            Controls.Add(lblSingleDesc);
            Controls.Add(lblOverallDesc);
            Controls.Add(lblSingle);
            Controls.Add(lblOverall);
            Controls.Add(prgSingle);
            Controls.Add(prgOverall);
            Controls.Add(btnCancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(468, 228);
            Name = "FormProgress";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Progress";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar prgSingle;
        private System.Windows.Forms.ProgressBar prgOverall;
        private System.Windows.Forms.Label lblOverall;
        private System.Windows.Forms.Label lblSingle;
        private System.Windows.Forms.Label lblOverallDesc;
        private System.Windows.Forms.Label lblSingleDesc;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Timer tmrStopwatch;
    }
}