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

using FluentValidation;
using Myriadbits.MXF;
using Myriadbits.MXF.ConformanceValidators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Myriadbits.MXFInspect
{
    public partial class FormConformanceReport : Form
    {
        private readonly string enumerableSeparator = ",";
        private readonly string valuesSeparator = " || ";
        private readonly string nullString = "<null>";

        private MXFFile m_mainFile = null;
        private Stopwatch m_stopWatch = new Stopwatch();
        private int m_lastPercentage = -1;

        public IList<ValidationResult> reports = null;

        /// <summary>
        /// Constructor, duh
        /// </summary>
        public FormConformanceReport(MXFFile file)
        {
            InitializeComponent();
            SetFile(file);
        }

        /// <summary>
        /// Set the file
        /// </summary>
        /// <param name="file"></param>
        public void SetFile(MXFFile file)
        {
            m_mainFile = file;
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            if (m_mainFile != null)
            {
                ValidatorOptions.Global.LanguageManager.Enabled = false;
                var validator = new MXFProfile01Validator(m_mainFile);

                reports = validator.ValidationResults;

                DisplayFileInfo();
            }
            else MessageBox.Show("No MXF file loaded.");


        }

        private void InitializeTreeList()
        {
            //OLVColumn colState = (OLVColumn)this.tlvConformanceResult.Columns[0];
            //colState.Renderer = null;

            //OLVColumn colRule = (OLVColumn)this.tlvConformanceResult.Columns[1];
            //colState.Renderer = null;

            //OLVColumn colActualValue = (OLVColumn)this.tlvConformanceResult.Columns[2];
            //colActualValue.Renderer = null;// this.tlvResults.TreeColumnRenderer;

            //OLVColumn colExpectedValue = (OLVColumn)this.colExpectedValue;
            //colExpectedValue.Renderer = null;// this.tlvResults.TreeColumnRenderer;

            // don't know what this is good for
            Pen pen = new Pen(Color.Black, 1.001f);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            tlvConformanceResult.TreeColumnRenderer.LinePen = pen;


            tlvConformanceResult.SetObjects(reports);

            // when do we can expand the tree?
            tlvConformanceResult.CanExpandGetter = delegate (object x)
            {
                var result = x as ValidationResult;
                return result != null && result.GetExecutedRules().Any();

            };

            tlvConformanceResult.ChildrenGetter = delegate (object x)
            {
                return (x as ValidationResult)?.GetExecutedRules() ?? null;
            };


            // always return error icon for state

            colState.ImageGetter = delegate (object x)
            {
                if (x is ValidationResult result)
                {
                    return result.IsValid ? 1 : 0;
                }
                else if (x is ValidationResultEntry entry)
                {
                    return entry.Passed.Value ? 1 : 0;
                }

                return 0;
            };

            // always return empty string for state
            colState.AspectToStringConverter = delegate (object x)
             {
                 return string.Empty;
             };

            colRule.AspectGetter = delegate (object x)
            {
                if (x is ValidationResult validator)
                {
                    return validator.ToString();
                }
                else if (x is ValidationResultEntry entry)
                {
                    return entry.Name;
                }
                else
                {
                    // should never occur
                    return string.Empty;
                }
            };

            colActualValue.AspectGetter = delegate (object x)
            {
                
                if (x is ValidationResultEntry entry)
                {
                    if (entry.ActualValue is System.Collections.IEnumerable enumerable)
                    {
                        return EnumerableToString(enumerable, enumerableSeparator);
                    }
                    else return entry?.ActualValue ?? nullString;
                }
                else return string.Empty;
            };

            colExpectedValue.AspectGetter = delegate (object x)
            {
                if (x is ValidationResultEntry entry)
                {
                    return ExpectedValuesToString(entry.ExpectedValues, valuesSeparator, enumerableSeparator);
                }
                else return string.Empty;
            };
        }


        private string EnumerableToString(System.Collections.IEnumerable enumerable, string separator)
        {
            var sb = new StringBuilder();
            sb.Append("{");

            foreach (var item in enumerable)
            {
                sb.Append(item);
                sb.Append(separator);
            }

            sb.Remove(sb.Length - separator.Length, separator.Length);
            sb.Append("}");
            return sb.ToString();
        }

        private string ExpectedValuesToString(IEnumerable<object> expectedValues, string separator, string enumerableSeparator)
        {
            var sb = new StringBuilder();
            bool separatorAdded = false;

            foreach (var ev in expectedValues)
            {
                if (ev is System.Collections.IEnumerable enumerable)
                {
                    sb.Append(EnumerableToString(enumerable, enumerableSeparator));
                }
                else
                {
                    sb.Append(ev.ToString());
                }

                sb.Append(separator);
                separatorAdded = true;

            }
            if (separatorAdded)
            {
                sb.Remove(sb.Length - separator.Length, separator.Length);
            }

            return sb.ToString();
        }

        private void DisplayFileInfo()
        {
            if (this.m_mainFile != null)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(string.Format("Filename: {0}", this.m_mainFile.Filename));
                //sb.AppendLine(string.Format("File size: {0:0.00} Mb", (this.m_mainFile.Filesize) / (1024 * 1024)));
                //sb.AppendLine(string.Format("Number of partitions: {0}", this.m_mainFile.PartitionCount));
                //if (this.m_mainFile.RIP != null)
                //	sb.AppendLine(string.Format("RIP Found (containing {0} entries)", this.m_mainFile.RIPEntryCount));
                //if (this.m_mainFile.FirstSystemItem != null)
                //	sb.AppendLine(string.Format("First system item time: {0}", this.m_mainFile.FirstSystemItem.UserDateFullFrameNb));
                //if (this.m_mainFile.LastSystemItem != null)
                //	sb.AppendLine(string.Format("Last system item time: {0}", this.m_mainFile.LastSystemItem.UserDateFullFrameNb));

                //this.txtGeneralInfo.Text = sb.ToString();


            }

            InitializeTreeList();

            //this.tlvResults.ChildrenGetter = delegate(object x)
            //{
            //	MXFValidationResult vr = x as MXFValidationResult;
            //	if (vr == null) return null;
            //	return vr;
            //};


            //StringBuilder summary = new StringBuilder();
            //int errorCnt = 0;
            //int warningCnt = 0;
            //if (this.m_mainFile != null)
            //{
            //	errorCnt = this.m_mainFile.Results.Count(a => a.State == MXFValidationState.Error);
            //	warningCnt = this.m_mainFile.Results.Count(a => a.State == MXFValidationState.Warning);
            //	if (errorCnt == 0 && warningCnt == 0)
            //		summary.AppendLine(string.Format("There are no errors found, file seems to be ok!"));
            //	else
            //		summary.AppendLine(string.Format("Found {0} errors and {1} warnings!", errorCnt, warningCnt));
            //	summary.AppendLine(string.Format("(Double click on an item to see more details)"));
            //}
            //else
            //{
            //	summary.AppendLine(string.Format("ERROR WHILE PARSING THE MXF FILE"));
            //}

            //txtSum.Text = summary.ToString();
        }

        /// <summary>
        /// Close this dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Execute all tests
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExecuteAllTests_Click(object sender, EventArgs e)
        {
            //if (this.m_mainFile != null)
            //{
            //    this.prbProcessing.Visible = true;
            //    this.txtGeneralInfo.Visible = false;
            //    this.Enabled = false;
            //    this.m_lastPercentage = 0;

            //    backgroundWorker.RunWorkerAsync(this.m_mainFile);
            //}
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //MXFFile mainFile = e.Argument as MXFFile;
            //if (mainFile != null)
            //{
            //    BackgroundWorker worker = sender as BackgroundWorker;
            //    mainFile.ExecuteValidationTest(worker, true);
            //}
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //if (e.ProgressPercentage > 0)
            //{
            //    if (!m_stopWatch.IsRunning)
            //    {
            //        m_stopWatch.Start();
            //        m_lastPercentage = e.ProgressPercentage;
            //    }
            //    else
            //    {
            //        string currentTask = e.UserState as string;
            //        if (currentTask == null) currentTask = "";
            //        if (e.ProgressPercentage - m_lastPercentage > 0)
            //        {
            //            int estimate = 100 - e.ProgressPercentage;
            //            int msecPerPercentage = (int)(m_stopWatch.ElapsedMilliseconds / (e.ProgressPercentage - m_lastPercentage));
            //            txtSum.Text = string.Format("{0} - Estimated time: {1} s", currentTask, (estimate * msecPerPercentage) / 1000);
            //        }
            //        m_lastPercentage = e.ProgressPercentage;
            //    }
            //}
            //this.prbProcessing.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //this.prbProcessing.Visible = false;
            //this.txtGeneralInfo.Visible = true;
            //this.Enabled = true;

            //DisplayFileInfo();
        }
    }
}
