﻿#region license
//
// MXFInspect - Myriadbits MXF Viewer. 
// Inspect MXF Files.
// Copyright (C) 2015 Myriadbits, Jochem Bakker
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// For more information, contact me at: info@myriadbits.com
//
#endregion

using BrightIdeasSoftware;
using Myriadbits.MXF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Myriadbits.MXFInspect
{
    public partial class MXFView : MyFormPage
    {
        #region Public props
        
        public MXFFile File { get; private set; }
        
        public MXFObject PhysicalTreeSelectedObject { get; private set; }
        public MXFLogicalObject LogicalTreeSelectedObject { get; private set; }
        public bool PhysicalViewShown { get; set; } = true;
        public string Filename { get; set; }
        
        private bool _currentTypeFiltered = false;
        public bool FilterCurrentType
        {
            get => _currentTypeFiltered;
            set
            {
                this._currentTypeFiltered = value;
                this.SetTypeFilter(value);
            }
        }

        private bool _fillerHidden = true;
        public bool FillerHidden
        {
            get => _fillerHidden;
            set
            {
                this._fillerHidden = value;
                this.HideFiller(value);
            }
        }

        private bool _showPropInfo = false;
        public bool ShowPropertyInfo
        {
            get => _showPropInfo;
            set
            {
                this._showPropInfo = value;
                this.ShowPropInfo(value);
            }
        }

        #endregion

        private FormMain ParentMainForm { get; set; }

        private FileParseMode FileParseMode { get; set; }

        private bool m_fDoNotSelectOther = false;



        public MXFView(string fileName, FileParseMode fileParseMode)
        {
            InitializeComponent();
            this.Filename = fileName;
            this.FileParseMode = fileParseMode;
            this.Text = fileName;
            this.FillerHidden = true;
            this.MainPanel = this.mainPanel; // Do this AFTER the InitializeComponent call!!!

        }

        private async void MXFView_Load(object sender, EventArgs e)
        {
            this.ParentMainForm = this.MdiParent as FormMain;

            ObjectListView.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            this.Text = this.Filename;

            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.splitMain.Visible = false;

            // bug that means you have to set the desired icon again otherwise it reverts to default when child form is maximised
            this.Icon = Myriadbits.MXFInspect.Properties.Resources.ChildIcon;

            // wiring treelist selectionchanged events
            this.tlvPhysical.SelectionChanged += PhysicalTree_SelectionChanged;
            this.tlvLogical.SelectionChanged += LogicalTree_SelectionChanged;


            ParentMainForm.EnableUI(false);
            this.ApplyUserSettings();

            var cts = new CancellationTokenSource();
            var prgForm = new FormProgress("Reading file...", cts);
            var progressHandler = new Progress<(int, string)>(t => prgForm.UpateUI(t.Item1));
            var progressFormTask = prgForm.ShowDialogAsync();

            // Open the selected file to read.
            try
            {
                // Process the file
                //this.File = new MXFFile(this.Filename, worker, this.FileParseMode);
                this.File = await MXFFile.CreateAsync(this.Filename, progressHandler, cts.Token);

                FillTrees();
                this.splitMain.Visible = true;
                this.tabMain.SelectedIndex = 0;
                
                // Show report
                //FormReport fr = new FormReport(this.File);
                //fr.ShowDialog(ParentMainForm);
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine("Operation aborted by user {0}", ex);
                this.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while opening the file");
            }
            finally
            {
                // TODO: really needed?
                prgForm.Close();
                await progressFormTask;
                ParentMainForm.EnableUI(true);
            }

        }


        /// <summary>
        /// Fill the tree
        /// </summary>
        private void FillTrees()
        {
            try
            {
                this.tlvPhysical.FillTree(this.File.Children.OrderBy(c => c.Offset));
                this.tlvPhysical.RevealAndSelectObject(this.tlvPhysical.GetFirstPartition());
                this.tlvPhysical.HideFillers(this.FillerHidden);

                var logicalList = new List<MXFLogicalObject>() { this.File.LogicalBase };
                this.tlvLogical.FillTree(logicalList);

                this.txtOverall.Text = string.Format("Total objects: {0}", this.File.Descendants().Count());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while populating the trees");
                this.Close();
            }
        }

        private void PhysicalTree_SelectionChanged(object sender, EventArgs e)
        {
            PhysicalTreeSelectedObject = this.tlvPhysical.SelectedObject as MXFObject;
            var logicalObj = PhysicalTreeSelectedObject?.LogicalWrapper;

            if (PhysicalTreeSelectedObject != null)
            {
                if (!m_fDoNotSelectOther)
                {
                    this.propGrid.SelectedObject = PhysicalTreeSelectedObject;

                    // Try to select this object in the logical list as well
                    m_fDoNotSelectOther = true;
                    this.tlvLogical.RevealAndSelectObject(logicalObj);

                    // Display the mxfobject as hex dump
                    rtfHexViewer.SetObject(PhysicalTreeSelectedObject);

                    m_fDoNotSelectOther = false;
                }
            }
            ParentMainForm.UpdateMenu();
        }

        /// <summary>
        /// User clicked another item in the logical tree, show the details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogicalTree_SelectionChanged(object sender, EventArgs e)
        {
            LogicalTreeSelectedObject = this.tlvLogical.SelectedObject as MXFLogicalObject;
            if (LogicalTreeSelectedObject != null)
            {
                MXFObject obj = LogicalTreeSelectedObject.Object;
                if (obj != null)
                {
                    if (!m_fDoNotSelectOther)
                    {
                        this.propGrid.SelectedObject = obj;


                        // Try to select this item in the main list as well
                        m_fDoNotSelectOther = true;
                        this.tlvPhysical.RevealAndSelectObject(obj);
                        // Display the mxfobject as hex dump
                        rtfHexViewer.SetObject(obj);
                        m_fDoNotSelectOther = false;
                    }
                }
            }
            ParentMainForm.UpdateMenu();
        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PhysicalViewShown = tabMain.SelectedTab == tpPhysical;
            this.ParentMainForm.UpdateMenu();
        }

        private void SetTypeFilter(bool filtered)
        {
            tlvPhysical.SetTypeFilter(filtered);
            ParentMainForm.UpdateMenu();
        }

        private void HideFiller(bool exclude)
        {
            tlvPhysical.HideFillers(exclude);
        }

        /// <summary>
        /// Show/hide property info help
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowPropInfo(bool showInfo)
        {
            this.propGrid.HelpVisible = showInfo;
        }

        public void SelectNextObject()
        {
            MXFObject selectedObject = this.tlvPhysical.SelectedObject as MXFObject;
            if (selectedObject != null)
            {
                MXFObject nextObject = selectedObject.FindNextObjectOfType(selectedObject.GetType());

                if (nextObject != null)
                {
                    this.tlvPhysical.RevealAndSelectObject(nextObject);
                }

            }
        }

        public void SelectPreviousObject()
        {
            MXFObject selectedObject = this.tlvPhysical.SelectedObject as MXFObject;
            if (selectedObject != null)
            {
                MXFObject prevObject = selectedObject.FindPreviousObjectOfType(selectedObject.GetType());

                if (prevObject != null)
                {
                    this.tlvPhysical.RevealAndSelectObject(prevObject);
                }

            }
        }

        public void CollapseAll()
        {
            if (this.PhysicalViewShown)
            {
                this.tlvPhysical.CollapseAll();
            }
            else
            {
                this.tlvLogical.CollapseAll();
            }

        }

        /// <summary>
        /// Apply all user settings
        /// </summary>
        public void ApplyUserSettings()
        {
            this.tlvLogical.SetOffsetStyle(Properties.Settings.Default.ShowOffsetAsHex);
            this.tlvPhysical.SetOffsetStyle(Properties.Settings.Default.ShowOffsetAsHex);
            this.rtfHexViewer.SetOffsetStyle(Properties.Settings.Default.ShowOffsetAsHex);
        }
    }
}
