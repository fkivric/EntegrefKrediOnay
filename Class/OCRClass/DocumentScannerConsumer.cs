using DevExpress.XtraSplashScreen;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using FluentFTP;
using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.Aztec.Internal;
using ZXing.Common;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class DocumentScannerConsumer : DocumentCreationForm
    {
        private ImageCodecInfo _jpegCodecInfo;

        private TwainSession _twain;

        private bool disposed;

        private List<eBarcodeInformation> lBarcodeInformation = new List<eBarcodeInformation>();

        private List<Bitmap> images = new List<Bitmap>();

        public bool isFinished = false;

        private string path = "";

        private FtpClient _ftpClient = null;

        public List<eBarcodeInformation> successfullyScannedDocuments = new List<eBarcodeInformation>();

        public List<eBarcodeInformation> unknownDocuments = new List<eBarcodeInformation>();

        public string OperationInformation = "";

        private bool forProduct = false;

        private DocumentScannerConsumer()
        {
        }

        private void SetupTwain()
        {
            PlatformInfo.Current.PreferNewDSM = false;
            TWIdentity appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetEntryAssembly());
            _twain = new TwainSession(appId);
            _twain.DataTransferred += _twain_DataTransferred;
        }

        private void _twain_DataTransferred(object sender, DataTransferredEventArgs e)
        {
            IEnumerable<TWInfo> infos = from it in e.GetExtImageInfo(ExtendedImageInfo.Camera)
                                        where it.ReturnCode == ReturnCode.Success
                                        select it;
            if (e.NativeData != IntPtr.Zero)
            {
                Stream stream = e.GetNativeImageStream();
                if (stream != null)
                {
                    TransferImage(stream);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_twain != null)
            {
                if (e.CloseReason == CloseReason.UserClosing && _twain.State > 4)
                {
                    e.Cancel = true;
                }
                else
                {
                    CleanupTwain();
                }
            }
            base.OnFormClosing(e);
        }

        private void CleanupTwain()
        {
            if (_twain.State == 4)
            {
                _twain.CurrentSource.Close();
            }
            if (_twain.State == 3)
            {
                _twain.Close();
            }
            if (_twain.State >= 2)
            {
                _twain.ForceStepDown(2);
            }
        }

        public DocumentScannerConsumer(FtpClient ftpClient, string path, bool forProduct = false)
        {
            ImageCodecInfo[] ımageEncoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo enc in ımageEncoders)
            {
                if (enc.MimeType == "image/jpeg")
                {
                    _jpegCodecInfo = enc;
                    break;
                }
            }
            _ftpClient = ftpClient;
            this.path = path;
            this.forProduct = forProduct;
        }

        public void DocumentCreateCustomer()
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    _ftpClient.UploadRateLimit = 0u;
                    _ftpClient.EnableThreadSafeDataConnections = true;
                    _ftpClient.ConnectTimeout = 60000;
                    _ftpClient.DataConnectionConnectTimeout = 60000;
                    _ftpClient.SocketPollInterval = 0;
                    _ftpClient.Connect();
                    _ftpClient.BeginCreateDirectory(path, force: true, callback: null, state: null);
                    _ftpClient.Connect();
                }
            }
            catch (FtpException fe)
            {
                MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                _ftpClient.Disconnect();
            }
        }

        public void DocumentCreateCustomer(string newParh)
        {
            try
            {
                _ftpClient.UploadRateLimit = 0u;
                _ftpClient.EnableThreadSafeDataConnections = true;
                _ftpClient.ConnectTimeout = 60000;
                _ftpClient.DataConnectionConnectTimeout = 60000;
                _ftpClient.SocketPollInterval = 0;
                _ftpClient.Connect();
                _ftpClient.BeginCreateDirectory(newParh, force : true, callback : null, state: null);
            }
            catch (FtpException fe)
            {
                MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                _ftpClient.Disconnect();
            }
        }

        public void ScanDocument(string scannerName)
        {
            try
            {
                SetupTwain();
                _twain.Open();
                foreach (DataSource src in _twain)
                {
                    if (!(src.Name == scannerName))
                    {
                        continue;
                    }
                    if (src.Open() == ReturnCode.Success)
                    {
                        LoadSourceCaps();
                        if (_twain.State == 4)
                        {
                            if (_twain.CurrentSource.Capabilities.CapUIControllable.IsSupported && _twain.CurrentSource.Enable(SourceEnableMode.NoUI, modal: false, windowHandle : this.Handle) == ReturnCode.Success)
                            {
                                while (_twain.IsTransferring)
                                {
                                    Thread.Sleep(1);
                                    Application.DoEvents();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Tarayıcıya erişilemiyor");
                        }
                        continue;
                    }
                    throw new Exception("Özel Durum Oluştu");
                }
                while (_twain.IsTransferring)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                }
                if (lBarcodeInformation != null && lBarcodeInformation.Count > 0)
                {
                    Twain_ScanningComplete();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CleanupTwain();
            }
        }

        private void LoadSourceCaps()
        {
            DataSource src = _twain.CurrentSource;
            if (src != null)
            {
                if (src.Capabilities.ICapPixelType.IsSupported)
                {
                    src.Capabilities.ICapPixelType.SetValue(PixelType.RGB);
                }
                if (src.Capabilities.CapDuplexEnabled.IsSupported)
                {
                    src.Capabilities.CapDuplexEnabled.SetValue(BoolType.False);
                }
                if (src.Capabilities.CapClearBuffers.IsSupported)
                {
                    src.Capabilities.CapClearBuffers.SetValue(ClearBuffer.Auto);
                }
                if (src.Capabilities.CapDuplex.IsSupported)
                {
                }
                src.Capabilities.ICapXResolution.SetValue(300f);
                src.Capabilities.ICapYResolution.SetValue(300f);
            }
        }

        private void Twain_ScanningComplete()
        {
            SplashScreenManager.ShowForm(null, typeof(SplashScreen), useFadeIn: true, useFadeOut: true, throwExceptionIfAlreadyOpened: false);
            _ftpClient.UploadRateLimit = 0u;
            _ftpClient.EnableThreadSafeDataConnections = true;
            _ftpClient.ConnectTimeout = 60000;
            _ftpClient.DataConnectionConnectTimeout = 60000;
            _ftpClient.SocketPollInterval = 0;
            _ftpClient.Connect();
            try
            {
                List<eBarcodeInformation> distinctList = new List<eBarcodeInformation>();
                foreach (eBarcodeInformation item in lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType != null))
                {
                    if (distinctList.FirstOrDefault((eBarcodeInformation x) => x.DocumentID == item.DocumentID && x.DocumentType == item.DocumentType) == null)
                    {
                        distinctList.Add(item);
                    }
                }
                foreach (eBarcodeInformation item2 in lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentID == null))
                {
                    distinctList.Add(item2);
                }
                foreach (eBarcodeInformation item3 in distinctList)
                {
                    string date = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond;
                    PdfDocument pdf = new PdfDocument();
                    try
                    {
                        PdfPage newPage = new PdfPage();
                        pdf.Pages.Add(newPage);
                        newPage.Size = PageSize.A4;
                        XGraphics xgr = XGraphics.FromPdfPage(pdf.Pages[pdf.PageCount - 1]);
                        int newWidth = 768;
                        int newHeight = 1024;
                        Bitmap bitmap = ResizeImage(item3.Image, newWidth, newHeight);
                        if (bitmap != null)
                        {
                            XImage img = XImage.FromGdiPlusImage(bitmap);
                            newPage.Width = XUnit.FromPoint(img.Size.Width);
                            newPage.Height = XUnit.FromPoint(img.Size.Height);
                            xgr.DrawImage(bitmap, 0.0, 0.0, img.Size.Width, img.Size.Height);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    using (MemoryStream fileStream = new MemoryStream())
                    {
                        pdf.Save(fileStream, closeStream: false);
                        DocumentCreateCustomer(path);
                        _ftpClient.Disconnect();
                        _ftpClient.Connect();
                        _ftpClient.Upload(fileStream, path + "Belge " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        item3.DocumentType = "";
                        successfullyScannedDocuments.Add(item3);
                        unknownDocuments.Add(item3);
                        pdf.Close();
                    }
                }
            }
            catch (FtpException fe)
            {
               MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + exp.Message);
            }
            finally
            {
                _ftpClient.Disconnect();
                isFinished = true;
                SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false);
            }
        }

        public string Detect(Bitmap bitmap)
        {
            try
            {
                LuminanceSource source = new BitmapLuminanceSource(bitmap);
                HybridBinarizer binarizer = new HybridBinarizer(source);
                BinaryBitmap binBitmap = new BinaryBitmap(binarizer);
                BitMatrix bm = binBitmap.BlackMatrix;
                Detector detector = new Detector(bm);
                DetectorResult result = detector.detect();
                string retStr = "Found at points ";
                ResultPoint[] points = result.Points;
                foreach (ResultPoint point in points)
                {
                    retStr = retStr + point.ToString() + ", ";
                }
                return retStr;
            }
            catch
            {
                return "Failed to detect QR code.";
            }
        }

        public Bitmap GrayScale(Bitmap Bmp)
        {
            for (int y = 0; y < Bmp.Height; y++)
            {
                for (int x = 0; x < Bmp.Width; x++)
                {
                    Color c = Bmp.GetPixel(x, y);
                    int rgb = (c.R + c.G + c.B) / 3;
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
            return Bmp;
        }

        private void TransferImage(Stream stream)
        {
            using (Bitmap bitmap = new Bitmap(stream))
            {
                if (!images.Contains(bitmap))
                {
                    lBarcodeInformation.Add(new eBarcodeInformation
                    {
                        Image = (bitmap.Clone() as Bitmap)
                    });
                    images.Add(bitmap);
                }
            }
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);
            try
            {
                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                using (Graphics graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    using (ImageAttributes wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image.Clone() as Bitmap, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
            }
            catch
            {
                return null;
            }
            return destImage;
        }

        private Bitmap GetBitMap(Mat inputImage)
        {
            return inputImage.ToBitmap();
            //OpenCvSharp.Size size = inputImage.Size();
            //using (Bitmap bitmap = inputImage.ToBitmap())
            //{
            //    return bitmap;
            //}
        }

        public Bitmap cropAtRect(Bitmap b, Rectangle r)
        {
            using (Bitmap nb = new Bitmap(r.Width, r.Height))
            {
                using (Graphics g = Graphics.FromImage(nb))
                {
                    g.DrawImage(b, -r.X, -r.Y);
                    return nb;
                }
            }
        }

        public new void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected new virtual void Dispose(bool disposing)
        {
            if (disposed || disposing)
            {
            }
            disposed = true;
        }

        ~DocumentScannerConsumer()
        {
            Dispose(disposing: false);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.ClientSize = new System.Drawing.Size(284, 261);
            base.Name = "DocumentScannerConsumer";
            base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(DocumentScanner_FormClosed);
            base.ResumeLayout(false);
        }

        private void DocumentScanner_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        public void ScanPdf(string path, string ftpPath)
        {
            SplashScreenManager.ShowForm(null, typeof(SplashScreen), useFadeIn: true, useFadeOut: true, throwExceptionIfAlreadyOpened: false);
            try
            {
                _ftpClient.UploadRateLimit = 0u;
                _ftpClient.EnableThreadSafeDataConnections = true;
                _ftpClient.ConnectTimeout = 60000;
                _ftpClient.DataConnectionConnectTimeout = 60000;
                _ftpClient.SocketPollInterval = 0;
                _ftpClient.Connect();
                int index = path.LastIndexOf("\\") + 1;
                _ftpClient.UploadFile(path, ftpPath + path.Substring(index, path.Length - index), FtpExists.Overwrite, createRemoteDir: true);
                _ftpClient.Disconnect();
            }
            catch (FtpException fe)
            {
                MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + exp.Message);
            }
            finally
            {
                SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false);
            }
        }

        public void UploadImage(string path, string folderName)
        {
            SplashScreenManager.ShowForm(null, typeof(SplashScreen), useFadeIn: true, useFadeOut: true, throwExceptionIfAlreadyOpened: false);
            try
            {
                _ftpClient.UploadRateLimit = 0u;
                _ftpClient.EnableThreadSafeDataConnections = true;
                _ftpClient.ConnectTimeout = 60000;
                _ftpClient.DataConnectionConnectTimeout = 60000;
                _ftpClient.SocketPollInterval = 0;
                _ftpClient.Connect();
                int index = path.LastIndexOf("\\") + 1;
                _ftpClient.UploadFile(path, "/" + folderName + "/" + path.Substring(index, path.Length - index), FtpExists.Overwrite, createRemoteDir: true);
                _ftpClient.Disconnect();
            }
            catch (FtpException fe)
            {
                MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + exp.Message);
            }
            finally
            {
                SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false);
            }
        }
    }
}
