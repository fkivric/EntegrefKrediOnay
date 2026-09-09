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
using EntegreFDLL;
using static DataTableExtensions;
using DevExpress.LookAndFeel;
using EntegrefKrediOnay.Class;
using System.Data.SqlClient;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmUrunTemelGrup : DevExpress.XtraEditors.XtraForm
    {
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        static Convertler convertler = new Convertler();
        public frmUrunTemelGrup()
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Stok İşlemleri Açılıyor");
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
                navBarUNITED.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
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
        bool Yeni = false;
        private void frmUrunTemelGrup_Load(object sender, EventArgs e)
        {
            Liste();
            layoutControl1.Enabled = false;
        }
        private void Liste()
        {
            var dt = conn.GetData("select * from PRODUCTSUNITED", sql1);
            gridUNITED.DataSource = dt;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Liste();
            Yeni = false;
            layoutControl1.Enabled = false;
            navBarUNITED.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Yeni)
            {
                var PROUID = REGISTER("136", sql1);
                conn.InsertValue($@"insert into PRODUCTSUNITED values ({PROUID},'{txtPROUVAL.Text}','5','{txtPROUNAME.Text}','','U',0)", sql1);
            }
            Liste();
            navBarUNITED.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
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
            navBarUNITED.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            layoutControl1.Enabled = false;
            Yeni = false;
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            layoutControl1.Enabled = true;
            txtPROUVAL.Enabled = true;
            txtPROUNAME.Enabled = true;
            txtPROUVAL.Text = null;
            txtPROUNAME.Text = null;
            Yeni = true;
            navBarUNITED.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            layoutControl1.Enabled = true;
            navBarUNITED.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
        }

        private void ViewUNITED_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Clicks ==2 && e.Button == MouseButtons.Left && e.RowHandle >= 0)
            {
                if (!Yeni)
                {
                    try
                    {
                        EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Stok İşlemleri Açılıyor");
                        txtPROUNAME.Text = ViewUNITED.GetRowCellValue(e.RowHandle, "PROUNAME").ToString();
                        txtPROUVAL.Text = ViewUNITED.GetRowCellValue(e.RowHandle, "PROUVAL").ToString();
                        txtPROUVAL.Tag = ViewUNITED.GetRowCellValue(e.RowHandle, "PROUID").ToString();
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
    }
}