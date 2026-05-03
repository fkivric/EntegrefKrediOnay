using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace EntegrefKrediOnay
{
    public partial class frmCustomSplash : DevExpress.XtraEditors.XtraForm
    {

        public CancellationTokenSource _cts;
        private System.Windows.Forms.Timer fadeTimer;
        private bool fadeIn = true;
        public CancellationToken Token => _cts.Token;


        ProgressBarControl progressBar = new ProgressBarControl();
        LabelControl lblSubtitle = new LabelControl();
        LabelControl lblTitle = new LabelControl();
        LabelControl lblFooterLeft = new LabelControl();
        LabelControl lblFooterRight = new LabelControl();
        public frmCustomSplash(bool manage)
        {
            InitializeComponent();
            InitializeFade();
            this.BackColor = Color.FromArgb(16, 110, 190);
            this.ShowInTaskbar = false;
            this.TopMost = true;

            lblTitle.Text = "EntegreF";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 80;

            lblFooterRight.Text = $"{Properties.Settings.Default.CompanyName}";
            lblFooterRight.ForeColor = Color.WhiteSmoke;
            lblFooterRight.Dock = DockStyle.Top;
            lblFooterRight.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            //lblFooterRight.Padding = new Padding(10);

            lblFooterLeft.Text = "EntegreF® 2023 Tüm Hakları Saklıdır.";
            lblFooterLeft.ForeColor = Color.WhiteSmoke;
            lblFooterLeft.Dock = DockStyle.Bottom;
            lblFooterLeft.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            //lblFooterLeft.Padding = new Padding(10);


            lblSubtitle.Text = "Başlatılıyor...";
            lblSubtitle.Font = new Font("Segoe UI", 11F);
            lblSubtitle.ForeColor = Color.WhiteSmoke;
            lblSubtitle.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            lblSubtitle.Dock = DockStyle.Bottom;
            lblSubtitle.Height = 40;
            lblSubtitle.Padding = new Padding(10);

            MarqueeProgressBarControl marquee = new MarqueeProgressBarControl();
            marquee.Dock = DockStyle.Bottom;
            marquee.Properties.ShowTitle = false;
            marquee.Properties.ProgressAnimationMode = ProgressAnimationMode.Cycle;
            marquee.BackColor = Color.Transparent;

            progressBar.Dock = DockStyle.Bottom;
            progressBar.Height = 20;
            progressBar.Properties.Minimum = 0;
            progressBar.Properties.Maximum = 100;
            progressBar.Properties.ShowTitle = true;
            progressBar.Properties.PercentView = true;
            progressBar.Properties.Appearance.BackColor = Color.Transparent;
            progressBar.Properties.Appearance.ForeColor = Color.White;
            btnCancel.Text = "İptal";

            this.Controls.Add(lblFooterRight);
            this.Controls.Add(lblFooterLeft);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(marquee);
            this.Controls.Add(progressBar);
            if (manage)
            {
                marquee.Visible = false;
                
            }
            else
            {
                progressBar.Visible = false;
            }
        }
        public void SetMaxValue(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetMaxValue(value)));
                return;
            }
            progressBar.Position = 0;
            progressBar.Properties.Minimum = 0;
            progressBar.Properties.Maximum = value;
        }
        public void SetProgresName(string uygulama)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetProgresName(uygulama)));
                return;
            }
            lblFooterRight.Text = $"{Properties.Settings.Default.CompanyName} { uygulama}";
        }
        public void SetProgress2(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetProgress2(message)));
                return;
            }
            lblSubtitle.Text = message;
        }
        public void SetProgress(int value, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetProgress(value, message)));
                return;
            }

            progressBar.Position = value;
            lblSubtitle.Text = message;
        }
        private void InitializeFade()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 15;
            fadeTimer.Tick += (s, e) =>
            {
                if (fadeIn)
                {
                    if (Opacity < 0.9)
                        Opacity += 0.05;
                    else
                        fadeTimer.Stop();
                }
                else
                {
                    if (Opacity > 0)
                        Opacity -= 0.05;
                    else
                    {
                        fadeTimer.Stop();
                        Close();
                    }
                }
            };
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            fadeIn = true;
            fadeTimer.Start();
        }
        public void CloseWithFade()
        {
            fadeIn = false;
            fadeTimer.Start();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts == null || _cts.IsCancellationRequested)
                return;

            _cts.Cancel();
            lblSubtitle.Text = "İşlem iptal ediliyor...";
            btnCancel.Enabled = false;
        }
    }
}