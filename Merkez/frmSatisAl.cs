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
using DevExpress.Spreadsheet;
using static EntegreFDLL.Class.DataTableClass;
using EntegreFDLL;
using EntegreFDLL.Main;
using EntegrefKrediOnay.Class;
using ComboBox = System.Windows.Forms.ComboBox;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Formatting = Newtonsoft.Json.Formatting;
using System.Threading;
using DevExpress.XtraGrid.Views.Grid;
using System.Data.SqlClient;
using static EntegrefKrediOnay.Class.Satis;
using DevExpress.LookAndFeel;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmSatisAl : DevExpress.XtraEditors.XtraForm
    {
        public frmSatisAl()
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.CompanyName, "Mağaza Satışı İşleme Ekranı Açılıyor");
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
                httpClient = new HttpClient();
                var ss = Properties.Settings.Default.VolantApiUrl;
                httpClient.BaseAddress = new Uri(Properties.Settings.Default.VolantApiUrl.Replace("/api", ""));
                GetSALEs = conn.GetData("Select * from SALES", sql).ToList<SALES>();
                //InitUI();
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace} {Environment.NewLine} Inner: {ex.InnerException?.ToString()}";
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
        string sql = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        private readonly HttpClient httpClient;
        List<Customer> GetCustomers = new List<Customer>();
        List<CreaditSales> GetCreaditSales = new List<CreaditSales>();
        List<PaymentSales> GetPaymentSales = new List<PaymentSales>();
        List<MainRood> GetMainRoods = new List<MainRood>();
        List<SALES> GetSALEs = new List<SALES>();
        SqlConnectionObject conn = new SqlConnectionObject();
        private void frmSatisAl_Load(object sender, EventArgs e)
        {
        }
        private void btnDosyaSec_ItemClick(object sender, TileItemEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Dosyaları (*.xlsx;*.xls)|*.xlsx;*.xls|Tüm Dosyalar (*.*)|*.*";
            for (int i = spreadsheetControl1.Document.Worksheets.Count - 1; i >= 0; i--)
            {
                var sheet = spreadsheetControl1.Document.Worksheets[i];
                if (sheet != spreadsheetControl1.Document.Worksheets.ActiveWorksheet)
                {
                    spreadsheetControl1.Document.Worksheets.Remove(sheet);
                }
            }
            spreadsheetControl1.Document.Worksheets.ActiveWorksheet.Name = "Sheet1";

            //Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            //// Çalışma sayfasının içeriğini temizle
            //worksheet.Clear(worksheet.GetDataRange());
            spreadsheetControl1.Document.BeginUpdate();
            // Kullanıcıdan dosyayı seçmesini iste
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                spreadsheetControl1.Document.LoadDocument(openFileDialog.FileName, DocumentFormat.Xlsx);
            }
            spreadsheetControl1.Document.EndUpdate();
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.CompanyName, "Seçili Excel Verilieri İşleniyor");

                Worksheet worksheet2 = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
                CellRange usedRange = worksheet2.GetUsedRange();

                // Dolu sütunları dolaşarak sıralı harf isimlerini elde edin
                for (int columnIndex = usedRange.LeftColumnIndex; columnIndex <= usedRange.RightColumnIndex + 1; columnIndex++)
                {
                    // Sütunun harf karşılığını hesaplayın
                    string columnName = GetColumnName(columnIndex);
                    // Sütun ismini listeye ekleyin
                    cmbMusteriKodu.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbMusteriTipi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbMusteriAdi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbMusteriSoyadi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbTCKN.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbVD.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbGSM.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbMusteriDogumTarihi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbEMail.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbAdress.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbStokKodu.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbAdet.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbNetFiyat.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbIskonto.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbTaksitSayisi.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbTarih.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbSiparisNo.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbSatici.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbKampanya.Items.Add(new ComboBoxItem(columnIndex, columnName));
                    cmbMagaza.Items.Add(new ComboBoxItem(columnIndex, columnName));
                }
                var comboList = new List<ComboBox>
            {
                cmbMusteriKodu, cmbMusteriTipi, cmbMusteriAdi, cmbMusteriSoyadi,
                cmbTCKN, cmbVD, cmbGSM, cmbMusteriDogumTarihi, cmbEMail, cmbAdress,
                cmbStokKodu, cmbAdet, cmbNetFiyat, cmbIskonto, cmbTaksitSayisi,
                cmbTarih, cmbSiparisNo, cmbSatici, cmbKampanya, cmbMagaza
            };

                for (int col = usedRange.LeftColumnIndex; col <= usedRange.RightColumnIndex; col++)
                {
                    string headerText = worksheet2.Cells[usedRange.TopRowIndex, col].DisplayText?.Trim();

                    if (string.IsNullOrEmpty(headerText)) continue;

                    foreach (var combo in comboList)
                    {
                        if (combo.Tag != null &&
                            headerText.Equals(combo.Tag.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            // eşleşen combobox için doğru item’i seç
                            foreach (ComboBoxItem item in combo.Items)
                            {
                                if (item.Value == col + 1)
                                {
                                    combo.SelectedItem = item;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace} {Environment.NewLine} Inner: {ex.InnerException?.ToString()}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
            spreadsheetControl1.Document.Worksheets.ActiveWorksheet.Cells.AutoFitColumns();
            btnKontrol.Enabled = true;
        }
        private string GetColumnName(int index)
        {
            string columnName = "";

            while (index > 0)
            {
                int remainder = (index - 1) % 26;
                columnName = (char)('A' + remainder) + columnName;
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
            harf = harf.ToUpper();

            // Harfin alfabedeki sırasını bulmak için ASCII tablosundaki indeksi kullanırız.
            // Küçük harfler için indeksler 0'dan 25'e kadar, büyük harfler için 0'dan 25'e kadar.
            int sira = 0;
            if (harf == "")
            {
                sira = -1;
            }
            else
            {
                foreach (char c in harf)
                {
                    // 'A' = 1, 'B' = 2 ... 'Z' = 26
                    sira = sira * 26 + (c - 'A' + 1);
                }
            }

            return sira;
        }

        Customer customer = new Customer();
        CreaditSales creaditSales = new CreaditSales();
        PaymentSales paymentSales = new PaymentSales();
        DeliverAddress deliverAddress = new DeliverAddress();
        CustomerAddress customerAdress = new CustomerAddress();
        private void btnKontrol_ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                //EntegreFDLL.Class.Entegref.SplashScreen(this, "Satışlar İşleniyor", Properties.Settings.Default.CompanyName, "Lütfen Bekleyin");
                XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                args.AutoCloseOptions.Delay = 2000;
                args.Caption = "Uyarı";
                args.Buttons = new DialogResult[] { DialogResult.OK };
                args.AutoCloseOptions.ShowTimerOnDefaultButton = true;
                int index = 1;

                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;

                HashSet<string> uniqueSatisNo = new HashSet<string>();
                HashSet<string> uniqueMusteriNo = new HashSet<string>();
                CellRange usedRange = worksheet.GetUsedRange();
                ProgressBarFrm progressForm = new ProgressBarFrm()
                {
                    Start = 0,
                    Finish = usedRange.RowCount-1,
                    Position = 0,
                    ToplamAdet = (usedRange.RowCount-1).ToString(),
                };
                int SiraMusteriKodu = HarfinSirasi(cmbMusteriKodu.Text);
                int SiraMusteriTipi = HarfinSirasi(cmbMusteriTipi.Text);
                int SiraMusteriAdi = HarfinSirasi(cmbMusteriAdi.Text);
                int SiraMusteriSoyadi = HarfinSirasi(cmbMusteriSoyadi.Text);
                int SiraTCKN = HarfinSirasi(cmbTCKN.Text);
                int SiracmbVD = HarfinSirasi(cmbVD.Text);
                int SiraGSM = HarfinSirasi(cmbGSM.Text);
                int SiraDogumTarihi = HarfinSirasi(cmbMusteriDogumTarihi.Text);
                int SiraEMail = HarfinSirasi(cmbEMail.Text);
                int SiraAdress = HarfinSirasi(cmbAdress.Text);
                int SiraStokKodu = HarfinSirasi(cmbStokKodu.Text);
                int SiraAdet = HarfinSirasi(cmbAdet.Text);
                int SiraNetFiyat = HarfinSirasi(cmbNetFiyat.Text);
                int SiraIskonto = HarfinSirasi(cmbIskonto.Text);
                int SiraTaksitSayisi = HarfinSirasi(cmbTaksitSayisi.Text);
                int SiraTarih = HarfinSirasi(cmbTarih.Text);
                int SiraSiparisNo = HarfinSirasi(cmbSiparisNo.Text);
                int SiraSatici = HarfinSirasi(cmbSatici.Text);
                int SiraKampanya = HarfinSirasi(cmbKampanya.Text);
                int SiraMagaza = HarfinSirasi(cmbMagaza.Text);
                executeBackground(
                    () => 
                    {
                        try
                        {
                            progressForm.Show(this);
                            for (int rowIndex = index; rowIndex < usedRange.RowCount; rowIndex++)
                            {
                                MainRood MainRoodS = new MainRood();
                                if (uniqueMusteriNo.Add(usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()))
                                {
                                    MainRoodS = new MainRood();
                                    MainRoodS.Customers = new List<Customer>();
                                    customer = new Customer();
                                    customerAdress = new CustomerAddress();
                                    customer.CustomerCode = usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString();
                                    MainRoodS.CustomerCode = usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString();
                                    var Musteri = usedRange[rowIndex, SiraMusteriAdi - 1].Value.ToString();
                                    var ayrıisim = IsimAyir(Musteri);
                                    customer.CustomerFirstName = ayrıisim.FirstName;
                                    customer.CustomerLastName = ayrıisim.LastName;
                                    if (SiraDogumTarihi > 0)
                                    {
                                        customer.CustomerBirthday = DateTime.Parse(usedRange[rowIndex, SiraDogumTarihi - 1].Value.ToString());
                                    }
                                    else
                                    {
                                        customer.CustomerBirthday = new DateTime(1900, 1, 1);
                                    }
                                    customer.CustomerPhone = usedRange[rowIndex, SiraGSM - 1].Value.ToString();
                                    customer.Email = usedRange[rowIndex, SiraEMail - 1].Value.ToString();
                                    var Kurum = usedRange[rowIndex, SiraMusteriTipi - 1].Value;
                                    if (Kurum.TextValue == "Vergi numarası")
                                    {
                                        if (usedRange[rowIndex, SiraTCKN - 1].Value.ToString().Length == 11)
                                        {
                                            customer.CustomerCorparate = false;
                                            customer.CustomerIdentityNumber = usedRange[rowIndex, SiraTCKN - 1].Value.ToString();
                                            customer.CustomerWatNo = "";
                                            customer.CustomerWatP = "";
                                        }
                                        else
                                        {
                                            customer.CustomerCorparate = true;
                                            customer.CustomerWatNo = usedRange[rowIndex, SiraTCKN - 1].Value.ToString();
                                            customer.CustomerWatP = usedRange[rowIndex, SiracmbVD - 1].Value.ToString();
                                        }
                                    }
                                    else
                                    {
                                        customer.CustomerCorparate = false;
                                        customer.CustomerIdentityNumber = usedRange[rowIndex, SiraTCKN - 1].Value.ToString();
                                        customer.CustomerWatNo = "";
                                        customer.CustomerWatP = "";
                                    }
                                    string adres = usedRange[rowIndex, SiraAdress - 1].Value.ToString().Replace("/ TUR","");
                                    string[] satirlar = adres.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                                    // Mahalle: "Mah" öncesi
                                    string Region = "";
                                    int mahIndex = satirlar[0].IndexOf("Mah");
                                    if (mahIndex > 0)
                                        Region = satirlar[0].Substring(0, mahIndex).Trim();

                                    // Sokak: "Mah" ile "Sok" arasındaki kısım
                                    string sokak = "";
                                    int sokIndex = satirlar[1].IndexOf("Sok");
                                    if (sokIndex > 0)
                                        sokak = satirlar[1].Substring(0, sokIndex).Trim();

                                    // İlçe ve İl: son satırdan "/" ile ayır
                                    string[] bolumler = satirlar[satirlar.Length - 1].Split('/');
                                    string county = bolumler[0].Trim();   // BAĞCILAR
                                    string city = bolumler[1].Trim();   // İSTANBUL
                                    if (adres.Length > 100)
                                    {
                                        adres = adres.Replace(city, "").Replace(county, "");
                                    }
                                    customerAdress.City = city;
                                    customerAdress.County = county;
                                    customerAdress.Region = Region;
                                    customerAdress.Address = adres;
                                    customerAdress.Phone = usedRange[rowIndex, SiraGSM - 1].Value.ToString();
                                    customer.CustomerAddress = customerAdress;
                                    GetCustomers.Add(customer);
                                    MainRoodS.Customers.Add(customer);
                                }
                                var SatisVar = GetSALEs.Any(x => x.SALUSEFIELD1 != null
                                                        && x.SALUSEFIELD1 == usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString());
                                if (!SatisVar)
                                {
                                    if (MainRoodS.CustomerCode == null)
                                    {
                                        var Musterivar = GetCustomers
                                                        .Any(x => x.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()                                                        
                                        );
                                        MainRoodS.CustomerCode = GetCustomers.FirstOrDefault(x => x.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()).CustomerCode;
                                        MainRoodS.Customers = new List<Customer>();
                                        MainRoodS.Customers.Add(GetCustomers.FirstOrDefault(x => x.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()));
                                    }
                                    if (decimal.Parse(usedRange[rowIndex, SiraNetFiyat - 1].Value.ToString()) != 0)
                                    {
                                        bool exists = GetMainRoods
                                                .Any(z => z.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()
                                                && z.CreaditSales != null);
                                        bool exists2 = GetMainRoods
                                                .Any(z => z.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()
                                                && z.PaymentSales != null);
                                        if (exists || exists2)
                                        {
                                            var Analiste = GetMainRoods.FirstOrDefault(z => z.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString());

                                            if (Analiste.CreaditSales == null)
                                            {
                                                Analiste.CreaditSales = new List<CreaditSales>();
                                            }
                                            if (uniqueSatisNo.Add(usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString()))
                                            {
                                                creaditSales = new CreaditSales();
                                                creaditSales.Products = new List<Product>();
                                                creaditSales.CustomerCode = usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString();
                                                string magaza = usedRange[rowIndex, SiraMagaza - 1].Value.ToString().Replace("6227-", "");
                                                if (magaza != "")
                                                {
                                                    var StoreHouse = conn.GetValue($@"select DIVVAL from DIVISON where DIVREGION like '%{magaza}%'", sql);
                                                    creaditSales.StoreCode = "00";
                                                    creaditSales.StoreWareHouseCode = StoreHouse;
                                                }
                                                else
                                                {
                                                    creaditSales.StoreCode = "00";
                                                    creaditSales.StoreWareHouseCode = "00";
                                                }
                                                creaditSales.OrderDate = DateTime.Parse(usedRange[rowIndex, SiraTarih - 1].Value.ToString());
                                                creaditSales.OrderNumber = usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString();
                                                creaditSales.CargoNumber = "";
                                                creaditSales.CargoVal = "";
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = decimal.Parse(usedRange[rowIndex, SiraNetFiyat - 1].Value.ToString());
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                if (SiraTaksitSayisi == -1)
                                                {
                                                    urun.InstalmentCount = 1;
                                                    urun.PriceVal = "TK1";
                                                }
                                                else
                                                {
                                                    urun.InstalmentCount = int.Parse(usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString());
                                                    urun.PriceVal = "TK" + usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString();
                                                }
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                creaditSales.Products.Add(urun);
                                                creaditSales.Payments = new List<Payment>
                                            {
                                                new Payment
                                                {
                                                    PaymentAmount = 0,
                                                    PaymentTypeCode = "N"
                                                }
                                            };
                                                creaditSales.Instalments = new List<Instalment>
                                            {
                                                new Instalment
                                                {
                                                    instalmentFixDate = DateTime.Today.AddMonths(1),
                                                    instalmentAmount = urun.PriceWithTax
                                                }
                                            };
                                                creaditSales.CurrencyCode = "TL";
                                                string adres = usedRange[rowIndex, SiraAdress - 1].Value.ToString().Replace("/ TUR", "");
                                                string[] satirlar = adres.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                                                // Mahalle: "Mah" öncesi
                                                string Region = "";
                                                int mahIndex = satirlar[0].IndexOf("Mah");
                                                if (mahIndex > 0)
                                                    Region = satirlar[0].Substring(0, mahIndex).Trim();

                                                // Sokak: "Mah" ile "Sok" arasındaki kısım
                                                string sokak = "";
                                                int sokIndex = satirlar[1].IndexOf("Sok");
                                                if (sokIndex > 0)
                                                    sokak = satirlar[1].Substring(0, sokIndex).Trim();

                                                // İlçe ve İl: son satırdan "/" ile ayır
                                                string[] bolumler = satirlar[satirlar.Length - 1].Split('/');
                                                string county = bolumler[0].Trim();   // BAĞCILAR
                                                string city = bolumler[1].Trim();   // İSTANBUL

                                                if (adres.Length > 100)
                                                {
                                                    adres = adres.Replace(city, "").Replace(county, "");
                                                }
                                                creaditSales.DeliverAddress = new DeliverAddress
                                                {
                                                    City = city,
                                                    County = county,
                                                    Region = Region,
                                                    Address = adres,
                                                    Phone = usedRange[rowIndex, SiraGSM - 1].Value.ToString().Replace(city, "")
                                                };
                                                GetCreaditSales.Add(creaditSales);
                                                Analiste.CreaditSales.Add(creaditSales);
                                            }
                                            else
                                            {
                                                var satis = GetCreaditSales.FirstOrDefault(x => x.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString());
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = decimal.Parse(usedRange[rowIndex, SiraNetFiyat - 1].Value.ToString());
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                if (SiraTaksitSayisi == -1)
                                                {
                                                    urun.InstalmentCount = 1;
                                                    urun.PriceVal = "TK1";
                                                }
                                                else
                                                {
                                                    urun.InstalmentCount = int.Parse(usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString());
                                                    urun.PriceVal = "TK" + usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString();
                                                }
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                satis.Products.Add(urun);
                                            }
                                        }
                                        else
                                        {
                                            if (uniqueSatisNo.Add(usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString()))
                                            {
                                                MainRoodS.CreaditSales = new List<CreaditSales>();
                                                creaditSales = new CreaditSales();
                                                creaditSales.Products = new List<Product>();
                                                creaditSales.CustomerCode = usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString();
                                                creaditSales.StoreCode = "00";
                                                creaditSales.StoreWareHouseCode = "00";
                                                creaditSales.OrderDate = DateTime.Parse(usedRange[rowIndex, SiraTarih - 1].Value.ToString());
                                                creaditSales.OrderNumber = usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString();
                                                creaditSales.CargoNumber = "";
                                                creaditSales.CargoVal = "";
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = decimal.Parse(usedRange[rowIndex, SiraNetFiyat - 1].Value.ToString());
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                if (SiraTaksitSayisi == -1)
                                                {
                                                    urun.InstalmentCount = 1;
                                                    urun.PriceVal = "TK1";
                                                }
                                                else
                                                {
                                                    urun.InstalmentCount = int.Parse(usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString());
                                                    urun.PriceVal = "TK" + usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString();
                                                }
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                creaditSales.Products.Add(urun);
                                                creaditSales.Payments = new List<Payment>
                                            {
                                                new Payment
                                                {
                                                    PaymentAmount = 0,
                                                    PaymentTypeCode = "N"
                                                }
                                            };
                                                creaditSales.Instalments = new List<Instalment>
                                            {
                                                new Instalment
                                                {
                                                    instalmentFixDate = DateTime.Today.AddMonths(1),
                                                    instalmentAmount = urun.PriceWithTax
                                                }
                                            };
                                                creaditSales.CurrencyCode = "TL"; string adres = usedRange[rowIndex, SiraAdress - 1].Value.ToString().Replace("/ TUR", "");
                                                string[] satirlar = adres.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                                                // Mahalle: "Mah" öncesi
                                                string Region = "";
                                                int mahIndex = satirlar[0].IndexOf("Mah");
                                                if (mahIndex > 0)
                                                    Region = satirlar[0].Substring(0, mahIndex).Trim();

                                                // Sokak: "Mah" ile "Sok" arasındaki kısım
                                                string sokak = "";
                                                int sokIndex = satirlar[1].IndexOf("Sok");
                                                if (sokIndex > 0)
                                                    sokak = satirlar[1].Substring(0, sokIndex).Trim();

                                                // İlçe ve İl: son satırdan "/" ile ayır
                                                string[] bolumler = satirlar[satirlar.Length - 1].Split('/');
                                                string county = bolumler[0].Trim();   // BAĞCILAR
                                                string city = bolumler[1].Trim();   // İSTANBUL

                                                if (adres.Length > 100)
                                                {
                                                    adres = adres.Replace(city, "").Replace(county, "");
                                                }
                                                creaditSales.DeliverAddress = new DeliverAddress
                                                {
                                                    City = city,
                                                    County = county,
                                                    Region = Region,
                                                    Address = adres,
                                                    Phone = usedRange[rowIndex, SiraGSM - 1].Value.ToString().Replace(city, "")
                                                };
                                                GetCreaditSales.Add(creaditSales);
                                                MainRoodS.CreaditSales.Add(creaditSales);
                                                GetMainRoods.Add(MainRoodS);
                                            }
                                            else
                                            {
                                                var satis = GetCreaditSales.FirstOrDefault(x => x.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString());
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = decimal.Parse(usedRange[rowIndex, SiraNetFiyat - 1].Value.ToString());
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                if (SiraTaksitSayisi == -1)
                                                {
                                                    urun.InstalmentCount = 1;
                                                    urun.PriceVal = "TK1";
                                                }
                                                else
                                                {
                                                    urun.InstalmentCount = int.Parse(usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString());
                                                    urun.PriceVal = "TK" + usedRange[rowIndex, SiraTaksitSayisi - 1].Value.ToString();
                                                }
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                satis.Products.Add(urun);
                                            }
                                        }
                                    }
                                    #region peşin işlemler
                                    else
                                    {
                                        decimal UrunFiyati = decimal.Parse("0.01");
                                        if (usedRange[rowIndex, SiraNetFiyat - 1].Value.ToString() != "0")
                                        {
                                            UrunFiyati = decimal.Parse(usedRange[rowIndex, SiraNetFiyat - 1].Value.ToString());
                                        }
                                        bool exists = GetMainRoods
                                                        .Any(z => z.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()
                                                        && z.PaymentSales != null);
                                        bool exists2 = GetMainRoods
                                                        .Any(z => z.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()
                                                        && z.CreaditSales != null);
                                        if (exists || exists2)
                                        {
                                            var Analiste = GetMainRoods.FirstOrDefault(z => z.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString());
                                            if (Analiste.PaymentSales == null)
                                            {
                                                Analiste.PaymentSales = new List<PaymentSales>();
                                            }
                                            if (uniqueSatisNo.Add(usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString()))
                                            {
                                                paymentSales = new PaymentSales();
                                                paymentSales.Products = new List<Product>();
                                                paymentSales.CustomerCode = usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString();
                                                string magaza = usedRange[rowIndex, SiraMagaza - 1].Value.ToString().Replace("6227-", "");
                                                if (magaza != "")
                                                {
                                                    var StoreHouse = conn.GetValue($@"select DIVVAL from DIVISON where DIVREGION like '%{magaza}%'", sql);
                                                    paymentSales.StoreCode = "00";
                                                    paymentSales.StoreWareHouseCode = StoreHouse;
                                                }
                                                else
                                                {
                                                    paymentSales.StoreCode = "00";
                                                    paymentSales.StoreWareHouseCode = "00";
                                                }
                                                paymentSales.OrderDate = DateTime.Parse(usedRange[rowIndex, SiraTarih - 1].Value.ToString());
                                                paymentSales.OrderNumber = usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString();
                                                paymentSales.CargoNumber = "";
                                                paymentSales.CargoVal = "";
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = UrunFiyati;
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                urun.InstalmentCount = 0;
                                                urun.PriceVal = "P";
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                paymentSales.Products.Add(urun);
                                                paymentSales.Payments = new List<Payment>
                                            {
                                                new Payment
                                                {
                                                    PaymentAmount = UrunFiyati,
                                                    PaymentTypeCode = "N"
                                                }
                                            };
                                                paymentSales.CurrencyCode = "TL";
                                                string adres = usedRange[rowIndex, SiraAdress - 1].Value.ToString().Replace("/ TUR", "");
                                                string[] satirlar = adres.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                                                // Mahalle: "Mah" öncesi
                                                string Region = "";
                                                int mahIndex = satirlar[0].IndexOf("Mah");
                                                if (mahIndex > 0)
                                                    Region = satirlar[0].Substring(0, mahIndex).Trim();

                                                // Sokak: "Mah" ile "Sok" arasındaki kısım
                                                string sokak = "";
                                                int sokIndex = satirlar[1].IndexOf("Sok");
                                                if (sokIndex > 0)
                                                    sokak = satirlar[1].Substring(0, sokIndex).Trim();

                                                // İlçe ve İl: son satırdan "/" ile ayır
                                                string[] bolumler = satirlar[satirlar.Length - 1].Split('/');
                                                string county = bolumler[0].Trim();   // BAĞCILAR
                                                string city = bolumler[1].Trim();   // İSTANBUL

                                                if (adres.Length > 100)
                                                {
                                                    adres = adres.Replace(city, "").Replace(county, "");
                                                }
                                                paymentSales.DeliverAddress = new DeliverAddress
                                                {
                                                    City = city,
                                                    County = county,
                                                    Region = Region,
                                                    Address = adres,
                                                    Phone = usedRange[rowIndex, SiraGSM - 1].Value.ToString().Replace(city, "")
                                                };
                                                GetPaymentSales.Add(paymentSales);
                                                Analiste.PaymentSales.Add(paymentSales);
                                            }
                                            else
                                            {
                                                var satis = GetPaymentSales.FirstOrDefault(x => x.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString());
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = UrunFiyati;
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                urun.InstalmentCount = 0;
                                                urun.PriceVal = "P";
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                satis.Products.Add(urun);
                                                satis.Payments[0].PaymentAmount += UrunFiyati;
                                            }
                                        }
                                        else
                                        {

                                            if (uniqueSatisNo.Add(usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString()))
                                            {
                                                MainRoodS.PaymentSales = new List<PaymentSales>();
                                                MainRoodS.CustomerCode = usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString();
                                                paymentSales = new PaymentSales();
                                                paymentSales.Products = new List<Product>();
                                                paymentSales.CustomerCode = usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString();
                                                string magaza = usedRange[rowIndex, SiraMagaza - 1].Value.ToString().Replace("6227-", "");
                                                if (magaza != "")
                                                {
                                                    var StoreHouse = conn.GetValue($@"select DIVVAL from DIVISON where DIVREGION like '%{magaza}%'", sql);
                                                    paymentSales.StoreCode = "00";
                                                    paymentSales.StoreWareHouseCode = StoreHouse;
                                                }
                                                else
                                                {
                                                    paymentSales.StoreCode = "00";
                                                    paymentSales.StoreWareHouseCode = "00";
                                                }
                                                paymentSales.OrderDate = DateTime.Parse(usedRange[rowIndex, SiraTarih - 1].Value.ToString());
                                                paymentSales.OrderNumber = usedRange[rowIndex, SiraSiparisNo - 1].Value.ToString();
                                                paymentSales.CargoNumber = "";
                                                paymentSales.CargoVal = "";
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = UrunFiyati;
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                urun.InstalmentCount = 0;
                                                urun.PriceVal = "P";
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                paymentSales.Products.Add(urun);
                                                paymentSales.Payments = new List<Payment>
                                            {
                                                new Payment
                                                {
                                                    PaymentAmount = UrunFiyati,
                                                    PaymentTypeCode = "N"
                                                }
                                            };
                                                paymentSales.CurrencyCode = "TL";
                                                string adres = usedRange[rowIndex, SiraAdress - 1].Value.ToString().Replace("/ TUR", "");
                                                string[] satirlar = adres.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                                                // Mahalle: "Mah" öncesi
                                                string Region = "";
                                                int mahIndex = satirlar[0].IndexOf("Mah");
                                                if (mahIndex > 0)
                                                    Region = satirlar[0].Substring(0, mahIndex).Trim();

                                                // Sokak: "Mah" ile "Sok" arasındaki kısım
                                                string sokak = "";
                                                int sokIndex = satirlar[1].IndexOf("Sok");
                                                if (sokIndex > 0)
                                                    sokak = satirlar[1].Substring(0, sokIndex).Trim();

                                                // İlçe ve İl: son satırdan "/" ile ayır
                                                string[] bolumler = satirlar[satirlar.Length - 1].Split('/');
                                                string county = bolumler[0].Trim();   // BAĞCILAR
                                                string city = bolumler[1].Trim();   // İSTANBUL

                                                if (adres.Length > 100)
                                                {
                                                    adres = adres.Replace(city, "").Replace(county, "");
                                                }
                                                paymentSales.DeliverAddress = new DeliverAddress
                                                {
                                                    City = city,
                                                    County = county,
                                                    Region = Region,
                                                    Address = adres,
                                                    Phone = usedRange[rowIndex, SiraGSM - 1].Value.ToString().Replace(city, "")
                                                };
                                                GetPaymentSales.Add(paymentSales);
                                                MainRoodS.PaymentSales.Add(paymentSales);
                                                GetMainRoods.Add(MainRoodS);
                                            }
                                            else
                                            {
                                                var satis = GetPaymentSales.FirstOrDefault(x => x.CustomerCode == usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString());
                                                Product urun = new Product();
                                                urun.ItemCode = usedRange[rowIndex, SiraStokKodu - 1].Value.ToString();
                                                urun.Quantity = int.Parse(usedRange[rowIndex, SiraAdet - 1].Value.ToString());
                                                urun.PriceWithTax = UrunFiyati;
                                                urun.ListDisAmount = decimal.Parse(usedRange[rowIndex, SiraIskonto - 1].Value.ToString());
                                                var SALESMEN = usedRange[rowIndex, SiraSatici - 1].Value.ToString();
                                                var SMNENVAL = conn.GetValueConnection($@"select SMENVAL from SALESMEN where SMENNAME = '{SALESMEN}'", Properties.Settings.Default.connectionstring);
                                                urun.SalesmenVal = SMNENVAL;
                                                satis.Products.Add(urun);
                                                satis.Payments[0].PaymentAmount += UrunFiyati;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    memoEdit1.Text += "r\n\n işlem hatası : " + rowIndex.ToString() + " sirasındaki" + usedRange[rowIndex, SiraMusteriKodu - 1].Value.ToString()  + "Müşteri Satışı Daha Önceden İşlenmiş";
                                }
                                progressForm.PerformStep(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show(ex.Message,"",MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    },
                        null, 
                        () =>
                        {
                            gridVeriler.DataSource = GetMainRoods;
                            gridVeriler.ForceInitialize();
                            navigationFrame1.SelectedPage = navigationPage2;
                            for (int i = 0; i < ViewVeriler.DataRowCount; i++)
                            {
                                ViewVeriler.ExpandMasterRow(i);
                            }
                            completeProgress();
                            progressForm.Hide(this);
                        });
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
        public (string FirstName, string LastName) IsimAyir(string musteri)
        {
            if (string.IsNullOrWhiteSpace(musteri))
                return ("", "");

            var parts = musteri.Trim()
                       .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return (parts[0], "");
            }
            else if (parts.Length == 2)
            {
                return (parts[0], parts[1]);
            }
            else if (parts.Length == 3)
            {
                return ($"{parts[0]} {parts[1]}", parts[2]);
            }
            else
            {
                // 3’ten fazla kelime varsa ilk kelime isim, son kelime soyisim
                return (string.Join(" ", parts.Take(parts.Length - 1)), parts.Last());
            }
        }
        CreaditSales satisVeri;
        async void satisisle()
        {
            frmLogin.Token();
            string url = "http://fatihkivric.com.tr:5555" + "/creditsales";
            string jSon = JsonConvert.SerializeObject(satisVeri, Formatting.Indented);
            string requestBody = jSon;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.VolantToken);
            HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));
            var sonucne = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                //Satis.Satislem myDeserializedClass = JsonConvert.DeserializeObject<Satis.Satislem>(await response.Content.ReadAsStringAsync());
                //string SALID = (newSALID = myDeserializedClass.results[1].ToString());
                //kampanyasatis.SPEOVSALID = long.Parse(SALID);
                //string SPECIALOFFERSVALID = $@"insert into SPECIALOFFERSVALID values ({kampanyasatis.SPEOVSPEOID},{kampanyasatis.SPEOVSALID},{kampanyasatis.SPEOVQUAN},{kampanyasatis.SPEOVCOUNT})";
                ////kampanya.Add(kampanyasatis);
                //string SALUPDATEq = string.Format("update SALES set SALUSEFIELD2 = '{1}' where SALID = {0}", SALID.ToString(), WEBID);
                //string WebSALq = string.Format("update TicimaxSiparis set EntegrasyonAktarildi = 1, FaturaNo = '{1}',OzelAlan3 = 'WB', Mail = 'WB' where ID ={0}", WEBID, SALID.ToString());
                //string Tarihq = String.Format("update TicimaxSiparisUrun set MagazaAtamaTarihi = getdate(),MagazaDurum = 1, MagazaKodu = 'WB' where SiparisId = {0}", WEBID);
                //conn.InsertDataConnectionSql(SALUPDATEq, sql2);
                //conn.InsertDataConnectionSql(WebSALq, sql);
                //conn.InsertDataConnectionSql(Tarihq, sql);
                //conn.InsertDataConnectionSql(SPECIALOFFERSVALID, sql2);
                //ViewSiparisSonuc.SetRowCellValue(ViewSiparisSonuc.FocusedRowHandle, "FaturaNo", SALID.ToString());
                //ViewSiparisSonuc.RefreshRow(ViewSiparisSonuc.FocusedRowHandle);
                //SiparisServiceMethods.SetSiparisDurum(WebSiparisDurumlari.TedarikEdiliyor, WEBID, mail: true);
                //progressForm.PerformStep(this);
                //success2++;
                //işlemsonuc.Rows.Add(SALID, WEBID + " numaralı internet satışı " + SALID + " numarası ile volanta işlendi \r\nİşlem tamamlandı. \r\nToplam Başarılı: ." + success2 + " \r\nToplam Hatalı: " + error2);
                //Dictionary<string, string> Durumupdate = new Dictionary<string, string>();
                //frmSiparisDegisken siparisDegisken = new frmSiparisDegisken();
                //switch (siparisDegisken.ShowDialog())
                //{
                //    case DialogResult.Yes:
                //        Durumupdate.Add("@Durum", "TedarikEdiliyor");
                //        break;
                //    case DialogResult.No:
                //        Durumupdate.Add("@Durum", "20");
                //        break;
                //}
                //Durumupdate.Add("@sipID", WEBID);
                //conn.Insert("TicimaxDurumGuncelle", Durumupdate);
            }
            else
            {
                //error2++;
                //Satis.SatisHata myDeserializedClass2 = JsonConvert.DeserializeObject<Satis.SatisHata>(await response.Content.ReadAsStringAsync());
                //XtraMessageBox.Show(myDeserializedClass2.message);
                //işlemsonuc.Rows.Add(WEBID, "Satış işlerken Servisten dönen hata kodu. Hata kodu: " + myDeserializedClass2.message + "\r\nİşlem tamamlandı. Toplam Başarılı : ." + success2 + " Toplam Hatalı : " + error2);
                //progressForm.PerformStep(this);
            }
        }
        public static async Task<string> Post(string apiUrl, string json)
        {
            Form form = new frmSatisAl();
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Add("Authorization", Properties.Settings.Default.VolantToken);
                    string requestBody = json;
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(requestBody, Encoding.UTF8, "application/json"));

                    // Yanıtı okuyoruz
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                }

            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {System.Environment.NewLine} Program Adı: {ex.Source}\n {System.Environment.NewLine} İşlem: {ex.TargetSite}\n {System.Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("Hata Detayı", hataDetay, form, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }
        private static string ToQueryString(object jsonParams)
        {
            if (jsonParams == null) return "";

            var queryString = new StringBuilder();
            var properties = jsonParams.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(jsonParams)?.ToString();
                if (value != null)
                {
                    if (queryString.Length > 0)
                    {
                        queryString.Append("&");
                    }
                    queryString.Append(property.Name);
                    queryString.Append("=");
                    queryString.Append(Uri.EscapeDataString(value));
                }
            }

            if (queryString.Length > 0)
            {
                queryString.Insert(0, "?");
            }

            return queryString.ToString();
        }

        EntegreFDLL.Main.ListtoDataTableConverter converter = new ListtoDataTableConverter();
        private async void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = GetCustomers.Count,
                Position = 0,
                ToplamAdet = GetCustomers.Count.ToString(),
            };
            List<CustomerInsert> customers = new List<CustomerInsert>();
            //executeBackground(
            //    async () =>
            //    {
            //        progressForm.Show(this);
            foreach (var item in GetCustomers)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.VolantToken);
                var code = new Musteri.CustomerCode
                {
                    customerCode = item.CustomerCode,
                };
                // JSON parametrelerini URL'ye eklemek için QueryString'i oluşturun
                string queryString = ToQueryString(code);
                string apiUrl = "api/customers" + queryString;
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                string responseData = await response.Content.ReadAsStringAsync();
                Musteri.MusteriVar MVAR = JsonConvert.DeserializeObject<Musteri.MusteriVar>(responseData);
                if (!MVAR.success)
                {
                    Musteri.MusteriYok MYOK = JsonConvert.DeserializeObject<Musteri.MusteriYok>(responseData);
                    if (MYOK.message == "Müşteri koduyla eşleşen müşteri bulunamadı")
                    {
                        var jsons = JsonConvert.SerializeObject(item);
                        var url = Properties.Settings.Default.VolantApiUrl + "/customers";
                        var ss = await Post(url, jsons);
                        if (ss != null)
                        {
                            if (ss.Contains("Curval"))
                            {
                                Musteri.MusteriVar MSVAR = JsonConvert.DeserializeObject<Musteri.MusteriVar>(ss);

                                var ID = conn.GetValue($@"select CURID from CURRENTS where CURVAL = '{MSVAR.results[3].ToString()}'", sql);
                                customers.Add(new CustomerInsert
                                {
                                    CURID = long.Parse(ID),
                                    CustomerCode = item.CustomerCode
                                });
                            }
                            else
                            {


                                try
                                {
                                    Musteri.MusteriYok MYOK1 = JsonConvert.DeserializeObject<Musteri.MusteriYok>(ss);
                                    foreach (var validations in MYOK1.validations)
                                    {
                                        memoEdit1.Text += item.CustomerCode + " " + item.CustomerFirstName + " " + item.CustomerLastName + $@" işlem hatası : {validations.Message}" + "r\n\n";
                                    }
                                }
                                catch (Exception)
                                {
                                    Musteri.MusteriHata MYOK2 = JsonConvert.DeserializeObject<Musteri.MusteriHata>(ss);
                                    memoEdit1.Text += item.CustomerCode + " " + item.CustomerFirstName + " " + item.CustomerLastName + $@" işlem hatası : {MYOK2.message}" + "r\n\n";
                                }
                            }
                        }
                    }
                }
                else
                {
                    var CURVAL = MVAR.results[3].ToString();
                    var ID = conn.GetValue($@"select CURID from CURRENTS where CURVAL = '{MVAR.results[3].ToString()}'", sql);
                    customers.Add(new CustomerInsert
                    {
                        CURID = long.Parse(ID),
                        CustomerCode = item.CustomerCode
                    });

                    string apiUrl2 = "api/customersUpdate" + queryString;
                    HttpResponseMessage response2 = await httpClient.GetAsync(apiUrl2);
                    string responseData2 = await response2.Content.ReadAsStringAsync();
                }
                //progressForm.PerformStep(this);
            }
            foreach (var item in GetCreaditSales)
            {
                string url = Properties.Settings.Default.VolantApiUrl + "/creditsales";
                string jSon = JsonConvert.SerializeObject(item, Formatting.Indented);
                string requestBody = jSon;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.VolantToken);
                HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));
                var sonucne = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Satislem myDeserializedClass = JsonConvert.DeserializeObject<Satislem>(await response.Content.ReadAsStringAsync());
                    if (myDeserializedClass.success)
                    {
                        conn.InsertValue($@"update SALES set SALUSEFIELD1 = '{item.OrderNumber}', SALDATE = '{item.OrderDate.ToString("yyyy-MM-dd")}', SALDIVISON = '{item.StoreWareHouseCode}' where SALID = {myDeserializedClass.results[1].ToString()}", sql);
                        conn.InsertValue($@"update ORDERS set ORDDATE = '{item.OrderDate.ToString("yyyy-MM-dd")}', ORDDIVISON = '{item.StoreWareHouseCode}' where ORDSALID = {myDeserializedClass.results[1].ToString()}", sql);
                        conn.InsertValue($@"update ORDERSCHILD set ORDCHSHIPDIV = '{item.StoreWareHouseCode}'  from ORDERSCHILD
                        join ORDERS on ORDID = ORDCHORDID where ORDSALID = {myDeserializedClass.results[1].ToString()}", sql);
                        var CURID = customers.FirstOrDefault(x => x.CustomerCode == item.CustomerCode).CURID;
                        List<SALESINVESTIGATION> sALESINVESTIGATIONs = new List<SALESINVESTIGATION>
                        {
                            new SALESINVESTIGATION
                            {
                                SAINGTSALID = long.Parse(myDeserializedClass.results[1].ToString()),
                                SAINGTPOSTSOCODE = "00KR001",
                                SAINGTCURORWRTRID = CURID,
                                SAINGTWORKSTS = 0,
                            }
                        };
                        var SALESINVESTIGATION = converter.ToDataTable(sALESINVESTIGATIONs);
                        var BulkinsertRetourn = conn.BulkInsertRetorn(SALESINVESTIGATION, "SALESINVESTIGATION", sql);
                    }
                    else
                    {

                    }
                }

            }
            foreach (var item in GetPaymentSales)
            {
                string url = Properties.Settings.Default.VolantApiUrl + "/sales";
                string jSon = JsonConvert.SerializeObject(item, Formatting.Indented);
                string requestBody = jSon;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.VolantToken);
                HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));
                var sonucne = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Satislem myDeserializedClass = JsonConvert.DeserializeObject<Satislem>(await response.Content.ReadAsStringAsync());
                    if (myDeserializedClass.success)
                    {
                        conn.InsertValue($@"update SALES set SALUSEFIELD1 = '{item.OrderNumber}', SALDATE = '{item.OrderDate.ToString("yyyy-MM-dd")}', SALDIVISON = '{item.StoreWareHouseCode}' where SALID = {myDeserializedClass.results[1].ToString()}", sql);
                        conn.InsertValue($@"update INVOICE set INVDIVISON = '{item.StoreWareHouseCode}',INVDATE = '{item.OrderDate.ToString("yyyy-MM-dd")}' where INVSALID = {myDeserializedClass.results[1].ToString()}", sql);
                        conn.InsertValue($@"
                        update PRODUCTSBEHAVE set PROBHDATE = '{item.OrderDate.ToString("yyyy-MM-dd")}' from PRODUCTSBEHAVE
                        left outer join INVOICECHILDPROBH on INVCHPBHPROBHID = PROBHID
                        left outer join INVOICECHILD on INVCHINVID = INVCHPBHID
                        left outer join INVOICE on INVID = INVCHINVID
                        where INVSALID = { myDeserializedClass.results[1].ToString() }", sql);
                    }
                    else
                    {

                    }
                }
                else
                {
                    Hata400 myDeserializedClass2 = JsonConvert.DeserializeObject<Hata400>(await response.Content.ReadAsStringAsync());
                    memoEdit1.Text += item.CustomerCode + " " + item.OrderNumber + " " + $@" işlem hatası : {myDeserializedClass2.message}" + "r\n\n";
                    //myDeserializedClass2.message
                }

            }
            //}, 
            //    null, 
            //    () => 
            //    {
            //        progressForm.Close();
            //        completeProgress();
            //    });
        }

        private void ViewVeriler_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView childView = ViewVeriler.GetDetailView(e.RowHandle, e.RelationIndex) as GridView;
            if (childView != null)
            {
                for (int j = 0; j < childView.DataRowCount; j++)
                {
                    childView.ExpandMasterRow(j);
                }
            }
        }
    }
}