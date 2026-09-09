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
using FluentFTP;
using System.Net;
using System.IO;
using System.Management;
using System.Drawing.Imaging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevExpress.XtraSplashScreen;
using NTwain;
using System.Threading;
using PdfSharp.Drawing;
using EntegrefKrediOnay.Class;
using static EntegreFDLL.Class.Siniflar;
using EntegreFDLL;
using EntegreFDLL.Main;
using EntegreFDLL.Class;
using EntegrefKrediOnay.Class.OCRClass;
using static EntegrefKrediOnay.Class.BGVolantTables;
using System.Globalization;

namespace EntegrefKrediOnay.Magaza
{
    public partial class frmIlkMusteri : DevExpress.XtraEditors.XtraForm
    {
        public frmIlkMusteri()
        {
            InitializeComponent();
        }
        class DocumentItem
        {
            public byte[] Data { get; set; }
            public string FileType { get; set; } // ".pdf", ".png"
            public Image image { get; set; }
        }
        class Documents
        {
            public DocumentItem adres { get; set; }
            public DocumentItem arac { get; set; }
            public DocumentItem dava { get; set; }
            public DocumentItem icra { get; set; }
            public DocumentItem sgk { get; set; }
            public DocumentItem tapu { get; set; }
        }
        public class KimlikVerisi
        {
                public bool Success { get; set; }
                public string Message { get; set; }
                public int DocumentType { get; set; } // 1: Kimlik, 2: Ehliyet
                public string TcKimlikNo { get; set; }
                public string Ad { get; set; }
                public string Soyad { get; set; }
                public string DogumTarihi { get; set; }
                public string DogumYeri { get; set; }
                public string SonGecerlilikTarihi { get; set; }
                public string VerilisTarihi { get; set; }
                public string Cinsiyet { get; set; }
                public string SeriNo { get; set; }
                public string SicilNo { get; set; }
                public string KanGrubu { get; set; }
                public string EhliyetSiniflari { get; set; }
                public string VerenMakam { get; set; }
                public string AnneAdi { get; set; }
                public string BabaAdi { get; set; }
                public string RawOcrText { get; set; }
            
        }
        static Documents Belgeler = new Documents();
        string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"EntegreF", "Entegref Taranan Evraklar");
        string CurFilePath;
        string url = "ftp://192.168.4.20//";
        string pass = "Yon";
        string uys = "Yonavm123!";
        public static string CURID;
        public static string CURVAL;
        public static string CURNAME;
        FtpClient client = new FtpClient();
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        private Image kimlikresmi;
        private Image Portreresmi;
        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        public class Tarama
        {
            public string TUR { get; set; }
        }
        EntegreFDLL.Class.Entegref GetEntegref = new Entegref();
        private List<Tarama> scanOptions = new List<Tarama>();
        private void frmKasiyerEdevlet_Load(object sender, EventArgs e)
        {
            Yeni();
            url = Entegref.GetLogins.FTPURL;
            pass = Entegref.GetLogins.FTPPASS;
            uys = Entegref.GetLogins.FTPUSER;
            EntegreFDLL.Class.Entegref.CreateDirectoryIfNotExists(FilePath);
            GetEntegref.Securety(FilePath);
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit3.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            //pictureEdit1.MouseEnter += PictureEdit1_MouseEnter;
            //pictureEdit1.MouseMove += PictureEdit1_MouseMove;
            //pictureEdit1.MouseLeave += PictureEdit1_MouseLeave;            

            //pictureEdit2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            //var pdfsize = panelControl2.Size;
            //flyoutPanel1.OwnerControl = this;
            //flyoutPanel1.Size = new Size(700, 600);
            //var pdfloc = tileBar1.Location;
            //flyoutPanel1.Options.Location = new Point(600,50); //pdfloc; //
            //flyoutPanel1.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
            //flyoutPanel1.Options.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Fade;
            scanOptions.Add(new Tarama
            {
                TUR = "Tarama Seçimi"
            });
            scanOptions.Add(new Tarama
            {
                TUR = "Yeni Kimlik Tara"
            });

            scanOptions.Add(new Tarama
            {
                TUR = "Yeni Ehliyet Tara"
            });
            foreach (Tarama item in scanOptions)
            {
                scanCbx.Properties.Items.Add(item.TUR);
            }
            scanCbx.SelectedIndex = 0;

            txtTC.Enabled = false;
            txtAdi.Enabled = false;
            txtSoyadi.Enabled = false;
            txtAnne.Enabled = false;
            txtBaba.Enabled = false;
            cmbCinsiyet.Enabled = false;
            dteDogum.Enabled = false;
        }
        private void PictureEdit1_MouseLeave(object sender, EventArgs e)
        {
            pictureEdit1.Image = Portreresmi;
            flyoutPanel1.HidePopup();
        }
        int zoomFacet = 400;
        private void PictureEdit1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureEdit1.Image != null)
            {
                if (!flyoutPanel1.IsPopupOpen)
                    flyoutPanel1.ShowPopup();
                Point offsetLocation = pictureEdit1.ViewportToImage(e.Location);

                offsetLocation.X -= zoomFacet / 2;
                offsetLocation.Y -= zoomFacet / 3;

                offsetLocation.X = offsetLocation.X + zoomFacet > pictureEdit1.Image.Width
                    ? pictureEdit1.Image.Width - zoomFacet : offsetLocation.X;
                offsetLocation.X = offsetLocation.X < 0 ? 0 : offsetLocation.X;
                offsetLocation.Y = offsetLocation.Y + zoomFacet > pictureEdit1.Image.Height
                    ? pictureEdit1.Image.Height - zoomFacet : offsetLocation.Y;
                offsetLocation.Y = offsetLocation.Y < 0 ? 0 : offsetLocation.Y;

                pictureEdit2.Image = cropImage(pictureEdit1.Image,
                    new Rectangle(offsetLocation, new Size(zoomFacet, zoomFacet)));
            }
            else
            {
                pictureEdit2.Image = null;
            }
        }
        private void PictureEdit1_MouseEnter(object sender, EventArgs e)
        {
            pictureEdit1.Image = kimlikresmi;
            flyoutPanel1.ShowPopup();
        }
        private static Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics g = Graphics.FromImage(bmpImage))
            {
                g.DrawImage(img, new Rectangle(0, 0, bmpImage.Width, bmpImage.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmpImage;
        }
        private TwainScanner scanner = null;
        TwainHelper GetTwainList = new TwainHelper();
        private async void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {

                Enums.ScanObject scanObject = Enums.ScanObject.Pasaport;
                switch (scanCbx.Text)
                {
                    case "Kimlik Tara":
                        scanObject = Enums.ScanObject.TcKimlik;
                        break;
                    case "Yeni Kimlik Tara":
                        scanObject = Enums.ScanObject.YeniTcKimlik;
                        break;
                    case "Yeni Ehliyet Tara":
                        scanObject = Enums.ScanObject.YeniEhliyet;
                        break;
                }
                string secilenTarayici = "";
                var scanlist = TwainHelper.GetScannerList();
                if (scanlist.Count == 0)
                {
                    // Eğer liste boşsa manuel giriş iste veya SetupTwain'i tekrar dene
                    secilenTarayici = DevExpress.XtraEditors.XtraInputBox.Show("Sistemde otomatik tarayıcı bulunamadı. Lütfen tarayıcı adını el ile girin:", "Tarayıcı Bulunamadı", "");
                }
                else
                {
                    // Kullanıcıya listeden seçtir (DevExpress RadioGroup veya basit bir seçim penceresi)
                    // Örnek olarak listenin ilkini alıyoruz veya kullanıcıya seçtiriyoruz:
                    DevExpress.XtraEditors.XtraInputBoxArgs args = new DevExpress.XtraEditors.XtraInputBoxArgs();
                    args.Caption = "Tarayıcı Seçimi";
                    args.Prompt = "Lütfen kullanmak istediğiniz tarayıcıyı seçin:";
                    args.DefaultButtonIndex = 0;

                    // ComboBox Editörünü Tanımla
                    DevExpress.XtraEditors.ComboBoxEdit editor = new DevExpress.XtraEditors.ComboBoxEdit();
                    foreach (var tarayici in scanlist)
                    {
                        editor.Properties.Items.Add(tarayici);
                    }
                    editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor; // Elle yazmayı kapat

                    args.Editor = editor;
                    args.DefaultResponse = scanlist[0]; // İlk tarayıcıyı varsayılan seç

                    // Seçimi Göster
                    var result = DevExpress.XtraEditors.XtraInputBox.Show(args);
                    secilenTarayici = result?.ToString();
                }

                if (scanner == null)
                {
                    scanner = new TwainScanner();
                }                
                scanner.rCusIdentity = new eCusIdentity();
                scanner.ScanIdentity(secilenTarayici, scanObject);
                try
                {
                    while (!scanner.isFinished)
                    {
                        Thread.Sleep(100);
                    }
                    if (!scanner.isCompleted)
                    {
                        scanner.isFinished = false;
                        if (scanner.scannedImages == null || scanner.scannedImages.Count == 0)
                        {
                            MessageBox.Show("Kimlik taranırken bir problemle karşılaşıldı!");
                        }
                        else if (scanner.scannedImages.Count > 0)
                        {
                            MessageBox.Show("Kimliğin diğer yüzünü çevirin ve Tekrar seçiniz");
                        }
                    }
                    else
                    {
                        Image head = scanner.Head;
                        Image ıdentity = scanner.Identity;
                        if (head != null)
                        {
                            Portreresmi = head;
                        }
                        else
                        {
                            MessageBox.Show("Kimlikteki Resim Algılanamadı!");
                        }
                        if (ıdentity != null)
                        {
                            kimlikresmi = ıdentity;
                        }
                        else
                        {
                            MessageBox.Show("Kimlik algılanamadı!");
                        }
                        //kimlikresmi.Save(FilePath+"/3.png", ImageFormat.Png);
                        await SendScannedBitmapToApiAsync(kimlikresmi);
                        scanner = null;
                        scanCbx.Enabled = false;
                        pictureEdit1.Image = Portreresmi;
                        pictureEdit3.Image = kimlikresmi;
                        pictureEdit1.Refresh();
                        pictureEdit3.Refresh();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Hata: " + ex.Message);
            }

        }
        public async Task SendScannedBitmapToApiAsync(Image scannedBitmap)
        {
            // 1. Bitmap'i Bellekte JPEG Formatında Base64 String'e Dönüştür
            string base64Image;
            using (MemoryStream ms = new MemoryStream())
            {
                // fi-6130 resim formatı fark etmeksizin JPEG olarak serileştirilir
                scannedBitmap.Save(ms, ImageFormat.Jpeg);
                byte[] imageBytes = scannedBitmap.imageToByteArray();
                base64Image = Convert.ToBase64String(imageBytes);
            }

            // 2. HTTP POST İsteği Hazırla
            using (HttpClient client = new HttpClient())
            {
                // API Adresinizi Buraya Yazın
                string apiUrl = "";
                int Type = 1;
                switch (scanCbx.Text)
                {
                    case "Yeni Kimlik Tara":
                        Type = 1;
                        apiUrl = "http://localhost:1135/api/idcardnet/";
                        break;
                    case "Yeni Ehliyet Tara":
                        Type = 2;
                        apiUrl = "http://localhost:1135/api/idcard3/";
                        break;
                }
                var requestBody = new { Type, Base64Image = base64Image };
                string jsonPayload = JsonConvert.SerializeObject(requestBody, Formatting.Indented);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    KimlikVerisi myDeserializedClass = JsonConvert.DeserializeObject<KimlikVerisi>(jsonResult);
                    // Null kontrolleri
                    lbllimlik.Text = myDeserializedClass?.Message ?? string.Empty;
                    lbllimlik.Tag = myDeserializedClass?.DocumentType.ToString();
                    lbllimlik.Visible = true;
                    txtTC.Text = myDeserializedClass?.TcKimlikNo ?? string.Empty;
                    txtSerino.Text = myDeserializedClass?.SeriNo ?? string.Empty;
                    txtAdi.Text = myDeserializedClass?.Ad ?? string.Empty;
                    txtSoyadi.Text = myDeserializedClass?.Soyad ?? string.Empty;
                    txtAnne.Text = myDeserializedClass?.AnneAdi?.ToString() ?? string.Empty;
                    txtBaba.Text = myDeserializedClass?.BabaAdi?.ToString() ?? string.Empty;

                    // Tarih kontrolü (yyyy-MM-dd formatı ile geliyor)
                    if (!string.IsNullOrEmpty(myDeserializedClass?.DogumTarihi))
                    {
                        if (DateTime.TryParseExact(
                                myDeserializedClass.DogumTarihi,
                                "yyyy-MM-dd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out DateTime parsedDate))
                        {
                            dteDogum.EditValue = parsedDate;
                        }
                        else
                        {
                            dteDogum.EditValue = DateTime.Parse(myDeserializedClass.DogumTarihi).ToString("yyyy-MM-dd"); // format hatalıysa boş bırak
                        }
                    }
                    else
                    {
                        dteDogum.EditValue = null;
                    }

                    // Cinsiyet kontrolü
                    cmbCinsiyet.EditValue = myDeserializedClass?.Cinsiyet ?? string.Empty;

                    txtTC.Enabled = true;
                    txtSerino.Enabled = true;
                    txtAdi.Enabled = true;
                    txtSoyadi.Enabled = true;
                    txtAnne.Enabled = true;
                    txtBaba.Enabled = true;
                    cmbCinsiyet.Enabled = true;
                    dteDogum.Enabled = true;
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Hata: {response.StatusCode} - {error}");
                }
            }
        }
        void Yeni()
        {
            txtTC.Text = null;
            txtSerino.Text = null;
            txtAdi.Text = null;
            txtSoyadi.Text = null;
            txtAnne.Text = null;
            txtBaba.Text = null;
            dteDogum.EditValue = null;
            lbllimlik.Text = null;
            lbllimlik.Tag = null;
            lbllimlik.Visible = false;
            txtTC.Enabled = false;
            txtSerino.Enabled = false;
            txtAdi.Enabled = false;
            txtSoyadi.Enabled = false;
            txtAnne.Enabled = false;
            txtBaba.Enabled = false;
            cmbCinsiyet.Enabled = false;
            dteDogum.Enabled = false;
        }
        public static int GetAutomaticScanLimit()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong totalMemory = (ulong)obj["TotalPhysicalMemory"]; // Byte cinsinden
                    ulong usableMemory = totalMemory / 5; // %20'sini kullanalım
                    int usableMB = (int)(usableMemory / (1024 * 1024));

                    int estimatedLimit = usableMB / 4; // Her belge 4MB varsayımıyla
                    return Math.Max(5, Math.Min(estimatedLimit, 100)); // 5–100 arası sınırla
                }
            }
            catch { }

            return 10; // Hata durumunda varsayılan
        }
        void DeleteFile(string tempPdfPath)
        {
            if (Directory.Exists(tempPdfPath))
            {
                try
                {
                    // Dosyaları tek tek sil
                    foreach (string file in Directory.GetFiles(tempPdfPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.SetAttributes(file, FileAttributes.Normal); // Salt okunur gibi engelleri kaldır
                            File.Delete(file);
                        }
                        catch { /* Gerekirse logla */ }
                    }

                    // Alt klasörleri sil
                    foreach (string dir in Directory.GetDirectories(tempPdfPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch { /* Gerekirse logla */ }
                    }

                    // En son klasörün kendisini sil
                    Directory.Delete(tempPdfPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Klasör silinirken hata oluştu: " + ex.Message);
                }
            }
        }
        public async Task SaveImageAsPdfWithLimitAsync(Image image, string filePath, int maxSizeInBytes = 1_000_000)
        {
            await Task.Run(() =>
            {
                long quality = 80;
                int attempt = 0;

                while (attempt < 5)
                {
                    SaveImageAsPdf2(image, filePath, quality);
                    var fileSize = new FileInfo(filePath).Length;

                    if (fileSize <= maxSizeInBytes)
                        break;

                    quality -= 10;
                    if (quality < 20) break;

                    attempt++;
                }
            });
        }
        private static void SaveImageAsPdf2(Image image, string filePath, long jpegQuality = 50L)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var doc = new PdfSharp.Pdf.PdfDocument())
            {
                var page = doc.AddPage();
                page.Width = XUnit.FromMillimeter(210); //image.Width;
                page.Height = XUnit.FromMillimeter(310); //image.Height;

                using (var gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page))
                using (var ms = new MemoryStream())
                {
                    using (var reduced = ReduceDpi(image))
                    {
                        var encoder = GetEncoder(ImageFormat.Jpeg);
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality); // 1-100

                        reduced.Save(ms, encoder, encoderParams);
                    }
                    ms.Position = 0;

                    using (var img = System.Drawing.Image.FromStream(ms))
                    {
                        using (var xImage = PdfSharp.Drawing.XImage.FromGdiPlusImage(img))
                        {
                            gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                        }
                    }

                }

                doc.Save(filePath);
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
        }
        private static Bitmap ReduceDpi(Image originalImage, int targetDpi = 150)
        {
            Bitmap bitmap = new Bitmap(originalImage);
            bitmap.SetResolution(targetDpi, targetDpi);
            return bitmap;
        }
        private void EnsureFtpDirectoriesExist(string ftpBaseUrl, string ftpUser, string ftpPass, string curID)
        {
            if (string.IsNullOrWhiteSpace(curID)) return;

            string[] altKlasorler = new[]
            {
                "E-Devlet",
            };
            client.ConnectTimeout = 5000;
            client.ReadTimeout = 5000;
            client.DataConnectionConnectTimeout = 5000;
            client.DataConnectionReadTimeout = 5000;

            try
            {
                if (!client.IsConnected)
                {
                    client.Connect();
                }

                string anaKlasor = $"/{curID}";
                if (!client.DirectoryExists(anaKlasor))
                {
                    client.CreateDirectory(anaKlasor, true);
                }

                foreach (var folder in altKlasorler)
                {
                    string fullPath = $"{anaKlasor}/{folder}";
                    if (!client.DirectoryExists(fullPath))
                    {
                        client.CreateDirectory(fullPath, true);
                    }
                }
            }
            catch (FtpException ftpEx)
            {
                Console.WriteLine("FTP hatası: " + ftpEx.Message);
            }
        }
        public async Task<string> UploadFileToFtpIfNotExists(string ftpBaseUrl, string curID, string remoteFolder, string remoteFileName, string filePath, string ftpUser, string ftpPass)
        {
            string fullRemotePath = $"/{curID}/{remoteFolder}/{remoteFileName}";

            try
            {
                client.ConnectTimeout = 5000;
                client.ReadTimeout = 5000;
                client.DataConnectionConnectTimeout = 5000;
                client.DataConnectionReadTimeout = 5000;
                string directoryPath = $"/{curID}/{remoteFolder}";
                if (!await client.DirectoryExistsAsync(directoryPath))
                    await client.CreateDirectoryAsync(directoryPath, true);

                if (await client.FileExistsAsync(fullRemotePath))
                {
                    return "Aynı isimde dosya FTP'de zaten var. Yükleme yapılmadı.";
                }

                var status = await client.UploadFileAsync(filePath, fullRemotePath, FtpExists.Overwrite, true);

                if (status == true)
                {
                    return "Dosya başarıyla yüklendi.";
                }
                else
                {
                    return "Dosya yüklenemedi.";
                }
            }
            catch (Exception ex)
            {
                return "FTP işleminde hata: " + ex.Message;
            }
        }
        async Task<string> FtpFilePDF(Image image, string FileName, string TempPath)
        {
            try
            {
                client = new FtpClient(url, new NetworkCredential(uys, pass));
                await client.ConnectAsync();
            }
            catch (Exception)
            {

            }
            try
            {
                string filename = $"{FileName}_{DateTime.Today.ToString("yyyy-MM-dd")}.pdf";
                string tempFullPath = Path.Combine(TempPath, filename);
                await SaveImageAsPdfWithLimitAsync(image, tempFullPath);
                EnsureFtpDirectoriesExist(url, uys, pass, CURID);
                var FtpSonuc = await UploadFileToFtpIfNotExists(url, CURID, "E-Devlet", filename, tempFullPath, uys, pass);
                return FtpSonuc;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private async Task<bool> Scannn(string FileName, string CURID, PictureEdit pictureEdit,TextEdit textEdit, DocumentItem  document)
        {
            string tempPdfPath = "";
            List<ScanResult> results = new List<ScanResult>();
            try
            {
                var scanner = new DocumentScan(this);
                scanner.MaxScanLimit = GetAutomaticScanLimit();
                results = await scanner.StartScanAsync(false,true);
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("Hata Detayı", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            tempPdfPath = Path.Combine(CurFilePath, FileName);
            if (results != null && results.Count > 0)
            {
                try
                {
                    //await FtpFilePDF(results[0].Image, FileName, tempPdfPath);
                    string filename = $"{FileName}_{DateTime.Today.ToString("yyyy-MM-dd")}.pdf";
                    string filePath = Path.Combine(tempPdfPath, filename);
                    textEdit.Text = "Tarama Başarılı";
                    pictureEdit.Image = results[0].Image;
                    document.image = results[0].Image;
                    await SaveImageAsPdfWithLimitAsync(results[0].Image, filePath);
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    string ext = Path.GetExtension(filePath).ToLower();
                    document.Data = fileBytes;
                    document.FileType = ext; // ".pdf", ".jpg"
                    return true;
                }
                catch (Exception ex)
                {
                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    CustomMessageBox.ShowMessage("Hata Detayı", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        async Task<bool> Dosya(string FileName,string NewName, DocumentItem documentItem)
        {
            try
            {
                if (documentItem == null)
                {
                    documentItem = new DocumentItem();
                }
                if (Path.GetExtension(FileName).ToLower() == ".pdf")
                {
                    //using (var document = PdfDocument.Load(FileName))
                    //{
                    //    for (int i = 0; i < document.PageCount; i++)
                    //    {
                    //        // 👇 PDF sayfasını image çevir
                    //        Image img = document.Render(i, 300, 300, true);

                    //        var outputPath = Path.Combine(
                    //            CurFilePath,
                    //            $"{NewName}_{DateTime.Today.ToString("yyyy-MM-dd")}_{i + 1}.png");
                    //        img.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
                    //        if (i == 0)
                    //        {
                    //            documentItem.image = img;
                    //        }
                    //    }
                    //}
                    //documentItem.image = img;
                    byte[] fileBytes = File.ReadAllBytes(FileName);
                    string ext = Path.GetExtension(FileName).ToLower();
                    documentItem.Data = fileBytes;
                    documentItem.FileType = ext; // ".pdf", ".jpg"
                }
                else
                {
                    // dosyayı byte[] olarak al
                    byte[] fileBytes = File.ReadAllBytes(FileName);
                    string ext = Path.GetExtension(FileName).ToLower();


                    // 👇 Image oluştur (preview için)
                    using (var ms = new MemoryStream(fileBytes))
                    {
                        Image img = Image.FromStream(ms);

                        // ilk image'ı sakla (senin yapına göre)
                        documentItem.image = (Image)img.Clone();

                        // diske kayıt (PDF'teki gibi)
                        var outputPath = Path.Combine(
                            CurFilePath,
                            $"{NewName}_{DateTime.Today:yyyy-MM-dd}.png");
                        img.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

                        var outputPath1 = Path.Combine(
                            CurFilePath,
                            $"{NewName}_{DateTime.Today:yyyy-MM-dd}.pdf");
                        await SaveImageAsPdfWithLimitAsync(documentItem.image, outputPath1);
                        byte[] fileBytes2 = File.ReadAllBytes(outputPath1);
                        string ext2 = Path.GetExtension(outputPath1).ToLower();
                        documentItem.Data = fileBytes;
                        documentItem.FileType = ext2;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //private void pictureEdit3_Click(object sender, EventArgs e)
        //{
        //    Form previewForm = new Form();
        //    previewForm.StartPosition = FormStartPosition.CenterScreen;
        //    previewForm.Size = new Size(800, 800);
        //    if (Belgeler.adres.FileType == ".pdf")
        //    {
        //        DevExpress.XtraPdfViewer.PdfViewer pdfViewer = new DevExpress.XtraPdfViewer.PdfViewer();
        //        pdfViewer.Dock = DockStyle.Fill;
        //        pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
        //        //pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfViewerZoomMode.FitWidth;

        //        // 👇 PDF'i yükle (senin eksik olan kısım burası)
        //        MemoryStream ms = new MemoryStream(Belgeler.adres.Data);
        //        pdfViewer.LoadDocument(ms);

        //        previewForm.Controls.Add(pdfViewer);
        //    }
        //    else
        //    {
        //        PictureBox pb = new PictureBox();
        //        pb.Dock = DockStyle.Fill;
        //        pb.SizeMode = PictureBoxSizeMode.Zoom;
        //        pb.Image = pictureEdit3.Image;
        //        previewForm.Controls.Add(pb);
        //    }

        //    previewForm.ShowDialog();
        //}
        //private void pictureEdit4_Click(object sender, EventArgs e)
        //{
        //    Form previewForm = new Form();
        //    previewForm.StartPosition = FormStartPosition.CenterScreen;
        //    previewForm.Size = new Size(800, 800);

        //    PictureBox pb = new PictureBox();
        //    pb.Dock = DockStyle.Fill;
        //    pb.SizeMode = PictureBoxSizeMode.Zoom;
        //    pb.Image = pictureEdit4.Image;

        //    previewForm.Controls.Add(pb);
        //    previewForm.ShowDialog();
        //}
        //private void pictureEdit5_Click(object sender, EventArgs e)
        //{
        //    Form previewForm = new Form();
        //    previewForm.StartPosition = FormStartPosition.CenterScreen;
        //    previewForm.Size = new Size(800, 800);

        //    PictureBox pb = new PictureBox();
        //    pb.Dock = DockStyle.Fill;
        //    pb.SizeMode = PictureBoxSizeMode.Zoom;
        //    pb.Image = pictureEdit5.Image;

        //    previewForm.Controls.Add(pb);
        //    previewForm.ShowDialog();
        //}
        //private void pictureEdit6_Click(object sender, EventArgs e)
        //{
        //    Form previewForm = new Form();
        //    previewForm.StartPosition = FormStartPosition.CenterScreen;
        //    previewForm.Size = new Size(800, 800);

        //    PictureBox pb = new PictureBox();
        //    pb.Dock = DockStyle.Fill;
        //    pb.SizeMode = PictureBoxSizeMode.Zoom;
        //    pb.Image = pictureEdit6.Image;

        //    previewForm.Controls.Add(pb);
        //    previewForm.ShowDialog();
        //}
        //private void pictureEdit7_Click(object sender, EventArgs e)
        //{
        //    Form previewForm = new Form();
        //    previewForm.StartPosition = FormStartPosition.CenterScreen;
        //    previewForm.Size = new Size(800, 800);

        //    PictureBox pb = new PictureBox();
        //    pb.Dock = DockStyle.Fill;
        //    pb.SizeMode = PictureBoxSizeMode.Zoom;
        //    pb.Image = pictureEdit7.Image;

        //    previewForm.Controls.Add(pb);
        //    previewForm.ShowDialog();
        //}
        //private void pictureEdit8_Click(object sender, EventArgs e)
        //{
        //    Form previewForm = new Form();
        //    previewForm.StartPosition = FormStartPosition.CenterScreen;
        //    previewForm.Size = new Size(800, 800);

        //    PictureBox pb = new PictureBox();
        //    pb.Dock = DockStyle.Fill;
        //    pb.SizeMode = PictureBoxSizeMode.Zoom;
        //    pb.Image = pictureEdit8.Image;

        //    previewForm.Controls.Add(pb);
        //    previewForm.ShowDialog();
        //}
        
        private byte[] BitmapToByte(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private byte[] Combine(Bitmap front, Bitmap back)
        {
            if (back == null)
                return BitmapToByte(front);

            int w = front.Width + back.Width;
            int h = Math.Max(front.Height, back.Height);

            using (Bitmap final = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(final))
            {
                g.DrawImage(front, 0, 0);
                g.DrawImage(back, front.Width, 0);

                return BitmapToByte(final);
            }
        }

        private byte[] CropFaceBitmap(Bitmap bmp)
        {
            Rectangle r = new Rectangle(20, 100, 220, 260);

            using (Bitmap face = bmp.Clone(r, bmp.PixelFormat))
            {
                return BitmapToByte(face);
            }
        }

        private async Task<string> OCR(string pdfPath)
        {
            //HttpClient httpClient = new HttpClient
            //{
            //    Timeout = new TimeSpan(1, 1, 1)
            //};
            //MultipartFormDataContent form = new MultipartFormDataContent
            //    {
            //        {
            //            new StringContent("7b0ede3ee988957"),
            //            "apikey"
            //        },
            //        {
            //            new StringContent("tur"),
            //            "language"
            //        }
            //    };
            //if (string.IsNullOrEmpty(ImagePath) == false)
            //{
            //    byte[] imageData = File.ReadAllBytes(ImagePath);
            //    //byte[] imageData = File.ReadAllBytes(ImagePath);
            //    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
            //}
            //else if (string.IsNullOrEmpty(PdfPath) == false)
            //{
            //    byte[] imageData = File.ReadAllBytes(PdfPath);
            //    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "PDF", "pdf.pdf");
            //}
            //HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);

            //string strContent = await response.Content.ReadAsStringAsync();

            //Rootobject ocrResult = JsonConvert.DeserializeObject<Rootobject>(strContent);

            //string result = "";
            //if (ocrResult.OCRExitCode == 1)
            //{
            //    for (int i = 0; i < ocrResult.ParsedResults.Length; i++)
            //    {
            //        result += ocrResult.ParsedResults[i].ParsedText;
            //    }
            //}

            string result = "";
            AiAnalysisService AiAnalysisService = new AiAnalysisService();
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            var base64Data = Convert.ToBase64String(pdfBytes);
            await SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
                    async (progress, token) =>
                    {
                        progress.Report((0, $"Yapay Zeka Danışmanına Soruluyor"));
                        await Task.Delay(10, token);
                        token.ThrowIfCancellationRequested();                        
                        string myPrompt2 = @"
                    metin gibi tüm sayfayı oku aşagıdaki formatta ver, gelen sonucu class içine sorunsuz JsonConvert.DeserializeObject ile alabilmeliyim. yani cevabın josun formatında olsun
                    public class Kimlik
                    {
                        public string Identity {get ; set;}
                        public string Surname  {get ; set;}
                        public string GivenName  {get ; set;}
                        public string DateofBirth  {get ; set;}
                        public string Gender {get ; set;}
                        public string DocumentNo  {get ; set;}
                        public string Nationality  {get ; set;}
                        public string ValidUntil  {get ; set;}
                    }";

                        result = await AiAnalysisService.AnalyzePdfWithGemini(myPrompt2, base64Data);
                    });

            return result;
        }
        private string SaveImageAsPng(Bitmap image, string _brkod)
        {
            var resim = image; // ResizeImageWithAspectRatio(image, 800, 1000);
            try
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scans");
                Directory.CreateDirectory(directory);

                string fileName = Path.Combine(directory, $"Scan_{_brkod}.png");
                resim.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                return fileName;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + "Görüntü kaydedilirken hata oluştu: " + ex.Message + Environment.NewLine);
                return "";
            }
        }

        private async void tileBarItem2_ItemClick(object sender, TileItemEventArgs e)
        {
            DialogResult dialogResult = XtraMessageBox.Show("Müşteri Kaydı Yapılsın mı", "", MessageBoxButtons.OKCancel);
            switch (dialogResult)
            {
                case DialogResult.None:
                    break;
                case DialogResult.OK:

                    // Kullanıcıya listeden seçtir (DevExpress RadioGroup veya basit bir seçim penceresi)
                    // Örnek olarak listenin ilkini alıyoruz veya kullanıcıya seçtiriyoruz:
                    //DevExpress.XtraEditors.XtraInputBoxArgs args = new DevExpress.XtraEditors.XtraInputBoxArgs();
                    //args.Caption = "Müşteri Kaydı";
                    //args.Prompt = "Kayıt için Müşteri TC girin:";
                    //args.DefaultButtonIndex = 0;

                    //// ComboBox Editörünü Tanımla
                    //DevExpress.XtraEditors.TextEdit editor = new DevExpress.XtraEditors.TextEdit();
                    //args.Editor = editor;

                    //// Seçimi Göster
                    //var sonuc = DevExpress.XtraEditors.XtraInputBox.Show(args);
                    var tc = txtTC.Text;
                    if (tc != null)
                    {
                        var kayitli = conn.GetData($"select * from EntegreF.dbo.IDENTYPICTURE where IPIDENTY = '{tc}' and IPTYPE = '{lbllimlik.Tag}'", sql2);

                        if (kayitli != null)
                        {
                            CustomMessageBox.ShowMessage("Girilen TC kaytlı", "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Yeni();
                            pictureEdit1.Image = null;
                            pictureEdit3.Image = null;
                        }
                        else
                        {
                            await SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
                                async (progress, token) =>
                                {
                                    await Task.Delay(100, token);
                                    token.ThrowIfCancellationRequested();
                                    progress.Report((0, $"Kaydediliyor... "));
                                    var NEWIPID = conn.GetValueConnection("select max(isnull(IPID,0))+1 from EntegreF.dbo.IDENTYPICTURE", sql1);
                                    if (NEWIPID == "")
                                        NEWIPID = "1";
                                    List<IDENTYPICTURE> cUSTOMERPICTUREs = new List<IDENTYPICTURE>
                                    {
                                        new IDENTYPICTURE
                                        {
                                            IPID = long.Parse(NEWIPID),
                                            IPIDENTY = tc,
                                            IPCARDNUMBER = txtSerino.Text,
                                            IPNAME = txtAdi.Text,
                                            IPSURNAME = txtSoyadi.Text,
                                            IPBIRTHDAY = DateTime.Parse(dteDogum.EditValue.ToString()),
                                            IPMOTHER = txtAnne.Text,
                                            IPFATHER = txtBaba.Text,
                                            IPSEX = cmbCinsiyet.EditValue.ToString().Replace(" ","").Trim(),
                                            IPDIVVAL = Entegref.GetLogins.userDIVVAL,
                                            IPCREATECUR = false,
                                            IPCURID = null,
                                            IPPORTRAIT = Convert.ToBase64String(Portreresmi.imageToByteArray()),
                                            IPFULLSIDE = Convert.ToBase64String(kimlikresmi.imageToByteArray()),
                                            IPTYPE = short.Parse(lbllimlik.Tag.ToString()),
                                        }
                                    };
                                    await BGVolantTables.INSERTIDENTYPICTURE(cUSTOMERPICTUREs, Properties.Settings.Default.connectionstring2);
                                });
                            tileBarItem3_ItemClick(null, null);
                        }
                    }
                    break;
                case DialogResult.Cancel:
                    break;
                default:
                    break;
            }
        }

        private void tileBarItem3_ItemClick(object sender, TileItemEventArgs e)
        {
            Yeni();
            pictureEdit1.Image = null;
            pictureEdit3.Image = null;
            scanCbx.Enabled = true;
        }
    }
}