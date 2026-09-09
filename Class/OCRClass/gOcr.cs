using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Xml;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
//using Volant.Ocr.Dev.Extensions;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class gOcr
    {
        public eIdCard rIdCard;

        public static eKimlik rKimlik;

        public Enums.ScanObject ScanObjEnum = Enums.ScanObject.TcKimlik;

        private eIdCard newRIdCardOCR;

        private YandexTranslate trnslt = new YandexTranslate();

        private eKimlik rKimlikStatic;

        private eJarvis rJarvis;

        public string ImagePath { get; set; }

        public void RefleshrKimlik()
        {
            rKimlik = null;
        }

        public static void SaveImage(eKimlik rIdCardImg)
        {
            try
            {
                Bitmap bmp = new Bitmap(1000, 2444);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    if (rIdCardImg.SERI != null)
                    {
                        g.DrawImage(rIdCardImg.SERI, 0, 0, rIdCardImg.SERI.Width, 111);
                    }
                    if (rIdCardImg.NO != null)
                    {
                        g.DrawImage(rIdCardImg.NO, 0, 111, rIdCardImg.NO.Width, 111);
                    }
                    if (rIdCardImg.TCKIMLIKNO != null)
                    {
                        g.DrawImage(rIdCardImg.TCKIMLIKNO, 0, 190, rIdCardImg.TCKIMLIKNO.Width, 111);
                    }
                    if (rIdCardImg.SOYADI != null)
                    {
                        g.DrawImage(rIdCardImg.SOYADI, 0, 345, rIdCardImg.SOYADI.Width, 111);
                    }
                    if (rIdCardImg.ADI != null)
                    {
                        g.DrawImage(rIdCardImg.ADI, 0, 444, rIdCardImg.ADI.Width, 111);
                    }
                    if (rIdCardImg.BABAADI != null)
                    {
                        g.DrawImage(rIdCardImg.BABAADI, 0, 555, rIdCardImg.BABAADI.Width, 111);
                    }
                    if (rIdCardImg.ANAADI != null)
                    {
                        g.DrawImage(rIdCardImg.ANAADI, 0, 666, rIdCardImg.BABAADI.Width, 111);
                    }
                    if (rIdCardImg.DOGUMYERI != null)
                    {
                        g.DrawImage(rIdCardImg.DOGUMYERI, 0, 777, rIdCardImg.DOGUMYERI.Width, 111);
                    }
                    if (rIdCardImg.DOGUMTARIHI != null)
                    {
                        g.DrawImage(rIdCardImg.DOGUMTARIHI, 0, 888, rIdCardImg.DOGUMTARIHI.Width, 111);
                    }
                    if (rIdCardImg.MEDENIHAL != null)
                    {
                        g.DrawImage(rIdCardImg.MEDENIHAL, 0, 999, rIdCardImg.MEDENIHAL.Width, 111);
                    }
                    if (rIdCardImg.DIN != null)
                    {
                        g.DrawImage(rIdCardImg.DIN, 0, 1111, rIdCardImg.DIN.Width, 111);
                    }
                    if (rIdCardImg.KANGRUBU != null)
                    {
                        g.DrawImage(rIdCardImg.KANGRUBU, 0, 1222, rIdCardImg.KANGRUBU.Width, 111);
                    }
                    if (rIdCardImg.IL != null)
                    {
                        g.DrawImage(rIdCardImg.IL, 0, 1333, rIdCardImg.IL.Width, 111);
                    }
                    if (rIdCardImg.ILCE != null)
                    {
                        g.DrawImage(rIdCardImg.ILCE, 0, 1444, rIdCardImg.ILCE.Width, 111);
                    }
                    if (rIdCardImg.MAHALLEKOY != null)
                    {
                        g.DrawImage(rIdCardImg.MAHALLEKOY, 0, 1555, rIdCardImg.MAHALLEKOY.Width, 111);
                    }
                    if (rIdCardImg.CILTNO != null)
                    {
                        g.DrawImage(rIdCardImg.CILTNO, 0, 1666, rIdCardImg.CILTNO.Width, 111);
                    }
                    if (rIdCardImg.AILESIRANO != null)
                    {
                        g.DrawImage(rIdCardImg.AILESIRANO, 0, 1777, rIdCardImg.AILESIRANO.Width, 111);
                    }
                    if (rIdCardImg.SIRANO != null)
                    {
                        g.DrawImage(rIdCardImg.SIRANO, 0, 1888, rIdCardImg.SIRANO.Width, 111);
                    }
                    if (rIdCardImg.VERILDIGIYER != null)
                    {
                        g.DrawImage(rIdCardImg.VERILDIGIYER, 0, 1999, rIdCardImg.VERILDIGIYER.Width, 111);
                    }
                    if (rIdCardImg.VERILISNEDENI != null)
                    {
                        g.DrawImage(rIdCardImg.VERILISNEDENI, 0, 2111, rIdCardImg.VERILISNEDENI.Width, 111);
                    }
                    if (rIdCardImg.KAYITNO != null)
                    {
                        g.DrawImage(rIdCardImg.KAYITNO, 0, 2222, rIdCardImg.KAYITNO.Width, 111);
                    }
                    if (rIdCardImg.VERILISTARIHI != null)
                    {
                        g.DrawImage(rIdCardImg.VERILISTARIHI, 0, 2333, rIdCardImg.VERILISTARIHI.Width, 111);
                    }
                }
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Volant KTS Tarama\\";
                Directory.CreateDirectory(path);
                using (Image<Bgr, byte> colorImage = new Image<Bgr, byte>(bmp))
                {
                    // 2. Gri tonlamaya çeviriyoruz
                    using (Image<Gray, byte> grayImage = colorImage.Convert<Gray, byte>())
                    {
                        // 3. Jpeg Encoder ayarlarını yapıyoruz
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                        using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
                        {
                            using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L))
                            {
                                myEncoderParameters.Param[0] = myEncoderParameter;

                                // 4. Bitmap üzerinden kaydetme işlemi
                                // Emgu CV'nin .Bitmap özelliği bir referans döner, 
                                // bu yüzden doğrudan Save metodunu kullanabiliriz.
                                grayImage.Bitmap.Save("rKimlik.jpg", jpgEncoder, myEncoderParameters);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        //// Helper metod (Sınıf içinde olmalı)
        //private static ImageCodecInfo GetEncoder(ImageFormat format)
        //{
        //    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        //    foreach (ImageCodecInfo codec in codecs)
        //    {
        //        if (codec.FormatID == format.Guid)
        //        {
        //            return codec;
        //        }
        //    }
        //    return null;
        //}

        public static void SaveImageFront(eKimlik rIdCardImg)
        {
            try
            {
                Bitmap bmp = new Bitmap(1000, 888);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    if (rIdCardImg.SERI != null)
                    {
                        g.DrawImage(rIdCardImg.SERI, 0, 0, rIdCardImg.SERI.Width, 111);
                    }
                    if (rIdCardImg.NO != null)
                    {
                        g.DrawImage(rIdCardImg.NO, 0, 111, rIdCardImg.NO.Width, 111);
                    }
                    if (rIdCardImg.TCKIMLIKNO != null)
                    {
                        g.DrawImage(rIdCardImg.TCKIMLIKNO, 0, 222, rIdCardImg.TCKIMLIKNO.Width, 111);
                    }
                    if (rIdCardImg.SOYADI != null)
                    {
                        g.DrawImage(rIdCardImg.SOYADI, 0, 333, rIdCardImg.SOYADI.Width, 111);
                    }
                    if (rIdCardImg.ADI != null)
                    {
                        g.DrawImage(rIdCardImg.ADI, 0, 444, rIdCardImg.ADI.Width, 111);
                    }
                    if (rIdCardImg.BABAADI != null)
                    {
                        g.DrawImage(rIdCardImg.BABAADI, 0, 555, rIdCardImg.BABAADI.Width, 111);
                    }
                    if (rIdCardImg.ANAADI != null)
                    {
                        g.DrawImage(rIdCardImg.ANAADI, 0, 666, rIdCardImg.BABAADI.Width, 111);
                    }
                    if (rIdCardImg.DOGUMYERI != null)
                    {
                        g.DrawImage(rIdCardImg.DOGUMYERI, 0, 777, rIdCardImg.DOGUMYERI.Width, 111);
                    }
                    if (rIdCardImg.DOGUMTARIHI != null)
                    {
                        g.DrawImage(rIdCardImg.DOGUMTARIHI, 0, 888, rIdCardImg.DOGUMTARIHI.Width, 111);
                    }
                }
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Volant KTS Tarama\\";
                Directory.CreateDirectory(path);
                using (Image<Bgr, byte> colorImage = new Image<Bgr, byte>(bmp))
                {
                    // 2. Gri tonlamaya çeviriyoruz
                    using (Image<Gray, byte> grayImage = colorImage.Convert<Gray, byte>())
                    {
                        // 3. Jpeg Encoder ayarlarını yapıyoruz
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                        using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
                        {
                            using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L))
                            {
                                myEncoderParameters.Param[0] = myEncoderParameter;

                                // 4. Bitmap üzerinden kaydetme işlemi
                                // Emgu CV'nin .Bitmap özelliği bir referans döner, 
                                // bu yüzden doğrudan Save metodunu kullanabiliriz.
                                grayImage.Bitmap.Save("rKimlik.jpg", jpgEncoder, myEncoderParameters);
                            }
                        }
                    }
                }
                //Image<Bgr, byte> imgGray = new Image<Bgr, byte>(bmp);
                //Image<Gray, byte> imgGray2 = imgGray.Convert<Gray, byte>();
                //ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                //Encoder myEncoder = Encoder.Quality;
                //EncoderParameters myEncoderParameters = new EncoderParameters(1);
                //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                //myEncoderParameters.Param[0] = myEncoderParameter;
                //imgGray2.Bitmap.Save("rKimlikFront.jpg", jgpEncoder, myEncoderParameters);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public static void SaveImageBack(eKimlik rIdCardImg)
        {
            try
            {
                Bitmap bmp = new Bitmap(1000, 1333);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    if (rIdCardImg.MEDENIHAL != null)
                    {
                        g.DrawImage(rIdCardImg.MEDENIHAL, 0, 0, rIdCardImg.MEDENIHAL.Width, 111);
                    }
                    if (rIdCardImg.DIN != null)
                    {
                        g.DrawImage(rIdCardImg.DIN, 0, 111, rIdCardImg.DIN.Width, 111);
                    }
                    if (rIdCardImg.KANGRUBU != null)
                    {
                        g.DrawImage(rIdCardImg.KANGRUBU, 0, 222, rIdCardImg.KANGRUBU.Width, 111);
                    }
                    if (rIdCardImg.IL != null)
                    {
                        g.DrawImage(rIdCardImg.IL, 0, 333, rIdCardImg.IL.Width, 111);
                    }
                    if (rIdCardImg.ILCE != null)
                    {
                        g.DrawImage(rIdCardImg.ILCE, 0, 444, rIdCardImg.ILCE.Width, 111);
                    }
                    if (rIdCardImg.MAHALLEKOY != null)
                    {
                        g.DrawImage(rIdCardImg.MAHALLEKOY, 0, 555, rIdCardImg.MAHALLEKOY.Width, 111);
                    }
                    if (rIdCardImg.CILTNO != null)
                    {
                        g.DrawImage(rIdCardImg.CILTNO, 0, 666, rIdCardImg.CILTNO.Width, 111);
                    }
                    if (rIdCardImg.AILESIRANO != null)
                    {
                        g.DrawImage(rIdCardImg.AILESIRANO, 0, 777, rIdCardImg.AILESIRANO.Width, 111);
                    }
                    if (rIdCardImg.SIRANO != null)
                    {
                        g.DrawImage(rIdCardImg.SIRANO, 0, 888, rIdCardImg.SIRANO.Width, 111);
                    }
                    if (rIdCardImg.VERILDIGIYER != null)
                    {
                        g.DrawImage(rIdCardImg.VERILDIGIYER, 0, 999, rIdCardImg.VERILDIGIYER.Width, 111);
                    }
                    if (rIdCardImg.VERILISNEDENI != null)
                    {
                        g.DrawImage(rIdCardImg.VERILISNEDENI, 0, 1111, rIdCardImg.VERILISNEDENI.Width, 111);
                    }
                    if (rIdCardImg.KAYITNO != null)
                    {
                        g.DrawImage(rIdCardImg.KAYITNO, 0, 1222, rIdCardImg.KAYITNO.Width, 111);
                    }
                    if (rIdCardImg.VERILISTARIHI != null)
                    {
                        g.DrawImage(rIdCardImg.VERILISTARIHI, 0, 1333, rIdCardImg.VERILISTARIHI.Width, 111);
                    }
                }
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Volant KTS Tarama\\";
                Directory.CreateDirectory(path);
                using (Image<Bgr, byte> colorImage = new Image<Bgr, byte>(bmp))
                {
                    // 2. Gri tonlamaya çeviriyoruz
                    using (Image<Gray, byte> grayImage = colorImage.Convert<Gray, byte>())
                    {
                        // 3. Jpeg Encoder ayarlarını yapıyoruz
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                        using (EncoderParameters myEncoderParameters = new EncoderParameters(1))
                        {
                            using (EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L))
                            {
                                myEncoderParameters.Param[0] = myEncoderParameter;

                                // 4. Bitmap üzerinden kaydetme işlemi
                                // Emgu CV'nin .Bitmap özelliği bir referans döner, 
                                // bu yüzden doğrudan Save metodunu kullanabiliriz.
                                grayImage.Bitmap.Save("rKimlik.jpg", jpgEncoder, myEncoderParameters);
                            }
                        }
                    }
                }
                //Image<Bgr, byte> imgGray = new Image<Bgr, byte>(bmp);
                //Image<Gray, byte> imgGray2 = imgGray.Convert<Gray, byte>();
                //ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                //Encoder myEncoder = Encoder.Quality;
                //EncoderParameters myEncoderParameters = new EncoderParameters(1);
                //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                //myEncoderParameters.Param[0] = myEncoderParameter;
                //imgGray2.Bitmap.Save("rKimlikBack.jpg", jgpEncoder, myEncoderParameters);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Gif);
                return ms.ToArray();
            }
        }

        public async void GetOcrNew(eKimlik GetEKimlik, eIdCard newRIdCard)
        {
            try
            {
                newRIdCardOCR = new eIdCard();
                newRIdCardOCR = newRIdCard;
                HttpClient httpClient = new HttpClient
                {
                    Timeout = new TimeSpan(1, 1, 1)
                };
                MultipartFormDataContent form = new MultipartFormDataContent
            {
                {
                    new StringContent("7b0ede3ee988957"),
                    "apikey"
                },
                {
                    new StringContent("tur"),
                    "language"
                }
            };
                byte[] imageData = ImageToByteArray(GetEKimlik.MAHALLEKOY);
                form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
                Rootobject ocrResult = JsonConvert.DeserializeObject<Rootobject>(await (await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form)).Content.ReadAsStringAsync());
                string result = "";
                if (ocrResult.OCRExitCode == 1)
                {
                    for (int i = 0; i < ocrResult.ParsedResults.Count(); i++)
                    {
                        result += ocrResult.ParsedResults[i].ParsedText;
                    }
                }
                newRIdCardOCR.MAHALLEKOY = result;
            }
            catch (Exception ex)
            {
                Exception exp = ex;
                throw exp;
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            ImageCodecInfo[] array = codecs;
            foreach (ImageCodecInfo codec in array)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public eIdCard SaveImageStarts(eKimlik GetEKimlik)
        {
            if (GetEKimlik.TCKIMLIKNO == null)
            {
                return null;
            }
            if (GetEKimlik.MAHALLEKOY == null)
            {
                return null;
            }
            SaveImage(GetEKimlik);
            gOcr ocr = new gOcr();
            rIdCard = ocr.GetOcr("rKimlik.jpg");
            return rIdCard;
        }

        public eIdCard SaveImageStartsEhliyet(eKimlik GetEKimlik)
        {
            if (GetEKimlik.TCKIMLIKNO == null)
            {
                return null;
            }
            if (GetEKimlik.MAHALLEKOY == null)
            {
                return null;
            }
            SaveImage(GetEKimlik);
            gOcr ocr = new gOcr();
            rIdCard = ocr.GetOcr("rKimlik.jpg");
            return rIdCard;
        }

        public eIdCard SaveImageStartsYeniKimlik(eKimlik GetEKimlik)
        {
            if (GetEKimlik.TCKIMLIKNO == null)
            {
                return null;
            }
            if (GetEKimlik.BABAADI == null)
            {
                return null;
            }
            SaveImage(GetEKimlik);
            gOcr ocr = new gOcr();
            rIdCard = ocr.GetOcr("rKimlik.jpg");
            return rIdCard;
        }

        public eIdCard SaveImageStartsYeniEhliyet(eKimlik GetEKimlik)
        {
            if (GetEKimlik.TCKIMLIKNO == null)
            {
                return null;
            }
            if (GetEKimlik.KANGRUBU == null)
            {
                return null;
            }
            SaveImage(GetEKimlik);
            gOcr ocr = new gOcr();
            rIdCard = ocr.GetOcr("rKimlik.jpg");
            return rIdCard;
        }

        public eIdCard StartOCRFront(string fileName)
        {
            try
            {
                Bitmap img = (Bitmap)resizeImage(825, 1110, fileName);
                return StartOCRFrontAli(img, Enums.ScanObject.TcKimlik);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public Image resizeImage(int newWidth, int newHeight, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;
                newWidth = newHeight;
                newHeight = buff;
            }
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;
            float nPercent = 0f;
            float nPercentW = 0f;
            float nPercentH = 0f;
            nPercentW = (float)newWidth / (float)sourceWidth;
            nPercentH = (float)newHeight / (float)sourceHeight;
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16(((float)newWidth - (float)sourceWidth * nPercent) / 2f);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16(((float)newHeight - (float)sourceHeight * nPercent) / 2f);
            }
            int destWidth = (int)((float)sourceWidth * nPercent);
            int destHeight = (int)((float)sourceHeight * nPercent);
            Bitmap bmPhoto = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }

        public eIdCard StartOCRFrontAli(Bitmap file, Enums.ScanObject ScanObjEnums)
        {
            try
            {
                ScanObjEnum = ScanObjEnums;
                rKimlikStatic = ImageHelper.ImageHelperFrontErp(file, ScanObjEnum);
                if (rKimlik == null)
                {
                    rKimlik = new eKimlik();
                }
                switch (ScanObjEnums)
                {
                    case Enums.ScanObject.TcKimlik:
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.ANAADI = rKimlikStatic.ANAADI;
                        rKimlik.BABAADI = rKimlikStatic.BABAADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rKimlik.DOGUMYERI = rKimlikStatic.DOGUMYERI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rKimlik.SERI = rKimlikStatic.SERI;
                        rKimlik.NO = rKimlikStatic.NO;
                        rIdCard = SaveImageStarts(rKimlik);
                        break;
                    case Enums.ScanObject.YeniTcKimlik:
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rIdCard = SaveImageStartsYeniKimlik(rKimlik);
                        break;
                    case Enums.ScanObject.Ehliyet:
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rIdCard = SaveImageStartsEhliyet(rKimlik);
                        break;
                    case Enums.ScanObject.YeniEhliyet:
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rKimlik.DOGUMYERI = rKimlikStatic.DOGUMYERI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rIdCard = SaveImageStartsYeniEhliyet(rKimlik);
                        break;
                }
                if (rIdCard != null)
                {
                    return rIdCard;
                }
                return null;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public void rKimlikClear()
        {
            rKimlik = null;
        }

        public eIdCard StartOCRBack(string fileName)
        {
            try
            {
                Bitmap img = (Bitmap)resizeImage(825, 1110, fileName);
                return StartOCRBackAli(img, Enums.ScanObject.TcKimlik);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public eIdCard StartOCRBackAli(Bitmap file, Enums.ScanObject ScanObjEnums)
        {
            try
            {
                ScanObjEnum = ScanObjEnums;
                rKimlikStatic = ImageHelper.ImageHelperBackErp(file, ScanObjEnum);
                if (rKimlik == null)
                {
                    rKimlik = new eKimlik();
                }
                switch (ScanObjEnums)
                {
                    case Enums.ScanObject.TcKimlik:
                        rKimlik.MEDENIHAL = rKimlikStatic.MEDENIHAL;
                        rKimlik.DIN = rKimlikStatic.DIN;
                        rKimlik.KANGRUBU = rKimlikStatic.KANGRUBU;
                        rKimlik.IL = rKimlikStatic.IL;
                        rKimlik.ILCE = rKimlikStatic.ILCE;
                        rKimlik.MAHALLEKOY = rKimlikStatic.MAHALLEKOY;
                        rKimlik.CILTNO = rKimlikStatic.CILTNO;
                        rKimlik.AILESIRANO = rKimlikStatic.AILESIRANO;
                        rKimlik.SIRANO = rKimlikStatic.SIRANO;
                        rKimlik.VERILDIGIYER = rKimlikStatic.VERILDIGIYER;
                        rKimlik.VERILISNEDENI = rKimlikStatic.VERILISNEDENI;
                        rKimlik.KAYITNO = rKimlikStatic.KAYITNO;
                        rKimlik.VERILISTARIHI = rKimlikStatic.VERILISTARIHI;
                        rIdCard = SaveImageStarts(rKimlik);
                        break;
                    case Enums.ScanObject.YeniTcKimlik:
                        rKimlik.ANAADI = rKimlikStatic.ANAADI;
                        rKimlik.BABAADI = rKimlikStatic.BABAADI;
                        rIdCard = SaveImageStartsYeniKimlik(rKimlik);
                        break;
                    case Enums.ScanObject.Ehliyet:
                        rKimlik.KANGRUBU = rKimlikStatic.KANGRUBU;
                        rKimlik.IL = rKimlikStatic.IL;
                        rKimlik.ILCE = rKimlikStatic.ILCE;
                        rKimlik.MAHALLEKOY = rKimlikStatic.MAHALLEKOY;
                        rKimlik.CILTNO = rKimlikStatic.CILTNO;
                        rKimlik.AILESIRANO = rKimlikStatic.AILESIRANO;
                        rKimlik.SIRANO = rKimlikStatic.SIRANO;
                        rKimlik.ANAADI = rKimlikStatic.ANAADI;
                        rKimlik.BABAADI = rKimlikStatic.BABAADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rKimlik.DOGUMYERI = rKimlikStatic.DOGUMYERI;
                        rIdCard = SaveImageStartsEhliyet(rKimlik);
                        break;
                    case Enums.ScanObject.YeniEhliyet:
                        rKimlik.KANGRUBU = rKimlikStatic.KANGRUBU;
                        rIdCard = SaveImageStartsYeniEhliyet(rKimlik);
                        break;
                }
                if (rIdCard != null)
                {
                    return rIdCard;
                }
                return null;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public void CofigSettingRead()
        {
            try
            {
                rJarvis = new eJarvis();
                XmlTextReader reader = null;
                reader = new XmlTextReader("ConfigSettings.xml");
                if (!File.Exists("ConfigSettings.xml"))
                {
                    return;
                }
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Config")
                    {
                        rJarvis.JarConfigSettingFileSource = reader.GetAttribute("JarConfigSettingFileSource");
                        rJarvis.JarConfigSettingPrint = reader.GetAttribute("JarConfigSettingPrint");
                        rJarvis.JarConfigSettingPrintDefault = reader.GetAttribute("JarConfigSettingPrintDefault");
                        rJarvis.JarConfigSettingLanguage = reader.GetAttribute("JarConfigSettingLanguage");
                    }
                }
                reader.Close();
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public eIdCard GetOcr(string ImagePath)
        {
            string txtResult = "";
            if (rIdCard == null)
            {
                rIdCard = new eIdCard();
            }
            GoogleAnnotate annotate = new GoogleAnnotate();
            CofigSettingRead();
            if (rJarvis != null)
            {
                if (rJarvis.JarConfigSettingLanguage == "tr")
                {
                    annotate.GetText(ImagePath, "tr");
                }
                else if (rJarvis.JarConfigSettingLanguage == "eng")
                {
                    annotate.GetText(ImagePath, "en");
                }
                else
                {
                    annotate.GetText(ImagePath, "tr");
                }
            }
            if (string.IsNullOrEmpty(annotate.Error))
            {
                txtResult = annotate.TextResult;
                rIdCard = annotate.rIdCard;
            }
            return rIdCard;
        }

        public eIdCard GetIdCard()
        {
            return rIdCard;
        }

        public eIdCard StartOCRFrontErp(Bitmap file, Enums.ScanObject ScanObjEnums)
        {
            try
            {
                ScanObjEnum = ScanObjEnums;
                rKimlikStatic = ImageHelper.ImageHelperFrontErp(file, ScanObjEnum);
                if (rKimlikStatic == null)
                {
                    return null;
                }
                if (rKimlik == null)
                {
                    rKimlik = new eKimlik();
                }
                switch (ScanObjEnums)
                {
                    case Enums.ScanObject.TcKimlik:
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.ANAADI = rKimlikStatic.ANAADI;
                        rKimlik.BABAADI = rKimlikStatic.BABAADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rKimlik.DOGUMYERI = rKimlikStatic.DOGUMYERI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rKimlik.SERI = rKimlikStatic.SERI;
                        rKimlik.NO = rKimlikStatic.NO;
                        rIdCard = SaveImageStarts(rKimlik);
                        break;
                    case Enums.ScanObject.YeniTcKimlik:
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rIdCard = SaveImageStartsYeniKimlik(rKimlik);
                        break;
                    case Enums.ScanObject.Ehliyet:
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rIdCard = SaveImageStartsEhliyet(rKimlik);
                        break;
                    case Enums.ScanObject.YeniEhliyet:
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.ADI = rKimlikStatic.ADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rKimlik.DOGUMYERI = rKimlikStatic.DOGUMYERI;
                        rKimlik.TCKIMLIKNO = rKimlikStatic.TCKIMLIKNO;
                        rKimlik.SOYADI = rKimlikStatic.SOYADI;
                        rIdCard = SaveImageStartsYeniEhliyet(rKimlik);
                        break;
                }
                if (rIdCard != null)
                {
                    return rIdCard;
                }
                return null;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public eIdCard StartOCRBackErp(Bitmap file, Enums.ScanObject ScanObjEnums)
        {
            try
            {
                ScanObjEnum = ScanObjEnums;
                rKimlikStatic = ImageHelper.ImageHelperBackErp(file, ScanObjEnum);
                if (rKimlikStatic == null)
                {
                    return null;
                }
                if (rKimlik == null)
                {
                    rKimlik = new eKimlik();
                }
                switch (ScanObjEnums)
                {
                    case Enums.ScanObject.TcKimlik:
                        rKimlik.MEDENIHAL = rKimlikStatic.MEDENIHAL;
                        rKimlik.DIN = rKimlikStatic.DIN;
                        rKimlik.KANGRUBU = rKimlikStatic.KANGRUBU;
                        rKimlik.IL = rKimlikStatic.IL;
                        rKimlik.ILCE = rKimlikStatic.ILCE;
                        rKimlik.MAHALLEKOY = rKimlikStatic.MAHALLEKOY;
                        rKimlik.CILTNO = rKimlikStatic.CILTNO;
                        rKimlik.AILESIRANO = rKimlikStatic.AILESIRANO;
                        rKimlik.SIRANO = rKimlikStatic.SIRANO;
                        rKimlik.VERILDIGIYER = rKimlikStatic.VERILDIGIYER;
                        rKimlik.VERILISNEDENI = rKimlikStatic.VERILISNEDENI;
                        rKimlik.KAYITNO = rKimlikStatic.KAYITNO;
                        rKimlik.VERILISTARIHI = rKimlikStatic.VERILISTARIHI;
                        rIdCard = SaveImageStarts(rKimlik);
                        break;
                    case Enums.ScanObject.YeniTcKimlik:
                        rKimlik.ANAADI = rKimlikStatic.ANAADI;
                        rKimlik.BABAADI = rKimlikStatic.BABAADI;
                        rIdCard = SaveImageStartsYeniKimlik(rKimlik);
                        break;
                    case Enums.ScanObject.Ehliyet:
                        rKimlik.KANGRUBU = rKimlikStatic.KANGRUBU;
                        rKimlik.IL = rKimlikStatic.IL;
                        rKimlik.ILCE = rKimlikStatic.ILCE;
                        rKimlik.MAHALLEKOY = rKimlikStatic.MAHALLEKOY;
                        rKimlik.CILTNO = rKimlikStatic.CILTNO;
                        rKimlik.AILESIRANO = rKimlikStatic.AILESIRANO;
                        rKimlik.SIRANO = rKimlikStatic.SIRANO;
                        rKimlik.ANAADI = rKimlikStatic.ANAADI;
                        rKimlik.BABAADI = rKimlikStatic.BABAADI;
                        rKimlik.DOGUMTARIHI = rKimlikStatic.DOGUMTARIHI;
                        rKimlik.DOGUMYERI = rKimlikStatic.DOGUMYERI;
                        rIdCard = SaveImageStartsEhliyet(rKimlik);
                        break;
                    case Enums.ScanObject.YeniEhliyet:
                        rKimlik.KANGRUBU = rKimlikStatic.KANGRUBU;
                        rIdCard = SaveImageStartsYeniEhliyet(rKimlik);
                        break;
                }
                if (rIdCard != null)
                {
                    return rIdCard;
                }
                return null;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }

}
