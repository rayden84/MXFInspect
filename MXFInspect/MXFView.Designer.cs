namespace Myriadbits.MXFInspect
{
    partial class MXFView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MXFView));
            splitMain = new System.Windows.Forms.SplitContainer();
            tabMain = new System.Windows.Forms.TabControl();
            tpPhysical = new System.Windows.Forms.TabPage();
            tlvPhysical = new PhysicalTreeListView();
            tpLogical = new System.Windows.Forms.TabPage();
            tlvLogical = new LogicalTreeListView();
            splitRight = new System.Windows.Forms.SplitContainer();
            propGrid = new ReadOnlyPropertyGrid();
            rtfHexViewer = new HexViewer();
            mainPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            tabMain.SuspendLayout();
            tpPhysical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tlvPhysical).BeginInit();
            tpLogical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tlvLogical).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitRight).BeginInit();
            splitRight.Panel1.SuspendLayout();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // splitMain
            // 
            splitMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitMain.Location = new System.Drawing.Point(0, 3);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(tabMain);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitRight);
            splitMain.Size = new System.Drawing.Size(1181, 575);
            splitMain.SplitterDistance = 558;
            splitMain.SplitterWidth = 6;
            splitMain.TabIndex = 0;
            // 
            // tabMain
            // 
            tabMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabMain.Controls.Add(tpPhysical);
            tabMain.Controls.Add(tpLogical);
            tabMain.Location = new System.Drawing.Point(4, 4);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new System.Drawing.Size(556, 572);
            tabMain.TabIndex = 0;
            tabMain.SelectedIndexChanged += tabMain_SelectedIndexChanged;
            // 
            // tpPhysical
            // 
            tpPhysical.Controls.Add(tlvPhysical);
            tpPhysical.Location = new System.Drawing.Point(4, 24);
            tpPhysical.Name = "tpPhysical";
            tpPhysical.Padding = new System.Windows.Forms.Padding(3);
            tpPhysical.Size = new System.Drawing.Size(548, 544);
            tpPhysical.TabIndex = 0;
            tpPhysical.Text = "Physical";
            tpPhysical.UseVisualStyleBackColor = true;
            // 
            // tlvPhysical
            // 
            tlvPhysical.AlternateRowBackColor = System.Drawing.Color.WhiteSmoke;
            tlvPhysical.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tlvPhysical.EmptyListMsg = "No items present";
            tlvPhysical.EmptyListMsgFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tlvPhysical.FullRowSelect = true;
            tlvPhysical.Location = new System.Drawing.Point(3, 3);
            tlvPhysical.MultiSelect = false;
            tlvPhysical.Name = "tlvPhysical";
            tlvPhysical.RowHeight = 19;
            tlvPhysical.SelectedBackColor = System.Drawing.SystemColors.Highlight;
            tlvPhysical.ShowGroups = false;
            tlvPhysical.Size = new System.Drawing.Size(542, 538);
            tlvPhysical.TabIndex = 15;
            tlvPhysical.TintSortColumn = true;
            tlvPhysical.UnfocusedSelectedBackColor = System.Drawing.SystemColors.Highlight;
            tlvPhysical.UseAlternatingBackColors = true;
            tlvPhysical.UseCellFormatEvents = true;
            tlvPhysical.UseCompatibleStateImageBehavior = false;
            tlvPhysical.UseFilterIndicator = true;
            tlvPhysical.UseFiltering = true;
            tlvPhysical.UseHotItem = true;
            tlvPhysical.UseHyperlinks = true;
            tlvPhysical.UseTranslucentHotItem = true;
            tlvPhysical.View = System.Windows.Forms.View.Details;
            tlvPhysical.VirtualMode = true;
            tlvPhysical.CellOver += tlvPhysical_CellOver;
            tlvPhysical.CellRightClick += tlvPhysical_CellRightClick;
            tlvPhysical.CellToolTipShowing += tlvPhysical_CellToolTipShowing;
            tlvPhysical.MouseHover += tlvPhysical_MouseHover;
            tlvPhysical.MouseMove += tlvPhysical_MouseMove;
            // 
            // tpLogical
            // 
            tpLogical.Controls.Add(tlvLogical);
            tpLogical.Location = new System.Drawing.Point(4, 24);
            tpLogical.Name = "tpLogical";
            tpLogical.Padding = new System.Windows.Forms.Padding(3);
            tpLogical.Size = new System.Drawing.Size(548, 544);
            tpLogical.TabIndex = 1;
            tpLogical.Text = "Logical";
            tpLogical.UseVisualStyleBackColor = true;
            // 
            // tlvLogical
            // 
            tlvLogical.AlternateRowBackColor = System.Drawing.Color.WhiteSmoke;
            tlvLogical.Dock = System.Windows.Forms.DockStyle.Fill;
            tlvLogical.EmptyListMsg = "No items present";
            tlvLogical.EmptyListMsgFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tlvLogical.FullRowSelect = true;
            tlvLogical.Location = new System.Drawing.Point(3, 3);
            tlvLogical.MultiSelect = false;
            tlvLogical.Name = "tlvLogical";
            tlvLogical.RowHeight = 19;
            tlvLogical.SelectedBackColor = System.Drawing.SystemColors.Highlight;
            tlvLogical.ShowGroups = false;
            tlvLogical.Size = new System.Drawing.Size(542, 538);
            tlvLogical.TabIndex = 16;
            tlvLogical.TintSortColumn = true;
            tlvLogical.UnfocusedSelectedBackColor = System.Drawing.SystemColors.Highlight;
            tlvLogical.UseAlternatingBackColors = true;
            tlvLogical.UseCellFormatEvents = true;
            tlvLogical.UseCompatibleStateImageBehavior = false;
            tlvLogical.UseFilterIndicator = true;
            tlvLogical.UseFiltering = true;
            tlvLogical.View = System.Windows.Forms.View.Details;
            tlvLogical.VirtualMode = true;
            // 
            // splitRight
            // 
            splitRight.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitRight.Location = new System.Drawing.Point(3, 3);
            splitRight.Name = "splitRight";
            splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitRight.Panel1
            // 
            splitRight.Panel1.Controls.Add(propGrid);
            // 
            // splitRight.Panel2
            // 
            splitRight.Panel2.Controls.Add(rtfHexViewer);
            splitRight.Size = new System.Drawing.Size(586, 576);
            splitRight.SplitterDistance = 407;
            splitRight.SplitterWidth = 6;
            splitRight.TabIndex = 16;
            // 
            // propGrid
            // 
            propGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(127, 1, 0, 0);
            propGrid.HelpVisible = false;
            propGrid.Location = new System.Drawing.Point(3, 25);
            propGrid.Name = "propGrid";
            propGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propGrid.ReadOnly = true;
            propGrid.Size = new System.Drawing.Size(577, 379);
            propGrid.TabIndex = 1;
            propGrid.ToolbarVisible = false;
            propGrid.ViewForeColor = System.Drawing.Color.FromArgb(1, 0, 0);
            // 
            // rtfHexViewer
            // 
            rtfHexViewer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            rtfHexViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            rtfHexViewer.BytesPerLine = 16;
            rtfHexViewer.DisplayableBytesThreshold = 1000000L;
            rtfHexViewer.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            rtfHexViewer.HideSelection = false;
            rtfHexViewer.Location = new System.Drawing.Point(3, 3);
            rtfHexViewer.Name = "rtfHexViewer";
            rtfHexViewer.ReadOnly = true;
            rtfHexViewer.Size = new System.Drawing.Size(577, 126);
            rtfHexViewer.TabIndex = 16;
            rtfHexViewer.Text = "";
            // 
            // mainPanel
            // 
            mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            mainPanel.Controls.Add(splitMain);
            mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            mainPanel.Location = new System.Drawing.Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new System.Drawing.Size(1184, 581);
            mainPanel.TabIndex = 12;
            // 
            // MXFView
            // 
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.SystemColors.Window;
            ClientSize = new System.Drawing.Size(1184, 581);
            Controls.Add(mainPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MXFView";
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            TransparencyKey = System.Drawing.SystemColors.Window;
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += MXFView_Load;
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            tabMain.ResumeLayout(false);
            tpPhysical.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tlvPhysical).EndInit();
            tpLogical.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tlvLogical).EndInit();
            splitRight.Panel1.ResumeLayout(false);
            splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
            mainPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tpPhysical;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.SplitContainer splitRight;
        private System.Windows.Forms.TabPage tpLogical;
        private HexViewer rtfHexViewer;
        private PhysicalTreeListView tlvPhysical;
        private LogicalTreeListView tlvLogical;
        private ReadOnlyPropertyGrid propGrid;
    }
}

