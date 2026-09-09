using EntegreFDLL.Class;
using EntegrefKrediOnay.Class;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntegrefKrediOnay
{
    static class Program
    {
        public static EntegreFConfigProvider configProvider = new EntegreFConfigProvider();
        public static EntegreFBGConfigProvider FBGConfigProvider = new EntegreFBGConfigProvider();
        public static EntegreFIAConfigProvider EntegreFIAConfigProvider = new EntegreFIAConfigProvider();
        public static VolantApiClass.Filter filter = new VolantApiClass.Filter();        
        public static string sql1;
        public static string sql2 = "Data Source=62.244.219.23,1435;Initial Catalog=VDB_JUNEMED01;Persist Security Info=True;User ID=sa;Password=MagicUser2026!;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Timeout=0";
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary> 
        [STAThread]        
        static void Main()
        {
            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegrefKrediOnay");
            var sonuc = key2.GetValue("ApplicationSetupComplate");
            if (sonuc == null)
            {
                RunAsync().Wait();
            }
            bool acikmi = false;
            string exePath = Application.ProductName;
            Mutex mtex = new Mutex(true, exePath, out acikmi);
            if (acikmi)
            {
                string _s4 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegrefKrediOnay");
                key.SetValue("ApplicationVersion", _s4);
                key.Close();


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("tr-TR");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                EntegreFDLL.Main.StartupExtension.connectionString = Properties.Settings.Default.connectionstring;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmLogin());
                //Application.Run(new FrmKimlikTarama());
            }
        }
        static async Task RunAsync()
        {
            //Asenkron işlemleri burada gerçekleştirin
            await Task.Delay(1000); // Örnek bir asenkron işlem

            string Cpuid = "";
            string Motherboardid = "";
            //string fileName = "Abto\\Register_SIP_SDK_ActiveX.bat"; // .bat dosyanızın adı

            //// Proje dizini içindeki .bat dosyanızın tam yolunu oluşturun
            //string pathToBatFile = System.IO.Path.Combine(Application.StartupPath, fileName);


            //Process process = new Process();
            //process.StartInfo.FileName = "cmd.exe"; // Komutları çalıştırmak için komut istemcisini (cmd.exe) kullanıyoruz.
            //process.StartInfo.Arguments = "/c " + pathToBatFile; // /c parametresi, komut istemcisini kapatır (cmd.exe'yi çalıştırdıktan sonra kapatır).
            //process.Start();

            string _s4 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); // versiyon
            string _s5 = System.Reflection.Assembly.GetExecutingAssembly().GetName().CultureInfo.ToString(); // kültür bilgisi
            string _s6 = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); // proje adı
            string _s7 = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company; // şirket
            string _s8 = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright; // Copyright
            ManagementObjectSearcher query = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            ManagementObjectSearcher CPU = new ManagementObjectSearcher("select * from Win32_Processor");
            ManagementObjectSearcher MOTHERBOARD = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            foreach (ManagementObject obj in CPU.Get())
            {
                Cpuid = obj["ProcessorID"].ToString();
            }
            foreach (ManagementObject obj in MOTHERBOARD.Get())
            {
                Motherboardid = obj["SerialNumber"].ToString();
            }
            ManagementObjectCollection queryCollection = query.Get();
            string pcModeli = "";
            string pcİsmi = "";
            foreach (var item in queryCollection)
            {
                pcModeli = item["model"].ToString();
                pcİsmi = item["name"].ToString();
            }

            configProvider.ComputerCpuID = Cpuid;
            configProvider.ComputerMboardID = Motherboardid;
            configProvider.ComputerName = pcİsmi;
            configProvider.ComputerModeli = pcModeli;
            configProvider.ProductName = _s6;
            var assembly = typeof(Program).Assembly;
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            var id = attribute.Value;
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\EntegrefKrediOnay");
            var vknvar = key.GetValue("ApplicationVKN");
            if (vknvar == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmVKN());
                //Application.Exit();
                //System.Environment.Exit(0);
            }
            //configProvider.VKN = vknvar.ToString();
            //Entegref client = new Entegref();
            //string response = await client.UpdateLicensingUser(configProvider);// "6200080458", pcİsmi.ToString(), pcModeli.ToString(), _s4, _s6);
            //List<Entegref.Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Entegref.Sonuc>>(response);
            //var ConnectionLisansingID = myDeserializedClass[0].message;
            //string response2 = await client.UpdateLicensing("6200080458", ConnectionLisansingID.ToString(), Cpuid.ToString(), Motherboardid.ToString(), _s6);
            //if (response2 != null)
            //{
            //List<Entegref.Sonuc> myDeserializedClass2 = JsonConvert.DeserializeObject<List<Entegref.Sonuc>>(response2);
            //VolantStart.StartupExtension.computerName = pcİsmi.ToString();
            key.SetValue("ApplicationGUID", id);
            key.SetValue("ApplicationVersion", _s4);
            key.SetValue("ApplicationVKN", vknvar);
            key.SetValue("ApplicationPhoneLisans", true);
            key.SetValue("CPU", Cpuid);
            key.SetValue("ApplicationSecretPhase", "");
            key.SetValue("motherboardid", Motherboardid);
            key.SetValue("ComputerName", pcİsmi.ToString());
            key.SetValue("ComputerID", pcModeli);
            key.SetValue("ComputerUUID", "");
            key.SetValue("ComputerLisansingID", "");
            key.SetValue("ApplicationSetupComplate", true);
            key.Close();
            //}
            //else
            //{
            //    XtraMessageBox.Show("Lisan Hatası");
            //}

        }
    }
}
