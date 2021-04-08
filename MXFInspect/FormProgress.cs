using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Myriadbits.MXFInspect
{
    public partial class FormProgress : Form
    {
        private CancellationTokenSource _cts;
        private readonly Stopwatch sw = new Stopwatch();
        private int stopWatchInterval = 1000;

        public TimeSpan ElapsedTime => sw.Elapsed;

        public string ElapsedTimeFormatted => ElapsedTime.ToString(@"hh\:mm\:ss");

        //public static async Task<DialogResult> ShowDialogAsync(this Form parent)
        //{
        //    await Task.Yield();
        //    if (parent.IsDisposed)
        //    { return DialogResult.OK; }
        //    return parent.ShowDialog();
        //}

        public FormProgress(string formTitle, CancellationTokenSource cts)
        {
            InitializeComponent();
            ResetUI();
            SetFormTitle(formTitle);
            _cts = cts;
            tmrStopwatch.Interval = stopWatchInterval;
            tmrStopwatch.Start();
            sw.Start();
        }

        public async Task<DialogResult> ShowDialogAsync()
        {
            await Task.Yield();
            if (this.IsDisposed)
            { return DialogResult.OK; }
            return this.ShowDialog();
        }

        private void SetFormTitle(string text)
        {
            Text = text;
        }

        //public void UpateUIOverallProgress(int current, int total)
        //{
        //    var percentage = (current * 100 / total);
        //    prgOverall.SetValueFast(percentage);
        //    lblOverall.Text = $"{percentage}%";
        //    lblOverallDesc.Text = Properties.Resources.OverallProgress;
        //    UpateUIFetchResultsProgress(current, total);
        //}

        public void UpateUI(int percent)
        {
            prgOverall.SetValueFast(percent);
            lblOverall.Text = $"{percent}%";
            //lblOverallDesc.Text = task;
        }

        //public void UpateUIFetchResultsProgress(int current, int total)
        //{
        //    var percentage = (current * 100 / total);
        //    prgSingle.Value = percentage;
        //    lblSingle.Text = $"{percentage}%";
        //    lblSingleDesc.Text = $"Fetching {current}/{total} results";
        //}

        //public void UpateUIDocumentListProgress(int current, int total)
        //{
        //    var percentage = (current * 100 / total);
        //    prgOverall.SetValueFast(percentage);
        //    lblOverall.Text = $"{percentage}%";
        //    lblOverallDesc.Text = $"Document: {current}/{total}";
        //}

        //public void UpdateUIAlreadyScanned()
        //{
        //    lblSingle.Text = string.Empty;
        //    lblSingleDesc.Text = Properties.Resources.AlreadyScanned;
        //}

        //public void UpdateUINoReferences()
        //{
        //    lblSingle.Text = string.Empty;
        //    lblSingleDesc.Text = Properties.Resources.NoReferences;
        //}

        //public void UpdateUIShowResultSetTooBig()
        //{
        //    lblSingle.Text = string.Empty;
        //    lblSingleDesc.Text = Properties.Resources.ResultSetTooBig;
        //}

        //public void UpdateUIShowRetrieveResult()
        //{
        //    lblSingle.Text = string.Empty;
        //    lblSingleDesc.Text = Properties.Resources.RetrieveResult;
        //}

        //public void UpdateUIShowAnalyseSimilarity()
        //{
        //    lblSingle.Text = string.Empty;
        //    lblSingleDesc.Text = Properties.Resources.AnalyseSimilarity;
        //}

        public void ResetUI()
        {
            prgOverall.Value = 0;
            prgSingle.Value = 0;
            lblSingle.Text = "0%";
            lblOverall.Text = "0%";
            lblOverallDesc.Text = string.Empty;
            lblSingleDesc.Text = string.Empty;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Text = "Cancelling...";
            btnCancel.Enabled = false;
            _cts.Cancel();
        }

        private void tmrStopwatch_Tick(object sender, EventArgs e)
        {
            lblTime.Text = $"{"Time Elapsed:"} {ElapsedTimeFormatted}";
        }
    }
}
