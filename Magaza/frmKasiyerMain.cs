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
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using EntegreFDLL;
using EntegrefKrediOnay.Class;
using System.Threading;
using EntegreFDLL.Class;
using static EntegreFDLL.Class.DataTableClass;
using System.IO;

namespace EntegrefKrediOnay.Magaza
{
    public partial class frmKasiyerMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmKasiyerMain()
        {
            InitializeComponent();
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
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql = Properties.Settings.Default.connectionstring;
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
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmMusteri());
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenTabForm(new frmIlkMusteri());
        }
        Entegref GetEntegref = new Entegref();
        private void frmKasiyerMain_Load(object sender, EventArgs e)
        {
            //string[] alicilar = { "fatihkivric@yonavm.com.tr" };
            //string[] seçiliDurum;
            //string konu = $"Müşteri Satış Onayı Sonuçlandırıldı";
            //string htmlBody = $@"
            //    <html><body style='font-family:Calibri; font-size:11pt;'>
            //    <p><b>Müşteri Kodu = Fatih KIVRIÇ:</b></p>
            //    <p><b>Müşteri Adı = KIVRIÇ</b></p>";
            //htmlBody = htmlBody + $@"<p><b>Satış Tarihi 2026-07-25</b></p> ";
            //htmlBody = htmlBody + $@"<p><b>Bölge Müdürü İşlem Onay Notu = Bir Sürü söz var</b></p>
            //    <p><b>İyi çalışmalar,</b></p>
            //    </body></html>";
            //var ekler = new List<string>();

            //using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //{
            //    // 2. Enable multi-file selection
            //    openFileDialog.Multiselect = true;

            //    // 3. Optional configurations
            //    openFileDialog.Title = "Select Multiple Files";
            //    openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            //    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //    // 4. Show the dialog and check if the user clicked OK
            //    if (openFileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        // 5. Use FileNames (plural) to loop through the selected paths
            //        foreach (string filePath in openFileDialog.FileNames)
            //        {
            //            string fileName = Path.GetFileName(filePath);
            //            ekler.Add(filePath);
            //            //MessageBox.Show($"Selected: {fileName}\nPath: {filePath}");
            //        }
            //    }
            //}
            //List<string> Gonderenler = new List<string>();
            //Gonderenler.Add(Entegref.GetLogins.MailAdress);
            //Gonderenler.Add(Entegref.GetLogins.MailPassword);
            //Gonderenler.Add("Akçaylar Ticaret A.Ş");
            //string[] gonderen = Gonderenler.ToArray();
            //MailRequest request = new MailRequest();
            //request.To = alicilar;  //new string[] { "ismailkelem@yonavm.com.tr", "fatihkivric@yonavm.com.tr"};//alicilar;
            //request.Sender = gonderen;
            //request.Body = htmlBody;
            //request.Subject = konu;
            //request.MailHost = Entegref.GetLogins.MailHost;
            //request.MailPort = Entegref.GetLogins.MailPort;
            //request.AttachmentPaths = ekler;
            //var result = await GetEntegref.SendMail(request);
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmMail mail = new frmMail();
            mail.ShowDialog();
        }
    }
}