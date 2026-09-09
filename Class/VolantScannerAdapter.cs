using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using Volant.Ocr.Dev.Extensions;

namespace EntegrefKrediOnay.Class
{
    public class VolantScannerAdapter
    {
        
        private object _instance;
        private Type _type;
        private Assembly _ocrAsm;
        private Assembly _entAsm;
        public VolantScannerAdapter(string dllPath)
        {
            Assembly.LoadFrom(Path.Combine(dllPath, "NTwain.dll"));
            var scannerAsm = Assembly.LoadFrom(Path.Combine(dllPath, "Vol.Scanner.dll"));
            _ocrAsm = Assembly.LoadFrom(Path.Combine(dllPath, "Volant.Ocr.Dev.dll"));
            _entAsm = Assembly.LoadFrom(Path.Combine(dllPath, "Vol.Erp.LIB.Entities.dll"));

            _type = scannerAsm.GetType("Vol.Scanner.TwainScanner");
            _instance = Activator.CreateInstance(_type);
        }
        // Property ve Field Erişimi İçin Yardımcı Metodlar
        public bool IsFinished => (bool)_type.GetField("isFinished").GetValue(_instance);
        public bool IsCompleted => (bool)_type.GetField("isCompleted").GetValue(_instance);
        public Image Identity => (Image)_type.GetProperty("Identity")?.GetValue(_instance);
        public Image Head => (Image)_type.GetProperty("Head")?.GetValue(_instance);

        // rCusIdentity nesnesine dinamik erişim
        //public dynamic RCusIdentity => _type.GetField("rCusIdentity").GetValue(_instance);
        public dynamic RCusIdentity
        {
            get
            {
                // Önce Field olarak dene
                var field = _type.GetField("rCusIdentity", BindingFlags.Public | BindingFlags.Instance);
                if (field != null) return field.GetValue(_instance);

                // Bulamazsa Property olarak dene
                var prop = _type.GetProperty("rCusIdentity", BindingFlags.Public | BindingFlags.Instance);
                return prop?.GetValue(_instance);
            }
        }

