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
using DevExpress.LookAndFeel;
using System.Data.SqlClient;
using System.Threading;
using System.Net.Mail;
using Microsoft.Win32;
using System.Net;
using EntegrefKrediOnay.Class;
using EntegreFDLL;
using EntegreFDLL.Class;

namespace EntegrefKrediOnay
{
    public partial class frmNewMessageBox : XtraForm
    {
        string ProductName = "";
        public string Detaynetini;
        static bool Eror;
        public frmNewMessageBox()
        {
            ProductName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); // proje adı
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
            DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
            defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            InitializeComponent();
            baslangicZamani = DateTime.Now;
            timer1.Start();
            this.TopMost = true;
        }
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        SqlConnectionObject conn = new SqlConnectionObject();
        //arka plan işlemleri
        private BackgroundWorker _backgroundWorker;
        private ManualResetEvent _workerCompletedEvent = new ManualResetEvent(false);
        private const string READY_TEXT = "Hazır";
        private void executeBackground(Action doWorkAction, Action progressAction = null, Action completedAction = null)
        {
            try
            {
                if (_backgroundWorker != null)
                {


                    if (_backgroundWorker.IsBusy)
                    {
                        XtraMessageBox.Show("Her oturum açıldığında 1 işlem yapacak. Eğer bu girişteki ilk işlemse uygulama çalışmaktadır. Lütfen Bekleyiniz");
                        return;
                    }
                }
                _backgroundWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _backgroundWorker.DoWork += (x, y) =>
                {
                    try
                    {
                        doWorkAction.Invoke();
                    }
                    catch (Exception ex)
                    {
                        y.Cancel = true;
                        XtraMessageBox.Show("Bilinmeyen Hata. Detay : " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // throw;
                    }
                };
                if (progressAction != null)
                {
                    _backgroundWorker.ProgressChanged += (x, y) =>
                    {
                        progressAction.Invoke();
                    };
                }
                if (completedAction != null)
                {
                    _backgroundWorker.RunWorkerCompleted += (x, y) =>
                    {
                        completedAction.Invoke();
                    };
                }
                _backgroundWorker.RunWorkerAsync();
            }
            catch (Exception)
            {

            }

        }
        private void completeProgress()
        {
            try
            {
                _backgroundWorker.Dispose();
                _backgroundWorker = null;
                if (!this.Enabled)
                {
                    this.Enabled = true;
                }

            }
            finally
            {
                //this.Cursor = Cursors.Default;
                _workerCompletedEvent.Set();

            }
        }
        private bool IsRtf(string text)
        {
            // RTF metinler genellikle {\rtf1 ile başlar
            if (string.IsNullOrWhiteSpace(text)) return false;
            string trimmed = text.TrimStart();
            return trimmed.StartsWith(@"{\rtf");
        }

        public Image MessageIcon
        {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }
        public string Message
        {
            get { return lblMessage.Rtf; }
            set
            {
                if (IsRtf(value))
                {
                    lblMessage.Rtf = value;
                }
                else
                {
                    lblMessage.Text = value;
                }
            }
        }

        public string detay
        {
            get { return rchtxt.Rtf; }
            set
            {
                if (IsRtf(value))
                {
                    rchtxt.Rtf = value;
                }            
                else
                {
                    rchtxt.Text = value;
                }
            }
        }
        public bool eror
        {
            get { return Eror; }
            set { Eror = value; }
        }

        public static string pcİsmi;
        public static string pcModeli;
        public static string Cpuid;
        public static string Motherboardid;
        public static string ComputerUUID;
        public static string ComputerLisansingID;
        private void frmMessageBox_Load(object sender, EventArgs e)
        {
            panelControl1.Visible = false;
            this.Size = new Size(549, 190);

            if (rchtxt.Text != "")
            {
                btnDetay.Enabled = true;
            }
            else
            {
                btnDetay.Enabled = false;
                tablePanel2.Columns[1].Visible = false;
            }
            if (lblMessage.Text == "Lütfen Bekleyin" )
            {
                btnOk.Enabled = false;
                btnDetay_Click(null, null);
            }
            else
            {
                btnDetay.Enabled = true;
            }
            if (!Eror)
            {
                tablePanel2.Columns[2].Visible = false;
            }
            Detaynetini = rchtxt.Text;
            pcİsmi = frmLogin.pcİsmi;
            pcModeli = frmLogin.pcModeli;
            ComputerUUID = frmLogin.ComputerUUID;
            ComputerLisansingID = frmLogin.ComputerLisansingID;
        }
        int totalHeight = 0;
        private void btnDetay_Click(object sender, EventArgs e)
        {
            if (totalHeight == 0)
            {
                int firstCharIndex = rchtxt.GetCharIndexFromPosition(new Point(0, 0));
                int firstLine = rchtxt.GetLineFromCharIndex(firstCharIndex);

                int lastCharIndex = rchtxt.GetCharIndexFromPosition(new Point(rchtxt.Width - 1, rchtxt.Height - 1));
                int lastLine = rchtxt.GetLineFromCharIndex(lastCharIndex);

                int lineCount = lastLine - firstLine + 1;

                int firstLineHeight = rchtxt.GetPositionFromCharIndex(rchtxt.GetFirstCharIndexFromLine(firstLine)).Y;
                int lastLineHeight = rchtxt.GetPositionFromCharIndex(rchtxt.GetFirstCharIndexFromLine(lastLine)).Y;

                totalHeight = lastLineHeight - firstLineHeight + (rchtxt.Font.Height * (lineCount));
            }

            // Get the current DPI scale factor of the screen
            float dpiScale = this.CreateGraphics().DpiX / 96f; // 96 is the default DPI at 100% scale

            // Adjust totalHeight based on the DPI scaling factor
            totalHeight = (int)(totalHeight * dpiScale);

            var boyut = this.Size.Height;
            if (boyut == 190)
            {
                if (rchtxt.Text != "")
                {
                    // Apply DPI scaling to the height of the form
                    AdjustFormSize(549, boyut+totalHeight);
                    //this.Size = new Size((int)(549 * dpiScale), (int)(boyut + totalHeight));
                    panelControl1.Visible = true;
                }
            }
            else
            {
                // Adjust for DPI scaling in the else branch as well
                AdjustFormSize((int)(549 * dpiScale), (int)(190));
                //this.Size = new Size((int)(549 * dpiScale), (int)(190 * dpiScale));
                panelControl1.Visible = false;
            }
            //var boyut = this.Size.Height;
            //if (boyut == 190)
            //{
            //    if (rchtxt.Text != "")
            //    {
            //        this.Size = new Size(549, boyut+ totalHeight);
            //        panelControl1.Visible = true;
            //    }
            //}
            //else
            //{
            //    this.Size = new Size(549, 190);
            //    panelControl1.Visible = false;
            //}

        }

        private void AdjustFormSize(int w, int h)
        {
            float scale = GetScreenScale();
            int width = (int)(w * scale);
            int height = (int)(h * scale);
            this.Size = new Size(width, height);
        }

        private float GetScreenScale()
        {
            using (Graphics g = this.CreateGraphics())
            {
                return g.DpiX / 96.0f;
            }
        }
        int kapanis = 5;
        private DateTime baslangicZamani;
        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan gecenSure = DateTime.Now - baslangicZamani;

            // Saat, dakika ve saniye bilgilerini alıyoruz
            int saat = gecenSure.Hours;
            int dakika = gecenSure.Minutes;
            int saniye = gecenSure.Seconds;

            // Label üzerine saat bilgisini yazdırma
            lblTime.Text = $"{saat:D2}:{dakika:D2}:{saniye:D2}";
            lblsimdi.Text = DateTime.Now.ToString("HH:mm:ss");
            if (lblMessage.Text == "Lütfen Bekleyin: ")
            {
                if (kapanis  >= saniye)
                {
                    this.Close();
                    this.Dispose();
                }
                else
                {
                    btnOk.Text = "Kapat (" + (kapanis - saniye).ToString() + ")";
                }
            }
            //if (rchtxt.Text == "Volant Sunucu için Diğer Tanımlı Erişim Adresleri Deneniyor....")
            //{
            //    TimeSpan gecenSure = DateTime.Now - baslangicZamani;

            //    // Saat, dakika ve saniye bilgilerini alıyoruz
            //    int saat = gecenSure.Hours;
            //    int dakika = gecenSure.Minutes;
            //    int saniye = gecenSure.Seconds;

            //    // Label üzerine saat bilgisini yazdırma
            //    lblTime.Text = $"{saat:D2}:{dakika:D2}:{saniye:D2}";
            //    if (saniye >= 2)
            //    {
            //        btnOk.Enabled = true;
            //    }

            //}
            //else
            //{
            //    lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
            //}
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = 1,
                Position = 0,
                ToplamAdet = 1.ToString(),
            };
            int success = 0;
            int error = 0;
            var DIVNAME = conn.GetValue($"select DIVNAME from DIVISON where DIVVAL = '{Entegref.GetLogins.userDIVVAL}'",Properties.Settings.Default.connectionstring);
            string sonuc = "";
            executeBackground(
       () =>
       {
           progressForm.Show(this);
           try
           {
               // Alıcı ve gönderen
               string gonderen = Properties.Settings.Default.MailAdress;
               string sifre = Properties.Settings.Default.MailPassword;

               // MailMessage ayarları
               MailMessage mail = new MailMessage();
               mail.From = new MailAddress(gonderen, ProductName);
               mail.To.Add("fatihkivric@yonavm.com.tr");
               mail.To.Add("ismailkelem@yonavm.com.tr");
               mail.To.Add("ahmetyoldas@yonavm.com.tr");
               mail.To.Add("eyupkaya@yonavm.com.tr");
               mail.Subject = $"{ProductName} Hata Bildirimi...";
               // HTML gövde oluştur
               var sb = new StringBuilder();
               sb.AppendLine("<html><body style=\"font-family:Calibri,Arial,sans-serif; font-size:11pt;\">");
               sb.AppendLine($"<p><span style=\"font-weight:bold; font-size:18pt;\">BİLGİSAYAR ADI = {pcİsmi}</span></p>");
               sb.AppendLine($"<p><span style=\"font-weight:bold; font-size:18pt;\">BİLGİSAYAR MODELİ = {pcModeli}</span></p>");
               sb.AppendLine($"<p><span style=\"font-weight:bold; font-size:18pt;\">PROGRAM LİSANS ID = {ComputerLisansingID}</span></p>");
               sb.AppendLine($"<p>OLUŞAN HATA = {Detaynetini.ToString()};</p>");
               //sb.AppendLine($"<p><span style=\"font-weight:bold; font-size:18pt;\">Montaj Numarası = {montajNo}</span></p>");
               

               sb.AppendLine("<p>İyi çalışmalar,</p>");
               sb.AppendLine($"<p><em>{Entegref.GetLogins.userDIVVAL} kodlu {DIVNAME} mağaza / {Entegref.GetLogins.userID}</em></p>");
               sb.AppendLine("</body></html>");

               mail.Body = sb.ToString();
               mail.IsBodyHtml = true;
               mail.BodyEncoding = Encoding.UTF8;
               //mail.SubjectEncoding = Encoding.UTF8;

               // SMTP (Yandex)
               // SmtpClient doğru sıra
               SmtpClient smtp = new SmtpClient();
               smtp.Host = "smtp.yandex.com";
               smtp.Port = 587;
               smtp.EnableSsl = true;

               // !!! ÖNCE kimlik doğrulama ver
               smtp.Credentials = new NetworkCredential(gonderen, sifre);

               smtp.Send(mail);
               progressForm.PerformStep(this);
               sonuc = "Mail başarıyla gönderildi!";
               progressForm.PerformStep(this);
           }
           catch (Exception ex)
           {
               progressForm.PerformStep(this);
           }
       },
                              null,
                              () =>
                              {
                                  completeProgress();
                                  progressForm.Hide(this);
                                  if (sonuc == "Mail başarıyla gönderildi!")
                                  {
                                      this.Close();
                                      this.Dispose();
                                  }
                              });

        }
    }
}