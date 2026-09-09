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
using System.Net;
using System.IO;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraBars.Ribbon;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevExpress.XtraSplashScreen;
using DevExpress.LookAndFeel;
using EntegreFDLL.Class;
using EntegrefKrediOnay.Class;
using EntegreFDLL;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmBGFtp : DevExpress.XtraEditors.XtraForm
    {
        string CURID;
        public frmBGFtp(string _id)
        {
            try
            {
                Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.CompanyName, "Yüklü Evraklar Açılıyor");
                InitializeComponent();
                CURID = _id;
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
        private string ftpServer = Entegref.GetLogins.FTPURL;//Properties.Settings.Default.VolFtpHost;
        private string ftpUser = Entegref.GetLogins.FTPUSER; //Properties.Settings.Default.VolFtpUser;
        private string ftpPass = Entegref.GetLogins.FTPPASS;  //Properties.Settings.Default.VolFtpPass;

        BGClass GetBGClass = new BGClass();
        SqlConnection sql = new SqlConnection(Properties.Settings.Default.connectionstring);
        SqlConnection sql2 = new SqlConnection(Properties.Settings.Default.connectionstring2);
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
                        string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
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
        void SplashScreen()
        {
            FluentSplashScreenOptions splashScreen = new FluentSplashScreenOptions();
            splashScreen.Title = "ENTEGREF";
            splashScreen.Subtitle = $@"{Properties.Settings.Default.CompanyName} Saha Yönetim Destek Tools";
            splashScreen.RightFooter = "Yükleniyor...";
            splashScreen.LeftFooter = $"CopyRight ® 2023 {Environment.NewLine} Tüm Hahkları Saklıdır.";
            splashScreen.LoadingIndicatorType = FluentLoadingIndicatorType.Dots;
            splashScreen.OpacityColor = System.Drawing.Color.FromArgb(16, 110, 190);
            splashScreen.Opacity = 90;
            splashScreen.AppearanceLeftFooter.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowFluentSplashScreen(splashScreen, parentForm: this, useFadeIn: true, useFadeOut: true);
        }
        private void frmBGFtp_Load(object sender, EventArgs e)
        {
            pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            xtraTabControl1.TabPages[0].PageVisible = false;
            xtraTabControl1.TabPages[1].PageVisible = false;
            // Grid view sütun ayarı (basit)
            viewFiles.Columns.Clear();
            viewFiles.Columns.AddVisible("Name", "Dosya/Folder");
            viewFiles.Columns.AddVisible("Path", "Path");
            viewFiles.Columns[1].Visible = false;
            LoadFtpRoot(CURID);
        }
        // 📂 FTP'den klasörleri çek


        // Kökü veya verilen path içeriğini (sadece klasörler) gallery'e yükler
        private void LoadFtpRoot(string relativePath)
        {
            Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.CompanyName, "Yüklü Evraklar Açılıyor");
            //await Entegref.RunWithSplashAsync(this, false, 0, this.Text,
            //   async (progress, token) =>
            //   {
            //       progress.Report((0, $"Dosyalar Yükleniyor"));
            //       await Task.Delay(10, token);
            //       token.ThrowIfCancellationRequested();
            try
            {
                this.Enabled = false;
                string ftpPath = CombineFtpPath(ftpServer, relativePath);

                var items = GetFtpDirectoryList(ftpPath, onlyFolders: true);

                var gallery = galleryControlFolders.Gallery;
                gallery.Groups.Clear();
                var group = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
                gallery.Groups.Add(group);

                foreach (var fi in items)
                {
                    var it = new GalleryItem();
                    it.Caption = fi.Name;                         // başlık (isim)
                    it.Tag = fi.Path;                             // tam ftp path
                    it.Hint = fi.IsFolder ? "Klasör" : "Dosya";
                    // ikon: kendi resource veya null
                    try { it.Image = Properties.Resources.folder; } catch { }

                    group.Items.Add(it);
                }

                gallery.ItemClick -= Gallery_ItemClick;
                gallery.ItemClick += Gallery_ItemClick;
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                // UI thread'e güvenli dönüş
                this.Enabled = true;
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
            this.Enabled = true;
            //});
        }

        // Gallery öğesine tıklanınca
        private void Gallery_ItemClick(object sender, GalleryItemClickEventArgs e)
        {
            try
            {
                //this.Enabled = false;
                if (e.Item == null) return;
                string path = e.Item.Tag as string;
                if (string.IsNullOrEmpty(path))
                {
                    MessageBox.Show("Seçilen öğe yolu boş.");
                    return;
                }

                // 1) Alt klasörleri göster (yeni gallery)
                // ftp path'den relative kısmı alıp LoadFtpRoot ile tekrar yükleyebiliriz
                string relative = GetRelativePathFromServer(path);
                LoadFtpRoot(relative);
                if (galleryControlFolders.Gallery.Groups.Count == 1)
                {
                    LoadFtpRoot(CURID);
                }
                // 2) Dosyaları grid'e yükle
                var files = GetFtpDirectoryList(path, onlyFolders: false)
                .Where(x => !x.IsFolder)
                .Select(file =>
                {
                    string originalName = file.Name;
                    string normalizedName = NormalizeFileName(originalName);
                    if (!originalName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase))
                    {
                        RenameFtpFile(path, originalName, normalizedName);
                    }

                    var parts = Path.GetFileNameWithoutExtension(normalizedName).Split('_');
                    if (parts.Length == 3)
                    {
                        var tür = parts[0];
                        var tarih = parts[2];
                        file.Name = $"{tarih}_{tür}{Path.GetExtension(normalizedName)}";
                    }
                    else
                    {
                        file.Name = normalizedName;
                    }

                    return file;

                    //// Eğer isim değişmişse FTP üzerinde rename yap
                    //if (!originalName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    RenameFtpFile(path, originalName, normalizedName);
                    //    var parts = file.Name.Split('_');
                    //    if (parts.Length == 3)
                    //    {
                    //        var tür = parts[0];
                    //        var tarih = Path.GetFileNameWithoutExtension(parts[2]);
                    //        var extension = Path.GetExtension(file.Name);
                    //        file.Name = $"{tarih}_{tür}{extension}";
                    //    }
                    //    //file.Name = normalizedName;
                    //}
                    //else
                    //{
                    //    var parts = originalName.Split('_');
                    //    if (parts.Length == 3)
                    //    {
                    //        var tür = parts[0];
                    //        var tarih = Path.GetFileNameWithoutExtension(parts[2]);
                    //        var extension = Path.GetExtension(originalName);
                    //        file.Name = $"{tarih}_{tür}{extension}";
                    //    }
                    //    //file.Name = originalName;
                    //}

                    //return file;
                })
                .ToList();
                gridFiles.DataSource = files;
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                // UI thread'e güvenli dönüş
                this.Enabled = true;
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
        }
        private string NormalizeFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            nameWithoutExt = nameWithoutExt
                .Replace("İ", "I").Replace("ı", "i")
                .Replace("Ş", "S").Replace("ş", "s")
                .Replace("Ç", "C").Replace("ç", "c")
                .Replace("Ö", "O").Replace("ö", "o")
                .Replace("Ü", "U").Replace("ü", "u")
                .Replace("Ğ", "G").Replace("ğ", "g");

            // Boşluk ve özel karakterleri temizle
            nameWithoutExt = nameWithoutExt.Replace(" ", "_");
            nameWithoutExt = Regex.Replace(nameWithoutExt, @"[^a-zA-Z0-9_\-]", "");

            //return nameWithoutExt.ToLowerInvariant() + extension.ToLowerInvariant();
            var cleaned = fileName.Trim();
            cleaned = cleaned.Replace("()", "").Replace("  ", " ");
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(cleaned);
            return $"{fileNameWithoutExt}{extension}";


        }

        private void RenameFtpFile(string currentPath, string oldName, string newName)
        {
            try
            {
                string baseUri = currentPath.TrimEnd('/');
                string oldFileUri = $"{baseUri}/{oldName}";
                string newFileUri = $"{baseUri}/{Uri.EscapeDataString(newName)}";

                // RNFR komutu
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(oldFileUri);
                request.Method = WebRequestMethods.Ftp.Rename;
                request.Credentials = new NetworkCredential(ftpUser, ftpPass);
                request.RenameTo = newName;
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = false;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"FTP Rename Success: {oldName} -> {newName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FTP Rename Error: {ex.Message}");
            }
        }

        // FTP klasör/dosya listesini getirir. onlyFolders==true ise yalnız klasörleri döner.
        private List<FtpItem> GetFtpDirectoryList(string ftpPath, bool onlyFolders = false)
        {
            var list = new List<FtpItem>();

            try
            {
                // Öncelikle detaylı liste isteyelim (ListDirectoryDetails)
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpPath);
                req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                req.Credentials = new NetworkCredential(ftpUser, ftpPass);
                req.Timeout = 15000;

                using (var resp = (FtpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        // Unix tarzıysa (ilk karakter 'd' => directory)
                        // Windows tarzıysa '<DIR>' içeriyor olabilir
                        bool isFolder = false;
                        string name = line;

                        // Denek 1: Windows format = "01-01-20  12:00PM       <DIR>     FolderName"
                        if (line.ToLower().Contains("<dir>"))
                        {
                            isFolder = true;
                            // name = en son boşluktan sonrası
                            int idx = line.LastIndexOf(' ');
                            name = line.Substring(line.LastIndexOf("<DIR>") + 5).Trim();
                        }
                        else
                        {
                            // Denek 2: Unix format = "drwxr-xr-x  2 owner group 4096 Jan 1 12:00 FolderName"
                            // Unix ise ilk token baş harfi d ise klasör, isim en son token
                            var tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (tokens.Length > 0)
                            {
                                string first = tokens[0];
                                if (first.Length > 0 && first[0] == 'd')
                                {
                                    isFolder = true;
                                }
                                // name = son token (veya son tokenların join'i)
                                int lastSpace = line.LastIndexOf(' ');
                                if (lastSpace > 0)
                                {
                                    name = string.Join(" ", tokens.Skip(8));
                                }
                            }
                        }

                        // fallback: Eğer hala name boşsa try simple ListDirectory (tek satır isimler)
                        if (string.IsNullOrEmpty(name))
                            continue;

                        // Eğer onlyFolders ise dosyaları atla
                        if (onlyFolders && !isFolder) continue;
                        var dosyassayisi = GetFileCount(CombineFtpPath(ftpPath, name), ftpUser, ftpPass);
                        // Oluştur
                        var fi = new FtpItem()
                        {
                            Name = name +$" ({dosyassayisi.ToString()}) ",
                            Path = CombineFtpPath(ftpPath, name),
                            IsFolder = isFolder
                        };
                        list.Add(fi);
                    }
                }
            }
            catch (WebException ex)
            {
                // Eğer ListDirectoryDetails başarısız olduysa (bazı sunucular izin vermez),
                // fallback olarak basit ListDirectory çağrısı yapalım (sadece isimler)
                try
                {
                    list = new List<FtpItem>();
                    FtpWebRequest req2 = (FtpWebRequest)WebRequest.Create(ftpPath);
                    req2.Method = WebRequestMethods.Ftp.ListDirectory;
                    req2.Credentials = new NetworkCredential(ftpUser, ftpPass);
                    using (var resp2 = (FtpWebResponse)req2.GetResponse())
                    using (var sr2 = new StreamReader(resp2.GetResponseStream()))
                    {
                        string name;
                        while ((name = sr2.ReadLine()) != null)
                        {
                            name = name.Trim();
                            if (string.IsNullOrEmpty(name)) continue;
                            var fi = new FtpItem()
                            {
                                Name = name,
                                Path = CombineFtpPath(ftpPath, name),
                                // Fallback: isFolder bilinmiyor, burada isimde '.' varsa dosya kabul et, yoksa klasör kabul et.
                                IsFolder = !name.Contains(".")
                            };
                            if (onlyFolders && !fi.IsFolder) continue;
                            list.Add(fi);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    string hataDetay2 = $"Hata Mesajı: {ex2.Message}\n {Environment.NewLine} Program Adı: {ex2.Source}\n {Environment.NewLine} İşlem: {ex2.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex2.StackTrace}";
                    CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", $"{hataDetay} {Environment.NewLine} {hataDetay2}", this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            return list;
        }

        // ftp adreslerini düzgün birleştirir (çift slash önler)
        private string CombineFtpPath(string baseUrl, string relative)
        {
            if (string.IsNullOrEmpty(relative)) return baseUrl.TrimEnd('/');
            string a = baseUrl.TrimEnd('/');
            string b = relative.TrimStart('/');
            return $"{a}/{b}";
        }

        // server kökünden relative path çıkarır
        private string GetRelativePathFromServer(string fullPath)
        {
            if (fullPath.StartsWith(ftpServer, StringComparison.InvariantCultureIgnoreCase))
                return fullPath.Substring(ftpServer.Length).TrimStart('/');
            return fullPath;
        }
        int? GetFileCount(string folderPath, string ftpUser, string ftpPass)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(folderPath);
                req.Method = WebRequestMethods.Ftp.ListDirectory;
                req.Credentials = new NetworkCredential(ftpUser, ftpPass);
                req.Timeout = 10000;

                using (var resp = (FtpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    var lines = new List<string>();
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            lines.Add(line);
                    }
                    return lines.Count;
                }
            }
            catch
            {
                return null; // Hata varsa boş bırak
            }
        }
        // FtpItem sınıfı
        public class FtpItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public bool IsFolder { get; set; }
        }
        bool IsValidPdf(byte[] data)
        {
            return data.Length > 4 &&
                   data[0] == 0x25 && // %
                   data[1] == 0x50 && // P
                   data[2] == 0x44 && // D
                   data[3] == 0x46;   // F
        }
        public void LoadPdfFromFtp(string uri, string username, string password)
        {
            string extension = Path.GetExtension(uri).ToLower();
            try
            {
                using (WebClient ftpClient = new WebClient())
                {
                    ftpClient.Credentials = new NetworkCredential(username, password);

                    string encodedUri = Uri.EscapeUriString(uri); // Türkçe karakter ve boşlukları encode eder
                    byte[] fileBytes = ftpClient.DownloadData(encodedUri);


                    if (extension == ".pdf")
                    {
                        if (!IsValidPdf(fileBytes))
                        {
                            MessageBox.Show("Geçersiz PDF dosyası.");
                            return;
                        }

                        string tempPdfPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
                        File.WriteAllBytes(tempPdfPath, fileBytes);
                        tempFileName = tempPdfPath;
                        pdfViewer.LoadDocument(tempPdfPath);
                        pdfViewer.Visible = true;
                        pictureEdit.Visible = false;
                    }
                    else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif")
                    {
                        using (var ms = new MemoryStream(fileBytes))
                        {
                            Image img = Image.FromStream(ms);
                            pictureEdit.Image = img;
                        }
                        pictureEdit.Visible = true;
                        pdfViewer.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Desteklenmeyen dosya türü.");
                    }
                }
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("PDF yüklenemedi: ", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        string tempFileName;
        public void PDF(String uri, String username, String password)
        {
            pdfViewer.DocumentFilePath = "";
            WebClient ftpClient = new WebClient();
            ftpClient.Credentials = new NetworkCredential(username, password);
            byte[] imageByte = ftpClient.DownloadData(uri);


            tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            System.IO.File.WriteAllBytes(tempFileName, imageByte);
            pdfViewer.LoadDocument(tempFileName);
        }

        private byte[] DownloadFileFromFtp(string uri)
        {
            // FTP sunucusundan dosyayı indirin
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(uri));
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUser, ftpPass);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (Stream ftpStream = response.GetResponseStream())
                    {
                        ftpStream.CopyTo(stream);
                    }
                    return stream.ToArray();
                }
            }
        }
        //private void ShowPdfAsImage(byte[] pdfBytes)
        //{
        //    using (var stream = new MemoryStream(pdfBytes))
        //    using (var document = PdfDocument.Load(stream))
        //    {
        //        // İlk sayfayı render et
        //        var image = document.Render(0, 300, 300, true); // Sayfa 0, DPI 300

        //        // PictureEdit'e ata
        //        pictureEdit.Image = image;
        //    }
        //}

        private void viewFiles_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //await Entegref.RunWithSplashAsync(this, false, 0, this.Text,
            //   async (progress, token) =>
            //   {
            //       progress.Report((0, $"Dosyalar Yükleniyor"));
            //       await Task.Delay(10, token);
            //       token.ThrowIfCancellationRequested();
            Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.CompanyName, "Yüklü Evraklar Açılıyor");
            try
            {
                var name = viewFiles.GetRowCellValue(e.RowHandle, "Name").ToString();
                var patt = viewFiles.GetRowCellValue(e.RowHandle, "Path").ToString();
                string extension = Path.GetExtension(patt).ToLower();
                if (extension.Contains("pdf"))
                {
                    //PDF(patt,ftpUser,ftpPass);
                    LoadPdfFromFtp(patt, ftpUser, ftpPass);
                    xtraTabControl1.TabPages[0].Text = name;
                    xtraTabControl1.TabPages[0].PageVisible = true;
                    xtraTabControl1.TabPages[1].PageVisible = false;
                }
                else
                {
                    LoadPdfFromFtp(patt, ftpUser, ftpPass);
                    xtraTabControl1.TabPages[1].Text = name;
                    xtraTabControl1.TabPages[0].PageVisible = false;
                    xtraTabControl1.TabPages[1].PageVisible = true;
                }
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                // UI thread'e güvenli dönüş
                this.Enabled = true;
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
            //});
        }
        private async void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            string result = "";
            AiAnalysisService AiAnalysisService = new AiAnalysisService();
            byte[] pdfBytes = File.ReadAllBytes(tempFileName);
            var base64Data = Convert.ToBase64String(pdfBytes);
            await SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
                    async (progress, token) =>
                    {
                        progress.Report((0, $"Yapay Zeka Danışmanına Soruluyor"));
                        await Task.Delay(10, token);
                        token.ThrowIfCancellationRequested();
                        string myPrompt = $@"bana buradaki başlıklar altında kalem ile yazılan alanların tam doğru bir çıktısı lazım. Sadece İstihbaret alanlarını seç ve yap. Başlık bilgisi = yazılmış metin gibi tüm sayfayı oku ve ver. aynı sonucu ek olarak en sonda derleyip SQL veri tabanındaki aşaıdaki örnekteki gibi satıra işleyeceğim bir kod da ver. 
                        CURNTKIND olabilecek veri tipleri
                        0:Genel Not
                        1:Satış Onay Notu
                        2:İstihbarat Notu
                        3:Tahsilat Arş Notu
                        4:Sevkiyat Kurgu Notu
                        5:Sevkiyat Kurgu Notu
                        7:Özel Not
                        8:Uyarı Notu
                        Z: Tahsilat Notu
                        Y: IVR Notu
                        V: SMS Notu
                        B: Borç Takip
                        C: Satış Onay Sevkiyat Notu
                        T: Tahsilat Notu
                        A: Yapay Zeka Notu
                        default:Diğer                    
                        CURNTID	CURNTCURID	CURNTNOTES	CURNTSOCODE	CURNTDATETIME	CURNTKIND	CURNTORDCHORSALID 
                        474 3   FATNO: 23642 923 SEMA    2007 - 04 - 25 09:29:57.013 9   NULL
buna göre CURNTID = = {1},
CURNTCURID = {CURID}, 
CURNTSOCODE = {Entegref.GetLogins.userID} alanlarını da bu veriler ile oluştur.";
                        string myPrompt2 = $@"bana buradaki başlıklar altında kalem ile yazılan alanların tam doğru bir çıktısı lazım. Sadece İstihbaret alanlarını seç ve yap. Başlık bilgisi = yazılmış metin gibi tüm sayfayı oku ve ver.";
                        
                        result = await AiAnalysisService.AnalyzePdfWithGemini(myPrompt2, base64Data);

                    });

        }
    }
}