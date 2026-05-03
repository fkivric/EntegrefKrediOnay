using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;
using System.Reflection.Emit;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraEditors.Design;
using Microsoft.Win32;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Globalization;
using FoxLearn.License;
using DevExpress.LookAndFeel;
using System.Threading;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Security.Principal;
using EntegrefKrediOnay.Class;
using static EntegreFDLL.Class.Volant;
using static EntegreFDLL.Class.DataTableClass;
using EntegreFDLL.Class;
using EntegreFDLL;
using EntegreFDLL.Main;

namespace EntegrefKrediOnay
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        private string clientName;
        //await Class.Entegref.RunWithSplashAsync(this, false, 0, this.Text,
        //async (progress, token) =>
        //            {
        //            await Task.Delay(100, token);
        //            token.ThrowIfCancellationRequested();
        //            progress.Report((0, $"Yükleniyor... "));
        //        });
        //public static string userID;
        //public static string userPass;
        //public static string userName;
        //public static string userCHVAL;
        //public static string userDIVVAL;
        //public static string userDEPART;
        //public static string userREGION;
        //public static string NKOLAYUSER;
        //public static string NKOLAYPASS;

        public static string pcİsmi;
        public static string pcModeli;
        public static string Cpuid;
        public static string Motherboardid;
        public static string ComputerUUID;
        public static string ComputerLisansingID;
        private static string mimariGec;
        private static int mimari;
        public static string CompanyName;
        public frmLogin()
        {
            try
            {
                //Task.Run(() =>
                //{
                //    Application.Run(circular); // ayrı thread’de splash çalışır
                //});
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                InitializeComponent();
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://lisans.entegref.com/");
                RegistryKey key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\EntegrefKrediOnay");
                if (Properties.Settings.Default.EntegreFProductName != "")
                {
                    key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\{Properties.Settings.Default.EntegreFProductName}");
                }
                else
                {
                    key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\EntegrefKrediOnay");
                }
                if (!string.IsNullOrEmpty(key.GetValue("ApplicationVKN").ToString()))
                {
                    VKN = key.GetValue("ApplicationVKN").ToString();
                    pcİsmi = key.GetValue("ComputerName").ToString();
                    pcModeli = key.GetValue("ComputerID").ToString();
                    Cpuid = key.GetValue("CPU").ToString();
                    Motherboardid = key.GetValue("motherboardid").ToString();
                    ComputerUUID = key.GetValue("ComputerUUID").ToString();
                }
                else
                {
                    VKN = Properties.Settings.Default.VKN;
                    pcİsmi = key.GetValue("ComputerName").ToString();
                    pcModeli = key.GetValue("ComputerID").ToString();
                    Cpuid = key.GetValue("CPU").ToString();
                    Motherboardid = key.GetValue("motherboardid").ToString();
                    ComputerUUID = key.GetValue("ComputerUUID").ToString();
                }
                
       
                mimari = Environment.Is64BitProcess ? 64 : 86;
                mimariGec = Environment.Is64BitProcess ? "x32 Geç" : "x64 Geç";
                lblMimari.Text = $"Versiyon: x{mimari}";
                btnDegistir.Text = mimariGec;
                //this.TopMost = true;
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Properties.Settings.Default.Upgrade();

                //circular.Invoke(new Action(() => circular.Close()));
                //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }

        }
        private List<eDatabase> lDatabase;
        List<Firma> firmas = new List<Firma>();
        public static string version = "";
        public static string ProductName = "";
        public static string VKN = null;
        public static int lisansKalan = 0;
        private readonly HttpClient httpClient;
        List<VKNSettings> VKNSettings = new List<VKNSettings>();
        DB_Connection dB = new DB_Connection();
        Entegref entegreF = new Entegref();
        SqlConnection Entgref = new SqlConnection("Server=31.145.19.56;Database=Netbil_Connector; User ID=fatih;Password=05101981;");
        SqlConnectionObject conn = new SqlConnectionObject();
        public class TokenSonuc
        {
            public int statusCode { get; set; }
            public bool success { get; set; }
            public string results { get; set; }
            public string message { get; set; }
            public string internalMessage { get; set; }
            public List<Validation1> validations { get; set; }
        }
        public class Validation1
        {
            public string Field { get; set; }
            public string Message { get; set; }
        }
        public class SMSOP
        {
            public long SSOPID { get; set; }

            public string SSOPVAL { get; set; }

            public string DSSOPUSRNAME { get; set; }

            public string DSSOPPASS { get; set; }

            public bool DSSOPSTS { get; set; }

        }
        string servisAdi = "VOLANT SERVER Zamanı güncelle";
        public static EntegreFConfigProvider configProvider = new EntegreFConfigProvider();
        private async void frmLogin_LoadAsync(object sender, EventArgs e)
        {

                configProvider.VKN = VKN;
                configProvider.CompanyName = pcİsmi;
                configProvider.ComputerModeli = pcModeli.ToString();
                configProvider.ComputerUUID = ComputerUUID;
                configProvider.ComputerVersion = version;
                configProvider.ProductName = ProductName;
            
            await SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
            async (progress, token) =>
            {
                await Task.Delay(100, token);
                token.ThrowIfCancellationRequested();
                progress.Report((0, $"Yükleniyor... "));
                //await Class.Entegref.RunWithSplashAsync(this, false,
                //async (progress, token) =>
                //{
                //    await Task.Delay(100, token);
                //    VolXml();
                //    token.ThrowIfCancellationRequested();
                //    progress.Report((0, $"Yükleniyor... "));
                //});
                VolXml();
                this.Enabled = false;
                ProductName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); // proje adı            
                if (Properties.Settings.Default.EntegreFProductName != "")
                {
                    ProductName = Properties.Settings.Default.EntegreFProductName;
                }
                else
                {
                    ProductName = "EntegrefKrediOnay";
                }
                configProvider.ProductName = ProductName;
                RegistryKey key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\{ProductName}");
                //System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);


                //var sdf = Properties.Settings.Default.connectionstring;
                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.connectionstring))
                {
                    var result = await entegreF.GetCompanySettings(configProvider);
                    VKNSettings = JsonConvert.DeserializeObject<List<VKNSettings>>(result);
                    Properties.Settings.Default.connectionstring = VKNSettings[0].VolantConnectionLocal;
                    Properties.Settings.Default.Save();
                }
                CheckAndRemoveTask_FirstRunOnly();
                string[] valueNames2 = key.GetValueNames();
                try
                {
                    // Proje dizini içindeki .bat dosyanızın tam yolunu oluşturun
                    string exeYolu = Path.Combine(@"C:\EdevletServis", "VolantZaman.exe");
                    if (!File.Exists(exeYolu))
                    {
                        MessageBox.Show("Servis dosyası bulunamadı:\n" + exeYolu);
                    }
                    if (!ServisYukluMu(servisAdi))
                    {
                        ServisiYukle(exeYolu);
                    }
                    var ServisStatus = ServisDurumunuGoster(servisAdi);
                    if (ServisStatus != "Running")
                    {
                        ServisiBaslat(servisAdi);
                    }
                }
                catch (Exception ex)
                {
                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    CustomMessageBox.ShowMessage("Servis Hatası", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (!valueNames2.Contains("ComputerUUID"))
                {
                    await Task.Run(() =>
                    {
                        ComputerUUID = ComputerInfo.GetComputerId();
                    });
                    key.SetValue("ComputerUUID", ComputerUUID);
                }
                else
                {
                    if (key.GetValue("ComputerUUID").ToString() == "")
                    {
                        await Task.Run(() =>
                        {
                            ComputerUUID = ComputerInfo.GetComputerId();
                        });
                    }
                    key.SetValue("ComputerUUID", ComputerUUID);
                }
                if (!valueNames2.Contains("ComputerLisansingID"))
                {
                    key.SetValue("ComputerLisansingID", "");
                }
                if (key.GetValue("ComputerLisansingID").ToString() != "")
                {
                    ComputerLisansingID = key.GetValue("ComputerLisansingID").ToString();
                    lblProID.Text = key.GetValue("ComputerLisansingID").ToString();
                }
                else
                {
                    if (ComputerLisansingID != "" && ComputerLisansingID != null)
                    {
                        lblProID.Text = ComputerLisansingID;
                    }
                    else
                    {                        
                        string response = await entegreF.UpdateLicensingUser(configProvider);
                        List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
                        ComputerLisansingID = myDeserializedClass[0].message;
                        key.SetValue("ComputerLisansingID", myDeserializedClass[0].message);
                        lblProID.Text = myDeserializedClass[0].message;
                    }
                }
                configProvider.ComputerLisansingID = ComputerLisansingID;
                configProvider.ComputerUUID = ComputerUUID;
                if (!valueNames2.Contains("ApplicationSecretPhase"))
                {
                    var newkey = await entegreF.Newkey(configProvider);
                    key.SetValue("ApplicationSecretPhase", newkey);

                }
                else
                {
                    if (key.GetValue("ApplicationSecretPhase").ToString() == "")
                    {
                        var newkey = await entegreF.Newkey(configProvider);
                        key.SetValue("ApplicationSecretPhase", newkey);
                        Properties.Settings.Default.EntegrefSecretPhase = newkey.ToString();
                        Properties.Settings.Default.Save();
                    }
                }
                try
                {
                    DataCek();
                    if (btnNewDatabase.Enabled)
                    {
                        pnlLisans.Visible = false;
                        txtKreidPuanUser.Enabled = false;
                        txtKreidPuanPass.Enabled = false;
                        simpleButton2.Enabled = false;
                        navigationPage1.Enabled = false;
                        navigationFrame.SelectedPage = navigationPage2;
                    }
                    var Compny = conn.GetValueConnection("select distinct CATALOG_NAME from INFORMATION_SCHEMA.SCHEMATA", Properties.Settings.Default.connectionstring);
                    cmbVolantSirket.Properties.DataSource = firmas;// Compny;
                    cmbVolantSirket.Properties.DisplayMember = "COMPANYNAME";
                    cmbVolantSirket.Properties.ValueMember = "COMPANYDB";

                    cmbVolantSirket.EditValue = Compny;
                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                    {
                        System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                        //lblversion.Text = "Version : " + ad.CurrentVersion.Major + "." + ad.CurrentVersion.Minor + "." + ad.CurrentVersion.Build + "." + ad.CurrentVersion.Revision;
                        version = ad.CurrentVersion.Revision.ToString();
                        lblversion.Text = version;
                    }
                    else
                    {
                        string _s1 = Application.ProductVersion; // versiyon
                                                                 //lblversion.Text = "Version : " + _s1.ToString();
                        version = _s1;
                        lblversion.Text = version;
                    }
                    if (pcİsmi.Contains("00-BM"))
                    {
                        configProvider.baseUrl = @"http://192.168.4.8:99";
                    }
                    else
                    {
                        configProvider.baseUrl = @"http://lisans.entegref.com.tr";
                    }
                    //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseData);
                    //List<Sonuc> Guncellemesonuc = JsonConvert.DeserializeObject<List<Sonuc>>(guncelleme);
                    //await entegreF.UpdateLicensingUser(VKN, pcİsmi.ToString(), pcModeli.ToString(), ComputerUUID, version, ProductName);
                    RegistryKey key2 = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\{ProductName}");
                    string[] valueNames = key2.GetValueNames();
                    if (lblversion.Text == "label1")
                    {
                        lblversion.Text = version;
                    }
                    if (version == "")
                    {
                        version = key2.GetValue("ApplicationVersion").ToString();
                    }
                    SKGL.Validate validate = new SKGL.Validate();
                    validate.secretPhase = VKN;
                    if (key2.GetValue("ApplicationSecretPhase").ToString() != "")
                    {
                        validate.Key = key2.GetValue("ApplicationSecretPhase").ToString();
                    }
                    else
                    {
                        var newkey = await entegreF.Newkey(configProvider);
                        key2.SetValue("ApplicationSecretPhase", newkey);
                        validate.Key = newkey;
                    }
                    if (!valueNames.Contains("ComputerUUID"))
                    {
                        await Task.Run(() =>
                        {
                            ComputerUUID = ComputerInfo.GetComputerId();
                        });
                        key2.SetValue("ComputerUUID", ComputerUUID);
                    }
                    else
                    {
                        if (key2.GetValue("ComputerUUID").ToString() == "")
                        {
                            await Task.Run(() =>
                            {
                                ComputerUUID = ComputerInfo.GetComputerId();
                            });
                        }
                        key2.SetValue("ComputerUUID", ComputerUUID);
                    }
                    configProvider.ComputerUUID = ComputerUUID;
                    if (!valueNames.Contains("ComputerLisansingID"))
                    {
                        key2.SetValue("ComputerLisansingID", "");
                    }
                    if (key2.GetValue("ComputerLisansingID").ToString() != "")
                    {
                        ComputerLisansingID = key2.GetValue("ComputerLisansingID").ToString();
                        lblProID.Text = key2.GetValue("ComputerLisansingID").ToString();
                    }
                    else
                    {
                        if (ComputerLisansingID != "" && ComputerLisansingID != null)
                        {
                            lblProID.Text = ComputerLisansingID;
                        }
                        else
                        {
                            string response = await entegreF.UpdateLicensingUser(configProvider);
                            List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
                            ComputerLisansingID = myDeserializedClass[0].message;
                            key2.SetValue("ComputerLisansingID", myDeserializedClass[0].message);
                            lblProID.Text = myDeserializedClass[0].message;
                        }
                    }
                    configProvider.ComputerLisansingID = ComputerLisansingID;
                    if (Properties.Settings.Default.EntegrefSecretPhase == "")
                    {
                        Properties.Settings.Default.EntegrefSecretPhase = key2.GetValue("ApplicationSecretPhase").ToString();
                    }
                    txtLisansing2.Text = "Başlangıç Tarihi : \r\n " + validate.CreationDate.ToShortDateString();
                    txtLisansing3.Text = "Sona Erme Tarihi : \r\n " + validate.ExpireDate.ToShortDateString();
                    txtLisansing1.Text = "Kalan Gün : \r\n" + validate.DaysLeft;
                    lisansKalan = validate.DaysLeft;
                    if (validate.DaysLeft > 0)
                    {
                        pnlLisans.Visible = false;
                        this.Size = new Size(718, 320);
                        AdjustFormSize(718, 320);
                    }
                    else
                    {
                        CustomMessageBox.ShowMessage("Lütfen Bekleyin: ", "Propgram Lisanslanıyor", this, "Dikkat", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        try
                        {
                            var ApplicationSecretPhase = key2.GetValue("ApplicationSecretPhase").ToString();
                            var newkey = await entegreF.Newkey(configProvider);
                            if (ApplicationSecretPhase != newkey)
                            {
                                key2.SetValue("ApplicationSecretPhase", newkey);
                                key2.SetValue("ComputerUUID", ComputerUUID);
                                Properties.Settings.Default.EntegrefSecretPhase = newkey;
                                Properties.Settings.Default.Save();
                                CustomMessageBox.ShowMessage("Entegref Lisans Anahtarınız Güncellendi...!", "Kullanım süreniz dolan lisans anahtarı otomatik güncellendi.", this, "Dikkat", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                SKGL.Validate validate2 = new SKGL.Validate();
                                validate2.secretPhase = VKN;
                                validate2.Key = newkey.ToString();
                                txtLisansing2.Text = "Başlangıç Tarihi : \r\n " + validate2.CreationDate.ToShortDateString();
                                txtLisansing3.Text = "Sona Erme Tarihi : \r\n " + validate2.ExpireDate.ToShortDateString();
                                txtLisansing1.Text = "Kalan Gün : \r\n " + validate2.DaysLeft;
                                AdjustFormSize(718, 325);
                            }
                            else
                            {
                                CustomMessageBox.ShowMessage("Entegref ile iletişime geçerek Lütfen Lisansınızı uzatınız", "Kullanım süreniz dolmuştur. Programnı Kullanmaya devam etmek için lütfen Yeni Lisans Anahtarı Satın Alın", this, "Dikkat", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //XtraMessageBox.Show("Entegref ile iletişime geçerek Lütfen Lisansınızı uzatınız");
                                Properties.Settings.Default.EntegrefSecretPhase = "";
                                Properties.Settings.Default.Save();
                                key.SetValue("ApplicationSecretPhase", "");
                                key2.SetValue("ApplicationSecretPhase", "");
                                key.Close();
                                key2.Close();
                                System.Environment.Exit(0);
                            }
                        }
                        catch (Exception ex)
                        {
                            string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                            //System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);
                            CustomMessageBox.ShowMessage("Servis Donuşu", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Application.Exit();
                        }
                    }
                    Properties.Settings.Default.Save();
                    key.Close();
                    key2.Close();
                    try
                    {
                        ProductName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); // proje adı            
                        if (Properties.Settings.Default.EntegreFProductName != "")
                        {
                            ProductName = Properties.Settings.Default.EntegreFProductName;
                        }
                        else
                        {
                            ProductName = "EntegrefKrediOnay";
                        }
                        // Kayıt defteri alt anahtarının yolunu tanımlayın
                        string registryPath = $@"HKEY_CURRENT_USER\SOFTWARE\{ProductName}";
                        string exePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)); //AppDomain.CurrentDomain.BaseDirectory;
                        string savePath = Path.Combine(exePath, ProductName + ".reg");

                        // Reg.exe komutunu kullanarak kayıt defterini dışa aktar
                        ProcessStartInfo processInfo = new ProcessStartInfo
                        {
                            FileName = "reg.exe",
                            Arguments = $"export \"{registryPath}\" \"{savePath}\" /y",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using (Process process = Process.Start(processInfo))
                        {
                            process.WaitForExit(); // İşlemin tamamlanmasını bekle
                        }
                    }
                    catch (Exception ex)
                    {
                        string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                        System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);
                    }
                    configProvider.ProductName = ProductName;
                    var checkSonuc = await entegreF.Versiyon(configProvider);
                    List<Sonuc> myDeserializedClass3 = JsonConvert.DeserializeObject<List<Sonuc>>(checkSonuc);
                    int newVersion = int.Parse(myDeserializedClass3[0].message.Replace(".", ""));
                    int lastVersion = int.Parse(version.Replace(".", ""));
                    if (newVersion > lastVersion)
                    {
                        CustomMessageBox.ShowMessage("Program Versiyonu Güncel Değil. Update Başarısız Olursanız Merkezden Destek Alın", "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        try
                        {
                            entegreF.Securety(AppDomain.CurrentDomain.BaseDirectory);
                            if (IsInstallerFailed())
                            {
                                if (File.Exists("Kasa Update.exe"))
                                {
                                    var psi = new ProcessStartInfo { FileName = @"Kasa Update.exe", WindowStyle = ProcessWindowStyle.Normal };
                                    Process.Start(psi);
                                    Application.Exit();
                                }
                            }
                            else
                            {
                                if (File.Exists("EntegreFToolsUpdater.exe"))
                                {
                                    var psi2 = new ProcessStartInfo { FileName = @"EntegreFToolsUpdater.exe", WindowStyle = ProcessWindowStyle.Normal };
                                    Process.Start(psi2);
                                    Application.Exit();
                                }
                                //string exePath = @"EntegreFToolsUpdater.exe";
                                // Eğer exe yoksa indir
                                //if (!File.Exists(exePath))
                                //{
                                //    frmNewVersion vv = new frmNewVersion(exePath, AppDomain.CurrentDomain.BaseDirectory);
                                //    DialogResult result = vv.ShowDialog();

                                //    if (result != DialogResult.OK)
                                //    {
                                //        throw new Exception("Entegref Güncelleyici Açılmadı");
                                //    }

                                //    //using (System.Net.WebClient client = new System.Net.WebClient())
                                //    //{
                                //    //    Console.WriteLine("Dosya bulunamadı, indiriliyor...");
                                //    //    client.DownloadFile(downloadLink, exePath);
                                //    //    Console.WriteLine("İndirme tamamlandı.");
                                //    //}
                                //}

                                //ProcessStartInfo psi = new ProcessStartInfo();
                                //psi.FileName = exePath;
                                //psi.WindowStyle = ProcessWindowStyle.Normal;
                                //// Ortam değişkeni eklemek için UseShellExecute false olmalı
                                //psi.UseShellExecute = false;
                                //psi.RedirectStandardOutput = true;

                                //// Ortam değişkeni ekle
                                //psi.EnvironmentVariables["MY_VAR"] = "Entegref Güncelleme";

                                //using (Process proc = Process.Start(psi))
                                //{
                                //    string output = proc.StandardOutput.ReadToEnd();
                                //    proc.WaitForExit();
                                //    Console.WriteLine(output);
                                //}

                                ////var psi = new ProcessStartInfo { FileName = @"EntegreFToolsUpdater.exe", WindowStyle = ProcessWindowStyle.Normal };
                                //Process.Start(psi);
                            }
                            Application.Exit();
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.ShowMessage(
                                "Güncelleme Var Hata = " + ex.Message,
                                "Güncelleme var Oto güncelleme çalışmadı! Elle güncelleyiniz.\r\n Seçenekler = \r\n 1-) C:\\Program Files (x86)\\Entegref Yazılım Tic. Ltd.Şti\\EntegreF Connector\\Kasa Update.exe\r\n veya Başlat butonundan Updater aratınız.\r\n 2-) Masa Üstüne Otomatik indirilen EntegreF Tools Setup.exe Yüklemesini çalıştırınız.",
                                this, "Güncelleme Kontrol", false, MessageBoxButtons.OK, MessageBoxIcon.Information);                            
                        }
                    }
                    else
                    {
                        this.Enabled = true;
                    }
                    #region lisans kontrol eski
                    //    if (!valueNames.Contains("ApplicationSetupComplate"))
                    //    {
                    //        if (!valueNames.Contains("ComputerLisansingID"))
                    //        {
                    //            try
                    //            {
                    //                CustomMessageBox.ShowMessage("Lütfen Bekleyin: ", "Propgram Lisanslanıyor", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //                string response = await client.UpdateLicensingUser(VKN, pcİsmi.ToString(), pcModeli.ToString(), version, ProductName);
                    //                //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseData);
                    //                List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
                    //                System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + myDeserializedClass[0].message + VKN + " - " + pcİsmi.ToString() + " - " + pcModeli.ToString() + " - " + version + " - " + ProductName + Environment.NewLine);
                    //                ComputerLisansingID = myDeserializedClass[0].message;
                    //                lblProID.Text = "PrgID = " + ComputerLisansingID;
                    //                string response2 = await client.UpdateLicensing(VKN, ComputerLisansingID, Cpuid.ToString(), Motherboardid.ToString(), ProductName);
                    //                List<Sonuc> myDeserializedClass2 = JsonConvert.DeserializeObject<List<Sonuc>>(response2);
                    //                key2.SetValue("ComputerLisansingID", myDeserializedClass2[0].message);
                    //                key2.SetValue("ApplicationSetupComplate", true);
                    //                if (!valueNames.Contains("ApplicationSecretPhase"))
                    //                {
                    //                    //string SecretPhase = key2.GetValue("ApplicationSecretPhase").ToString();

                    //                    //if (string.IsNullOrEmpty(SecretPhase))
                    //                    //{
                    //                    Lisansing(VKN);
                    //                    //await Task.Delay(1000);
                    //                    RegistryKey lisans = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegrefKrediOnay");
                    //                    SKGL.Validate validate = new SKGL.Validate();
                    //                    validate.secretPhase = VKN;
                    //                    validate.Key = lisans.GetValue("ApplicationSecretPhase").ToString();
                    //                    txtLisansing2.Text = "Başlangıç Tarihi : \r\n " + validate.CreationDate.ToShortDateString();
                    //                    txtLisansing3.Text = "Sona Erme Tarihi : \r\n " + validate.ExpireDate.ToShortDateString();
                    //                    txtLisansing1.Text = "Kalan Gün : \r\n " + validate.DaysLeft;
                    //                    lisansKalan = validate.DaysLeft;

                    //                    if (Properties.Settings.Default.EntegrefSecretPhase != "")
                    //                    {
                    //                        key2.SetValue("ApplicationSetupComplate", "true");
                    //                        key2.SetValue("ApplicationSecretPhase", Properties.Settings.Default.EntegrefSecretPhase);// Properties.Settings.Default.EntegrefSecretPhase);
                    //                    }
                    //                    if (validate.DaysLeft > 0)
                    //                    {
                    //                        pnlLisans.Visible = false;
                    //                        this.Size = new Size(718, 325);
                    //                        AdjustFormSize(718, 325);
                    //                    }
                    //                    //}
                    //                }
                    //                key2.Close();
                    //                try
                    //                {
                    //                    // Kayıt defteri alt anahtarının yolunu tanımlayın
                    //                    string registryPath = @"HKEY_CURRENT_USER\SOFTWARE\EntegrefKrediOnay";
                    //                    string exePath = AppDomain.CurrentDomain.BaseDirectory;
                    //                    string savePath = Path.Combine(exePath, "EntegrefKrediOnay.reg");

                    //                    // Reg.exe komutunu kullanarak kayıt defterini dışa aktar
                    //                    ProcessStartInfo processInfo = new ProcessStartInfo
                    //                    {
                    //                        FileName = "reg.exe",
                    //                        Arguments = $"export \"{registryPath}\" \"{savePath}\" /y",
                    //                        RedirectStandardOutput = true,
                    //                        UseShellExecute = false,
                    //                        CreateNoWindow = true
                    //                    };

                    //                    using (Process process = Process.Start(processInfo))
                    //                    {
                    //                        process.WaitForExit(); // İşlemin tamamlanmasını bekle
                    //                    }
                    //                    CreatePowerShellScript(ProductName);
                    //                }
                    //                catch (Exception ex)
                    //                {
                    //                    System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);
                    //                }
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    #endregion
                    #region iptal mi değilmi bak buraya tekrar eden kod gibi geldi

                    //else
                    //{
                    //    try
                    //    {
                    //        string response = await client.UpdateLicensingUser(VKN, pcİsmi.ToString(), pcModeli.ToString(), version, ProductName);
                    //        List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
                    //        ComputerLisansingID = myDeserializedClass[0].message;
                    //        lblProID.Text = "PrgID = " + ComputerLisansingID;
                    //        string response2 = await client.UpdateLicensing(VKN, ComputerLisansingID, Cpuid.ToString(), Motherboardid.ToString(), ProductName);
                    //        List<Sonuc> myDeserializedClass2 = JsonConvert.DeserializeObject<List<Sonuc>>(response2);
                    //        key2.SetValue("ComputerLisansingID", myDeserializedClass2[0].message);
                    //        key2.SetValue("ApplicationSetupComplate", true);
                    //        CustomMessageBox.ShowMessage("Lütfen Bekleyin: ", "Propgram Lisanslanıyor", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //        SKGL.Validate validate = new SKGL.Validate();
                    //        validate.secretPhase = VKN;
                    //        validate.Key = key2.GetValue("ApplicationSecretPhase").ToString();
                    //        txtLisansing2.Text = "Başlangıç Tarihi : \r\n " + validate.CreationDate.ToShortDateString();
                    //        txtLisansing3.Text = "Sona Erme Tarihi : \r\n " + validate.ExpireDate.ToShortDateString();
                    //        txtLisansing1.Text = "Kalan Gün : \r\n" + validate.DaysLeft;
                    //        lisansKalan = validate.DaysLeft;
                    //        if (Properties.Settings.Default.EntegrefSecretPhase != "")
                    //        {
                    //            key2.SetValue("ApplicationSetupComplate", "true");
                    //            key2.SetValue("ApplicationSecretPhase", Properties.Settings.Default.EntegrefSecretPhase);// Properties.Settings.Default.EntegrefSecretPhase);
                    //        }
                    //        if (lisansKalan <= 0)
                    //        {
                    //            var ApplicationSecretPhase = key2.GetValue("ApplicationSecretPhase").ToString();
                    //            var newkey = await client.Newkey(VKN, ProductName, ComputerLisansingID, ComputerUUID);
                    //            SKGL.Validate validate2 = new SKGL.Validate();
                    //            validate2.secretPhase = VKN;
                    //            validate2.Key = newkey.ToString();
                    //            if (validate2.DaysLeft > 0)
                    //            {
                    //                key2.SetValue("ApplicationSecretPhase", newkey);
                    //                Properties.Settings.Default.EntegrefSecretPhase = newkey;
                    //                Properties.Settings.Default.Save();
                    //                key2.Close();
                    //                CustomMessageBox.ShowMessage("Entegref Lisans Anahtarınız Güncellendi...!", "Kullanım süreniz dolan lisans anahtarı otomatik güncellendi.", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //                Application.Exit();

                    //                #region uodate
                    //                try
                    //                {
                    //                    System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " Versiyon=> " + VKN + "-" + pcİsmi.ToString() + "-" + pcModeli.ToString() + "-" + version + "-" + ProductName + Environment.NewLine);
                    //                    string guncelleme = await client.Versiyon(VKN.ToString(), pcİsmi.ToString(), ProductName);
                    //                    List<Sonuc> guncellemesonuc = JsonConvert.DeserializeObject<List<Sonuc>>(guncelleme);

                    //                    new_version = int.Parse(guncellemesonuc[0].message.Replace(".", ""));
                    //                    last_version = int.Parse(version.Replace(".", ""));
                    //                    if (new_version > last_version)
                    //                    {
                    //                        this.Enabled = false;
                    //                        try
                    //                        {
                    //                            string pathToUpdater = @"Kasa Update.exe"; // updater.exe dosyasının adını belirtin

                    //                            ProcessStartInfo startInfo = new ProcessStartInfo
                    //                            {
                    //                                FileName = pathToUpdater,
                    //                                WindowStyle = ProcessWindowStyle.Normal // İsteğe bağlı: Pencere stili
                    //                            };

                    //                            Process.Start(startInfo);
                    //                            Application.Exit();
                    //                        }
                    //                        catch (Exception ex)
                    //                        {
                    //                            string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    //                            CustomMessageBox.ShowMessage("Güncelleme Var Hata = " + ex.Message, "Güncelleme var Oto güncelleme Çalışmadı! Elle güncelleyiniz.\r\n Seçenekler = \r\n 1-) C:\\Program Files (x86)\\Entegref Yazılım Tic. Ltd.Şti\\EntegreF Connector\\updater.exe dosya yoluna gidip güncelleme programını çalıştırınız.\r\n 2-) Başlat butonundan Updater aratarak çıkanı çalıştırınız", this, "Güncelleme Kontrol", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //                            System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine + "Güncelleme var Oto güncelleme Çalışmadı!Elle güncelleyiniz.\r\n Seçenekler = \r\n 1 -) C:\\Program Files(x86)\\Entegref Yazılım Tic.Ltd.Şti\\EntegreF Connector\\updater.exe dosya yoluna gidip güncelleme programını çalıştırınız.\r\n 2 -) Başlat butonundan Updater aratarak çıkanı çalıştırınız" + Environment.NewLine);
                    //                            this.Enabled = true;
                    //                        }
                    //                    }
                    //                }
                    //                catch (Exception ex)
                    //                {
                    //                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    //                    CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //                    System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);
                    //                }
                    //                #endregion
                    //            }
                    //            else
                    //            {
                    //                CustomMessageBox.ShowMessage("Entegref ile iletişime geçerek Lütfen Lisansınızı uzatınız", "Kullanım süreniz dolmuştur. Programnı Kullanmaya devam etmek için lütfen Yeni Lisans Anahtarı Satın Alın", this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //                XtraMessageBox.Show("Entegref ile iletişime geçerek Lütfen Lisansınızı uzatınız");
                    //                Properties.Settings.Default.EntegrefSecretPhase = "";
                    //                Properties.Settings.Default.Save();
                    //                Application.Exit();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            pnlLisans.Visible = false;
                    //            this.Size = new Size(718, 325);
                    //            AdjustFormSize(718, 325);
                    //        }
                    //        this.Enabled = true;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    //        CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //        System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);
                    //        Application.Exit();
                    //    }
                    //}
                    #endregion
                }
                catch (Exception ex)
                {
                    //entegreF.GlobalHataYakala(this, new ThreadExceptionEventArgs(ex), "Bir hata oluştu", "Hata", this);
                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    //System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + hataDetay + Environment.NewLine);
                    XtraMessageBox.Show(hataDetay, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }
        public static bool ServisYukluMu(string servisAdi)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == servisAdi);
        }
        public static void ServisiYukle(string exeYolu)
        {
            string installUtilPath = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe";

            var process = new Process();
            process.StartInfo.FileName = installUtilPath;
            process.StartInfo.Arguments = $"\"{exeYolu}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Verb = "runas"; // Yönetici olarak çalıştır

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            MessageBox.Show("Servis yüklendi:\n" + output);
        }
        public static void ServisiKaldir(string exeYolu)
        {
            string installUtilPath = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe";

            var process = new Process();
            process.StartInfo.FileName = installUtilPath;
            process.StartInfo.Arguments = $"/u \"{exeYolu}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Verb = "runas"; // Yönetici olarak çalıştır

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            MessageBox.Show("Servis kaldırıldı:\n" + output);
        }
        public static void ServisiBaslat(string servisAdi)
        {
            var sc = new ServiceController(servisAdi);
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            }
        }
        public static void ServisiDurdur(string servisAdi)
        {
            var sc = new ServiceController(servisAdi);
            if (sc.Status == ServiceControllerStatus.Running)
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
        }
        private string ServisDurumunuGoster(string servisAdi)
        {
            ServiceController sc = new ServiceController(servisAdi);
            return sc.Status.ToString();
        }

        bool IsTaskExists(string taskName)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "schtasks",
                Arguments = $"/query /tn \"{taskName}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            {
                p.WaitForExit();
                return p.ExitCode == 0; // 0 = görev var
            }
        }
        void DeleteTask(string taskName)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "schtasks",
                Arguments = $"/delete /tn \"{taskName}\" /f",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }
        bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        bool IsTaskAlreadyRemoved()
        {
            using (var key = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\{ProductName}"))
            {
                return key?.GetValue("KasaUpdateRemoved") != null;
            }
        }
        void MarkTaskAsRemoved()
        {
            using (var key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\{ProductName}"))
            {
                key.SetValue("KasaUpdateRemoved", "1");
            }
        }
        void CheckAndRemoveTask_FirstRunOnly()
        {
            string taskName = "Kasa Update";

            // Daha önce silindiyse tekrar deneme
            if (IsTaskAlreadyRemoved())
                return;

            // Admin değilse deneme
            if (!IsAdministrator())
                return;

            // Görev yoksa işaretle ve çık
            if (!IsTaskExists(taskName))
            {
                MarkTaskAsRemoved();
                return;
            }

            // Görev varsa sil
            DeleteTask(taskName);

            // Bir daha çalışmaması için işaretle
            MarkTaskAsRemoved();
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
        DateTime now = DateTime.Now;
        private async void Lisansing(string vknid)
        {
            try
            {
                if (string.IsNullOrEmpty((Properties.Settings.Default.EntegrefAIPToken)))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, "http://lisans.entegref.com/token");
                        //var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44371/token");
                        request.Content = new StringContent("grant_type=password&username=admin&password=Madam3169");

                        // İstek başlığına gerekli kimlik doğrulama bilgilerini ekleyin
                        //string clientId = "your_client_id";
                        //string clientSecret = "your_client_secret";
                        //string credentials = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(clientId + ":" + clientSecret));
                        //request.Headers.Add("Authorization", "Basic " + credentials);

                        // İsteği gönderin ve yanıtı alın
                        HttpResponseMessage responses = await client.SendAsync(request);

                        // Yanıtın başarılı olup olmadığını kontrol edin
                        if (responses.IsSuccessStatusCode)
                        {
                            string responseData = await responses.Content.ReadAsStringAsync();
                            Token myDeserializedClass = JsonConvert.DeserializeObject<Token>(responseData);
                            Properties.Settings.Default.EntegrefAIPToken = myDeserializedClass.access_token;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            MessageBox.Show("API isteği başarısız: " + responses.StatusCode, "Servis Uyarısı");
                        }
                    }
                }
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.EntegrefAIPToken);
                HttpResponseMessage response = await httpClient.GetAsync($"api/data/Lisans?VKN={vknid}&AppName={ProductName}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseData);
                    List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(responseData);
                    foreach (var item in myDeserializedClass)
                    {
                        if (item.status)
                        {
                            SKGL.Validate validate = new SKGL.Validate();
                            validate.secretPhase = VKN;
                            validate.Key = item.message;
                            Properties.Settings.Default.EntegrefSecretPhase = item.message;
                            Properties.Settings.Default.Save();
                            var assembly = typeof(Program).Assembly;
                            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                            var id = attribute.Value;
                            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegrefKrediOnay");
                            key.SetValue("ApplicationSetupComplate", "true");
                            key.SetValue("ApplicationSecretPhase", item.message);// Properties.Settings.Default.EntegrefSecretPhase);
                            key.Close();
                        }
                        else
                        {
                            MessageBox.Show(item.message, "Dikkat", MessageBoxButtons.YesNo);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(response.Content.ToString());
                    //CustomMessageBox.ShowMessage("API isteği başarısız: ", response.Content.ToString(), this, "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("HataDetay Detayı!", hataDetay, this, "Detaya Bakınız", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void VolXml()
        {
            string ConStrg = "";
            try
            {
                clientName = SystemInformation.ComputerName;
                if (!File.Exists("C:\\Program Files (x86)\\Volant Yazılım\\Volant Erp Setup\\VolErpConnection.xml"))
                {
                    throw new Exception("VolErpConnection Dosyası Eksik!");
                }
                XmlTextReader reader = new XmlTextReader("C:\\Program Files (x86)\\Volant Yazılım\\Volant Erp Setup\\VolErpConnection.xml");
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element && reader.Name == "PARAMS") || reader.NodeType != XmlNodeType.Element || !(reader.Name == "DB"))
                    {
                        continue;
                    }
                    eDatabase rDatabase = new eDatabase();
                    try
                    {
                        ConStrg = string.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=0;",
                        reader.GetAttribute("SERVERNAME").TextSifreCoz(),
                        reader.GetAttribute("DATABASE").TextSifreCoz(),
                        reader.GetAttribute("LOGIN").TextSifreCoz(),
                        reader.GetAttribute("PASSWORD").TextSifreCoz());
                        dB.Db_Server = reader.GetAttribute("SERVERNAME").TextSifreCoz();
                        dB.Db_Database = reader.GetAttribute("DATABASE").TextSifreCoz();
                        dB.Db_User = reader.GetAttribute("LOGIN").TextSifreCoz();
                        dB.Db_Password = reader.GetAttribute("PASSWORD").TextSifreCoz();

                    }
                    catch
                    {
                        ConStrg = string.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=0;",
                        reader.GetAttribute("SERVERNAME").ToString(),
                        reader.GetAttribute("DATABASE").ToString(),
                        reader.GetAttribute("LOGIN").ToString(),
                        reader.GetAttribute("PASSWORD").ToString());
                        dB.Db_Server = reader.GetAttribute("SERVERNAME").ToString();
                        dB.Db_Database = reader.GetAttribute("DATABASE").ToString();
                        dB.Db_User = reader.GetAttribute("LOGIN").ToString();
                        dB.Db_Password = reader.GetAttribute("PASSWORD").ToString();
                    }
                    Properties.Settings.Default.Company = reader.GetAttribute("DATABASE").ToString();
                    Properties.Settings.Default.Save();
                }
                Properties.Settings.Default.connectionstring = ConStrg;
                Properties.Settings.Default.connectionstring2 = ConStrg.Replace(Properties.Settings.Default.Company, Properties.Settings.Default.DbName);
                Properties.Settings.Default.Save();

                reader.Close();
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("Hata Detayı!", hataDetay, this, "Detaya Bakınız", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        void DataCek()
        {
            List<AllDataBase> allDatas = new List<AllDataBase>();
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connectionstring))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception)
                {
                    connection.ConnectionString = Properties.Settings.Default.connectionstring2;
                    connection.Open();
                }


                DataTable databases = connection.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    AllDataBase dts = new AllDataBase { DbNAme = database["database_name"].ToString() };
                    allDatas.Add(dts);
                    string databaseName = database["database_name"].ToString();
                    if (databaseName == cmbDatabase.Text || databaseName == txtKrediPuanDbName.Text)
                    {
                        cmbDatabase.Properties.DataSource = databases;
                        cmbDatabase.Properties.DisplayMember = "database_name";
                        cmbDatabase.Properties.ValueMember = "dbid";
                        cmbDatabase.EditValue = database["dbid"];
                        btnNewDatabase.Enabled = false;
                        txtKreidPuanUser.Enabled = true;
                        txtKreidPuanPass.Enabled = true;
                        simpleButton2.Enabled = true;
                        navigationPage1.Enabled = true;
                        //tablePanel4.Rows[0].Visible = true;
                        //tablePanel4.Rows[1].Visible = false;
                        //tablePanel4.Rows[2].Visible = false;
                        //tablePanel4.Rows[3].Visible = true;
                        //tablePanel4.Rows[4].Visible = true;
                        Properties.Settings.Default.DbName = cmbDatabase.Text;

                    }
                    else
                    {
                        //tablePanel4.Rows[0].Visible = false;
                        //tablePanel4.Rows[1].Visible = true;
                        //tablePanel4.Rows[2].Visible = true;
                        //tablePanel4.Rows[3].Visible = false;
                        //tablePanel4.Rows[4].Visible = false;
                    }
                    var dd = Sorgu(string.Format("select COMPANYVAL,COMPANYNAME from {0}.dbo.COMPANY", database["database_name"]), Properties.Settings.Default.connectionstring);
                    if (dd != null)
                    {
                        var ff = new Firma();
                        ff.COMPANYNAME = dd.Rows[0]["COMPANYNAME"].ToString();
                        ff.COMPANYDB = database["database_name"].ToString();
                        if (!firmas.Any(f => f.COMPANYNAME == ff.COMPANYNAME))
                        {
                            firmas.Add(ff);
                        }
                    }
                }
                connection.Close();
            }
            FirmaBilgileri();
        }
        void FirmaBilgileri()
        {
            cmbVolantMagaza.Properties.DataSource = null;
            var compnay = Properties.Settings.Default.Company;
            DataTable Divison = Sorgu("select DIVVAL,DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", Properties.Settings.Default.connectionstring);
            if (Divison == null)
            {
                Divison = Sorgu($"select DIVVAL,DIVNAME from {compnay}..DIVISON where DIVSTS = 1 and DIVSALESTS = 1", Properties.Settings.Default.connectionstring);
            }
            cmbVolantMagaza.Properties.DataSource = Divison;
            cmbVolantMagaza.Properties.DisplayMember = "DIVNAME";
            cmbVolantMagaza.Properties.ValueMember = "DIVVAL";
            cmbVolantMagaza.EditValue = Divison.Rows[0]["DIVVAL"];
            //if (VKNSettings != null)
            //{
            //    var ftp = Sorgu("select MTFTPIP,MTFTPUSER,MTFTPPASSWORD from MANAGEMENT", Settings.Default.connectionstring);
            //    Properties.Settings.Default.VolFtpHost = VKNSettings[0].VolantFtpHostOutside;
            //    Properties.Settings.Default.VolFtpUser = ftp.Rows[0]["MTFTPUSER"].ToString();
            //    Properties.Settings.Default.VolFtpPass = ftp.Rows[0]["MTFTPPASSWORD"].ToString();
            //    BGClass.baseLoginUrl = "http://volant.yonavm.com.tr:1930/api/Investigation/";
            //    var Vrk = Sorgu("select COMPANYWATNO from COMPANY", Settings.Default.connectionstring);
            //    VKN = Vrk.Rows[0]["COMPANYWATNO"].ToString();
            //}
            if (Properties.Settings.Default.connectionstring.Contains("212.174.235.106"))
            {
                var ftp = Sorgu("select MTFTPIP,MTFTPUSER,MTFTPPASSWORD from MANAGEMENT", Properties.Settings.Default.connectionstring);
                Properties.Settings.Default.VolFtpHost = "ftp://212.174.235.106:1025";
                Properties.Settings.Default.VolFtpUser = ftp.Rows[0]["MTFTPUSER"].ToString();
                Properties.Settings.Default.VolFtpPass = ftp.Rows[0]["MTFTPPASSWORD"].ToString();
                var Vrk = Sorgu("select COMPANYWATNO from COMPANY", Properties.Settings.Default.connectionstring);
                VKN = Vrk.Rows[0]["COMPANYWATNO"].ToString();
            }
            else
            {
                var ftp = Sorgu("select MTFTPIP,MTFTPUSER,MTFTPPASSWORD from MANAGEMENT", Properties.Settings.Default.connectionstring);
                Properties.Settings.Default.VolFtpHost = ftp.Rows[0]["MTFTPIP"].ToString();
                Properties.Settings.Default.VolFtpUser = ftp.Rows[0]["MTFTPUSER"].ToString();
                Properties.Settings.Default.VolFtpPass = ftp.Rows[0]["MTFTPPASSWORD"].ToString();
                var Vrk = Sorgu("select COMPANYWATNO from COMPANY", Properties.Settings.Default.connectionstring);
                VKN = Vrk.Rows[0]["COMPANYWATNO"].ToString();
            }
            Properties.Settings.Default.Save();
        }
        public static async Task<string> GetAuthToken(string apiUrl, string username, string password)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // İstek için body içeriğini JSON formatında oluşturuyoruz
                    string requestBody = $"{{ \"Username\":\"{username}\", \"Password\":\"{password}\" }}";

                    // İstek başlıklarını ayarlıyoruz
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                    // Token almak için POST isteği gönderiyoruz
                    HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(requestBody, Encoding.UTF8, "application/json"));

                    // Yanıtı okuyoruz
                    if (response.IsSuccessStatusCode)
                    {
                        // Yanıttan tokeni alıyoruz
                        string responseContent = await response.Content.ReadAsStringAsync();
                        // Tokeni döndürüyoruz
                        return responseContent;
                    }
                    else
                    {
                        // Hata durumunda uygun işlemler yapabilirsiniz
                        MessageBox.Show("Volant Token alma başarısız. KAPATIP TEKRAR AÇIN.....!!!! Hata kodu: " + response.StatusCode);
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static async void Token()
        {
            try
            {
                string username = "TICIMAX";
                string password = "TicimaxYonAvm2023!";
                string apiUrl = Properties.Settings.Default.VolantApiUrl + "/auth";
                string token = await GetAuthToken(apiUrl, username, password);
                TokenSonuc myDeserializedClass = JsonConvert.DeserializeObject<TokenSonuc>(token);
                if (myDeserializedClass.success)
                {
                    Properties.Settings.Default.VolantToken = myDeserializedClass.results;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex1)
            {
                XtraMessageBox.Show("Volant Token alırken oluşan Hata \r\n" + ex1.Message);
            }
        }
        private void cmbVolantSirket_EditValueChanged(object sender, EventArgs e)
        {
            if (cmbVolantSirket.EditValue == null)
                return;

            var builder = new SqlConnectionStringBuilder(Properties.Settings.Default.connectionstring);

            // Seçilen veritabanını ata
            builder.InitialCatalog = cmbVolantSirket.EditValue.ToString();

            // Güncellenmiş connection string
            Properties.Settings.Default.connectionstring = builder.ConnectionString;

            //Settings.Default.connectionstring = Settings.Default.connectionstring.Replace(Settings.Default.Company.ToString(), cmbVolantSirket.EditValue.ToString());            
            Properties.Settings.Default.Company = cmbVolantSirket.EditValue.ToString();
            Properties.Settings.Default.Save();
            string company = cmbVolantSirket.EditValue.ToString();
            FirmaBilgileri();
            //string imagesFolder = Path.Combine(Application.StartupPath, "Image");
            //if (Directory.Exists(imagesFolder))
            //{

            //    string[] files = Directory.GetFiles(imagesFolder);
            pictureEdit4.Visible = false;
            pictureEdit4.Image = null;
            using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.connectionstring))
            {
                sqlConnection.Open();

                // Önce direkt eşleşme
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 ResimVerisi FROM EntegreF..CompanyImages WHERE @Company LIKE '%' + ResimAdi + '%'", sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Company", company);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])result;
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            pictureEdit4.Image = Image.FromStream(ms);
                            pictureEdit4.Visible = true;
                        }
                    }
                    else
                    {
                        // Alternatif parçalı kontrol
                        string[] companyParts = company.Substring(0, company.Length-2).Split('_');
                        foreach (string part in companyParts)
                        {
                            using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 ResimVerisi FROM EntegreF..CompanyImages WHERE ResimAdi LIKE '%' + @Part + '%'", sqlConnection))
                            {
                                cmd2.Parameters.AddWithValue("@Part", part);
                                object result2 = cmd2.ExecuteScalar();

                                if (result2 != null && result2 != DBNull.Value)
                                {
                                    byte[] imageBytes = (byte[])result2;
                                    using (MemoryStream ms = new MemoryStream(imageBytes))
                                    {
                                        pictureEdit4.Image = Image.FromStream(ms);
                                        pictureEdit4.Visible = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //foreach (string file in files)
            //{
            //    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            //    byte[] imageBytes = File.ReadAllBytes(file);
            //    var control = conn.GetValue($@"select * from EntegreF..CompanyImages where ResimAdi = '{fileNameWithoutExtension}'");
            //    if (control == null)
            //    {
            //        using (SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.connectionstring2))
            //        {
            //            sqlConnection.Open();
            //            using (SqlCommand cmd = new SqlCommand("INSERT INTO CompanyImages (ResimAdi, ResimVerisi) VALUES (@ResimAdi, @ResimVerisi)", sqlConnection))
            //            {
            //                cmd.Parameters.AddWithValue("@ResimAdi", fileNameWithoutExtension);
            //                cmd.Parameters.Add("@ResimVerisi", SqlDbType.VarBinary).Value = imageBytes;
            //                cmd.ExecuteNonQuery();
            //            }
            //        }
            //    }
            //    // Değişken kısmını kontrol et ve eşleşen bir dosya bul
            //    if (company.Contains(fileNameWithoutExtension))
            //    {
            //        pictureEdit4.Visible = true;
            //        pictureEdit4.Image = Image.FromFile(file);
            //        break;
            //    }
            //    else
            //    {
            //        string[] companyParts = company.Replace("01", "").Split('_');
            //        foreach (string part in companyParts)
            //        {
            //            if (fileNameWithoutExtension.Contains(part))
            //            {
            //                pictureEdit4.Visible = true;
            //                pictureEdit4.Image = Image.FromFile(file);
            //                break;
            //            }
            //        }
            //    }
            //}
            //}

            //if (company.Contains("YON"))
            //{
            //    pictureEdit4.Visible = true;
            //    pictureEdit4.Image = Properties.Resources.YON_AVM_400;
            //}
            //else if (company.Contains("KAMALAR"))
            //{
            //    pictureEdit4.Visible = true;
            //    pictureEdit4.Image = Properties.Resources.Kamalar_logo;
            //}
            //else
            //{
            //    pictureEdit4.Visible = false;
            //    pictureEdit4.Image = null;
            //}

        }
        public DataTable Sorgu(string sorgu, string connection)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sorgu, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        Form formInstance;
        private async void simpleButton2_Click(object sender, EventArgs e)
        {
            if (txtVolantUser.Text.Contains("00-BM") || txtVolantUser.Text.Contains("00-bm"))
            {
                if (!pcİsmi.Contains("00-BM"))
                {
                    CustomMessageBox.ShowMessage("Lütfen Sadece Kendi Bilgisayarınız Üzerinden İşlem Yapınız", "", this, "Uyarı", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            // UI'de tekrar tıklamayı engelle
            simpleButton2.Enabled = false;
            this.Enabled = false;
            Type formType = null;
            //bool ekranAc = false;
            try
            {
                // 1) UI thread: Splash aç
                var splashOpts = new FluentSplashScreenOptions
                {
                    Title = "",
                    Subtitle = $@"{Properties.Settings.Default.DbName}® Tools Başlatılıyor",
                    RightFooter = "Made By Fatih KIVRIÇ",
                    LeftFooter = $"CopyRight ® 2023 {Environment.NewLine} Tüm Hakları Saklıdır.",
                    LoadingIndicatorType = FluentLoadingIndicatorType.Spinner,
                    OpacityColor = System.Drawing.Color.FromArgb(16, 110, 190),
                    Opacity = 90
                };
                splashOpts.AppearanceLeftFooter.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

                DevExpress.XtraSplashScreen.SplashScreenManager
                    .ShowFluentSplashScreen(splashOpts, parentForm: this, useFadeIn: true, useFadeOut: true);

                // 2) Ağır işleri arka planda çalıştır
                var result = await Task.Run(async () =>
                {
                    var SMSOP = conn.GetData($@"select * from SMSOP where DSSOPSTS = 1", Properties.Settings.Default.connectionstring);
                    if (SMSOP != null)
                    {
                        var Kullanici = SMSOP.ToList<SMSOP>();
                        var extend = Kullanici.FirstOrDefault(x => x.SSOPVAL == "TTMESAJ API" && x.DSSOPUSRNAME == "yonavm.api");
                        Entegref.GetLogins.SMSUname = extend.DSSOPUSRNAME;
                        Entegref.GetLogins.SMSPassword = Volant.KriptoCoz(extend.DSSOPPASS);
                    }
                    if (IsInstallerFailed())
                    {
                        return (ekranAc: false, formToOpen: new frmEntegrefSettings(), msg: "");
                    }
                    // ——— Bağlantı string normalizasyonu ———
                    if (Properties.Settings.Default.connectionstring == "Server=192.168.4.24;Database=VDB_YON02;User Id=sa;Password=MagicUser2023!;Connect Timeout=0;")
                    {
                        Properties.Settings.Default.connectionstring = "Server=192.168.4.24;Database=VDB_YON01;User Id=sa;Password=MagicUser2023!;Connect Timeout=0;";
                    }

                    if (Properties.Settings.Default.connectionstring2 == Properties.Settings.Default.connectionstring)
                    {
                        Properties.Settings.Default.connectionstring2 = "Server=192.168.4.24;Database=EntegreF;User Id=sa;Password=MagicUser2023!;";
                    }

                    if (!Properties.Settings.Default.connectionstring2.Contains($"{Properties.Settings.Default.DbName}"))
                    {
                        Properties.Settings.Default.connectionstring2 =
                            Properties.Settings.Default.connectionstring.Replace(Properties.Settings.Default.Company, Properties.Settings.Default.DbName);
                    }
                    Properties.Settings.Default.Save();

                    var yetki = Sorgu(
                        string.Format(@"select * from SOCIAL  
                                left outer join SOCIALCONNECT on SOCOSOCODE = SOCODE 
                                left outer join CASHIER on CHSOCODE = SOCODE 
                                where SOSTS = 1 and SOCODE = '{0}' and SOENTERKEY = '{1}'",
                            txtVolantUser.Text, txtVolantPassword.Text),
                        Properties.Settings.Default.connectionstring);

                    if (yetki.Rows.Count == 0)
                        return (ekranAc: false, formToOpen: (Form)null, msg: "Giriş Bilgilerinizi Kontrol Ediniz");

                    // Kullanıcı bilgilerini çek
                    Entegref.GetLogins.userID = yetki.Rows[0][0].ToString();
                    Entegref.GetLogins.userPass = yetki.Rows[0]["SOENTERKEY"].ToString();
                    Entegref.GetLogins.userCHVAL = yetki.Rows[0]["CHVAL"].ToString();
                    Entegref.GetLogins.userDEPART = yetki.Rows[0]["SODEPART"].ToString();
                    Entegref.GetLogins.userDIVVAL = cmbVolantMagaza.EditValue?.ToString();
                    Entegref.GetLogins.userREGION = conn.GetValueConnection($"select DIVREGION from DIVISON where DIVVAL = '{Entegref.GetLogins.userDIVVAL}'",Properties.Settings.Default.connectionstring);
                    //userID = yetki.Rows[0][0].ToString();
                    //userPass = yetki.Rows[0]["SOENTERKEY"].ToString();
                    //userCHVAL = yetki.Rows[0]["CHVAL"].ToString();
                    //userDEPART = yetki.Rows[0]["SODEPART"].ToString();
                    //userDIVVAL = cmbVolantMagaza.EditValue?.ToString();
                    //userREGION = conn.GetValueConnection($"select DIVREGION from DIVISON where DIVVAL = '{userDIVVAL}'", Properties.Settings.Default.connectionstring);
                    Properties.Settings.Default.MailAdress = yetki.Rows[0]["SOCOMAILADRESS"].ToString();
                    Properties.Settings.Default.MailPassword = yetki.Rows[0]["SOCOMAILPASS"].ToString();

                    // Admin departman özel kontrol
                    if (Entegref.GetLogins.userDEPART == "ADMIN")
                    {
                        int count;
                        string query = $"select count(*) from EntegreF..UserDepart where USRDPNAME = '{cmbVolantSirket.EditValue}' and USRDPDEPVAL = ''";
                        string resultCount = conn.GetValueConnection(query, Properties.Settings.Default.connectionstring);
                        if (int.TryParse(resultCount, out count) && count > 1)
                        {
                            // Not: UI'de açılmalı, bu yüzden işaret bırakacağız
                            frmEntegrefSettings settings = new frmEntegrefSettings();
                            return (ekranAc: true, formToOpen: settings, msg: "");
                        }
                    }
                    // Hangi form açılacak?
                    var formAdi = conn.GetData($@"select rtrim(ltrim(USRFRNAME)) as USRFRNAME,
                                                 rtrim(ltrim(USRFRREGNAME)) as USRFRREGNAME
                                          from EntegreF..UserDepart
                                          left outer join EntegreF..UserForm on USRDPFORMID = USRFRDPID 
                                          where USRDPDEPVAL like '%{Entegref.GetLogins.userDEPART}%' 
                                            and USRDBNAME = '{cmbVolantSirket.EditValue}'",
                                                Properties.Settings.Default.connectionstring);

                    if (formAdi == null || formAdi.Rows.Count == 0)
                        return (ekranAc: false, formToOpen: (Form)null, msg: "Form konfigürasyonu bulunamadı.");

                    var asm = Assembly.GetExecutingAssembly();
                    var formName = formAdi.Rows[0]["USRFRNAME"].ToString();
                    var formRegName = formAdi.Rows[0]["USRFRREGNAME"].ToString();
                    formType = asm.GetTypes().FirstOrDefault(t => t.Name == formName);
                    if (formType == null)
                        return (ekranAc: false, formToOpen: (Form)null, msg: $"Form bulunamadı: {formName}");

                    // Muhasebe özel connectionstring değişimi
                    if (formName == "frmMuhasebeNewMain")
                    {
                        var builder = new SqlConnectionStringBuilder(Properties.Settings.Default.connectionstring);
                        // Seçilen veritabanını ata
                        builder.InitialCatalog = conn.GetValueConnection("select MTTODBNAME from MANAGEMENT", builder.ConnectionString);
                        // Güncellenmiş connection string
                        Properties.Settings.Default.connectionstring = builder.ConnectionString;
                        Properties.Settings.Default.Save();
                    }

                    // Versiyon kontrol
                    var checkSonuc = await entegreF.GetNewCheck(configProvider);
                    if (checkSonuc == null || checkSonuc.Count != 1)
                        return (ekranAc: false, formToOpen: (Form)null, msg: "Versiyon kontrol verisi alınamadı.");

                    if (!checkSonuc[0].status)
                        return (ekranAc: false, formToOpen: (Form)null, msg: checkSonuc[0].message);

                    Properties.Settings.Default.EntegreFProductName = formRegName;
                    Properties.Settings.Default.Save();                  
                    // Versiyon karşılaştır
                    int newVersion = int.Parse(checkSonuc[0].message.Replace(".", ""));
                    int lastVersion = int.Parse(version.Replace(".", ""));
                    if (newVersion > lastVersion)
                    {
                        return (ekranAc: false, formToOpen: (Form)null, msg: "update-required");
                        
                    }

                    // Form instance oluştur (UI’de göstereceğiz)
                    //formInstance = (Form)Activator.CreateInstance(formType);
                    return (ekranAc: true, formToOpen: (Form)null, msg: "");
                });

                // 3) UI thread: Splash kapat
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();

                // 4) UI thread: Sonuçlara göre hareket et
                if (result.msg == "update-required")
                {
                    var psi2 = new ProcessStartInfo { FileName = @"EntegreFToolsUpdater.exe", WindowStyle = ProcessWindowStyle.Normal };
                    Process.Start(psi2);
                    Application.Exit();
                }

                if (!string.IsNullOrEmpty(result.msg) && result.msg != "update-required" && !result.ekranAc)
                {
                    lblversion.Text = ProductName;
                    // Hata veya bilgilendirme mesajı
                    XtraMessageBox.Show(result.msg);
                    return;
                }

                if (result.ekranAc)
                {
                    // Login formunu gizle, hedef formu göster
                    this.Hide();
                    if (result.formToOpen != null)
                    {
                        result.formToOpen.ShowDialog();
                    }
                    else
                    {
                        //result.formToOpen.ShowDialog();
                        var formInstancee = (Form)Activator.CreateInstance(formType);
                        if (formInstancee.Name.Contains("Admin"))
                        {
                            SplashHelper.ShowWhile(() =>
                            {
                                formInstance = (Form)Activator.CreateInstance(formType);
                                // diğer hazırlıklar...
                            }, 
                            parent: this, subtitle: $@"{Properties.Settings.Default.DbName}® Tools Başlatılıyor", title:"EntegRef");
                            formInstance.ShowDialog();
                        }
                        else
                        {
                            formInstancee.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                XtraMessageBox.Show(hataDetay);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
                //CustomMessageBox.ShowMessage("Program Hatası", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                // 5) UI thread: her durumda formu tekrar erişilebilir yap
                if (!this.Visible)
                {
                    //if (formInstance.Name.Contains("Admin"))
                    //{
                    //    formInstance.ShowDialog();
                    //}
                    this.Show();
                }
                this.Enabled = true;
                simpleButton2.Enabled = true;
            }
        }
        private bool IsInstallerFailed()
        {
            try
            {
                EventLog eventLog = new EventLog("Application");
                // En son 20 logu sondan başa tarayalım
                for (int i = eventLog.Entries.Count - 1; i >= 0 && i > eventLog.Entries.Count - 200; i--)
                {
                    EventLogEntry entry = eventLog.Entries[i];
                    // Sadece Windows Installer kaynaklı logları al
                    if (entry.Source == "MsiInstaller")
                    {
                        string msg = entry.Message;
                        // Ürün adını kontrol et
                        if (msg.Contains("Entegref Kasa Tools"))
                        {
                            // Hata kodu içeriyor mu?
                            // Örnek: "Yükleme başarı veya hata durumu: 0."
                            var match = Regex.Match(msg, @"durumu:\s*(\d+)");
                            if (match.Success)
                            {
                                int status = int.Parse(match.Groups[1].Value);
                                // 0 = başarılı. 0 dışındaki her durum hata sayılacak
                                if (status != 0)
                                    return true;  // HATA
                                else
                                    return false; // BAŞARILI
                            }
                        }
                    }
                }
            }
            catch { }

            return false; // Log yoksa hata saymayalım
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void txtVolantPassword_Enter(object sender, EventArgs e)
        {
            if (txtVolantPassword.Text == "Parola")
            {
                txtVolantPassword.Properties.PasswordChar = '*';
                txtVolantPassword.ForeColor = SystemColors.WindowText;
                txtVolantPassword.Text = "";
            }
        }
        private void txtVolantPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                simpleButton2_Click(null, null);
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                this.Dispose();
                Application.Exit();
            }

        }
        private void txtVolantPassword_Leave(object sender, EventArgs e)
        {
            if (txtVolantPassword.Text.Length == 0)
            {
                txtVolantPassword.Properties.PasswordChar = '\0';
                txtVolantPassword.Text = "Parola";
                txtVolantPassword.ForeColor = SystemColors.GrayText;
            }
        }
        private void btnNewDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                var path = Path.GetDirectoryName(Application.ExecutablePath);
                // SQL.script dosyasını okuyun
                string scriptContent = File.ReadAllText("EntegreF.sql");
                using (SqlConnection con = new SqlConnection(Properties.Settings.Default.connectionstring))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("USE master\r\nCreate Database EntegreF\r\n", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connectionstring))
                {
                    connection.Open();

                    // SQL sorgusunu çalıştırın
                    using (SqlCommand command = new SqlCommand(scriptContent, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    CustomMessageBox.ShowMessage("Bilgilendirme", txtKrediPuanDbName.Text + " başarıyla oluşturuldu.", this, "Başarılı", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Properties.Settings.Default.DbName = txtKrediPuanDbName.Text;
                    Properties.Settings.Default.Save();
                    connection.Close();
                }
                DataCek();
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("HataDetay Detayı!", hataDetay, this, "Detaya Bakınız", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Properties.Settings.Default.DbName = "";
                Properties.Settings.Default.Save();
            }
            //txtKrediPuanDbName.Text;
        }
        private void txtVolantUser_TextChanged(object sender, EventArgs e)
        {
            var sonuc = Sorgu(@"select SONAME +SPACE(1)+SOSURNAME,isnull(CHDIVISON,'00') as CHDIVISON,SODEPART from SOCIAL
            left outer join CASHIER on CHSOCODE = SOCODE
            where SOCODE = '" + txtVolantUser.Text + "'", Properties.Settings.Default.connectionstring);
            if (sonuc.Rows.Count > 0)
            {
                if (sonuc.Rows[0]["SODEPART"].ToString() == "ADMIN")
                {
                    togsKullanici.IsOn = true;
                    togsKullanici.Properties.OnText = sonuc.Rows[0][0].ToString();
                    togsKullanici.Properties.AppearanceDisabled.ForeColor = Color.DarkRed;
                    Entegref.GetLogins.userName = sonuc.Rows[0][0].ToString();
                    cmbVolantMagaza.Enabled = true;
                    cmbVolantMagaza.EditValue = sonuc.Rows[0][1].ToString();
                    if (!Environment.Is64BitProcess)
                    {
                        btnDegistir.Visible = true;
                        tablePanel3.Columns[2].Visible = true;
                    }
                }
                else
                {
                    togsKullanici.IsOn = true;
                    togsKullanici.Properties.OnText = sonuc.Rows[0][0].ToString();
                    togsKullanici.Properties.AppearanceDisabled.ForeColor = Color.DarkRed;
                    Entegref.GetLogins.userName = sonuc.Rows[0][0].ToString();
                    cmbVolantMagaza.EditValue = sonuc.Rows[0][1].ToString();
                    cmbVolantMagaza.Enabled = false;
                    btnDegistir.Visible = false;
                    tablePanel3.Columns[2].Visible = false;
                }
            }
            else
            {
                cmbVolantMagaza.Enabled = false;
                togsKullanici.Properties.OffText = "Yok";// sonuc.Rows[0][0].ToString();
                togsKullanici.Properties.AppearanceDisabled.ForeColor = Color.Black;
                togsKullanici.IsOn = false;
                btnDegistir.Visible = false;
                tablePanel3.Columns[2].Visible = false;
            }
        }
        private void btnDegistir_Click(object sender, EventArgs e)
        {
            string yeniExeYolu = "";
            if (mimari != 64)
            {

                // 2. EntegreFKasaTools_x64.exe'yi başlat
                yeniExeYolu = @"EntegreFKasaTools_x64.exe"; // Yolunu güncelle
                
                if (File.Exists(yeniExeYolu))
                {
                    Process.Start(yeniExeYolu);

                    // 1. EntegrefKrediOnay.exe'yi kapat
                    foreach (var process in Process.GetProcessesByName("EntegrefKrediOnay"))
                    {
                        process.Kill(); // Zorla kapatır
                        process.WaitForExit(); // Tamamen kapanmasını bekler
                    }
                }
                else
                {
                    MessageBox.Show("Yeni exe bulunamadı.");
                }
            }
        }
    }
}