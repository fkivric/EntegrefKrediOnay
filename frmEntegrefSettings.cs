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
using static EntegreFDLL.Db.DbServerSettings;
using EntegreFDLL.Db;
using System.Xml;
using System.IO;
using static EntegreFDLL.Class.DataTableClass;

namespace EntegrefKrediOnay
{
    public partial class frmEntegrefSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmEntegrefSettings()
        {
            InitializeComponent();
        }
        private CancellationTokenSource _cts;
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
        private DbServerSettings _settings2;
        private DatabaseManager _manager;
        private void frmEntegrefSettings_Load(object sender, EventArgs e)
        {
            //srcFilitre.Properties.DataSource = conn.GetData($"select USRDPID,rtrim(ltrim(USRDPNAME)) as USRDPNAME from EntegreF..UserDepart where USRDBNAME = '{Properties.Settings.Default.Company}'", Properties.Settings.Default.connectionstring);
            //srcFilitre.Properties.DisplayMember = "USRDPNAME";
            //srcFilitre.Properties.ValueMember = "USRDPID";
            //DataCek();
            DataCek();
            VolXml();
            LoadSettings();
        }
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Properties.Settings.Default.connectionstring);
        private void LoadSettings()
        {
            
            // Formdaki TextBox'lardan oku
            _settings2 = new DbServerSettings
            {
                SourceServer = txtEntegreFServer.Text,   // Uzak sunucu IP/adı
                SourceDatabase = txtEntegreFDbName.Text,
                SourceUser = txtEntegreFUser.Text,
                SourcePassword = txtEntegreFPass.Text,

                TargetServer = builder.DataSource,   // Yerel sunucu (. veya localhost)
                TargetDatabase = "EntegreF",
                TargetUser = builder.UserID,
                TargetPassword = builder.Password,
            };
            _manager = new DatabaseManager(_settings2);
        }

        // ─── Ayarları formdan oku ───────────────────
        private DatabaseSettings GetSourceSettings() => new DatabaseSettings
        {
            Server = builder.DataSource,
            Database = txtEntegreFDbName.Text.Trim(),
            Username = builder.UserID,
            Password = builder.Password,
        };

        private DatabaseSettings GetTargetSettings() => new DatabaseSettings
        {
            Server = txtEntegreFServer.Text.Trim(),
            Database = txtEntegreFDbName.Text.Trim(),
            Username = txtEntegreFUser.Text.Trim(),
            Password = txtEntegreFPass.Text.Trim(),
            UseWindowsAuth = chkSrcWinAuth.Checked
        };

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
                    if (databaseName == cmbDatabase.Text || databaseName == txtEntegreFDbName.Text)
                    {
                        cmbDatabase.Properties.DataSource = databases;
                        cmbDatabase.Properties.DisplayMember = "database_name";
                        cmbDatabase.Properties.ValueMember = "dbid";
                        cmbDatabase.EditValue = database["dbid"];
                        //btnNewDatabase.Enabled = false;
                        //txtKreidPuanUser.Enabled = false;
                        //txtKreidPuanPass.Enabled = false;
                        //navigationPage2.Enabled = false;
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
                Secimler.Add(new VOL_DEPARTMANT {
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
        public void VolXml()
        {
            string ConStrg = "";
            if (!File.Exists("C:\\Program Files (x86)\\Volant Yazılım\\Volant Erp Setup\\VolErpConnectio_junmed.xml"))
            {
                throw new Exception("VolErpConnection Dosyası Eksik!");
            }
            XmlTextReader reader = new XmlTextReader("C:\\Program Files (x86)\\Volant Yazılım\\Volant Erp Setup\\VolErpConnectio_junmed.xml");
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element && reader.Name == "PARAMS") || reader.NodeType != XmlNodeType.Element || !(reader.Name == "DB"))
                {
                    continue;
                }
                eDatabase rDatabase = new eDatabase();
                try
                {
                    ConStrg = string.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=0;",
                           reader.GetAttribute("SERVERNAME").TextSifreCoz(),
                           reader.GetAttribute("DATABASE").TextSifreCoz(),
                           reader.GetAttribute("LOGIN").TextSifreCoz(),
                           reader.GetAttribute("PASSWORD").TextSifreCoz());
                    this.Tag = reader.GetAttribute("DATABASE").TextSifreCoz();
                    txtEntegreFServer.Text = reader.GetAttribute("SERVERNAME").TextSifreCoz();
                    txtEntegreFUser.Text = reader.GetAttribute("LOGIN").TextSifreCoz();
                    txtEntegreFPass.Text = reader.GetAttribute("PASSWORD").TextSifreCoz();

                }
                catch
                {
                    ConStrg = string.Format("Server={0};Database={1};User Id={2};Password={3};Connect Timeout=0;",
                           reader.GetAttribute("SERVERNAME").ToString(),
                           reader.GetAttribute("DATABASE").ToString(),
                           reader.GetAttribute("LOGIN").ToString(),
                           reader.GetAttribute("PASSWORD").ToString());
                    this.Tag = reader.GetAttribute("DATABASE").ToString();
                    txtEntegreFServer.Text = reader.GetAttribute("SERVERNAME").ToString();
                    txtEntegreFUser.Text = reader.GetAttribute("LOGIN").ToString();
                    txtEntegreFPass.Text = reader.GetAttribute("PASSWORD").ToString();
                }
            }
            Program.sql2 = ConStrg;
            reader.Close();
        }

        private void frmEntegrefSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            string query = $"select count(*) from EntegreF..UserDepart where USRDBNAME = '{Properties.Settings.Default.Company}' and USRDPDEPVAL != ''";
            string result = conn.GetValueConnection(query, Properties.Settings.Default.connectionstring);
        }
        private void TestConnection(DatabaseSettings s, string label)
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(s.MasterConnectionString))
                {
                    conn.Open();
                    AppendLog($"✔ {label} bağlantı başarılı: {s.Server}");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"✘ {label} HATA: {ex.Message}");
            }
        }// ─── Yardımcı ───────────────────────────────
        private void AppendLog(string msg)
        {
            lstLog.Items.Add(msg);
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }
        private void SetUIState(bool running)
        {
            btnMigrate.Enabled = !running;
            btnTest.Enabled = !running;
            btnCancel.Enabled = running;
            progressBar1.Style = running
                ? ProgressBarStyle.Continuous
                : ProgressBarStyle.Continuous;
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(_settings2.SourceServer) ||
                string.IsNullOrWhiteSpace(_settings2.SourceDatabase))
            {
                MessageBox.Show("Kaynak sunucu ve veritabanı adı zorunludur.", "Uyarı",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(_settings2.TargetServer) ||
                string.IsNullOrWhiteSpace(_settings2.TargetDatabase))
            {
                MessageBox.Show("Hedef sunucu ve veritabanı adı zorunludur.", "Uyarı",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSrcWinAuth.Checked)
            {
                txtEntegreFUser.Enabled = false;
                txtEntegreFPass.Enabled = false;
            }
            else
            {
                txtEntegreFUser.Enabled = true;
                txtEntegreFPass.Enabled = true;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {


            int success = 0;
            int error = 0;
            this.Enabled = false;

            executeBackground(
       () =>
       {
           Invoke((MethodInvoker)delegate
           {
               TestConnection(GetSourceSettings(), "Kaynak");
               TestConnection(GetTargetSettings(), "Hedef");
           });
       },
                     null,
                     () =>
                     {
                         this.Enabled = true;
                     });
        }

        private async void btnMigrate_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            _cts = new CancellationTokenSource();
            SetUIState(running: true);
            lstLog.Items.Clear();
            progressBar1.Value = 0;

            var source = GetSourceSettings();
            var target = GetTargetSettings();
            var migrator = new DatabaseMigrator(source, target);

            migrator.OnLog += msg => Invoke(new Action(() => AppendLog(msg)));
            migrator.OnProgress += pct => Invoke(new Action(() => progressBar1.Value = pct));

            try
            {
                await Task.Run(() => migrator.Migrate(_cts.Token), _cts.Token);
                MessageBox.Show("Migration başarıyla tamamlandı!", "Başarılı",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                AppendLog("⚠ İşlem kullanıcı tarafından iptal edildi.");
            }
            catch (Exception ex)
            {
                AppendLog($"✘ HATA: {ex.Message}");
                MessageBox.Show($"Hata oluştu:\n\n{ex.Message}", "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUIState(running: false);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }
    }
}