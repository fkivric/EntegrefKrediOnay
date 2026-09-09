using DevExpress.XtraSplashScreen;
using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using EntegreFDLL.Class;
using EntegreFDLL;
//using Volant.Ocr.Dev.Extensions;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class TwainScanner
    {
        private const string LbpCascade = "C:\\Program Files(x86)\\Volant Yazılım\\Volant Erp Setup\\OpenCV\\data\\lbpcascade_frontalface.xml";

        private ImageCodecInfo _jpegCodecInfo;

        private TwainSession _twain;

        private KtsService sr = new KtsService();

        private CascadeClassifier lbpCascade = null;

        private Enums.ScanObject scanObject;

        private string documentName = null;

        public bool isFinished = false;

        public bool isCompleted = false;

        public eIdCard rIdCard;

        public eCusIdentity rCusIdentity;

        public List<Image> scannedImages = null;
        SqlConnectionObject conn = new SqlConnectionObject();

        public Image Head { get; set; }

        public Image Identity { get; set; }
        private void SetupTwain()
        {
            PlatformInfo.Current.PreferNewDSM = false;
            TWIdentity appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetEntryAssembly());
            _twain = new TwainSession(appId);
            _twain.DataTransferred += _twain_DataTransferred;
        }

        public TwainScanner()
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
            lbpCascade = new CascadeClassifier("C:\\Program Files (x86)\\Volant Yazılım\\Volant Erp Setup\\OpenCV\\data\\lbpcascade_frontalface.xml");
        }

        private void _twain_DataTransferred(object sender, DataTransferredEventArgs e)
        {
            IEnumerable<TWInfo> infos = from it in e.GetExtImageInfo(ExtendedImageInfo.Camera)
                                        where it.ReturnCode == ReturnCode.Success
                                        select it;
            using (IEnumerator<TWInfo> enumerator = infos.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    TWInfo it2 = enumerator.Current;
                    IList<object> values = it2.ReadValues();
                    PlatformInfo.Current.Log.Info($"{it2.InfoID} = {values.FirstOrDefault()}");
                }
            }
            if (e.NativeData != IntPtr.Zero)
            {
                Stream stream = e.GetNativeImageStream();
                if (stream != null)
                {
                    TransferImage(stream);
                }
            }
        }

        private void TransferImage(Stream stream)
        {
            if (scannedImages == null)
            {
                scannedImages = new List<Image>();
            }
            Bitmap bitmap = new Bitmap(stream);
            scannedImages.Add(DetectIdentityCard(bitmap));
        }

        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    if (_twain != null)
        //    {
        //        if (e.CloseReason == CloseReason.UserClosing && _twain.State > 4)
        //        {
        //            e.Cancel = true;
        //        }
        //        else
        //        {
        //            CleanupTwain();
        //        }
        //    }
        //    base.OnFormClosing(e);
        //}

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

        public async void ScanIdentity(string printerName, Enums.ScanObject scanObject)
        {
            try
            {
                SetupTwain();
                _twain.Open();
                this.scanObject = scanObject;
                foreach (DataSource src in _twain)
                {
                    if (!(src.Name == printerName) || src.Open() != 0)
                    {
                        continue;
                    }
                    LoadSourceCaps();
                    src.Enable(SourceEnableMode.ShowUI, false, IntPtr.Zero);
                    if (_twain.State == 4)
                    {
                        if (_twain.CurrentSource.Capabilities.CapUIControllable.IsSupported && _twain.CurrentSource.Enable(SourceEnableMode.NoUI, false, IntPtr.Zero) == ReturnCode.Success)
                        {

                        }
                    }
                    else
                    {
                        //MessageBox.Show("Tarayıcıya erişilemiyor");
                    }
                }
                while (_twain.IsTransferring)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                }
                _twain_ScanningComplete();
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
                    src.Capabilities.CapDuplexEnabled.SetValue(BoolType.True);
                }
                if (src.Capabilities.CapClearBuffers.IsSupported)
                {
                    src.Capabilities.CapClearBuffers.SetValue(ClearBuffer.Auto);
                }
                if (src.Capabilities.ICapBitDepth.IsSupported)
                {
                    var maxBit = src.Capabilities.ICapBitDepth.GetValues().Cast<int>().Max();
                    src.Capabilities.ICapBitDepth.SetValue(maxBit);
                }
                // 🔴 KRİTİK EKSİK OLAN
                if (src.Capabilities.CapDuplex.IsSupported)
                {
                    var duplex = src.Capabilities.CapDuplex.GetCurrent();
                    
                }
                src.Capabilities.ICapXResolution.SetValue(400F);
                src.Capabilities.ICapYResolution.SetValue(400F);
            }
        }

        public void _twain_ScanningComplete()
        {
            try
            {
                if (scannedImages != null)
                {
                    var dt = conn.GetData("select * from TERMINAL where  TERSTS = 1", Properties.Settings.Default.connectionstring);
                    TERMINAL rTerminal = dt.AsEnumerable()
                    .Select(row => new TERMINAL
                    {
                        TERID = row.Field<long>("TERID"),
                        TERCOMPANY = row.Field<string>("TERCOMPANY"),
                        TERDIVISON = row.Field<string>("TERDIVISON"),
                        TERNO = row.Field<short>("TERNO"),
                        TERNOTE = row.Field<string>("TERNOTE"),
                        TERCOM = row.Field<string>("TERCOM"),
                        TERINVOICESERIAL = row.Field<string>("TERINVOICESERIAL"),
                        TERIRSSERIAL = row.Field<string>("TERIRSSERIAL"),
                        TEREXPENSEBILLSERIAL = row.Field<string>("TEREXPENSEBILLSERIAL"),
                        TERCONTRACT = row.Field<string>("TERCONTRACT"),
                        TERRECEIPT = row.Field<string>("TERRECEIPT"),
                        TERINVOICE = row.Field<string>("TERINVOICE"),
                        TERSTS = row.Field<bool>("TERSTS"),
                        TERSCANNER = row.Field<string>("TERSCANNER"),
                        TERSCANNERSIDE = row.Field<bool?>("TERSCANNERSIDE"),
                        TERPOTENCY = row.Field<bool>("TERPOTENCY"),
                        TEROKCKIND = row.Field<short?>("TEROKCKIND"),
                        TEROKCCOM = row.Field<string>("TEROKCCOM"),
                        TEROKCATSERI = row.Field<string>("TEROKCATSERI"),
                        TERBIODEVICEID = row.Field<string>("TERBIODEVICEID"),
                    })
                    .FirstOrDefault();
                    if (rTerminal != null && scannedImages.Count >= 1)
                    {
                        string lastFilePath = "";
                        Bitmap kimlik = CombineBitmap(scannedImages);
                        if (kimlik == null)
                        {
                            throw new Exception("Kimlik taramasında problem oluştu tekrar taratınız!");
                        }
                        Identity = ResizeImage(kimlik.Clone() as Bitmap, 800, 350);
                        lastFilePath = Application.StartupPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".jpeg";
                        documentName = lastFilePath;
                        if (scanObject == Enums.ScanObject.YeniTcKimlik)
                        {
                            Bitmap bp = (Bitmap)scannedImages[0];
                            bp = bp.Clone(new Rectangle(0, 0, scannedImages[0].Width / 2, scannedImages[0].Height), scannedImages[0].PixelFormat);
                            bp.Save(documentName, ImageFormat.Jpeg);
                        }
                        else
                        {
                            scannedImages[0].Save(documentName, ImageFormat.Jpeg);
                        }
                        if (File.Exists(documentName))
                        {
                            DetectFace();
                            File.Delete(documentName);
                        }
                        if (scanObject == Enums.ScanObject.YeniTcKimlik || scanObject == Enums.ScanObject.YeniEhliyet)
                        {
                            kimlik = ResizeImage(kimlik, 800, 350);
                        }
                        else
                        {
                            kimlik = ResizeImage(kimlik, 800, 480);
                        }
                        scannedImages.Clear();
                        isCompleted = true;
                    }
                }
                isFinished = true;
            }
            catch (Exception exp)
            {
                scannedImages = null;
                isFinished = true;
                isCompleted = true;
                MessageBox.Show(exp.Message);
            }
        }
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);
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
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        private Bitmap GetBitMap(Mat inputImage)
        {
            return inputImage.ToBitmap();
        }

        public Bitmap cropAtRect(Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        private Bitmap CombineBitmap(List<Image> images)
        {
            SplashScreenManager.ShowForm(null, typeof(SplashScreen), useFadeIn: true, useFadeOut: true, throwExceptionIfAlreadyOpened: false);
            Bitmap finalImage = null;
            try
            {
                int width = 0;
                int height = 0;
                if ( 1 == 1 )
                {
                    try
                    {
                        if (images != null && images.Count >= 2)
                        {
                            rIdCard = new eIdCard();
                            rCusIdentity = new eCusIdentity();
                            gOcr ocr = new gOcr();
                            Bitmap bmp = (Bitmap)images[0].Clone();
                            
                            ocr.rKimlikClear();
                            rIdCard = ocr.StartOCRFrontErp((Bitmap)images[0].Clone(), scanObject);
                            rIdCard = ocr.StartOCRBackErp((Bitmap)images[1].Clone(), scanObject);
                            if (rIdCard != null)
                            {
                                rCusIdentity = MoveDataIdCart(rCusIdentity, rIdCard);
                                rCusIdentity.orjRecord = rCusIdentity.Clone() as eCusIdentity;
                                try
                                {
                                    string soyad = "";
                                    string usrsoyad = "";
                                    string dogumyeri = "";
                                    string usrdogumyeri = "";
                                    if (rCusIdentity.CUSIDTCNO.HasValue)
                                    {
                                        KTS usr = sr.GetByID(rCusIdentity.CUSIDTCNO.ToString());
                                        if (rCusIdentity.CUSIDSIRNAME != null && rCusIdentity.CUSIDSIRNAME != "")
                                        {
                                            soyad = SesliSil(rCusIdentity.CUSIDSIRNAME.ToUpper());
                                        }
                                        if (rCusIdentity.CUSIDBIRTHPLACE != null && rCusIdentity.CUSIDBIRTHPLACE != "")
                                        {
                                            dogumyeri = SesliSil(rCusIdentity.CUSIDBIRTHPLACE.ToUpper());
                                        }
                                        if (usr != null)
                                        {
                                            usrsoyad = SesliSil(usr.SOYAD.ToUpper());
                                            usrdogumyeri = SesliSil(usr.DOGUMYER.ToUpper());
                                            rCusIdentity.CUSIDNAME = usr.AD;
                                            if (soyad == usrsoyad)
                                            {
                                                rCusIdentity.CUSIDSIRNAME = usr.SOYAD;
                                            }
                                            if (scanObject != Enums.ScanObject.YeniEhliyet)
                                            {
                                                rCusIdentity.CUSIDMOTHER = usr.ANNEAD;
                                                rCusIdentity.CUSIDFATHER = usr.BABAAD;
                                            }
                                            if (dogumyeri == usrdogumyeri)
                                            {
                                                rCusIdentity.CUSIDBIRTHPLACE = usr.DOGUMYER;
                                            }
                                            if (!string.IsNullOrEmpty(usr.DOGUMTAH))
                                            {
                                                string[] dt = usr.DOGUMTAH.Split('/');
                                                if (dt.Length >= 1)
                                                {
                                                    string gun = "";
                                                    string ay = "";
                                                    gun = ((dt[1].Length != 1) ? dt[1] : ("0" + dt[1]));
                                                    DateTime birthDate = new DateTime(month: Convert.ToInt32((dt[0].Length != 1) ? dt[0] : ("0" + dt[0])), year: Convert.ToInt32(dt[2].Substring(0, 4)), day: Convert.ToInt32(gun));
                                                    rCusIdentity.CUSIDBIRTHDAY = birthDate;
                                                }
                                            }
                                        }
                                    }
                                    YandexTranslate trnslt = new YandexTranslate();
                                    if (rCusIdentity.CUSIDNAME != null && rCusIdentity.CUSIDNAME != "")
                                    {
                                        rCusIdentity.CUSIDNAME = trnslt.GetTranslate(rCusIdentity.CUSIDNAME);
                                        if (rCusIdentity.CUSIDNAME == null)
                                        {
                                            rCusIdentity.CUSIDNAME = "";
                                        }
                                    }
                                    if (rCusIdentity.CUSIDBIRTHPLACE != null && rCusIdentity.CUSIDBIRTHPLACE != "")
                                    {
                                        rCusIdentity.CUSIDBIRTHPLACE = trnslt.GetTranslate(rCusIdentity.CUSIDBIRTHPLACE);
                                        if (rCusIdentity.CUSIDBIRTHPLACE == null)
                                        {
                                            rCusIdentity.CUSIDBIRTHPLACE = "";
                                        }
                                    }
                                    if (rCusIdentity.CUSIDREGCITY != null && rCusIdentity.CUSIDREGCITY != "")
                                    {
                                        rCusIdentity.CUSIDREGCITY = trnslt.GetTranslate(rCusIdentity.CUSIDREGCITY);
                                        if (rCusIdentity.CUSIDREGCITY == null)
                                        {
                                            rCusIdentity.CUSIDREGCITY = "";
                                        }
                                    }
                                    if (rCusIdentity.CUSIDREGCOUNTY != null && rCusIdentity.CUSIDREGCOUNTY != "")
                                    {
                                        rCusIdentity.CUSIDREGCOUNTY = trnslt.GetTranslate(rCusIdentity.CUSIDREGCOUNTY);
                                        if (rCusIdentity.CUSIDREGCOUNTY == null)
                                        {
                                            rCusIdentity.CUSIDREGCOUNTY = "";
                                        }
                                    }
                                    if (rCusIdentity.CUSIDGIVENPLACE != null && rCusIdentity.CUSIDGIVENPLACE != "")
                                    {
                                        rCusIdentity.CUSIDGIVENPLACE = trnslt.GetTranslate(rCusIdentity.CUSIDGIVENPLACE);
                                        if (rCusIdentity.CUSIDGIVENPLACE == null)
                                        {
                                            rCusIdentity.CUSIDGIVENPLACE = "";
                                        }
                                    }
                                    if (rCusIdentity.CUSIDMOTHER != null && rCusIdentity.CUSIDMOTHER != "")
                                    {
                                        rCusIdentity.CUSIDMOTHER = trnslt.GetTranslate(rCusIdentity.CUSIDMOTHER);
                                        if (rCusIdentity.CUSIDMOTHER == null)
                                        {
                                            rCusIdentity.CUSIDMOTHER = "";
                                        }
                                    }
                                    if (rCusIdentity.CUSIDFATHER != null && rCusIdentity.CUSIDFATHER != "")
                                    {
                                        rCusIdentity.CUSIDFATHER = trnslt.GetTranslate(rCusIdentity.CUSIDFATHER);
                                        if (rCusIdentity.CUSIDFATHER == null)
                                        {
                                            rCusIdentity.CUSIDFATHER = "";
                                        }
                                    }
                                }
                                catch
                                {
                                    rCusIdentity = rCusIdentity.orjRecord;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                for (int i = 0; i < images.Count; i++)
                {
                    if (scanObject == Enums.ScanObject.YeniEhliyet || scanObject == Enums.ScanObject.YeniTcKimlik)
                    {
                        images[i] = ResizeImage(images[i], 550, 350);
                    }
                    else
                    {
                        images[i] = ResizeImage(images[i], 550, 350);
                    }
                }
                foreach (Image item in images)
                {
                    width += item.Width;
                    height = ((item.Height > height) ? item.Height : height);
                }
                finalImage = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    g.Clear(Color.White);
                    int offset = 0;
                    foreach (Bitmap image in images)
                    {
                        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }
                SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false);
                return finalImage;
            }
            catch
            {
                SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false);
                return null;
            }
        }

        public string SesliSil(string str)
        {
            str = str.Replace('A', ' ');
            str = str.Replace('E', ' ');
            str = str.Replace('I', ' ');
            str = str.Replace('İ', ' ');
            str = str.Replace('O', ' ');
            str = str.Replace('Ö', ' ');
            str = str.Replace('U', ' ');
            str = str.Replace('Ü', ' ');
            return str;
        }

        public eCusIdentity MoveDataIdCart(eCusIdentity rCusIdentity, eIdCard rIdCard)
        {
            if (rCusIdentity == null || rIdCard == null)
            {
                return rCusIdentity;
            }
            if (!string.IsNullOrEmpty(rIdCard.ADI))
            {
                rCusIdentity.CUSIDNAME = rIdCard.ADI;
            }
            if (!string.IsNullOrEmpty(rIdCard.SOYADI))
            {
                rCusIdentity.CUSIDSIRNAME = rIdCard.SOYADI;
            }
            if (!string.IsNullOrEmpty(rIdCard.TCKIMLIKNO) && (rCusIdentity.TCConfirmed == false || !rCusIdentity.TCConfirmed.HasValue))
            {
                rCusIdentity.CUSIDTCNO = Convert.ToInt64(Regex.Replace(rIdCard.TCKIMLIKNO, "[^0-9]", ""));
            }
            if (!string.IsNullOrEmpty(rIdCard.VERILDIGIYER))
            {
                rCusIdentity.CUSIDGIVENPLACE = rIdCard.VERILDIGIYER;
            }
            if (!string.IsNullOrEmpty(rIdCard.VERILISNEDENI))
            {
                rCusIdentity.CUSIDGIVENREASON = rIdCard.VERILISNEDENI;
            }
            if (!string.IsNullOrEmpty(rIdCard.VERILISTARIHI))
            {
                rIdCard.VERILISTARIHI = rIdCard.VERILISTARIHI.Replace('"', ' ');
                rIdCard.VERILISTARIHI = rIdCard.VERILISTARIHI.Replace('/', ' ');
                rIdCard.VERILISTARIHI = rIdCard.VERILISTARIHI.Replace(" ", string.Empty);
                if (rIdCard.VERILISTARIHI.Length == 10)
                {
                    rCusIdentity.CUSIDGIVENDATE = Convert.ToDateTime(rIdCard.VERILISTARIHI);
                }
            }
            if (!string.IsNullOrEmpty(rIdCard.SIRANO))
            {
                rCusIdentity.CUSIDSORTNO = rIdCard.SIRANO;
            }
            if (!string.IsNullOrEmpty(rIdCard.SERI))
            {
                rCusIdentity.CUSIDSERIALVAL = rIdCard.SERI;
            }
            if (!string.IsNullOrEmpty(rIdCard.MEDENIHAL))
            {
                rCusIdentity.CUSIDMARRIED = rIdCard.MEDENIHAL;
            }
            if (!string.IsNullOrEmpty(rIdCard.MAHALLEKOY))
            {
                rCusIdentity.CUSIDREGREGION = rIdCard.MAHALLEKOY;
            }
            if (!string.IsNullOrEmpty(rIdCard.KAYITNO))
            {
                rCusIdentity.CUSIDREGNO = rIdCard.KAYITNO;
            }
            if (!string.IsNullOrEmpty(rIdCard.NO))
            {
                rCusIdentity.CUSIDSERIALNO = rIdCard.NO;
            }
            if (!string.IsNullOrEmpty(rIdCard.ILCE))
            {
                rCusIdentity.CUSIDREGCOUNTY = rIdCard.ILCE;
            }
            if (!string.IsNullOrEmpty(rIdCard.IL))
            {
                rCusIdentity.CUSIDREGCITY = rIdCard.IL;
            }
            if (!string.IsNullOrEmpty(rIdCard.DOGUMYERI))
            {
                rCusIdentity.CUSIDBIRTHPLACE = rIdCard.DOGUMYERI;
            }
            if (!string.IsNullOrEmpty(rIdCard.DOGUMTARIHI))
            {
                rIdCard.DOGUMTARIHI = rIdCard.DOGUMTARIHI.Replace('"', ' ');
                rIdCard.DOGUMTARIHI = rIdCard.DOGUMTARIHI.Replace('/', ' ');
                rIdCard.DOGUMTARIHI = rIdCard.DOGUMTARIHI.Replace(" ", string.Empty);
                if (rIdCard.DOGUMTARIHI.Length == 10)
                {
                    rCusIdentity.CUSIDBIRTHDAY = Convert.ToDateTime(rIdCard.DOGUMTARIHI);
                }
            }
            if (!string.IsNullOrEmpty(rIdCard.CILTNO))
            {
                rCusIdentity.CUSIDVOLNO = rIdCard.CILTNO;
            }
            if (!string.IsNullOrEmpty(rIdCard.AILESIRANO))
            {
                rCusIdentity.CUSIDFAMILYNO = rIdCard.AILESIRANO;
            }
            if (!string.IsNullOrEmpty(rIdCard.ANAADI))
            {
                rCusIdentity.CUSIDMOTHER = rIdCard.ANAADI;
            }
            if (!string.IsNullOrEmpty(rIdCard.BABAADI))
            {
                rCusIdentity.CUSIDFATHER = rIdCard.BABAADI;
            }
            return rCusIdentity;
        }

        private Mat DetectFace()
        {
            CascadeClassifier cascade = lbpCascade;
            Mat result;
            using (Mat src = new Mat(documentName))
            {
                using (Mat gray = new Mat())
                {
                    result = src.Clone();
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                    Rect[] faces = cascade.DetectMultiScale(gray, 1.07, 13, HaarDetectionType.FindBiggestObject, new OpenCvSharp.Size(30, 30));
                    Rect[] array = faces;
                    for (int i = 0; i < array.Length; i++)
                    {
                        Rect face = array[i];
                        OpenCvSharp.Point point = default(OpenCvSharp.Point);
                        point.X = (int)((double)face.X + (double)face.Width * 0.5);
                        point.Y = (int)((double)face.Y + (double)face.Height * 0.5);
                        OpenCvSharp.Point center = point;
                        OpenCvSharp.Size size = default(OpenCvSharp.Size);
                        size.Width = (int)((double)face.Width * 0.7);
                        size.Height = (int)((double)face.Height * 0.9);
                        OpenCvSharp.Size axes = size;
                        Bitmap head = GetBitMap(result);
                        head = cropAtRect(head, new Rectangle(center.X - axes.Width + 15, center.Y - axes.Height + 20, axes.Width * 2, axes.Height * 2 - 55));
                        Head = head;
                    }
                }
            }
            return result;
        }

        private Bitmap DetectIdentityCard(Bitmap bitmap)
        {
            byte[] bytes = bitmap.imageToByteArray();
            Mat image = Mat.FromImageData(bytes, ImreadModes.AnyColor);
            Mat gray = new Mat();
            int channels = image.Channels();
            if (channels > 1)
            {
                Cv2.CvtColor(image, gray, ColorConversionCodes.BGRA2GRAY);
            }
            else
            {
                image.CopyTo(gray);
            }
            Mat gradX = new Mat();
            Cv2.Sobel(gray, gradX, 5, 1, 0, -1);
            Mat gradY = new Mat();
            Cv2.Sobel(gray, gradY, 5, 0, 1, -1);
            Mat gradient = new Mat();
            Cv2.Subtract(gradX, gradY, gradient);
            Cv2.ConvertScaleAbs(gradient, gradient);
            Mat blurred = new Mat();
            Cv2.Blur(gradient, blurred, new OpenCvSharp.Size(9, 9));
            Mat threshImage = new Mat();
            Cv2.Threshold(blurred, threshImage, 75.0, 255.0, ThresholdTypes.Binary);
            Mat closed = new Mat();
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(21, 7));
            Cv2.MorphologyEx(threshImage, closed, MorphTypes.Close, kernel);
            Cv2.Erode(closed, closed, null, null, 4);
            Cv2.Dilate(closed, closed, null, null, 4);
            Cv2.FindContours(closed, out var contours, out var hierarchyIndexes, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                throw new NotSupportedException("Couldn't find any object in the image.");
            }
            int contourIndex = 0;
            int previousArea = 0;
            Rect biggestContourRect = Cv2.BoundingRect(contours[0]);
            while (contourIndex >= 0)
            {
                OpenCvSharp.Point[] contour = contours[contourIndex];
                Rect boundingRect = Cv2.BoundingRect(contour);
                int boundingRectArea = boundingRect.Width * boundingRect.Height;
                if (boundingRectArea > previousArea)
                {
                    biggestContourRect = boundingRect;
                    previousArea = boundingRectArea;
                }
                contourIndex = hierarchyIndexes[contourIndex].Next;
            }
            return cropAtRect(GetBitMap(image), new Rectangle(biggestContourRect.X, biggestContourRect.Y, biggestContourRect.Width, biggestContourRect.Height));
        }
    }
}
