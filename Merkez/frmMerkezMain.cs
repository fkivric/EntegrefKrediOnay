using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using EntegreFDLL.Class;
using Newtonsoft.Json;
using static EntegreFDLL.Class.VolantApiClass;
using DevExpress.XtraSplashScreen;
using System.Threading;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using EntegreFDLL;
using EntegrefKrediOnay.Class;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using ImageMagick;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmMerkezMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        EntegreFDLL.Class.BGClass GetBGClass = new EntegreFDLL.Class.BGClass();
        public static LoginResponse LoginServisDonus = new LoginResponse();
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql = Properties.Settings.Default.connectionstring;
        public frmMerkezMain()
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.CompanyName, "Saha Yönetim Destek Tools Açılıyor");
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
                string bg = conn.GetValue($@"select count(*) from SOCIAL where SODEPART = '017' and  SOCODE = '{Entegref.GetLogins.userID}'",sql);
                if (bg == "0")
                {
                    ribbon.ApplicationButtonDropDownControl = backstageViewControl1;
                    //frmBGSec sec = new frmBGSec();
                    //sec.ShowDialog();
                }
                else
                {
                    backstageViewControl1.Visible = false;
                    ribbon.ApplicationButtonDropDownControl = null;
                }
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
        }
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
                        //XtraMessageBox.Show("Her oturum açıldığında 1 işlem yapacak. Eğer bu girişteki ilk işlemse uygulama çalışmaktadır. Lütfen Bekleyiniz");
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
                        y.Cancel = true; string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                        CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        public static List<RManagement> RManagements = new List<RManagement>();
        public static List<RSocial> RSocials = new List<RSocial>();
        private async void frmBGMain_Load(object sender, EventArgs e)
        {
            Program.FBGConfigProvider.Servis = "Login";
            var sonuc = await GetBGClass.VolantServisAsync(Program.FBGConfigProvider);
            try
            {
                LoginServisDonus = JsonConvert.DeserializeObject<LoginResponse>(sonuc);
                if (LoginServisDonus != null && LoginServisDonus.success)
                {
                    RManagements.Add(LoginServisDonus.rManagement);
                    RSocials.Add(LoginServisDonus.rSocial);
                    this.Text = this.Text + " Servis Ok";
                }
                else
                {
                    var Hata = JsonConvert.DeserializeObject<LoginHata>(sonuc);
                    this.Text = this.Text + " " + Hata.message;
                    CustomMessageBox.ShowMessage(Hata.message, "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ribbon.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Program.FBGConfigProvider.Servis = "GetDigitalArchilveDownload";
            Program.FBGConfigProvider.filter.directory = "/384/N Kolay Şirket Hesabına Para Yatırma API Teknik Dokümanı v1.1.pdf";
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{Program.FBGConfigProvider.basicAuthUsername}:{Program.FBGConfigProvider.basicAuthPassword}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var json = new StringContent(JsonConvert.SerializeObject(Program.FBGConfigProvider.filter), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://fatihkivric.com.tr:1930/api/Investigation/GetDigitalArchilveDownload", json);
                if (response.Content.Headers.ContentType?.MediaType == "application/octet-stream")
                {
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(@"C:\Users\FatihKivric\Desktop\indirilen.pdf", fileBytes);
                }
                else
                {
                    // Hata durumu — JSON response
                    var errorJson = await response.Content.ReadAsStringAsync();
                    // hata işle...
                }
            }
        }
        public void OpenTabForm(Form form)
        {
            bool isFormOpen = false;
            // İlgili formun açık olup olmadığını kontrol etmek için bir bayrak kullanıyoruz.

            for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
            {

                if (xtraTabControl.TabPages[i].Text == form.Text)
                {
                    isFormOpen = true;
                    // Aynı isme sahip bir formun açık olduğunu belirledik.
                    break;
                }
            }
            if (isFormOpen)
            {
                XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                args.AutoCloseOptions.Delay = 2000;
                args.Caption = "Uyarı";
                args.Text = form.Text + "Açık.";
                args.Buttons = new DialogResult[] { DialogResult.OK };
                args.AutoCloseOptions.ShowTimerOnDefaultButton = true;
                form.Focus();
                XtraMessageBox.Show(args).ToString();
                form.Close();
                form.Dispose();
                return;
                // Form zaten açıksa kullanıcıyı uyar ve işlemi sonlandır.
            }
            else
            {
                int imageIndex;
                if (!int.TryParse(form.Tag?.ToString(), out imageIndex))
                {
                    imageIndex = 0; // Eğer dönüşüm başarısız olursa imageIndex'i 0 olarak ayarlar
                }

                XtraTabPage newTabPage = new XtraTabPage();
                newTabPage.Text = form.Text;
                newTabPage.ImageIndex = imageIndex;
                xtraTabControl.TabPages.Add(newTabPage);
                xtraTabControl.SelectedTabPageIndex = xtraTabControl.TabPages.Count - 1;
                form.MdiParent = this;
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.Parent = xtraTabControl.TabPages[xtraTabControl.TabPages.Count - 1];
                form.Show();
            }
        }
        //bool FormMode = false;
        private void xtraTabControl_CloseButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (xtraTabControl.SelectedTabPage.Name == "anaSayfaTab")
                {
                    return;
                }
                if (xtraTabControl.SelectedTabPage.Controls.Count > 0)
                {
                    foreach (object item in xtraTabControl.SelectedTabPage.Controls)
                    {
                        if (item is Form && item is Form frm)
                        {
                            frm.Close();
                            frm.Dispose();
                        }
                    }
                }
                xtraTabControl.SelectedTabPage.Controls[0].Dispose();
            }
            catch
            {
            }
            xtraTabControl.TabPages.Remove(xtraTabControl.SelectedTabPage);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }
        public void xtraTabControlSizeRule()
        {
            try
            {
                for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
                {
                    Application.DoEvents();
                    xtraTabControl.TabPages[i].Width = xtraTabControl.Width;
                    xtraTabControl.TabPages[i].Height = xtraTabControl.Height;
                    foreach (Control item in xtraTabControl.TabPages[i].Controls)
                    {
                        Application.DoEvents();
                        xtraTabControl.TabPages[i].Controls.Remove(item);
                        if (item is Form frm)
                        {
                            frm.Dock = DockStyle.Fill;
                            frm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                            frm.AutoSize = false;
                            frm.Size = new Size(xtraTabControl.TabPages[i].Width, xtraTabControl.TabPages[i].Height);
                        }
                        xtraTabControl.TabPages[i].Controls.Add(item);
                        item.Refresh();
                        Application.DoEvents();
                    }
                    Application.DoEvents();
                }
            }
            catch
            {
            }
        }
        private void xtraTabControl_SizeChanged(object sender, EventArgs e)
        {
            if (!base.DesignMode)
            {
                xtraTabControlSizeRule();
            }
        }
        private void barBtnKredi_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmBGBekleyenOnaylar());
        }
        private void barBtnEnvanter_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGEnvanter());
        }
        private void barBtnUrunProfili_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGUrunProfili());
            
        }
        private void barBtnSatısRaporu_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGMagazaCiro());
        }
        private void barBtnKotaRaporu_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGMagazaCiroHedef());
        }
        private void barBtnGecikme_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGGecikme());
        }
        private void barBtnMasraf_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGMagazaMasraf());
        }
        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            //frmBGSec sec = new frmBGSec();
            //sec.ShowDialog();

            RManagements.Clear();
            RSocials.Clear();
            Program.FBGConfigProvider.Servis = "Login";
            var sonuc = await GetBGClass.VolantServisAsync(Program.FBGConfigProvider);
            try
            {
                LoginServisDonus = JsonConvert.DeserializeObject<LoginResponse>(sonuc);
                if (LoginServisDonus != null && LoginServisDonus.success)
                {
                    RManagements.Add(LoginServisDonus.rManagement);
                    RSocials.Add(LoginServisDonus.rSocial);
                    this.Text = this.Text + " Servis Ok";
                }
                else
                {
                    this.Text = this.Text + " Servis Fail";
                }
                List<XtraTabPage> lRemovePages = new List<XtraTabPage>();
                for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
                {
                    foreach (object item2 in xtraTabControl.TabPages[i].Controls)
                    {
                        if (item2 is Form && item2 is Form frm)
                        {
                            frm.Dispose();
                        }
                    }
                    lRemovePages.Add(xtraTabControl.TabPages[i]);
                }
                foreach (XtraTabPage item in lRemovePages)
                {
                    xtraTabControl.TabPages.Remove(item);
                }
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();

            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void barBtnMusteriKredi_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGKrediAc());
        }
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGMagazaKasa());
        }
        private void barBtnDepoMagaza_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmMagazaDepoKistas());
        }
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmBGOnayRaporu());
            //CustomMessageBox.ShowMessage("Hazırlanıyor", "", this, "Sabırrrr....", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void barBtnSayimRaporu_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new Prim.SayımRapor());
        }
        private void barBtnSayimAnlik_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new Prim.frmSayim());
        }
        private void barBtnSayimBaslat_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new Sayım.frmSayimYap());
        }

        private void barBtnTeslimatRaporu_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGMagazaTeslimat());
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmBGRiskUrunAyarlar());
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmBGAracYakitRaporu());
        }

        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmBGAIParametre());
        }

        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmMagazaBuyume());
        }

        private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            //OpenTabForm(new frmMagazaBuyumeGroup());
        }

        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmSatisAl());
        }

        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmKullanici());
        }

        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmSatici());
        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmStok());
        }

        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Entegref.GetLogins.userID == "FK" || Entegref.GetLogins.userID == "fk")
            {
                OpenTabForm(new frmUrunTemelGrup());
            }
            else
            {
                CustomMessageBox.ShowMessage("Sadece EntegreF üzerinden Eklenebilir", "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}