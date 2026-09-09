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
using DevExpress.XtraGrid.Views.Grid;
using System.Data.SqlClient;
using System.Threading;
using DevExpress.LookAndFeel;
using EntegreFDLL;
using EntegrefKrediOnay.Class;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmBGAIParametre : DevExpress.XtraEditors.XtraForm
    {
        public frmBGAIParametre()
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "ENTEGREF AI PARAMETRE", Properties.Settings.Default.Company, "Veriler Kontrol Ediliyor");
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
                //InitUI();
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
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
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
        private void frmAIParametre_Load(object sender, EventArgs e)
        {
            var dt = conn.GetData($@"select * from EntegreF.dbo.ENTEGREF_AISCORING where AIDEF_SOCODE = '{EntegreFDLL.Class.Entegref.GetLogins.userID}'", sql1);
            foreach (DataRow row in dt.Rows)
            {
                row["AIDEF_BOOL"] = Convert.ToBoolean(row["AIDEF_BOOL"]);
            }
            gridAIParametre.DataSource = dt;
            ViewAIParametre.ExpandAllGroups();
            ViewAIParametre.OptionsView.BestFitMaxRowCount = -1;
            ViewAIParametre.BestFitColumns(true);
            ViewAIParametre.OptionsView.RowAutoHeight = true;
        }
        private void toggleSwitchAktif_EditValueChanged(object sender, EventArgs e)
        {
            GridView view = ViewAIParametre;
            int rowHandle = view.FocusedRowHandle;

            // Yeni değer
            bool yeniDurum = Convert.ToBoolean(view.GetRowCellValue(rowHandle, "AIDEF_BOOL"));

            // SQL'e güncelleme gönder
            long id = Convert.ToInt64(view.GetRowCellValue(rowHandle, "ID")); // ID kolonunu senin tabloya göre ayarla
            int sqlDeger = yeniDurum ? 1 : 0;
            //string sql = $"UPDATE TabloAdi SET Aktif = {sqlDeger} WHERE ID = {id}";
            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();
            //    using (SqlCommand cmd = new SqlCommand(sql, conn))
            //    {
            //        cmd.ExecuteNonQuery();
            //    }
            //}

        }
    }
}