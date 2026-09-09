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
using DevExpress.XtraEditors.Controls;
using EntegreFDLL.Class;
using System.IO;
using Newtonsoft.Json;
using static EntegreFDLL.Class.VolantApiClass;
using static EntegreFDLL.Class.Entegref;
using System.Threading;
using EntegreFDLL.Class.AI;
using static EntegreFDLL.Class.AI.SalesApprovalScorer;
using System.Data.SqlClient;
using EntegreFDLL;
using EntegrefKrediOnay.Class;
using static EntegreFDLL.Class.DataTableClass;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmBGSatisOnay : MetroFramework.Forms.MetroForm
    {
        private static int Islem;
        private static string CURNAME;
        private static string[] MAIL;
        private static string WRTRID;
        private static string SALID;
        private static string CURID;
        private static double LATE;
        private static int RATE;
        private static string CURVAL;
        private static bool SatisAcik;
        private static string OnayTable = "INVESTIGATIONVALUE";
        private static string Firma = "YonAVM";
        public eCurrents rCurrents;
        eSalesConfirm rConfirmSales;
        private static string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Satis Kredi Onay İşlemleri");
        EntegreFDLL.Class.BGClass GetBGClass = new EntegreFDLL.Class.BGClass();
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        List<Class.BGClass.BGKefil.Kefil> GetKefils = new List<Class.BGClass.BGKefil.Kefil>();
        public frmBGSatisOnay(int _sc,string _curname,string[] _mail, List<Class.BGClass.BGKefil.Kefil> kefils, string _salid, string _curid, int _rate, double _late, bool _satisAcik, eCurrents rCur, eSalesConfirm rConf)
        {
            InitializeComponent();
            Islem = _sc;
            CURNAME = _curname;
            MAIL = _mail;
            GetKefils.AddRange(kefils);
            //WRTRID = _curOrWrtrID;
            SALID = _salid;
            CURID = _curid;
            LATE = _late;
            RATE = _rate;
            SatisAcik = _satisAcik;
            rConfirmSales = rConf;
            rCurrents = rCur;
            this.Focus();
            if (Properties.Settings.Default.Company.Contains("KAMALAR"))
            {
                if (!OnayTable.Contains("2"))
                {
                    OnayTable = OnayTable + "2";
                }
                Firma = "KamalarAVM";
            }
        }
        private BackgroundWorker _backgroundWorker;
        private ManualResetEvent _workerCompletedEvent = new ManualResetEvent(false);
        private void executeBackground(Action doWorkAction, Action progressAction = null, Action completedAction = null)
        {
            try
            {
                if (_backgroundWorker != null)
                {
                    if (_backgroundWorker.IsBusy)
                    {
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
                this.Enabled = false;
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
        private void frmOnayNote_Load(object sender, EventArgs e)
        {
            var dt = conn.GetData($@"select DINSID,DINSNAME from EntegreF.dbo.DEFINVESTIGATION where DINSACTIVE = 1 and DINSTYPE = {Islem}", sql1);
            chkSonuclar.DataSource = dt;
            chkSonuclar.ValueMember = "DINSID";
            chkSonuclar.DisplayMember = "DINSNAME";

            CURVAL = conn.GetValueConnection($"select CURVAL from CURRENTS where CURID = {CURID}", sql1);

            if (Islem == 1 ||Islem == 4)
            {
                chkSonuclar.SetItemChecked(0, true);
            }
            else if (Islem == 2)
            {
                chkSonuclar.CheckMode = CheckMode.Single;
                chkSonuclar.SelectionMode = SelectionMode.One; // Sadece 1 seçim
                chkSonuclar.CheckOnClick = true; // Tıklandığında otomatik seçim  
            }
            else
            {
                chkSonuclar.CheckMode = CheckMode.Multiple;
                chkSonuclar.SelectionMode = SelectionMode.One; // Çoklu seçim
                chkSonuclar.CheckOnClick = true;
            }
            if (GetKefils.Count > 0)
            {
                WRTRID = GetKefils[0].WRTRID;
                togKefil.IsOn = true;
            }
            else
            {
                WRTRID = "0";
                togKefil.IsOn = false;
            }
            //SalesConfirmRichEditRenderer.Render2(richEditControl1, data, Sonuc, kefilTutar, Risk);
            //richEditControl1.Text = Sonuc;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            this.Dispose();
        }
        public class İslemler
        {
            public string IslemID { get; set; }
            public string IslemName { get; set; }
            public int IslemType { get; set; }
        }
        Entegref GetEntegref = new Entegref();
        private async void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            string hataDetay = "";
            bool kefilnotu = false;
            if (toggleSwitch1.IsOn)
            {
                DialogResult dialog = XtraMessageBox.Show("Müşteri Notu Kefile de eklensin mi?", "", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    kefilnotu = true;
                }
            }
            DialogResult dialog2 = XtraMessageBox.Show("Müşteri Veri Dosyaları Maile Eklensin mi?", "", MessageBoxButtons.YesNo);
            await Class.SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
            async (progress, token) =>
            {
                progress.Report((0, $"Satış Onayı Yapılıyor"));
                await Task.Delay(10, token);
                token.ThrowIfCancellationRequested();

                string Note = "";
                if (txtOnayNotu.EditValue != null)
                {
                    Note = txtOnayNotu.EditValue.ToString();
                }
                else
                {
                    Note = "Mail ile iletildi Açıklama Yazılmadı Bilgi İçin Arayınzı";
                }
                string OnayDurumu = "";

                if (Islem == 1)
                {
                    OnayDurumu = "1";
                }
                else if (Islem == 2)
                {
                    OnayDurumu = "1";
                }
                else if (Islem == 3)
                {
                    OnayDurumu = "0";
                }
                else
                {
                    OnayDurumu = "0";
                }

                if (dialog2 == DialogResult.Yes)
                {
                    try
                    {
                        await OnayBelgesi($"{SALID} nolu satış için işlem açıklaması = {Note.ToUpper()}");
                        if (!Directory.Exists(MainPath))
                            return;

                        string hedefKlasor = Directory
                            .GetDirectories(MainPath, CURVAL, SearchOption.TopDirectoryOnly)
                            .FirstOrDefault();

                        if (string.IsNullOrEmpty(hedefKlasor))
                            return;

                        // Temp klasör oluştur
                        string tempKlasor = Path.Combine(Path.GetTempPath(), $"EkDosyalar_{Firma}");
                        Directory.CreateDirectory(tempKlasor);

                        // PDF dosyalarını al
                        string[] pdfDosyalari = Directory.GetFiles(hedefKlasor, "*.pdf", SearchOption.TopDirectoryOnly);

                        foreach (string pdf in pdfDosyalari)
                        {
                            string hedefYol = Path.Combine(tempKlasor, Path.GetFileName(pdf));

                            File.Copy(pdf, hedefYol, true); // overwrite

                            // Listeye ekle
                            listBoxControl1.Items.Add(hedefYol);
                        }
                    }
                    catch (Exception ex)
                    {
                        hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                        CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                CURVAL = conn.GetValue($"select CURVAL from CURRENTS where CURID ={CURID}",sql1);
                List<İslemler> islemlers = new List<İslemler>();
                string[] alicilar = MAIL;  //new string[] {"fatihkivric@yonavm.com.tr"};//
                string[] seçiliDurum;
                string konu = $"Müşteri Satış Onayı Sonuçlandırıldı";
                string htmlBody = $@"
                <html><body style='font-family:Calibri; font-size:11pt;'>
                <p><b>Müşteri Kodu = {CURVAL}</b></p>
                <p><b>Müşteri Adı = {CURNAME}</b></p>";
                htmlBody = htmlBody + $@"<p><b>Satış ID = {SALID.ToString()}</b></p> ";

                if (Islem != 1 && Islem != 4)
                {
                    for (int i = 0; i < chkSonuclar.ItemCount; i++)
                    {
                        if (chkSonuclar.GetItemChecked(i))
                        {
                            islemlers.Add(new İslemler
                            {
                                IslemID = chkSonuclar.GetItemValue(i).ToString(),
                                IslemName = chkSonuclar.GetItemText(i).ToString(),
                                IslemType = Islem
                            });
                            htmlBody = htmlBody + $@"<p><b>Satışa Özel Durum {i + 1} = {chkSonuclar.GetItemText(i).ToString() }</b></p> ";
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < chkSonuclar.ItemCount; i++)
                    {
                        if (chkSonuclar.GetItemChecked(i))
                        {
                            islemlers.Add(new İslemler
                            {
                                IslemID = chkSonuclar.GetItemValue(i).ToString(),
                                IslemName = chkSonuclar.GetItemText(i).ToString(),
                                IslemType = Islem
                            });
                            htmlBody = htmlBody + $@"<p><b>Satışa Özel Durum {i + 1} = { chkSonuclar.GetItemText(i).ToString() }</b></p> ";
                        }
                    }
                }
                var INVESDINSID = string.Join("-", islemlers.Select(m => m.IslemID));
                seçiliDurum = new string[] { string.Join(",", islemlers.Select(m => m.IslemName)) };
                htmlBody = htmlBody + $@"<p><b>Bölge Müdürü İşlem Onay Notu = {Note.ToUpper()}</b></p>
                <p><b>İyi çalışmalar,</b></p>
                </body></html>";

                // Dosya eklemek istersen:

                var ekler = new List<string>();
                foreach (string dosyaYolu in listBoxControl1.Items)
                {
                    if (File.Exists(dosyaYolu))
                    {
                        
                            ekler.Add(dosyaYolu);

                    }
                }
                //var result = await MailService.SendErrorMailAsync(alicilar, konu, htmlBody, ekler);
                List<string> Gonderenler = new List<string>();
                Gonderenler.Add(Entegref.GetLogins.MailAdress);
                Gonderenler.Add(Entegref.GetLogins.MailPassword);
                Gonderenler.Add(Entegref.GetLogins.userName);
                string[] gonderen = Gonderenler.ToArray();
                MailRequest request = new MailRequest();
                request.To = alicilar;  //new string[] { "ismailkelem@yonavm.com.tr", "fatihkivric@yonavm.com.tr"};//alicilar;
                request.Sender = gonderen;
                request.Body = htmlBody;
                request.Subject = konu;
                request.MailHost = Entegref.GetLogins.MailHost;
                request.MailPort = Entegref.GetLogins.MailPort;
                request.AttachmentPaths = ekler;

                var result = await GetEntegref.SendMail(request);
                var myDeserializedClass2 = JsonConvert.DeserializeObject<Sonuc>(result);

                if (myDeserializedClass2.status)
                {
                    progress.Report((0, $"Mail Gönderildi"));
                }
                else
                {
                    progress.Report((0, $"Maile Gönderirken Aşagıdaki Hata Oluştu {myDeserializedClass2.message}"));
                    //CustomMessageBox.ShowMessage("Maile Gönderirken Aşagıdaki Hata Oluştu", myDeserializedClass2.message, this, "Mail Gönderim Hatası", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //this.Focus();
                }
                using (var sqlConnection = new SqlConnection(Properties.Settings.Default.connectionstring))
                {
                    await sqlConnection.OpenAsync();

                    using (var tran = sqlConnection.BeginTransaction())
                    {
                        var db = new DbTrans(sqlConnection, tran);

                        try
                        {
                            if (Islem == 2 || Islem == 1)
                            {
                                db.InsertValue($@"UPDATE SALESINVESTIGATION set SAINGTISDONE = 1,SAINGTSALSTS = {OnayDurumu},SAINGTSALNOTES = '{Note.ToUpper()}' where SAINGTSALID = {SALID}");
                                db.InsertValue($@"UPDATE SALES SET SALINSSTS = 1, SALCREDITCHECK = 1 WHERE SALID={SALID} AND SALINSSTS=0 AND isnull(SALCREDITCHECK,0) = 0");
                                db.InsertValue($@"UPDATE SALES SET SALCREDITCHECK = 1  WHERE SALID={SALID} AND isnull(SALCREDITCHECK,0)=0");
                                db.InsertValue($@"UPDATE ORDERS SET ORDSALECONFIRM = '1'  WHERE ORDSALID = {SALID} AND isnull(ORDSALECONFIRM,0) = '0'");
                                db.InsertValue($@"UPDATE C SET C.CDRSTS = '1', C.CDRDATETIME = GETDATE() FROM CUSDELIVER C  LEFT OUTER JOIN ORDERSCHILD ON ORDCHID = C.CDRORDCHID  LEFT OUTER JOIN ORDERS ON ORDID = ORDCHORDID  WHERE ORDSALID = {SALID} AND C.CDRSTS='0' ");
                                if (Islem == 2)
                                {
                                    db.InsertValue($@"update CUSTOMER set CUSCREDIT = 0 where CUSCURID = {CURID}");
                                    for (int i = 0; i < islemlers.Count; i++)
                                    {
                                        if (chkSonuclar.GetItemChecked(i))
                                        {
                                            var DINSID = chkSonuclar.GetItemValue(i).ToString();
                                            var updatesınuc = db.InsertValue($@"update WAVECUSTOMER set WCUSVAL  = 'BM0{DINSID}' where WCUSCURID = {CURID} and WCUSUNIQ = 9");
                                            if (updatesınuc == 0)
                                            {
                                                db.InsertValue($@"insert into WAVECUSTOMER values ({CURID},9,'BM0{DINSID}')");
                                            }
                                            foreach (var item in GetKefils)
                                            {
                                                if (item.CURID != null)
                                                {
                                                    var kefilupdate = db.InsertValue($@"update WAVECUSTOMER set WCUSVAL  = 'BM0{DINSID}' where WCUSCURID = {item.CURID} and WCUSUNIQ = 9");
                                                    if (kefilupdate == 0)
                                                    {
                                                        db.InsertValue($@"insert into WAVECUSTOMER values ({item.CURID},9,'BM0{DINSID}')");
                                                    }
                                                }
                                            }
                                            //if (WRTRID != "" && WRTRID != "0")
                                            //{
                                            //    var WRTRCURID = db.GetValue($@"select CUSIDCURID from WARRANTERS
                                            //    left outer join CUSIDENTITY on WRTRIDNO = CUSIDTCNO
                                            //    where WRTRID = {WRTRID}");
                                            //    if (!string.IsNullOrWhiteSpace(WRTRCURID) || WRTRCURID == "NULL")
                                            //    {
                                            //        var kefilupdate = db.InsertValue($@"update WAVECUSTOMER set WCUSVAL  = 'BM0{DINSID}' where WCUSCURID = {WRTRCURID} and WCUSUNIQ = 9");
                                            //        if (kefilupdate == 0)
                                            //        {
                                            //            db.InsertValue($@"insert into WAVECUSTOMER values ({WRTRCURID},9,'BM0{DINSID}')");
                                            //        }
                                            //    }
                                            //}
                                        }
                                    }
                                }
                                if (!SatisAcik)
                                {
                                    var updatesınuc = db.InsertValue($@"update WAVECUSTOMER set WCUSVAL  = 'BM03' where WCUSCURID = {CURID} and WCUSUNIQ = 9");
                                    if (updatesınuc == 0)
                                    {
                                        db.InsertValue($@"insert into WAVECUSTOMER values ({CURID},9,'BM03')");
                                    }
                                }
                            }
                            else
                            {
                                db.InsertValue($@"UPDATE SALESINVESTIGATION set SAINGTISDONE = 1,SAINGTSALSTS = {OnayDurumu} ,SAINGTSALNOTES = '{Note.ToUpper()}' where SAINGTSALID = {SALID}");
                            }

                            var CURNTID = db.GetValue($@"
                                UPDATE REGISTER
                                SET RGID = RGID + 1
                                OUTPUT INSERTED.RGID
                                WHERE RGCOMPANY = ''
                                  AND RGKIND = 115
                                  AND RGVAL1 = ''
                                  AND RGVAL2 = ''
                                  AND RGDATE = 0");

                            var notes = db.InsertValue($@"
                            insert into CURNOTES values ({CURNTID},{CURID},'{SALID} nolu satış için işlem açıklaması = {Note.ToUpper()}' ,'{Entegref.GetLogins.userID}',Getdate(),2,{SALID},null)");
                            if (notes == 1)
                            {
                                if (kefilnotu)
                                {
                                    foreach (var item in GetKefils)
                                    {
                                        var WRTRNOTE = conn.InsertValue($@"update SALESWARRANTERS set SALWNOTES = isnull(SALWNOTES,'') + '{SALID} nolu satış için {CURNAME} isimli müşteri notu {Note.ToUpper()}' where SALWSALID = {SALID}", sql1);
                                        var SALWNOTES = conn.GetValueConnection($"select SALWNOTES from SALESWARRANTERS where SALWSALID = {SALID}", sql1);
                                        CURNTID = db.GetValue($@"
                                        UPDATE REGISTER
                                        SET RGID = RGID + 1
                                        OUTPUT INSERTED.RGID
                                        WHERE RGCOMPANY = ''
                                          AND RGKIND = 115
                                          AND RGVAL1 = ''
                                          AND RGVAL2 = ''
                                          AND RGDATE = 0");
                                        if (!string.IsNullOrWhiteSpace(item.CURID))
                                        {
                                            var kefilnotes = db.InsertValue($@"insert into CURNOTES values ({CURNTID},{item.CURID},{SALWNOTES},'{Entegref.GetLogins.userID}',Getdate(),2,{SALID},null)");
                                        }
                                    }
                                    //var WRTRCURID = conn.GetValueConnection($@"select CUSIDCURID from WARRANTERS
                                    //    left outer join CUSIDENTITY on WRTRIDNO = CUSIDTCNO
                                    //    where WRTRID = {WRTRID}", sql1);
                                    
                                }
                            }
                            db.InsertValue($@"insert into EntegreF.dbo.{OnayTable} values ({SALID},{CURID},{INVESDINSID},{OnayDurumu},'{Note.ToUpper()}',{RATE},{WRTRID},'{LATE}','{Entegref.GetLogins.userID}',GETDATE())");
                            tran.Commit();
                            this.DialogResult = DialogResult.OK;
                            this.Enabled = true;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                        }
                    }
                }
            });
        }
        static string GetFileCountInFolder(string folderPath)
        {
            int adet = 0;
            try
            {
                // Belirtilen klasördeki dosyaları al
                string[] files = Directory.GetFiles(folderPath);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(SALID))
                    {
                        adet++;
                    }
                }
                // Dosya sayısını döndür
                if (adet == 0)
                {
                    return "";
                }
                else
                {
                    return adet.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
                return ""; // Hata durumunda -1 döndür
            }
        }
        async Task OnayBelgesi(string OnayDurumu)
        {
            decimal kefilTutar = 0;
            eSalesConfirm data = new eSalesConfirm();
            List<eAiDef_Scoring> lPrm_100 = new List<eAiDef_Scoring>();
            DataTable dt = conn.GetData("select * from AIDEF_SCORING where AIDEF_ID in (55,100,101,102)", sql1);

            foreach (DataRow row in dt.Rows)
            {
                eAiDef_Scoring rec = new eAiDef_Scoring
                {
                    AIDEF_ID = row["AIDEF_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["AIDEF_ID"]),
                    AIDEF_VAL = row["AIDEF_VAL"]?.ToString(),
                    AIDEF_NAME = row["AIDEF_NAME"]?.ToString(),
                    AIDEF_VALUE = row["AIDEF_VALUE"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["AIDEF_VALUE"]),
                    AIDEF_RESULT = row["AIDEF_RESULT"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["AIDEF_RESULT"]),
                    AIDEF_BOOL = row["AIDEF_BOOL"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(row["AIDEF_BOOL"]),
                    AIDEF_GROUP = row["AIDEF_GROUP"]?.ToString()
                };
                lPrm_100.Add(rec);
            }

            data = rConfirmSales ?? throw new Exception("Satış objesi (eSalesConfirm) bulunamadı.");
            kefilTutar = lPrm_100.FirstOrDefault(p => p.AIDEF_ID == 55)?.AIDEF_VALUE ?? 0m;
            bool prm101 = lPrm_100.FirstOrDefault(p => p.AIDEF_ID == 101)?.AIDEF_BOOL ?? false;
            bool prm102 = lPrm_100.FirstOrDefault(p => p.AIDEF_ID == 102)?.AIDEF_BOOL ?? false;
            DevExpress.XtraRichEdit.API.Native.Document document = richEditControl1.Document;
            Program.EntegreFIAConfigProvider.Risk = RATE.ToString();
            Program.EntegreFIAConfigProvider.Doc = document;
            Program.EntegreFIAConfigProvider.SalesConfirm = data;
            Program.EntegreFIAConfigProvider.rCurrents = rCurrents;
            Program.EntegreFIAConfigProvider.SartliOnayAciklama = OnayDurumu;
            Program.EntegreFIAConfigProvider.bool_aiResult_IcraSadeceAcik_TG = true;
            Program.EntegreFIAConfigProvider.bool_aiResult_SgkBildirimSon3_TG = true;
            Program.EntegreFIAConfigProvider.KefilTutar = kefilTutar;
            Program.EntegreFIAConfigProvider.cleanNotes = true;
            document = SalesConfirmRichEditRenderer.Render(Program.EntegreFIAConfigProvider);
            Entegref.CreateDirectoryIfNotExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Satis Kredi Onay İşlemleri", rConfirmSales.musteriKodu));
            string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Satis Kredi Onay İşlemleri", rConfirmSales.musteriKodu);
            var sira = GetFileCountInFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Satis Kredi Onay İşlemleri", rConfirmSales.musteriKodu));
            string filename = SALID + "_Nolu Satış_" + DateTime.Now.ToString("yyyy-MM-dd") + "_Tarihli" + "_Onay Sonucu_" + sira + ".pdf";
            string filePath = Path.Combine(FolderPath, filename);
            richEditControl1.ExportToPdf(filePath);

            FtpVolant ftpVolant = new FtpVolant();
            var Ftp = await ftpVolant.UploadFileToFtpIfNotExists(Entegref.GetLogins.FTPURL, Entegref.GetLogins.FTPUSER, Entegref.GetLogins.FTPPASS, rConfirmSales.musteriRef.ToString(), "OnaySonuc", filename, filePath);
        }

        private void btnYukle_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string tempKlasor = Path.Combine(Path.GetTempPath(), "EkDosyalar_YonAVM");
                Directory.CreateDirectory(tempKlasor);

                string secilenDosya = ofd.FileName;
                string hedefYol = Path.Combine(tempKlasor, Path.GetFileName(secilenDosya));
                File.Copy(secilenDosya, hedefYol, true); // overwrite varsa

                // Editöre ekleme (örnek: ListBoxControl)
                listBoxControl1.Items.Add(hedefYol); // hem isim hem yol
            }

        }

        private void btnKalsir_Click(object sender, EventArgs e)
        {
            if (listBoxControl1.SelectedItem != null)
            {
                string dosyaYolu = listBoxControl1.SelectedItem.ToString();

                if (File.Exists(dosyaYolu))
                    File.Delete(dosyaYolu);

                listBoxControl1.Items.Remove(dosyaYolu);
            }

        }

        private void chkSonuclar_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
        }

        private void chkSonuclar_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int selectedIndex = chkSonuclar.SelectedIndex;
            if (selectedIndex >= 0)
            {
                // Tüm item'ların seçimini kaldır
                for (int i = 0; i < chkSonuclar.Items.Count; i++)
                {
                    chkSonuclar.SetItemChecked(i, false);
                }
                // Sadece seçilen item'i işaretle
                chkSonuclar.SetItemChecked(selectedIndex, true);
            }
        }
    }
}