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
using DevExpress.LookAndFeel;
using System.Threading;
using EntegrefKrediOnay.Class;
using System.Reflection;
using System.IO;
using EntegreFDLL;
using System.Data.SqlClient;
using static DataTableExtensions;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmKullanici : DevExpress.XtraEditors.XtraForm
    {
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        static Convertler convertler = new Convertler();
        public class SOCIAL
        {
            public string SOCODE { get; set; }

            public string SOENTERKEY { get; set; }

            public string SONAME { get; set; }

            public string SOSURNAME { get; set; }

            public string SODEPART { get; set; }

            public bool SOSTS { get; set; }

            public bool SOADMIN { get; set; }

            public string SOAGIENTID { get; set; }

            public bool SOMOBILE { get; set; }

            public bool SOAPP { get; set; }

            public short? SOMOBILEKIND { get; set; }

            public long? SOCURID { get; set; }

            public bool SOISTISARE { get; set; }

        }
        public class DEFSAFE
        {
            public long DSAFEID { get; set; }

            public string DSAFEVAL { get; set; }

            public string DSAFENAME { get; set; }

            public string DSAFEEXCH { get; set; }

            public string DSAFEUNITE { get; set; }

            public string DSAFECOMPANY { get; set; } = "01";

            public string DSAFEDIVISON { get; set; }

            public string DSAFEOLDVAL { get; set; }

            public string DSAFEKIND { get; set; }

            public bool DSAFESTS { get; set; }

        }
        public class CASHIER
        {
            public string CHVAL { get; set; }

            public string CHCOMPANY { get; set; } = "01";

            public string CHDIVISON { get; set; }

            public string CHSOCODE { get; set; }

            public string CHSAFEUNI { get; set; }

            public bool CHSTS { get; set; }

        }
        public class JOINSAFE
        {
            public long JSAFEID { get; set; }

            public int JSAFEYEAR { get; set; }

            public long JSAFEACCID { get; set; }

        }
        public class ACCPLAN
        {
            public long ACCID { get; set; }

            public int ACCYEAR { get; set; }

            public string ACCVAL { get; set; }

            public string ACCNAME { get; set; }

            public string ACCLEVEL { get; set; }

            public string ACCEXCH { get; set; }

            public string ACCDEFWAVEEXGRP { get; set; }

        }
        public frmKullanici()
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Kullanıcı Hesapları Açılıyor");
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
                navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
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
        bool NewUser = false;
        private void frmKullanici_Load(object sender, EventArgs e)
        {
            Kullanici();
            Department();
        }
        private void Department()
        {
            srcDepartman.Properties.DataSource = conn.GetData("select * from DEPARTMENT where DEPNAME not like '%VOL%' and DEPVAL != 'ADMIN'", sql1);
            srcDepartman.Properties.DisplayMember = "DEPNAME";
            srcDepartman.Properties.ValueMember = "DEPVAL";

            srcMagaza.Properties.DataSource = conn.GetData("select DIVVAL,DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", sql1);
            srcMagaza.Properties.DisplayMember = "DIVNAME";
            srcMagaza.Properties.ValueMember = "DIVVAL";            
        }
        private void Kullanici()
        {
            var dt = conn.GetData(@"select SOCODE,SOENTERKEY,SONAME,SOSURNAME,SONAME +space(1)+SOSURNAME as SOFULLNAME,SODEPART as DEPVAL,SOAGIENTID,DEPNAME,SOADMIN,SOAPP,SOSTS,SOISTISARE,SOMOBILE,SOMOBILEKIND from SOCIAL
            left outer join DEPARTMENT on DEPVAL = SODEPART
            where SOCODE not in ('FK', 'VOLANT')", sql1);
            gridKullanicilar.DataSource = dt;
            ViewKullanicilar.OptionsView.BestFitMaxRowCount = -1;
            ViewKullanicilar.BestFitColumns(true);
            layoutControl1.Enabled = false;
        }
        private void ViewKullanicilar_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            Department();
            var ss = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOMOBILEKIND").ToString();
            txtSOCODE.Text = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOCODE").ToString();
            txtSONAME.Text = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SONAME").ToString();
            txtSOSURNAME.Text = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOSURNAME").ToString();
            txtSOENTERKEY.Text = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOENTERKEY").ToString();
            txtSOENTERKEY2.Text = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOENTERKEY").ToString();
            srcDepartman.EditValue = ViewKullanicilar.GetRowCellValue(e.RowHandle, "DEPVAL").ToString();
            chkAktif.EditValue = (bool)ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOSTS");
            chkMobil.EditValue = (bool)ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOMOBILE");
            chkAdmin.EditValue = (bool)ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOADMIN");
            chkIstisare.EditValue = (bool)ViewKullanicilar.GetRowCellValue(e.RowHandle, "SOISTISARE");
            if (ss != "")
            {
                cmbMobilTip.SelectedIndex = int.Parse(ss.ToString()) -1;
            }
            else
            {
                cmbMobilTip.SelectedIndex = -1;
            }
            if (ViewKullanicilar.GetRowCellValue(e.RowHandle, "DEPVAL").ToString() == "001" || ViewKullanicilar.GetRowCellValue(e.RowHandle, "DEPVAL").ToString() == "002")
            {
                srcMagaza.EditValue = conn.GetValue($"select CHDIVISON from CASHIER where CHSOCODE = '{txtSOCODE.Text}'", sql1);
            }

        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Kullanıcı Hesapları Listeleniyor");
                Kullanici();
                Department();
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
        private async void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool tamam = false;
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Kullanıcı Hesapları Listeleniyor");

                if (string.IsNullOrWhiteSpace(txtSOENTERKEY2.Text))
                {
                    throw new Exception("Şifre Tekrarı Boş Olamaz");
                }
                else if (txtSOENTERKEY.Text != txtSOENTERKEY2.Text)
                {
                    throw new Exception("Şifreler Uyumsuz");
                }
                else
                {
                    if (NewUser)
                    {
                        if (srcDepartman.EditValue == null)
                        {
                            throw new Exception("Personel Çalışma Grubu Seçiniz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSOCODE.Text))
                        {
                            throw new Exception("Kullanıcı İşlem Kodu Boş Olamaz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSOENTERKEY.Text))
                        {
                            throw new Exception("Kullanıcı Şifresi Boş Olamaz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSONAME.Text))
                        {
                            throw new Exception("Kullanıcı Adı Boş Olamaz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSOSURNAME.Text))
                        {
                            throw new Exception("Kullanıcı Soyadı Boş Olamaz");
                        }
                        else
                        {
                            string BulkInsertReturn = "";
                            string BulkInsertReturn2 = "";
                            string BulkInsertReturn3 = "";
                            List<SOCIAL> sOCIALs = new List<SOCIAL>
                            {
                                    new SOCIAL
                                    {
                                        SOCODE = txtSOCODE.Text,
                                        SOENTERKEY = txtSOENTERKEY.Text,
                                        SONAME = txtSONAME.Text,
                                        SOSURNAME = txtSOSURNAME.Text,
                                        SODEPART = srcDepartman.EditValue.ToString(),
                                        SOSTS = true,
                                        SOADMIN = chkAdmin.Checked,
                                        SOAGIENTID = txtSOAGIENTID.Text,
                                        SOMOBILE = chkMobil.Checked,
                                        SOAPP = false,
                                        SOMOBILEKIND = System.Convert.ToInt16(cmbMobilTip.SelectedIndex+1),
                                        SOCURID = null,
                                        SOISTISARE = chkIstisare.Checked
                                    }
                            };
                            List<CASHIER> cASHIERs = new List<CASHIER>();
                            List<DEFSAFE> dEFSAves = new List<DEFSAFE>();
                            List<JOINSAFE> jOINSAves = new List<JOINSAFE>();
                            if (srcDepartman.EditValue.ToString() == "001" || srcDepartman.EditValue.ToString() == "002")
                            {
                                dEFSAves.Add(
                                new DEFSAFE
                                {
                                    DSAFEID = await REGISTER("119", sql1),
                                    DSAFEVAL = $"C0.{txtSOCODE.Text}.TL",
                                    DSAFENAME = $"Kasiyer {txtSONAME.Text} {txtSOSURNAME.Text} TL Kasası",
                                    DSAFEEXCH = "TL",
                                    DSAFEUNITE = $"Group{txtSOCODE.Text}",
                                    DSAFEDIVISON = srcMagaza.EditValue.ToString(),
                                    DSAFEOLDVAL = "",
                                    DSAFEKIND = "K",
                                    DSAFESTS = chkAktif.Checked,
                                });

                                cASHIERs.Add(
                                new CASHIER
                                {
                                    CHVAL = txtSOCODE.Text,
                                    CHDIVISON = srcDepartman.EditValue.ToString(),
                                    CHSOCODE = txtSOCODE.Text,
                                    CHSAFEUNI = dEFSAves[0].DSAFEUNITE,
                                    CHSTS = chkAktif.Checked,
                                });
                                var ACCID = conn.GetValueConnection($@"
                                select JSAFEACCID from DEFSAFE 
                                join JOINSAFE on JSAFEID = DSAFEID
                                where JSAFEYEAR = {DateTime.Today.Year} and DSAFEKIND = 'A' and DSAFEDIVISON = '{srcMagaza.EditValue}'", sql1);
                                jOINSAves.Add(
                                new JOINSAFE
                                {
                                    JSAFEID = dEFSAves[0].DSAFEID,
                                    JSAFEYEAR = DateTime.Today.Year,
                                    JSAFEACCID = long.Parse(ACCID),
                                });
                            }

                            using (var sqlConnection = new SqlConnection(sql1))
                            {
                                await sqlConnection.OpenAsync();
                                using (var tran = sqlConnection.BeginTransaction())
                                {
                                    var db = new DbTrans(sqlConnection, tran);
                                    try
                                    {
                                        var SOCIAL = convertler.ToDataTable(sOCIALs);
                                        BulkInsertReturn = db.BulkInsertRetorn(SOCIAL, "SOCIAL");
                                        if (BulkInsertReturn == "Aktarım Tamamlandı")
                                        {
                                            if (srcDepartman.EditValue.ToString() == "001" || srcDepartman.EditValue.ToString() == "002")
                                            {
                                                var CASHIER = convertler.ToDataTable(cASHIERs);
                                                BulkInsertReturn2 = db.BulkInsertRetorn(CASHIER, "CASHIER");
                                                if (BulkInsertReturn2 == "Aktarım Tamamlandı")
                                                {
                                                    var DEFSAFE = convertler.ToDataTable(dEFSAves);
                                                    BulkInsertReturn3 = db.BulkInsertRetorn(DEFSAFE, "DEFSAFE");
                                                    if (BulkInsertReturn3 == "Aktarım Tamamlandı")
                                                    {
                                                        var JOINSAFE = convertler.ToDataTable(jOINSAves);
                                                        db.BulkInsertRetorn(JOINSAFE, "JOINSAFE");
                                                        tran.Commit(); // Başarılıysa commit
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                tran.Commit(); // Başarılıysa commit
                                            }
                                        }
                                        else
                                        {
                                            tran.Rollback(); // Hata olursa rollback
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        tran.Rollback(); // Hata olursa rollback
                                        throw new Exception(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (srcDepartman.EditValue == null)
                        {
                            throw new Exception("Personel Çalışma Grubu Seçiniz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSOCODE.Text))
                        {
                            throw new Exception("Kullanıcı İşlem Kodu Boş Olamaz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSOENTERKEY.Text))
                        {
                            throw new Exception("Kullanıcı Şifresi Boş Olamaz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSONAME.Text))
                        {
                            throw new Exception("Kullanıcı Adı Boş Olamaz");
                        }
                        else if (string.IsNullOrWhiteSpace(txtSOSURNAME.Text))
                        {
                            throw new Exception("Kullanıcı Soyadı Boş Olamaz");
                        }
                        else
                        {
                            bool Cashier = false;
                            long DSAFEID = 0;
                            string ACCID = "";
                            string BulkInsertReturn2 = "";
                            string BulkInsertReturn3 = "";
                            List<CASHIER> cASHIERs = new List<CASHIER>();
                            List<DEFSAFE> dEFSAves = new List<DEFSAFE>();
                            List<JOINSAFE> jOINSAves = new List<JOINSAFE>();
                            var checkCashier = conn.GetData($@"select * from CASHIER
                            join DEFSAFE on DSAFEUNITE = CHSAFEUNI
                            where CHSOCODE = '{txtSOCODE.Text}'", sql1);
                            if (checkCashier != null)
                            {
                                if (checkCashier.Rows.Count > 0)
                                {
                                    DSAFEID = long.Parse(conn.GetValueConnection($@"select DSAFEID from DEFSAFE where DSAFEUNITE = '{checkCashier.Rows[0]["CHSAFEUNI"].ToString()}'", sql1));
                                    Cashier = true;
                                }
                            }
                            else
                            {
                                if (srcDepartman.EditValue.ToString() == "001" || srcDepartman.EditValue.ToString() == "002")
                                {
                                    dEFSAves.Add(
                                    new DEFSAFE
                                    {
                                        DSAFEID = await REGISTER("119", sql1),
                                        DSAFEVAL = $"C0.{txtSOCODE.Text}.TL",
                                        DSAFENAME = $"Kasiyer {txtSONAME.Text} {txtSOSURNAME.Text} TL Kasası",
                                        DSAFEEXCH = "TL",
                                        DSAFEUNITE = $"Group{txtSOCODE.Text}",
                                        DSAFEDIVISON = srcMagaza.EditValue.ToString(),
                                        DSAFEOLDVAL = "",
                                        DSAFEKIND = "K",
                                        DSAFESTS = chkAktif.Checked,
                                    });

                                    cASHIERs.Add(
                                    new CASHIER
                                    {
                                        CHVAL = txtSOCODE.Text,
                                        CHDIVISON = srcMagaza.EditValue.ToString(),
                                        CHSOCODE = txtSOCODE.Text,
                                        CHSAFEUNI = dEFSAves[0].DSAFEUNITE,
                                        CHSTS = chkAktif.Checked,
                                    });

                                    ACCID = conn.GetValueConnection($@"
                                select JSAFEACCID from DEFSAFE 
                                join JOINSAFE on JSAFEID = DSAFEID
                                where JSAFEYEAR = {DateTime.Today.Year} and DSAFEKIND = 'A' and DSAFEDIVISON = '{srcMagaza.EditValue}'", sql1);
                                    jOINSAves.Add(
                                    new JOINSAFE
                                    {
                                        JSAFEID = dEFSAves[0].DSAFEID,
                                        JSAFEYEAR = DateTime.Today.Year,
                                        JSAFEACCID = long.Parse(ACCID),
                                    });
                                }
                            }
                            using (var sqlConnection = new SqlConnection(sql1))
                            {
                                await sqlConnection.OpenAsync();
                                using (var tran = sqlConnection.BeginTransaction())
                                {
                                    var db = new DbTrans(sqlConnection, tran);
                                    using (SqlCommand cmd = new SqlCommand())
                                    {
                                        try
                                        {
                                            cmd.Transaction = tran;
                                            cmd.Connection = sqlConnection;
                                            cmd.CommandType = CommandType.Text;
                                            cmd.CommandText = $@"
                                            update SOCIAL set
                                            SOENTERKEY = @SOENTERKEY,
                                            SONAME = @SONAME,
                                            SOSURNAME = @SOSURNAME,
                                            SODEPART = @SODEPART,
                                            SOSTS = @SOSTS,
                                            SOADMIN = @SOADMIN ,
                                            SOAGIENTID = @SOAGIENTID,
                                            SOMOBILE = @SOMOBILE,
                                            SOMOBILEKIND = @SOMOBILEKIND,
                                            SOISTISARE = @SOISTISARE
                                            where SOCODE = @SOCODE";
                                            cmd.Parameters.AddWithValue("@SOENTERKEY", txtSOENTERKEY.Text);
                                            cmd.Parameters.AddWithValue("@SONAME", txtSONAME.Text);
                                            cmd.Parameters.AddWithValue("@SOSURNAME", txtSOSURNAME.Text);
                                            cmd.Parameters.AddWithValue("@SODEPART", srcDepartman.EditValue);
                                            cmd.Parameters.AddWithValue("@SOSTS", chkAktif.Checked ? 1 : 0);
                                            cmd.Parameters.AddWithValue("@SOADMIN", chkAdmin.Checked ? 1 : 0);
                                            cmd.Parameters.AddWithValue("@SOAGIENTID", txtSOAGIENTID.Text);
                                            cmd.Parameters.AddWithValue("@SOMOBILE", chkMobil.Checked ? 1 : 0);
                                            cmd.Parameters.AddWithValue("@SOMOBILEKIND", cmbMobilTip.SelectedIndex == -1 ? (object)DBNull.Value : cmbMobilTip.SelectedIndex + 1);
                                            cmd.Parameters.AddWithValue("@SOISTISARE", chkIstisare.Checked ? 1 : 0);
                                            cmd.Parameters.AddWithValue("@SOCODE", txtSOCODE.Text);
                                            var sonuc = cmd.ExecuteNonQuery();

                                            if (sonuc != 0)
                                            {                                                
                                                tamam = true;                                                
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            tran.Rollback(); // Hata olursa rollback
                                            throw new Exception(ex.Message);
                                        }

                                    }
                                    if (tamam)
                                    {
                                        if (!Cashier)
                                        {
                                            if (srcDepartman.EditValue.ToString() == "001" || srcDepartman.EditValue.ToString() == "002")
                                            {
                                                var CASHIER = convertler.ToDataTable(cASHIERs);
                                                BulkInsertReturn2 = db.BulkInsertRetorn(CASHIER, "CASHIER");
                                                if (BulkInsertReturn2 == "Aktarım Tamamlandı")
                                                {
                                                    var DEFSAFE = convertler.ToDataTable(dEFSAves);
                                                    BulkInsertReturn3 = db.BulkInsertRetorn(DEFSAFE, "DEFSAFE");
                                                    if (BulkInsertReturn3 == "Aktarım Tamamlandı")
                                                    {
                                                        tran.Commit(); // Başarılıysa commit
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                tran.Commit(); // Başarılıysa commit
                                            }
                                        }
                                        else
                                        {
                                            if (srcDepartman.EditValue.ToString() == "001" || srcDepartman.EditValue.ToString() == "002")
                                            {
                                                using (SqlCommand cmd = new SqlCommand())
                                                {
                                                    cmd.Transaction = tran;
                                                    cmd.Connection = sqlConnection;
                                                    cmd.CommandType = CommandType.Text;
                                                    cmd.CommandText = "update CASHIER set CHDIVISON = @CHDIVISON,CHSTS = @CHSTS where CHSOCODE = @SOCODE";
                                                    cmd.Parameters.AddWithValue("@CHDIVISON", srcMagaza.EditValue);
                                                    cmd.Parameters.AddWithValue("@CHSTS", chkAktif.Checked ? 1 : 0);
                                                    cmd.Parameters.AddWithValue("@SOCODE", txtSOCODE.Text);
                                                    cmd.ExecuteNonQuery();
                                                }
                                                using (SqlCommand cmd = new SqlCommand())
                                                {
                                                    cmd.Transaction = tran;
                                                    cmd.Connection = sqlConnection;
                                                    cmd.CommandType = CommandType.Text;
                                                    cmd.CommandText = "update DEFSAFE set DSAFEDIVISON = @CHDIVISON, DSAFESTS = @CHSTS where DSAFEID = @DSAFEID";
                                                    cmd.Parameters.AddWithValue("@CHDIVISON", srcMagaza.EditValue);
                                                    cmd.Parameters.AddWithValue("@CHSTS", chkAktif.Checked ? 1 : 0);
                                                    cmd.Parameters.AddWithValue("@DSAFEID", DSAFEID);
                                                    cmd.ExecuteNonQuery();
                                                }
                                                using (SqlCommand cmd = new SqlCommand())
                                                {
                                                    cmd.Transaction = tran;
                                                    cmd.Connection = sqlConnection;
                                                    cmd.CommandType = CommandType.Text;
                                                    cmd.CommandText = "update JOINSAFE set JSAFEACCID = @ACCID where JSAFEID = @DSAFEID";
                                                    cmd.Parameters.AddWithValue("@ACCID", ACCID);
                                                    cmd.Parameters.AddWithValue("@DSAFEID", DSAFEID);
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }
                                            else
                                            {
                                                db.InsertValue($"delete CASHIER where CHSOCODE = '{txtSOCODE.Text}'");
                                                db.InsertValue($"delete DEFSAFE where DSAFEID = {DSAFEID}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowMessage(ex.Message, "", this, "Uyarı", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
            if (tamam)
            {
                txtSOCODE.Text = null;
                txtSONAME.Text = null;
                txtSOSURNAME.Text = null;
                txtSOENTERKEY.Text = null;
                txtSOENTERKEY2.Text = null;
                srcDepartman.EditValue = null;
                srcMagaza.EditValue = null;
                chkAktif.EditValue = null;
                chkMobil.EditValue = null;
                chkAdmin.EditValue = null;
                chkIstisare.EditValue = null;
                cmbMobilTip.Enabled = false;
                cmbMobilTip.SelectedIndex = -1;
                navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
                layoutControl1.Enabled = false;
                NewUser = false;
                Kullanici();
                Department();
                CustomMessageBox.ShowMessage("İşlem Tamam", "", this, "Bilgilendirme", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public async static Task<long> REGISTER(string _RGKIND, string _ConnectionString)
        {
            long RGID = 0;
            using (var sqlConnection = new SqlConnection(_ConnectionString))
            {
                await sqlConnection.OpenAsync();

                using (var tran = sqlConnection.BeginTransaction())
                {
                    var db = new DbTrans(sqlConnection, tran);
                    try
                    {
                        var ID = db.GetValue($@"
                                UPDATE REGISTER
                                SET RGID = RGID + 1
                                OUTPUT INSERTED.RGID
                                WHERE RGCOMPANY = ''
                                  AND RGKIND = {_RGKIND}
                                  AND RGVAL1 = ''
                                  AND RGVAL2 = ''
                                  AND RGDATE = 0");
                        RGID = long.Parse(ID);
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
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtSOCODE.Text = null;
            txtSONAME.Text = null;
            txtSOSURNAME.Text = null;
            txtSOENTERKEY.Text = null;
            txtSOENTERKEY2.Text = null;
            srcDepartman.EditValue = null;
            srcMagaza.EditValue = null;
            chkAktif.EditValue = null;
            chkMobil.EditValue = null;
            chkAdmin.EditValue = null;
            chkIstisare.EditValue = null;
            cmbMobilTip.Enabled = false;
            cmbMobilTip.SelectedIndex = -1;
            navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            layoutControl1.Enabled = false;
            NewUser = false;
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtSOCODE.Enabled = true;
            txtSOCODE.Text = null;
            txtSONAME.Text = null;
            txtSOSURNAME.Text = null;
            txtSOENTERKEY.Text = null;
            txtSOENTERKEY2.Text = null;
            srcDepartman.EditValue = null;
            srcMagaza.EditValue = null;
            chkAktif.Checked = false;
            chkMobil.Checked = false;
            chkAdmin.Checked = false;
            chkIstisare.Checked = false;
            cmbMobilTip.Enabled = true;
            srcMagaza.Enabled = false;
            cmbMobilTip.SelectedIndex = -1;
            NewUser = true;
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            layoutControl1.Enabled = true;
            txtSOCODE.Enabled = false;
            NewUser = false;
            if (srcDepartman.EditValue.ToString() == "001" || srcDepartman.EditValue.ToString() == "002")
            {
                srcMagaza.EditValue = conn.GetValue($"select CHDIVISON from CASHIER where CHSOCODE = '{txtSOCODE.Text}'", sql1);
            }
            else
            {
                srcMagaza.Enabled = false;
            }
        }

        private async void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Kullanıcı Hesapları Listeleniyor");
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

        private void chkMobil_CheckedChanged(object sender, EventArgs e)
        {
            cmbMobilTip.Enabled = chkMobil.Checked;
            //var sonuc = EntegreFDLL.Main.ImageDownloader(string url, string savePath, string fileName, string format = "png");
        }

        private void srcDepartman_EditValueChanged(object sender, EventArgs e)
        {
            if (srcDepartman.EditValue != null)
            {
                if (srcDepartman.EditValue.ToString() == "001" || srcDepartman.EditValue.ToString() == "002")
                {
                    srcMagaza.Enabled = true;
                }
                else
                {
                    srcMagaza.Enabled = false;
                }
            }
        }
    }
}