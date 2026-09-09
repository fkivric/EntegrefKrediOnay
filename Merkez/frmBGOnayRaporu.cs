using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraEditors;
using System.Threading;
using DevExpress.XtraSplashScreen;
using System.Data.SqlClient;
using DevExpress.XtraGrid.Views.Grid;
using EntegreFDLL;
using EntegrefKrediOnay.Class;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmBGOnayRaporu : DevExpress.XtraEditors.XtraForm
    {
        SqlConnectionObject conn = new SqlConnectionObject();
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        public frmBGOnayRaporu()
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.CompanyName, "Onay Raporu Açılıyor");
                InitializeComponent();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
        }
        //arka plan işlemleri
        private BackgroundWorker _backgroundWorker;
        private ManualResetEvent _workerCompletedEvent = new ManualResetEvent(false);
        private const string READY_TEXT = "Hazır";
        private void executeBackground(Action doWorkAction, Action progressAction = null, Action completedAction = null)
        {
            try
            {
                if (_backgroundWorker != null)
                {


                    if (_backgroundWorker.IsBusy)
                    {
                        XtraMessageBox.Show("Her oturum açıldığında 1 işlem yapacak. Eğer bu girişteki ilk işlemse uygulama çalışmaktadır. Lütfen Bekleyiniz");
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
        private void frmBGOnayRaporu_Load(object sender, EventArgs e)
        {
            chklistInvestigation.DataSource = conn.GetData("select DINSID,DINSNAME from EntegreF..DEFINVESTIGATION", Properties.Settings.Default.connectionstring);
            chklistInvestigation.ValueMember = "DINSID";
            chklistInvestigation.DisplayMember = "DINSNAME";
        }
        class DEFINVESTIGATION
        {
            public string DINSID { get; set; }
            public string DINSNAME { get; set; }
        }
        private void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            List<DEFINVESTIGATION> dEFINVESTIGATIONs = new List<DEFINVESTIGATION>();
            string DINSID = "";
            DataTable dt = new DataTable();
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.CompanyName, "Onay durum Listesi Çekliyor");
                for (int i = 0; i < chklistInvestigation.ItemCount; i++)
                {
                    if (chklistInvestigation.GetItemChecked(i) == true)
                    {
                        dEFINVESTIGATIONs.Add(new DEFINVESTIGATION
                        {
                            DINSID = chklistInvestigation.GetItemValue(i).ToString(),
                            DINSNAME = chklistInvestigation.GetItemText(i).ToString()
                        });
                    }
                }
                if (dEFINVESTIGATIONs.Count == 0)
                {
                    CustomMessageBox.ShowMessage("İşlem Tipi Seçilmedi", "", this, "UYARI", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DINSID = string.Join(",", dEFINVESTIGATIONs
                            .Select(m => m.DINSID));
                    dt = conn.GetData($@"
                    select DIVVAL,DIVNAME,CURVAL,CURNAME,INVESCURID,INVESSALID,SALAMOUNT,INVESDATE,DINSNAME,s.INVESSALNOTES,INVESRISK,INVESCURLATE,WRTRNAME + ' ' + WRTRSURNAME as INVESWARANTER from EntegreF..INVESTIGATIONVALUE s
                    left outer join EntegreF..DEFINVESTIGATION on INVESDINSID = DINSID
                    left outer join SALES on SALID = INVESSALID
                    left outer join CURRENTS on CURID = INVESCURID
                    left outer join DIVISON on DIVVAL = SALDIVISON
                    left outer join WARRANTERS on WRTRID = INVESWARANTER
                    where INVESSOCODE = '{EntegreFDLL.Class.Entegref.GetLogins.userID}' and INVESDINSID in ({DINSID})
                    order by 3,7
                    ", Properties.Settings.Default.connectionstring);
                }
                gridOnayRapor.DataSource = dt;
                ViewOnayRapor.OptionsBehavior.Editable = false;
                ViewOnayRapor.OptionsBehavior.ReadOnly = true;
                ViewOnayRapor.OptionsSelection.EnableAppearanceFocusedRow = false;
                ViewOnayRapor.Appearance.FocusedRow.Options.UseBackColor = false;
                ViewOnayRapor.Appearance.SelectedRow.Options.UseBackColor = false;
                ViewOnayRapor.OptionsView.ColumnAutoWidth = false;
                ViewOnayRapor.ExpandAllGroups();
                ViewOnayRapor.OptionsView.BestFitMaxRowCount = -1;
                ViewOnayRapor.BestFitColumns(true);
                ViewOnayRapor.Appearance.FocusedCell.Options.UseBackColor = false;
                ViewOnayRapor.Appearance.FocusedCell.Options.UseForeColor = false;
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

        private void ViewOnayRapor_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                int dpdeedname = int.Parse(view.GetRowCellValue(e.RowHandle, "INVESRISK").ToString());
                if (dpdeedname > 50)
                {
                    e.Appearance.BackColor = Color.DarkRed;
                    e.Appearance.BackColor2 = Color.Black;
                    e.Appearance.ForeColor = Color.AntiqueWhite;
                    e.HighPriority = true;
                }
            }
        }
        private void ViewOnayRapor_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.RowHandle >= 0 && e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                var curid = ViewOnayRapor.GetRowCellValue(e.RowHandle, "INVESCURID").ToString();
                var salid = ViewOnayRapor.GetRowCellValue(e.RowHandle, "INVESSALID").ToString();
                var curname = ViewOnayRapor.GetRowCellValue(e.RowHandle, "CURNAME").ToString();
                var waranter = ViewOnayRapor.GetRowCellValue(e.RowHandle, "INVESWARANTER").ToString();
                frmBGMusteriOnayli musteriOnayli = new frmBGMusteriOnayli(curid, salid, curname, waranter);
                musteriOnayli.ShowDialog();
            }
        }
        private void müşteriİşlemDetayıGörToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var curid = ViewOnayRapor.GetRowCellValue(ViewOnayRapor.FocusedRowHandle, "INVESCURID").ToString();
            var salid = ViewOnayRapor.GetRowCellValue(ViewOnayRapor.FocusedRowHandle, "INVESSALID").ToString();
            var curname = ViewOnayRapor.GetRowCellValue(ViewOnayRapor.FocusedRowHandle, "CURNAME").ToString();
            var waranter = ViewOnayRapor.GetRowCellValue(ViewOnayRapor.FocusedRowHandle, "INVESWARANTER").ToString();
            frmBGMusteriOnayli musteriOnayli = new frmBGMusteriOnayli(curid, salid, curname, waranter);
            musteriOnayli.ShowDialog();
        }
    }
}