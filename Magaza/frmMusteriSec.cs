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
using System.Threading;
using DevExpress.XtraSplashScreen;
using DevExpress.LookAndFeel;
using EntegreFDLL;
using EntegrefKrediOnay.Class;
using EntegreFDLL.Class;

namespace EntegrefKrediOnay.Magaza
{
    public partial class frmMusteriSec : DevExpress.XtraEditors.XtraForm
    {
        Form Gonderen;
        public frmMusteriSec(Form _gon)
        {
            try
            {
                Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Müşteri Seçimi Açılıyor");
                InitializeComponent();
                Gonderen = _gon;
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;                
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
        DataTable dt = new DataTable();
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
        private void frmStokBul_Load(object sender, EventArgs e)
        {
            dt = conn.GetData($@"
            select distinct top 1000 CURID,CURVAL,CURNAME,
            sum(case when SALID > 0 then SALAMOUNT else -1*SALAMOUNT end) as SALAMOUNT,SALDATE,
            case when IPID is not null then 'Var' else 'Yok' end as Kimlik
            from CURRENTS
            left outer join CUSIDENTITY on CUSIDCURID = CURID
            left outer join CUSTOMER on CURID = CUSCURID
            left outer join SALES on CURID = SALCURID
            left outer join EntegreF..IDENTYPICTURE on IPIDENTY = CUSIDTCNO
            where CURCUSTOMER = 1
            group by CURID,CURVAL,CURNAME,SALDATE,IPID
            option (fast 1000)", Properties.Settings.Default.connectionstring);
            gridStoklar.DataSource = dt;
            ViewStoklar.OptionsView.BestFitMaxRowCount = -1;
            ViewStoklar.BestFitColumns(true);
            this.Text += "Müşteri listesi ";
        }

        private void ApplyFilterAndReloadData()
        {
            // Filtre boşsa varsayılan sorgu
            string sqlQuery="";
            DataTable dt = new DataTable();
            this.Enabled = false;
            executeBackground(
        () => {
            try
            {
                Entegref.SplashScreen(this, "EntegreF", Properties.Settings.Default.Company, "Müşteriler Listeleniyor");

                string filterText = ViewStoklar.ActiveFilterString;

                if (string.IsNullOrWhiteSpace(filterText))
                {
                    sqlQuery = $@"
                    select distinct top 1000 CURID,CURVAL,CURNAME,
                    sum(case when SALID > 0 then SALAMOUNT else -1*SALAMOUNT end) as SALAMOUNT,SALDATE,
                    case when IPID is not null then 'Var' else 'Yok' end as Kimlik
                    from CURRENTS
                    left outer join CUSIDENTITY on CUSIDCURID = CURID
                    left outer join CUSTOMER on CURID = CUSCURID
                    left outer join SALES on CURID = SALCURID
                    left outer join EntegreF..IDENTYPICTURE on IPIDENTY = CUSIDTCNO
                    where CURCUSTOMER = 1
                    group by CURID,CURVAL,CURNAME,SALDATE,IPID
                    option (fast 1000)";
                }
                else
                {
                    // Örnek: PRONAME filtresi varsa
                    string provalFilter = GetFilterValue(ViewStoklar, "CURVAL");
                    string pronameFilter = GetFilterValue(ViewStoklar, "CURNAME");
                    if (!string.IsNullOrEmpty(pronameFilter))
                    {
                        sqlQuery = $@"
                    select distinct top 1000 CURID,CURVAL,CURNAME,
                    sum(case when SALID > 0 then SALAMOUNT else -1*SALAMOUNT end) as SALAMOUNT,SALDATE,
                    case when IPID is not null then 'Var' else 'Yok' end as Kimlik
                    from CURRENTS
                    left outer join CUSIDENTITY on CUSIDCURID = CURID
                    left outer join CUSTOMER on CURID = CUSCURID
                    left outer join SALES on CURID = SALCURID
                    left outer join EntegreF..IDENTYPICTURE on IPIDENTY = CUSIDTCNO
                    where CURCUSTOMER = 1
                    and CURNAME LIKE '%{pronameFilter.Replace(" ", "%")}%'
                    group by CURID,CURVAL,CURNAME,SALDATE,IPID
                    UNION
                    SELECT 0,NULL AS CURVAL, '{pronameFilter}' AS CURNAME,0 as SALAMOUNT, '' as SALDATE, '' AS DINSNAME
                    WHERE NOT EXISTS (
                    SELECT 1 FROM CURRENTS WHERE CURNAME LIKE '%{pronameFilter}%')
                    order by 3 desc
                    option (fast 1000)";

                        //         SELECT TOP 100 PROID,PROVAL, PRONAME, PINVQUAN 
                        //         FROM PRODUCTS 
                        //outer apply (select PINVQUAN from PROINV 
                        //left outer join DEFSTORAGE on DSTORID = PINVSTORID
                        //where PINVYEAR = '' and PINVMONTH = ''
                        //and PINVPROID = PROID and DSTORDIVISON = '{Entegref.GetLogins.userDIVVAL}') Env
                        //         WHERE PRONAME LIKE '%{pronameFilter}%'
                        //         UNION
                        //         SELECT 0,NULL AS PROVAL, '{pronameFilter}' AS PRONAME, 0 AS PINVQUAN
                        //         WHERE NOT EXISTS (
                        //         SELECT 1 FROM PRODUCTS WHERE PRONAME LIKE '%{pronameFilter}%')
                        //         order by 3 desc
                        //         option (fast 100)";

                    }
                    else if (!string.IsNullOrEmpty(provalFilter))
                    {
                        sqlQuery = $@"
                    select distinct top 1000 CURID,CURVAL,CURNAME,
                    sum(case when SALID > 0 then SALAMOUNT else -1*SALAMOUNT end) as SALAMOUNT,SALDATE,
                    case when IPID is not null then 'Var' else 'Yok' end as Kimlik
                    from CURRENTS
                    left outer join CUSIDENTITY on CUSIDCURID = CURID
                    left outer join CUSTOMER on CURID = CUSCURID
                    left outer join SALES on CURID = SALCURID
                    left outer join EntegreF..IDENTYPICTURE on IPIDENTY = CUSIDTCNO
                    where CURCUSTOMER = 1
                    and CURVAL LIKE '%{provalFilter}%'
                    group by CURID,CURVAL,CURNAME,SALDATE,IPID
                    order by CURNAME 
                    UNION
                    SELECT 0,'{provalFilter}' AS CURVAL, NULL AS CURNAME,0 as SALAMOUNT, '' as SALDATE, '' AS DINSNAME
                    WHERE NOT EXISTS (
                    SELECT 1 FROM CURRENTS WHERE CURVAL LIKE '%{provalFilter}%')
                    order by 3 desc
                    option (fast 100)";
                    }
                    else
                    {
                        sqlQuery = $@"
                    select distinct top 1000 CURID,CURVAL,CURNAME,
                    sum(case when SALID > 0 then SALAMOUNT else -1*SALAMOUNT end) as SALAMOUNT,SALDATE,
                    case when IPID is not null then 'Var' else 'Yok' end as Kimlik
                    from CURRENTS
                    left outer join CUSIDENTITY on CUSIDCURID = CURID
                    left outer join CUSTOMER on CURID = CUSCURID
                    left outer join SALES on CURID = SALCURID
                    left outer join EntegreF..IDENTYPICTURE on IPIDENTY = CUSIDTCNO
                    where CURCUSTOMER = 1
                    group by CURID,CURVAL,CURNAME,SALDATE,IPID
                    option (fast 1000)";
                    }
                }
                dt = conn.GetData(sqlQuery, Properties.Settings.Default.connectionstring);
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
        },
        
                null,
                () =>
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
                    gridStoklar.DataSource = dt;
                    ViewStoklar.OptionsView.BestFitMaxRowCount = -1;
                    ViewStoklar.BestFitColumns(true);
                    ViewStoklar.ActiveFilter.Clear();
                    completeProgress();
                }
        );

            // SQL sorgusunu çalıştır ve sonucu grid'e yükle
        }
        private string GetFilterValue(GridView view, string columnFieldName)
        {
            var column = view.Columns[columnFieldName];
            if (column != null && column.FilterInfo != null)
            {
                var filterValue = column.FilterInfo.Value?.ToString();
                return filterValue;
            }
            return string.Empty;
        }

        private void ViewStoklar_RowClick(object sender, RowClickEventArgs e)
        {
            if (e.RowHandle >= 0 && e.Button == MouseButtons.Left && e.Clicks >=2 )
            {
                if (Gonderen.Name == "frmMusteri")
                {
                    frmMusteri.CURVAL = ViewStoklar.GetRowCellValue(e.RowHandle, "CURVAL").ToString();                    
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }                
                this.Close();
                this.Dispose();
            }
        }

        private void gridStoklar_EditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilterAndReloadData();
                e.Handled = true; // Enter tuşunun gridde başka işlem yapmasını engelle
            }

        }
    }
}