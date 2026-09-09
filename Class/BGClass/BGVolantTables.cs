using EntegreFDLL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataTableExtensions;

namespace EntegrefKrediOnay.Class
{
    public class BGVolantTables
    {
        static SqlConnectionObject conn = new SqlConnectionObject();
        static Convertler convertler = new Convertler();
        public class CUSTOMER
        {
            public long CUSCURID { get; set; }

            public string CUSFROM { get; set; } = "0";

            public string CUSCAST { get; set; } = "0";

            public string CUSCP { get; set; } = "G";

            public string CUSCURKIND { get; set; } = "01";

            public string CUSLETTER { get; set; } = "1";

            public bool? CUSCORPARATE { get; set; } = false;

            public bool? CUSPERSON { get; set; } = true;

            public short? CUSLATEDAY { get; set; } = null;

            public decimal CUSCREDIT { get; set; }

            public decimal CUSCOCREDIT { get; set; } = 0;

            public DateTime? CUSCREDITSTARTDATE { get; set; } = DateTime.Today;

            public DateTime? CUSCREDITENDDATE { get; set; } = DateTime.Today;

            public bool CUSAUTOCREDIT { get; set; } = false;

            public long? CUSACCID { get; set; } = null;

            public long? CUSEXSOID { get; set; } = null;

            public string CUSACCCANVAL { get; set; } = null;

            public int CUSINVRATEVAL { get; set; } = 0;

            public bool CUSCONFIRMTCID { get; set; } = false;

            public string CUSSOCODE { get; set; }

            public DateTime CUSDATETIME { get; set; } = DateTime.Now;

            public bool CUSCANPROCEEDWITHNOLATEINCOME { get; set; } = false;

            public short? CUSCREATESALETYPE { get; set; } = null;

            public string CUSCRCARDID { get; set; } = null;

            public DateTime? CUSCRCARDENDDATE { get; set; } = null;

            public string CUSCRNOTKEY { get; set; } = null;

            public decimal? CUSCRCOIN { get; set; } = null;

            public long? CUSCRCOINKEY { get; set; } = null;

        }
        public class CURRENTS
        {
            public long CURID { get; set; }

            public string CURVAL { get; set; }

            public string CURNAME { get; set; }

            public bool CURCUSTOMER { get; set; } = true;

            public bool CURSUPPLIER { get; set; } = false;

            public bool CURSTAFF { get; set; } = false;

            public bool CURBANK { get; set; } = false;

            public bool CURCREDITCARD { get; set; } = false;

            public bool CURBANKCREDIT { get; set; } = false;

            public bool CUROTHER { get; set; } = false;

            public string CURCOMPANY { get; set; } = "01";

            public string CURDIVISON { get; set; }

            public bool CURSTS { get; set; } = true;

            public string CURUSEFIELD1 { get; set; } = null;

            public string CURUSEFIELD2 { get; set; } = null;

            public string CURUSEFIELD3 { get; set; }

            public bool CURCOCARD { get; set; } = false;

        }
        public class IDENTYPICTURE
        {
            public long? IPID { get; set; }

            public string IPIDENTY { get; set; }

            public string IPCARDNUMBER { get; set; }

            public string IPNAME { get; set; }

            public string IPSURNAME { get; set; }

            public DateTime? IPBIRTHDAY { get; set; }

            public string IPMOTHER { get; set; }

            public string IPFATHER { get; set; }

            public string IPSEX { get; set; }

            public string IPDIVVAL { get; set; }

            public bool? IPCREATECUR { get; set; }

            public long? IPCURID { get; set; }

            public string IPPORTRAIT { get; set; }

            public string IPFULLSIDE { get; set; }

            public short IPTYPE { get; set; }

        }
        #region sql insert
        //using (var sqlConnection = new SqlConnection(Properties.Settings.Default.connectionstring))
        //{
        //    await sqlConnection.OpenAsync();

        //    using (var tran = sqlConnection.BeginTransaction())
        //    {
        //        var db = new DbTrans(sqlConnection, tran);
        //        try
        //        {
        //            tran.Commit(); // Başarılıysa commit
        //            sonuc = true;
        //        }
        //        catch (Exception)
        //        {
        //            tran.Rollback(); // Hata olursa rollback
        //        }
        //    }
        //}
        #endregion
        public async static Task<string> REGISTER(string _RGKIND, string connection)
        {
            string RGID = "";
            using (var sqlConnection = new SqlConnection(connection))
            {
                await sqlConnection.OpenAsync();

                using (var tran = sqlConnection.BeginTransaction())
                {
                    var db = new DbTrans(sqlConnection, tran);
                    try
                    {
                        RGID = db.GetValue($@"
                                UPDATE REGISTER
                                SET RGID = RGID + 1
                                OUTPUT INSERTED.RGID
                                WHERE RGCOMPANY = ''
                                  AND RGKIND = {_RGKIND}
                                  AND RGVAL1 = ''
                                  AND RGVAL2 = ''
                                  AND RGDATE = 0");
                        tran.Commit(); // Başarılıysa commit
                    }
                    catch (Exception)
                    {
                        tran.Rollback(); // Hata olursa rollback
                    }
                }
            }
            return RGID;
        }
        public async static Task<bool> INSERTIDENTYPICTURE(List<IDENTYPICTURE> items, string Connection)
        {
            string BulkInsertReturn = "";
            using (var sqlConnection = new SqlConnection(Connection))
            {
                await sqlConnection.OpenAsync();
                using (var tran = sqlConnection.BeginTransaction())
                {
                    var db = new DbTrans(sqlConnection, tran);
                    try
                    {
                        var IDENTYPICTURE = convertler.ToDataTable(items);
                        BulkInsertReturn = db.BulkInsertRetorn(IDENTYPICTURE, "IDENTYPICTURE");

                        tran.Commit(); // Başarılıysa commit
                    }
                    catch (Exception)
                    {
                        tran.Rollback(); // Hata olursa rollback
                    }
                }
            }
            if (BulkInsertReturn == "Aktarım Tamamlandı")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
