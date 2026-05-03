using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntegreFDLL
{
    public partial class ProgressBarFrm : DevExpress.XtraEditors.XtraForm
    {
        private DateTime baslangicZamani;
        public ProgressBarFrm()
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
            DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
            defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            InitializeComponent();
            this.ShowInTaskbar = false;
            //this.Opacity = 0.6;
            //this.BackColor = Color.FromArgb(16, 110, 190);
            //panelControl1.Appearance.BackColor = Color.FromArgb(16, 110, 190);
            //lblDetay.Appearance.BackColor = Color.FromArgb(16, 110, 190);
            baslangicZamani = DateTime.Now;
            this.Start = 0;
            this.Finish = 100;
            this.Position = 0;
            this.ToplamAdet = "";
            this.Islem = "";
            this.Detay = "";
            prgProgress.Properties.Step = 1;
        }
        public string ToplamAdet
        {
            get
            {
                return lblAdet.Text;
            }
            set
            {
                lblAdet.Text = value;
            }
        }
        public string Detay
        {
            get
            {
                return lblDetay.Text;
            }
            set
            {
                lblDetay.Text = value;
            }
        }
        public string Islem
        {
            get
            {
                return lblIslem.Text;
            }
            set
            {
                lblIslem.Text = value;
            }
        }
        public int Start
        {
            get
            {
                return prgProgress.Properties.Minimum;
            }
            set
            {
                prgProgress.Properties.Minimum = value;
            }
        }

        public int Finish
        {
            get
            {
                return prgProgress.Properties.Maximum;
            }
            set
            {
                prgProgress.Properties.Maximum = value;
            }
        }

        public int Position
        {
            get
            {
                return prgProgress.Position;
            }
            set
            {
                prgProgress.Position = value;
            }
        }
        public void UpdateProgress(int current, int total)
        {
            prgProgress.Properties.Maximum = total;
            prgProgress.EditValue = current;
            lblIslem.Text = $"{total}";
            lblAdet.Text = $"{current}";
        }
        public void UpdateDetails(string details)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateDetails(details)));
                return;
            }
            lblDetay.Text = details;
            this.Refresh(); // Görseli yenilemek için
        }
        public void PerformStep(Form owner)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => PerformStep(owner)));
                return;
            }
            else if (owner.InvokeRequired)
            {
                owner.Invoke(new Action(() => PerformStep(owner)));
                return;
            }
            else
            {
                prgProgress.PerformStep();
                lblIslem.Text = prgProgress.Position.ToString();
                this.Refresh();
            }
        }

        public void Show(Form owner)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => Show(owner)));
                return;
            }
            else if (owner.InvokeRequired)
            {
                owner.Invoke(new Action(() => Show(owner)));
                return;
            }
            else
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(owner.Location.X + (owner.Width - this.Width) / 2, owner.Location.Y + (owner.Height - this.Height) / 2);
                base.Show(owner);
            }
        }

        public void Hide(Form owner)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => Hide(owner)));
                return;
            }
            else if (owner.InvokeRequired)
            {
                owner.Invoke(new Action(() => Hide(owner)));
                return;
            }
            else
            {
                base.Hide();
                this.Refresh();
            }
        }
        private void ProgressBarFrm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan gecenSure = DateTime.Now - baslangicZamani;

            // Saat, dakika ve saniye bilgilerini alıyoruz
            int saat = gecenSure.Hours;
            int dakika = gecenSure.Minutes;
            int saniye = gecenSure.Seconds;

            // Label üzerine saat bilgisini yazdırma
            lblZaman.Text = $"{saat:D2}:{dakika:D2}:{saniye:D2}";
        }

        private void lblZaman_Click(object sender, EventArgs e)
        {

        }
    }
}