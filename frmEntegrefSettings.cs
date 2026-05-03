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
using System.Data.SqlClient;
using System.Threading;
using DevExpress.XtraGrid.Views.Grid;
using EntegreFDLL;
using static EntegreFDLL.Class.Volant;

namespace EntegrefKrediOnay
{
    public partial class frmEntegrefSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmEntegrefSettings()
        {
            InitializeComponent();
        }
        List<Firma> firmas = new List<Firma>();
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        SqlConnectionObject conn = new SqlConnectionObject();
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
                _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            }
            catch (Exception)
            {

            }

        }
        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

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
        private void frmEntegrefSettings_Load(object sender, EventArgs e)
        {
            srcFilitre.Properties.DataSource = conn.GetData($"select USRDPID,rtrim(ltrim(USRDPNAME)) as USRDPNAME from EntegreF..UserDepart where USRDBNAME = '{Properties.Settings.Default.Company}'", Properties.Settings.Default.connectionstring);
            srcFilitre.Properties.DisplayMember = "USRDPNAME";
            srcFilitre.Properties.ValueMember = "USRDPID";
            DataCek();
        }
        void DataCek()
        {
            List<AllDataBase> allDatas = new List<AllDataBase>();
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connectionstring))
            {
                connection.Open();

                DataTable databases = connection.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    AllDataBase dts = new AllDataBase { DbNAme = database["database_name"].ToString() };
                    allDatas.Add(dts);
                    string databaseName = database["database_name"].ToString();
                    if (databaseName == cmbDatabase.Text || databaseName == txtKrediPuanDbName.Text)
                    {
                        cmbDatabase.Properties.DataSource = databases;
                        cmbDatabase.Properties.DisplayMember = "database_name";
                        cmbDatabase.Properties.ValueMember = "dbid";
                        cmbDatabase.EditValue = database["dbid"];
                        btnNewDatabase.Enabled = false;
                        txtKreidPuanUser.Enabled = false;
                        txtKreidPuanPass.Enabled = false;
                        navigationPage2.Enabled = false;
                        //tablePanel4.Rows[0].Visible = true;
                        //tablePanel4.Rows[1].Visible = false;
                        //tablePanel4.Rows[2].Visible = false;
                        //tablePanel4.Rows[3].Visible = true;
                        //tablePanel4.Rows[4].Visible = true;
                        Properties.Settings.Default.DbName = cmbDatabase.Text;

                    }
                    else
                    {
                        //tablePanel4.Rows[0].Visible = false;
                        //tablePanel4.Rows[1].Visible = true;
                        //tablePanel4.Rows[2].Visible = true;
                        //tablePanel4.Rows[3].Visible = false;
                        //tablePanel4.Rows[4].Visible = false;
                    }
                    var dd = conn.GetData(string.Format("select COMPANYVAL,COMPANYNAME from {0}.dbo.COMPANY", database["database_name"]), Properties.Settings.Default.connectionstring);
                    if (dd != null)
                    {
                        var ff = new Firma();
                        ff.COMPANYNAME = dd.Rows[0]["COMPANYNAME"].ToString();
                        ff.COMPANYDB = database["database_name"].ToString();
                        if (!firmas.Any(f => f.COMPANYNAME == ff.COMPANYNAME))
                        {
                            firmas.Add(ff);
                        }
                    }
                }
                connection.Close();
            }
        }
        private void srcFilitre_EditValueChanged(object sender, EventArgs e)
        {
            if (srcFilitre.EditValue != null)
            {
                srcFilitre.Enabled = false;

                gridDepartment.DataSource = conn.GetData("select * from DEPARTMENT", Properties.Settings.Default.connectionstring);
                var List = conn.GetData($@"declare @VAL nvarchar(max)
                select @VAL = USRDPDEPVAL from EntegreF..UserDepart where USRDPID = {srcFilitre.EditValue}
                select * from SplitString(replace(@VAL,' ',''),',')", Properties.Settings.Default.connectionstring);

                for (int i = 0; i < List.Rows.Count; i++)
                {
                    var USRDPDEPVAL = List.Rows[i]["Part"].ToString();
                    for (int ii = 0; ii < ViewDepartment.RowCount; ii++)
                    {
                        var DEPVAL = ViewDepartment.GetRowCellValue(ii, "DEPVAL").ToString();

                        if (DEPVAL == USRDPDEPVAL)
                        {
                            ViewDepartment.BeginSelection();
                            ViewDepartment.SelectRow(ii); // Satırı seç
                            ViewDepartment.EndSelection();

                        }
                    }
                }
            }
            else
            {
                srcFilitre.Enabled = false;
            }
        }

        private void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage1;
        }

        private void tileBarItem2_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage2;
        }

        public class VOL_DEPARTMANT
        {
            public string DEPVAL { get; set; }
            public string DEPNAME { get; set; }
        }
        private void tileBarItem3_ItemClick(object sender, TileItemEventArgs e)
        {
            var selectedrow = ViewDepartment.GetSelectedRows();

            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = selectedrow.Length,
                Position = 0,
                ToplamAdet = selectedrow.Length.ToString(),
            };
            string deger = "";
            List<VOL_DEPARTMANT> Secimler = new List<VOL_DEPARTMANT>();
            executeBackground(
        () =>
        {
            progressForm.Show(this);
            for (int i = 0; i < selectedrow.Length; i++)
            {
                Secimler.Add( new VOL_DEPARTMANT {
                    DEPVAL = ViewDepartment.GetRowCellValue(i, "DEPVAL").ToString(),
                    DEPNAME = ViewDepartment.GetRowCellValue(i, "DEPNAME").ToString()
                });
                progressForm.PerformStep(this);
            }
            deger = string.Join(",", Secimler.Select(m => m.DEPVAL));
        },
                        null,
                        () =>
                        {
                            conn.InsertValue($@"update e set USRDPDEPVAL = '{deger}' from EntegreF..UserDepart where USRDPID = {srcFilitre.EditValue}", Properties.Settings.Default.connectionstring);
                            completeProgress();
                            this.Invoke((MethodInvoker)delegate
                            {
                                progressForm.Hide(this);
                            });
                        });
        }

        private void tileBarItem4_ItemClick(object sender, TileItemEventArgs e)
        {
            srcFilitre.Enabled = true;
            gridDepartment.DataSource = null;
            srcFilitre.EditValue = "1";
        }

        private void frmEntegrefSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            int count;
            string query = $"select count(*) from EntegreF..UserDepart where USRDBNAME = '{Properties.Settings.Default.Company}' and USRDPDEPVAL != ''";
            string result = conn.GetValueConnection(query, Properties.Settings.Default.connectionstring);
        }
    }
}