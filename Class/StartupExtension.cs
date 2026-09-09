using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace EntegrefKrediOnay.Class
{
    //using Vol.Erp.LIB;
    //using Vol.Erp.LIB.Entities.VolantUsings;
    //using Vol.Erp.LIB.FirstClassUsings;
    //public class VolantStart
    //{
    //    public class StartupExtension
    //    {
    //        public static string company { get; set; }

    //        public static string mirCompany { get; set; }

    //        public static string divison { get; set; }

    //        public static string companyName { get; set; }

    //        public static string companyTitle { get; set; }

    //        public static string companyWatP { get; set; }

    //        public static string companyWatNo { get; set; }

    //        public static string divisonName { get; set; }

    //        public static string divisonUnite { get; set; }

    //        public static string divShipmentDiv { get; set; }

    //        public static string divNote { get; set; }

    //        public static string soAgentId { get; set; }

    //        public static int? year { get; set; }

    //        public static int? month { get; set; }

    //        public static string rdms { get; set; }

    //        public static string connectionString { get; set; }

    //        public static string connectionStringMir { get; set; }

    //        public static string server { get; set; }

    //        public static string database { get; set; }

    //        public static string firstDatabase { get; set; }

    //        public static string firstServer { get; set; }

    //        public static string useReason { get; set; }

    //        public static string soCode { get; set; }

    //        public static string enterKey { get; set; }

    //        public static bool? admin { get; set; }

    //        public static bool? mobile { get; set; }

    //        public static string department { get; set; }

    //        public static long? terminal { get; set; }

    //        public static string computerName { get; set; }

    //        public static string localIp { get; set; }

    //        public static string computerTitle { get; set; }

    //        public static string clientName { get; set; }

    //        public static string serverPort { get; set; }

    //        public static DateTime startupTime { get; set; }

    //        public static string pathOfExe { get; set; }

    //        public static string pathOfPrints { get; set; }

    //        public static string pathOfArchive { get; set; }

    //        public static bool? mainDiv { get; set; }

    //        public static bool? dbFirst { get; set; }

    //        public static eInitiations rInitiations { get; set; }

    //        public static string lisanceVal { get; set; }

    //        public static string tabletConnectionString { get; set; }

    //        public static string tabletConnectionSa { get; set; }

    //        public static string tabletConnectionPassword { get; set; }

    //        public static SystemLanguage language { get; set; }

    //        public static List<eVolLanguage> lVolLanguage { get; set; }

    //        public static string systemBit
    //        {
    //            get
    //            {
    //                try
    //                {
    //                    return (IntPtr.Size == 8) ? "x64" : "x86";
    //                }
    //                catch
    //                {
    //                    return "x86";
    //                }
    //            }
    //        }

    //        public static string AssemblyDirectory
    //        {
    //            get
    //            {
    //                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
    //                UriBuilder uri = new UriBuilder(codeBase);
    //                string path = Uri.UnescapeDataString(uri.Path);
    //                return Path.GetDirectoryName(path);
    //            }
    //        }

    //        public static DivRule divRule
    //        {
    //            get
    //            {
    //                try
    //                {
    //                    if (divison == "00")
    //                    {
    //                        return DivRule.divAll;
    //                    }
    //                    if (divison != "00" && divShipmentDiv == null && divisonUnite == "A")
    //                    {
    //                        return DivRule.divShipment;
    //                    }
    //                    if (divison != "00" && mainDiv.GetValueOrDefault() && divisonUnite != "A" && divShipmentDiv == null)
    //                    {
    //                        return DivRule.divUnited;
    //                    }
    //                    if (divison != "00" && mainDiv.GetValueOrDefault() && divisonUnite != "A" && divShipmentDiv != null)
    //                    {
    //                        return DivRule.divMain;
    //                    }
    //                    return DivRule.divSelf;
    //                }
    //                catch
    //                {
    //                    return DivRule.divSelf;
    //                }
    //            }
    //        }
    //    }
    //}
}
