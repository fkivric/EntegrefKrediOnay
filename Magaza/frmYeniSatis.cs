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
using System.Threading;
using DevExpress.Spreadsheet;
using static DataTableExtensions;
using System.IO;
using DevExpress.XtraPrinting;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Globalization;
using Newtonsoft.Json;
using EntegreFDLL;
using EntegrefKrediOnay.Class;
using EntegreFDLL.Class;
using EntegreFDLL.Main;

namespace EntegrefKrediOnay.Magaza
{
    public partial class frmYeniSatis : DevExpress.XtraEditors.XtraForm
    {
        SqlConnectionObject conn = new SqlConnectionObject();
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        SqlConnection sqlEntegref = new SqlConnection(Properties.Settings.Default.connectionstring2);
        SqlConnection muhsebeDb;
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        string sql3 = "";
        Convertler convertler = new Convertler();
        string rootPath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Hukuk");
        public frmYeniSatis()
        {
            InitializeComponent();
            var builder = new SqlConnectionStringBuilder(Properties.Settings.Default.connectionstring);
            // Seçilen veritabanını ata
            builder.ConnectTimeout = 600;
            sql1 = builder.ConnectionString;
            builder.InitialCatalog = "EntegreF";
            sql2 = builder.ConnectionString;
            builder.InitialCatalog = conn.GetValueConnection("select MTTODBNAME from MANAGEMENT", sql1);
            // Güncellenmiş connection string
            muhsebeDb = new SqlConnection(builder.ConnectionString);
            sql3 = muhsebeDb.ConnectionString;

        }
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
                        //XtraMessageBox.Show("Her oturum açıldığında 1 işlem yapacak. Eğer bu girişteki ilk işlemse uygulama çalışmaktadır. Lütfen Bekleyiniz");
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
                        y.Cancel = true; string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                        CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        public class ComboBoxItem
        {
            public int Value { get; set; }
            public string DisplayText { get; set; }

            public ComboBoxItem(int value, string displayText)
            {
                Value = value;
                DisplayText = displayText;
            }

            public override string ToString()
            {
                return DisplayText;
            }
        }
        private string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string columnName = "";

            while (index > 0)
            {
                int remainder = (index - 1) % 26;
                columnName = letters[remainder] + columnName;
                index = (index - 1) / 26;
            }

