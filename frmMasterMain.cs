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
using DevExpress.XtraSplashScreen;
using DevExpress.LookAndFeel;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using System.Threading;
using Newtonsoft.Json;
using EntegreFDLL.Class;
using EntegreFDLL;
using static EntegreFDLL.Class.VolantApiClass;
using EntegrefKrediOnay.Class;

namespace EntegrefKrediOnay
{
    public partial class frmMasterMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmMasterMain()
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
                string bg = conn.GetValue($@"select count(*) from SOCIAL where SODEPART = '017' and  SOCODE = '{Entegref.GetLogins.userID}'",sql1);
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
        }
        EntegreFDLL.Class.BGClass GetBGClass = new EntegreFDLL.Class.BGClass();
        public static LoginResponse LoginServisDonus = new LoginResponse();
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
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
                        CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        public static VolantApiClass.Filter filter = new VolantApiClass.Filter();
        EntegreFBGConfigProvider BGConfigProvider = new EntegreFBGConfigProvider();
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
        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            //frmBGSec sec = new frmBGSec();
            //sec.ShowDialog();
            BGConfigProvider.baseLoginUrl = Program.configProvider.baseUrl;
            BGConfigProvider.basicAuthUsername = Entegref.GetLogins.userID;
            BGConfigProvider.Company = Program.configProvider.CompanyName;
            BGConfigProvider.port = "1930";
            BGConfigProvider.Servis = "Login";
            RManagements.Clear();
            RSocials.Clear();
            BGConfigProvider.filter.username = Entegref.GetLogins.userID;
            BGConfigProvider.filter.password = Entegref.GetLogins.userPass;
            BGConfigProvider.filter.soCode = Entegref.GetLogins.userID;
            var sonuc = await GetBGClass.VolantServisAsync(BGConfigProvider);
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
                CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void frmMasterMain_Load(object sender, EventArgs e)
        {
            if (Entegref.GetLogins.userDEPART == "014")
            {
                var sonuc = await GetBGClass.VolantServisAsync(BGConfigProvider);
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
                }
                catch (Exception ex)
                {
                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void barBtnKredi_Click(object sender, EventArgs e)
        {
        }

        private void barBtnMusteriKredi_Click(object sender, EventArgs e)
        {
        }

        private void barBtnSayimBaslat_Click(object sender, EventArgs e)
        {
        }
    }
}