        public void ScanIdentity(string scannerName, int scanObjectValue)
        {
            try
            {
                Type identityType = _entAsm.GetType("Vol.Erp.LIB.Entities.ModuleUsings.eCusIdentity");
                if (identityType == null) throw new Exception("eCusIdentity tipi bulunamadı! Vol.Erp.LIB.Entities.dll dosyasını kontrol edin.");

                object identityInstance = Activator.CreateInstance(identityType);
                var field = _type.GetField("rCusIdentity");
                if (field != null) field.SetValue(_instance, identityInstance);

                // 2. Enum tipini bul (Namespace kontrolü ekledik)
                // Önce bildiğimiz tam adı deneyelim
                Type scanObjectType = _ocrAsm.GetType("Volant.Ocr.Dev.Extensions.Enums.ScanObject");

                // Eğer bulamazsa DLL içindeki tüm tipleri tara (Daha garantili yol)
                if (scanObjectType == null)
                {
                    foreach (var t in _ocrAsm.GetTypes())
                    {
                        if (t.Name == "ScanObject")
                        {
                            scanObjectType = t;
                            break;
                        }
                    }
                }

                if (scanObjectType == null)
                    throw new Exception("ScanObject enum tipi Volant.Ocr.Dev.dll içinde bulunamadı!");

                // 3. Enum değerini oluştur
                object scanObjectEnum = Enum.ToObject(scanObjectType, scanObjectValue);

                // 4. Metodu bul ve çağır
                MethodInfo scanMethod = _type.GetMethod("ScanIdentity");
                if (scanMethod == null) throw new Exception("ScanIdentity metodu TwainScanner içinde bulunamadı!");

                scanMethod.Invoke(_instance, new object[] { scannerName, scanObjectEnum });
            }
            catch (Exception ex)
            {
                // Reflection hatalarında asıl hata her zaman InnerException içindedir
                Exception realException = ex;
                while (realException.InnerException != null)
                    realException = realException.InnerException;

                DevExpress.XtraEditors.XtraMessageBox.Show("Asıl Hata: " + realException.Message +
                    "\n\nKaynak: " + realException.Source +
                    "\n\nStack: " + realException.StackTrace);
                //CustomMessageBox.ShowMessage(ex.Message, "", null, "", false, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }
        public void InitializeManagement()
        {
            try
            {
                Assembly libAsm = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vol.Erp.LIB.dll"));

                // 1. StartupDynamics nesnesini oluştur
                Type startupDynamicsType = libAsm.GetType("Vol.Erp.LIB.Properties.StartupDynamics"); // Namespace'i kontrol edin
                object startupInstance = Activator.CreateInstance(startupDynamicsType);

                // StartupDynamics içindeki ConnectionString'i ata (Eğer field/property varsa)
                //SetStaticFieldValue(startupDynamicsType, "ConnectionString", connectionString);

                // 2. cManagement nesnesini StartupDynamics parametresiyle oluştur
                Type managementType = libAsm.GetType("Vol.Erp.LIB.Lib.Classes.CommonUsings.cManagement");

                // İkinci constructor'ı kullanıyoruz: public cManagement(StartupDynamics startup = null)
                object managementInstance = Activator.CreateInstance(managementType, new object[] { startupInstance });

                // 3. Statik Instance alanını doldur
                var instanceField = managementType.GetField("Instance", BindingFlags.Public | BindingFlags.Static)
                                 ?? managementType.GetField("Current", BindingFlags.Public | BindingFlags.Static);

                if (instanceField != null)
                {
                    instanceField.SetValue(null, managementInstance);
                }
            }
            catch (Exception ex)
            {
                // Reflection hatalarını detaylı görmek için
                Exception realEx = ex.InnerException ?? ex;
                throw new Exception("cManagement başlatılamadı: " + realEx.Message);
            }
        }
    
        public void InitializeVolantStartup()
        {
            try
            {
                // 1. Vol.Erp.LIB.dll'i yükle
                Assembly libAsm = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vol.Erp.LIB.dll"));

                // 2. StartupExtension sınıfını bul (Tam namespace'i ILSpy'dan teyit edin, genelde Vol.Erp.LIB altındadır)
                Type startupType = libAsm.GetType("Vol.Erp.LIB.Properties.StartupExtension");

                if (startupType != null)
                {
                    // 3. ConnectionString'i ata
                    //SetStaticFieldValue(startupType, "connectionString", VolantStart.StartupExtension.connectionString);
                    //SetStaticFieldValue(startupType, "connectionStringMir", VolantStart.StartupExtension.connectionStringMir);

                    //// 4. Diğer zorunlu alanları doldur (Hata almamak için boş veya varsayılan değerler)
                    //// Eğer bu alanlar null kalırsa başka hatalar alabilirsiniz.
                    //SetStaticFieldValue(startupType, "company", VolantStart.StartupExtension.company);
                    //SetStaticFieldValue(startupType, "mirCompany", VolantStart.StartupExtension.mirCompany);
                    //SetStaticFieldValue(startupType, "divison", VolantStart.StartupExtension.divison);
                    //SetStaticFieldValue(startupType, "companyName", VolantStart.StartupExtension.companyName);
                    //SetStaticFieldValue(startupType, "companyTitle", VolantStart.StartupExtension.companyTitle);
                    //SetStaticFieldValue(startupType, "year", VolantStart.StartupExtension.year);
                    //SetStaticFieldValue(startupType, "server", VolantStart.StartupExtension.server);
                    //SetStaticFieldValue(startupType, "database", VolantStart.StartupExtension.database);
                    //SetStaticFieldValue(startupType, "firstDatabase", VolantStart.StartupExtension.firstDatabase);
                    //SetStaticFieldValue(startupType, "soCode", VolantStart.StartupExtension.soCode);
                    //SetStaticFieldValue(startupType, "enterKey", VolantStart.StartupExtension.enterKey);
                    //SetStaticFieldValue(startupType, "admin", VolantStart.StartupExtension.admin);
                    //SetStaticFieldValue(startupType, "computerName", VolantStart.StartupExtension.computerName);
                    //SetStaticFieldValue(startupType, "localIp", VolantStart.StartupExtension.localIp);
                    //SetStaticFieldValue(startupType, "dbFirst", false);

                    // Eğer kullanıcı veya şube bilgisi gerekiyorsa onları da dummy olarak doldurabiliriz
                    // SetStaticFieldValue(startupType, "UserId", 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("StartupExtension başlatılamadı: " + ex.Message);
            }

        }

        // Yardımcı metod: Statik field veya property'e değer yazar
        private void SetStaticFieldValue(Type type, string memberName, object value)
        {
            var prop = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static);
            if (prop != null && prop.CanWrite) prop.SetValue(null, value);
            else
            {
                var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Static);
                if (field != null) field.SetValue(null, value);
            }
        }
        private static TwainSession _twain;
        public List<string> GetScannerSources()
        {
            List<string> sources = new List<string>();
            try
            {
                var appId = TWIdentity.CreateFromAssembly(DataGroups.Image, typeof(VolantScannerAdapter).Assembly);
                _twain = new TwainSession(appId);
                _twain.Open(); // TWAIN Manager ile bağlantı açılır
                try
                {
                    // Tüm kaynakları (scanner) al
                    foreach (var src in _twain.GetSources())
                    {
                        sources.Add(src.Name);
                    }

                    _twain.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Exception realException = ex;
                while (realException.InnerException != null)
                    realException = realException.InnerException;
                DevExpress.XtraEditors.XtraMessageBox.Show("Asıl Hata: " + realException.Message +
                    "\n\nKaynak: " + realException.Source +
                    "\n\nStack: " + realException.StackTrace);
                // Eğer üstteki yöntem başarısız olursa SetupTwain'in kendi diyaloğunu açmasını bekleyebiliriz
            }
            return sources;
        }
        public void SetConnectionString(string connectionString)
        {
            try
            {
                // Volant genellikle bağlantıyı Vol.Erp.LIB içindeki bir sınıfta tutar.
                // Eğer Vol.Scanner içinde özel bir yer varsa orayı hedefleyelim.

                // 1. İhtimal: Vol.Erp.LIB.CommonUsings içindeki ConnectionString
                Assembly libAsm = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vol.Erp.LIB.dll"));
                Type configType = libAsm.GetType("Vol.Erp.LIB.CommonUsings"); // Sınıf adı decompile sonucuna göre değişebilir

                if (configType != null)
                {
                    var prop = configType.GetProperty("ConnectionString", BindingFlags.Public | BindingFlags.Static);
                    if (prop != null) prop.SetValue(null, connectionString);
                    else
                    {
                        var field = configType.GetField("ConnectionString", BindingFlags.Public | BindingFlags.Static);
                        if (field != null) field.SetValue(null, connectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log veya hata yönetimi
            }
        }
        //private object _scannerInstance;
        //private Assembly _scannerAssembly;
        //private Assembly _entitiesAssembly;
        //private Assembly _ocrAssembly;
        //public VolantScannerAdapter(string dllPath)
        //{
        //    // DLL'leri çalışma zamanında yükle
        //    _scannerAssembly = Assembly.LoadFrom(Path.Combine(dllPath, "Vol.Scanner.dll"));
        //    _entitiesAssembly = Assembly.LoadFrom(Path.Combine(dllPath, "Vol.Erp.LIB.Entities.dll"));
        //    _ocrAssembly = Assembly.LoadFrom(Path.Combine(dllPath, "Volant.Ocr.Dev.dll"));

        //    // TwainScanner tipini bul ve instance oluştur
        //    Type scannerType = _scannerAssembly.GetType("Vol.Scanner.TwainScanner");
        //    _scannerInstance = Activator.CreateInstance(scannerType);
        //}

        //public void Scan(string scannerName, int scanObjectValue)
        //{
        //    // eCusIdentity oluştur ve ata
        //    Type identityType = _entitiesAssembly.GetType("Vol.Erp.LIB.Entities.ModuleUsings.eCusIdentity");
        //    object identityInstance = Activator.CreateInstance(identityType);

        //    _scannerInstance.GetType().GetField("rCusIdentity").SetValue(_scannerInstance, identityInstance);

        //    // ScanIdentity metodunu çağır
        //    // ScanObject enum değerini int olarak gönderiyoruz
        //    MethodInfo scanMethod = _scannerInstance.GetType().GetMethod("ScanIdentity");

        //    // Enum tipini çalışma zamanında al
        //    Type scanObjectType = _ocrAssembly.GetType("Volant.Ocr.Dev.Extensions.Enums.ScanObject");
        //    object scanObjectEnum = Enum.ToObject(scanObjectType, scanObjectValue);

        //    scanMethod.Invoke(_scannerInstance, new object[] { scannerName, scanObjectEnum });
        //}
    }
}