            return columnName;
        }
        public static int HarfinSirasi(string harf)
        {
            if (harf == "I")
            {
                harf = "i";
            }
            // Küçük harfleri kontrol etmek için harfin ASCII değerini 97'ye çıkarırız.
            // Büyük harfler için bu gerekli değildir.
            harf = harf.ToLower();

            // Harfin alfabedeki sırasını bulmak için ASCII tablosundaki indeksi kullanırız.
            // Küçük harfler için indeksler 0'dan 25'e kadar, büyük harfler için 0'dan 25'e kadar.
            int sira = harf[0] - 'a' + 1;

            return sira;
        }
        private void frmCallCenterAvOdemDus_Load(object sender, EventArgs e)
        {

            if (Entegref.GetLogins.userDIVVAL != "00")
            {

            }
            else
            {

            }
        }
        private void btnDosyaSec_ItemClick(object sender, TileItemEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Dosyaları (*.xlsx;*.xls)|*.xlsx;*.xls|Tüm Dosyalar (*.*)|*.*";
            // Kullanıcıdan dosyayı seçmesini iste
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //txtDosyaYolu.Text = openFileDialog.FileName;
                Worksheet worksheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;

                // Çalışma sayfasının içeriğini temizle
                worksheet.Clear(worksheet.GetDataRange());
                spreadsheetControl.Document.BeginUpdate();
                spreadsheetControl.Document.LoadDocument(openFileDialog.FileName, DocumentFormat.Xlsx);
                spreadsheetControl.Document.EndUpdate();

                Worksheet worksheet2 = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
                CellRange usedRange = worksheet2.GetUsedRange();
                spreadsheetControl.Document.Worksheets.ActiveWorksheet.Cells.AutoFitColumns();
            }
        }
        private void btnKaydet_ItemClick(object sender, TileItemEventArgs e)
        {
            
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            Worksheet worksheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
            CellRange usedRange = worksheet.GetUsedRange();

            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = 1,
                Position = 0,
                ToplamAdet = 1.ToString(),
            };
            HashSet<string> uniqueKocanNoSet = new HashSet<string>();
            this.Enabled = false;
            executeBackground(
        () =>
        {
            progressForm.Show(this);
            progressForm.UpdateDetails("Müşteri Ödemeleri Birleştiriliyor");
        },
                    null,
                    () =>
                    {
                    }
            );
        }
        private int CheckMusteri(string Kod)
        {
            int curId = 0;
            var result = conn.GetValueConnection(
                $"SELECT ISNULL(CURID, 0) FROM CURRENTS WHERE CURVAL = '{Kod}'",
                sql3
            );

            if (result != null && int.TryParse(result.ToString(), out int parsed))
            {
                curId = parsed;
            }
            return curId;
        }
        private bool CheckMusteriBakiye(string Kod)
        {
            string q = $@"
                select isnull((sum(SALAMOUNT) - PCDSAMOUNT),0) from CURRENTS
                join SALES on SALCURID = CURID
                outer apply(select sum(PCDSAMOUNT) as PCDSAMOUNT from PROCEEDS where PCDSCURID = CURID and PCDSKIND = 1 and PCDSSALID is NULL) PROCEEDS
                where CURVAL = '{Kod}'
                and not exists(select top 1 * from PROCEEDS where PCDSCURID = CURID and PCDSSALID = SALID)
                group by PCDSAMOUNT ";
            var dt = conn.GetValueConnection(q, sql3);
            if (dt != "")
            {
                var bakiye = decimal.Parse(dt);
                if (bakiye >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private bool CheckMagaza(string Kod)
        {
            var dt = conn.GetData($"select * from DIVISON where DIVSALESTS = 1 and DIVSTS = 1 and DIVVAL = '{Kod}'", sql3);
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private string NewMagaza(string kk)
        {
            var DIVVVAL = conn.GetValueConnection($@"
            select SALDIVISON from SALES
            where SALDATE in (
            select max(SALDATE) from SALES
            left outer join CURRENTS on CURID = SALCURID
            where CURVAL = '{kk}' and SALDIVISON != '00')
			and SALCURID in (select CURID from CURRENTS where CURVAL = '{kk}')", sql3);
            return DIVVVAL;
        }
        private void btnSmsSil_ItemClick(object sender, TileItemEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
            CellRange usedRange = worksheet.GetUsedRange();
            worksheet.Clear(worksheet.GetDataRange());
        }
        private void btnSmsYeni_ItemClick(object sender, TileItemEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
            CellRange usedRange = worksheet.GetUsedRange();
            worksheet.Clear(worksheet.GetDataRange());
        }
        static int GetFileCountInFolder(string folderPath)
        {
            try
            {
                // Belirtilen klasördeki dosyaları al
                string[] files = Directory.GetFiles(folderPath);

                // Dosya sayısını döndür
                return files.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
                return 0; // Hata durumunda -1 döndür
            }
        }
        private void barBtnExcel_ItemClick(object sender, TileItemEventArgs e)
        {
            ViewDusumSonuc.ExpandAllGroups();
            ViewHatalı.ExpandAllGroups();
            var Avukat = srcMagaza.Text.ToString();
            string path = Path.Combine(rootPath1, "Avukat", Avukat, DateTime.Now.ToString("yyyy-MM-dd"), "Düşüm Listesi");
            string path1 = Path.Combine(rootPath1, "Avukat", Avukat, DateTime.Now.ToString("yyyy-MM-dd"), "Hatalı");

            if (ViewDusumSonuc.RowCount > 0)
            {
                Entegref.CreateDirectoryIfNotExists(path);
                var sira = GetFileCountInFolder(path);
                string filePath = Path.Combine(path, "Baaşrılı Düşüm Listesi" + sira + ".xlsx");
                gridDusumSonuc.ExportToXlsx(filePath, new XlsxExportOptions
                {
                    ExportMode = XlsxExportMode.SingleFile,
                    TextExportMode = TextExportMode.Value,
                    ShowGridLines = true,
                    FitToPrintedPageWidth = true,
                    FitToPrintedPageHeight = true,
                });
            }
            if (ViewHatalı.RowCount > 0)
            {
                Entegref.CreateDirectoryIfNotExists(path1);
                var sira = GetFileCountInFolder(path1);
                string filePath2 = Path.Combine(path1, "Hatalı Düşüm Listesi" + sira + ".xlsx");
                gridHatalı.ExportToXlsx(filePath2, new XlsxExportOptions
                {
                    ExportMode = XlsxExportMode.SingleFile,
                    TextExportMode = TextExportMode.Value,
                    ShowGridLines = true,
                    FitToPrintedPageWidth = true,
                    FitToPrintedPageHeight = true,
                });
            }
            ViewDusumSonuc.CollapseAllGroups();
            ViewHatalı.CollapseAllGroups();
        }
        private void tileBarMuhasebe_ItemClick(object sender, TileItemEventArgs e)
        {
            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = ViewMuhasebeSorunsuz.RowCount,
                Position = 0,
                ToplamAdet = ViewMuhasebeSorunsuz.RowCount.ToString(),
            };

            executeBackground(
        () =>
        {
            progressForm.Show(this);
            progressForm.PerformStep(this);
            
        },
                        null,
                        () =>
                        {
                            completeProgress();
                            this.Invoke((MethodInvoker)delegate
                            {
                                

                            });
                        });
        }
        static decimal ParseToDouble(string value)
        {
            // Eğer değer boş veya null ise 0 döndür
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0; // Varsayılan olarak 0 atanabilir
            }
            // Nokta yerine virgül olan durumlar için normalize et
            string normalizedValue = value.Replace(',', '.');

            // Normalize edilmiş değeri InvariantCulture ile çevir
            if (decimal.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            else
            {
                throw new FormatException("Geçersiz sayı formatı!");
            }
        }
        internal int INSERT(string query, string cnn)
        {
            using (SqlConnection connection = new SqlConnection(cnn))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        private void tileBarHatali_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage3;
        }
        private void tileBarSonuclar_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage2;
        }
        Entegref GetEntegref = new Entegref();
        public static string Mesaj_Metini;
        private void tileBarMail_ItemClick(object sender, TileItemEventArgs e)
        {
        }
        private void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
           
        }
        private void tileBarYeniListe_ItemClick(object sender, TileItemEventArgs e)
        { 
            navigationFrame1.SelectedPage = navigationPage1;
            Worksheet worksheet = spreadsheetControl.Document.Worksheets.ActiveWorksheet;
            CellRange usedRange = worksheet.GetUsedRange();
            worksheet.Clear(worksheet.GetDataRange());
            gridDusumSonuc.DataSource = null;
            gridHatalı.DataSource = null;
            gridMuhasebeSonuc.DataSource = null;
            gridMuhasebeSorunlu.DataSource = null;
            gridMuhasebeSorunsuz.DataSource = null;
        }
    }
}