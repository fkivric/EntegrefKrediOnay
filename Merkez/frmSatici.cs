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
    public partial class frmSatici : DevExpress.XtraEditors.XtraForm
    {
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        static Convertler convertler = new Convertler();
        public class SALESMEN
        {
            public long SMENID { get; set; }

            public string SMENVAL { get; set; }

            public string SMENNAME { get; set; }

            public long? SMENCURID { get; set; } = null;

            public string SMENCOMPANY { get; set; } = "01";

            public string SMENDIVISON { get; set; }

            public bool SMENLEFT { get; set; }

            public bool SMENSTS { get; set; }

            public string SMENSOCODE { get; set; } = null;

            public bool SMENMANAGER { get; set; }

        }
        public frmSatici()
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
        }
        private void Kullanici()
        {
            var dt = conn.GetData(@"select SMENID,SMENVAL,SMENNAME,DIVVAL,DIVNAME,SMENSTS,SMENLEFT,SMENMANAGER from SALESMEN
            left outer join DIVISON on DIVVAL = SMENDIVISON", sql1);
            gridKullanicilar.DataSource = dt;
            ViewKullanicilar.OptionsView.BestFitMaxRowCount = -1;
            ViewKullanicilar.BestFitColumns(true);


            srcMagaza.Properties.DataSource = conn.GetData("select DIVVAL,DIVNAME from DIVISON where DIVSTS = 1 and DIVSALESTS = 1", sql1);
            srcMagaza.Properties.DisplayMember = "DIVNAME";
            srcMagaza.Properties.ValueMember = "DIVVAL";
            layoutControl1.Enabled = false;
        }
        private void ViewKullanicilar_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            txtSOCODE.Tag = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SMENID").ToString();
            txtSOCODE.Text = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SMENVAL").ToString();
            txtSOSURNAME.Text = ViewKullanicilar.GetRowCellValue(e.RowHandle, "SMENNAME").ToString();
            chkAktif.EditValue = (bool)ViewKullanicilar.GetRowCellValue(e.RowHandle, "SMENSTS");
            chkLeft.EditValue = (bool)ViewKullanicilar.GetRowCellValue(e.RowHandle, "SMENLEFT");
            chkManager.EditValue = (bool)ViewKullanicilar.GetRowCellValue(e.RowHandle, "SMENMANAGER");
            srcMagaza.EditValue = ViewKullanicilar.GetRowCellValue(e.RowHandle, "DIVVAL").ToString();
            NewUser = false;
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Kullanıcı Hesapları Listeleniyor");
                Kullanici();
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

                if (NewUser)
                {
                    if (string.IsNullOrWhiteSpace(txtSOCODE.Text))
                    {
                        throw new Exception("Kullanıcı İşlem Kodu Boş Olamaz");
                    }
                    else if (string.IsNullOrWhiteSpace(txtSOSURNAME.Text))
                    {
                        throw new Exception("Kullanıcı Soyadı Boş Olamaz");
                    }
                    else
                    {
                        string BulkInsertReturn = "";
                        List<SALESMEN> sALESMENs = new List<SALESMEN>();
                        sALESMENs.Add(
                        new SALESMEN
                        {
                            SMENID = await REGISTER("138", sql1),
                            SMENVAL = txtSOCODE.Text,
                            SMENNAME = txtSOSURNAME.Text,
                            SMENDIVISON = srcMagaza.EditValue.ToString(),
                            SMENSTS = chkAktif.Checked,
                            SMENLEFT = chkLeft.Checked,
                            SMENMANAGER = chkManager.Checked
                        });
                        using (var sqlConnection = new SqlConnection(sql1))
                        {
                            await sqlConnection.OpenAsync();
                            using (var tran = sqlConnection.BeginTransaction())
                            {
                                var db = new DbTrans(sqlConnection, tran);
                                try
                                {
                                    var SALESMEN = convertler.ToDataTable(sALESMENs);
                                    BulkInsertReturn = db.BulkInsertRetorn(SALESMEN, "SALESMEN");
                                    if (BulkInsertReturn == "Aktarım Tamamlandı")
                                    {

                                        tran.Commit(); // Başarılıysa commit
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
                    if (string.IsNullOrWhiteSpace(txtSOCODE.Text))
                    {
                        throw new Exception("Kullanıcı İşlem Kodu Boş Olamaz");
                    }
                    else if (string.IsNullOrWhiteSpace(txtSOSURNAME.Text))
                    {
                        throw new Exception("Kullanıcı Soyadı Boş Olamaz");
                    }
                    else
                    {
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
                                        cmd.CommandText = $@"update SALESMEN set SMENNAME = @SOSURNAME, SMENSTS = @SOSTS, SMENDIVISON = @DIVVAL, SMENLEFT = @Left, SMENMANAGER = @Manager where SMENID = @ID";
                                        cmd.Parameters.AddWithValue("@SOSURNAME", txtSOSURNAME.Text);
                                        cmd.Parameters.AddWithValue("@SOSTS", chkAktif.Checked ? 1 : 0);
                                        cmd.Parameters.AddWithValue("@Left", chkLeft.Checked ? 1 : 0);
                                        cmd.Parameters.AddWithValue("@Manager", chkManager.Checked ? 1 : 0);
                                        cmd.Parameters.AddWithValue("@DIVVAL", srcMagaza.EditValue);
                                        cmd.Parameters.AddWithValue("@ID", txtSOCODE.Tag);
                                        var sonuc = cmd.ExecuteNonQuery();

                                        if (sonuc != 0)
                                        {
                                            tran.Commit();
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
                txtSOSURNAME.Text = null;
                srcMagaza.EditValue = null;
                chkAktif.EditValue = null;
                chkLeft.EditValue = null;
                chkManager.EditValue = null;
                chkAktif.Checked = false;
                chkLeft.Checked = false;
                chkManager.Checked = false;
                navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
                layoutControl1.Enabled = false;
                NewUser = false;
                Kullanici();
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
            txtSOSURNAME.Text = null;
            srcMagaza.EditValue = null;
            chkAktif.EditValue = null;
            chkLeft.EditValue = null;
            chkManager.EditValue = null;
            chkAktif.Checked = false;
            chkLeft.Checked = false;
            chkManager.Checked = false;
            navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            layoutControl1.Enabled = false;
            NewUser = false;
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtSOCODE.Enabled = true;
            txtSOCODE.Text = null;
            txtSOSURNAME.Text = null;
            srcMagaza.EditValue = null;
            chkAktif.EditValue = null;
            chkLeft.EditValue = null;
            chkManager.EditValue = null;
            chkAktif.Checked = false;
            chkLeft.Checked = false;
            chkManager.Checked = false;
            NewUser = true;
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            layoutControl1.Enabled = true;
            txtSOCODE.Enabled = false;
            NewUser = false;
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
    }
}