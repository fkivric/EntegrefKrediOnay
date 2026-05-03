using EntegreFDLL.Class;
using EntegreFDLL.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class
{
    public class EntegreFConfigProvider : EntegreF_Interface.IEntegreFConfigProvider
    {
        public string baseUrl { get; set; } = @"http://lisans.entegref.com.tr";
        public string EntegrefToken { get; set; }
        public string EntegrefAIPToken { get; set; }
        public DateTime expiresAt { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProductName { get; set; }
        public string VKN { get; set; }
        public string CompanyName { get; set; }
        public string ComputerName { get; set; } = Environment.MachineName;
        public string ComputerModeli { get; set; }
        public string ComputerID { get; set; }
        public string ComputerUUID { get; set; }
        public string ComputerVersion { get; set; }
        public string ComputerCpuID { get; set; }
        public string ComputerMboardID { get; set; }
        public string ComputerLisansingID { get; set; }
    }
    public class EntegreFSmsConfigProvider : EntegreFDLL.Main.EntegreF_Interface.ISmsConfigProvider
    {
        public string SmsToken { get; set; }
        public string IYSToken { get; set; }
        public DateTime SmsGecerlilik { get; set; }

        public string SmsUrl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
    public class EntegreFBGConfigProvider : EntegreFDLL.Main.EntegreF_Interface.IBGConfigProvider
    {
        public string Servis { get; set; }

        public string Company { get; set; }

        public string baseLoginUrl { get; set; }
        public string port { get; set; }

        public string basicAuthUsername { get; set; }

        public string basicAuthPassword { get; set; }

        public VolantApiClass.Filter filter { get; set; }
    }
    public class EntegreFIAConfigProvider : EntegreFDLL.Main.EntegreF_Interface.IIAConfigProvider
    {
        public eSalesConfirm SalesConfirm { get; set; }

        public decimal KefilTutar { get; set; }

        public string OnayAciklama { get; set; }

        public string SartliOnayAciklama { get; set; }

        public string Risk { get; set; }

        public DevExpress.XtraRichEdit.API.Native.Document Doc { get; set; }

        public eCurrents rCurrents { get; set; }

        public string aiOutput { get; set; }

        public bool bool_aiResult_IcraSadeceAcik_TG { get; set; }

        public bool bool_aiResult_SgkBildirimSon3_TG { get; set; }

        public bool cleanNotes { get; set; }

        public string SOCODE { get; set; }

        public string ConnectionString { get; set; }
    }
}
