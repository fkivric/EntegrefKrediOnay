using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using EntegrefKrediOnay.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntegrefKrediOnay.Class.OCRClass
{ 
    public class ImageHelper
    {
        private static Image FrontImage;

        private static Image BackImage;

        private static eKimlik rKimlik;

        private static Enums.ScanObject ScanObjEnums;

        public static Bitmap CropUnwantedBackground(Bitmap bmp)
        {
            Color? backColor = GetMatchedBackColor(bmp);
            if (backColor.HasValue)
            {
                System.Drawing.Point[] bounds = GetImageBounds(bmp, backColor);
                int diffX = bounds[1].X - bounds[0].X + 1;
                int diffY = bounds[1].Y - bounds[0].Y + 1;
                Bitmap croppedBmp = new Bitmap(diffX, diffY);
                Graphics g = Graphics.FromImage(croppedBmp);
                Rectangle destRect = new Rectangle(0, 0, croppedBmp.Width, croppedBmp.Height);
                Rectangle srcRect = new Rectangle(bounds[0].X, bounds[0].Y, diffX, diffY);
                g.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
                return croppedBmp;
            }
            return null;
        }

        private static Point[] GetImageBounds(Bitmap bmp, Color? backColor)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            bool upperLeftPointFounded = false;
            Point[] bounds = new Point[2];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    if ((double)(int)c.R <= (double)(int)backColor.Value.R * 1.1 && (double)(int)c.R >= (double)(int)backColor.Value.R * 0.9 && (double)(int)c.G <= (double)(int)backColor.Value.G * 1.1 && (double)(int)c.G >= (double)(int)backColor.Value.G * 0.9 && (double)(int)c.B <= (double)(int)backColor.Value.B * 1.1 && (double)(int)c.B >= (double)(int)backColor.Value.B * 0.9)
                    {
                        continue;
                    }
                    if (!upperLeftPointFounded)
                    {
                        bounds[0] = new Point(x, y);
                        bounds[1] = new Point(x, y);
                        upperLeftPointFounded = true;
                        continue;
                    }
                    if (x > bounds[1].X)
                    {
                        bounds[1].X = x;
                    }
                    else if (x < bounds[0].X)
                    {
                        bounds[0].X = x;
                    }
                    if (y >= bounds[1].Y)
                    {
                        bounds[1].Y = y;
                    }
                }
            }
            return bounds;
        }

        private static Color? GetMatchedBackColor(Bitmap bmp)
        {
            Point[] corners = new Point[4]
            {
            new Point(0, 0),
            new Point(0, bmp.Height - 1),
            new Point(bmp.Width - 1, 0),
            new Point(bmp.Width - 1, bmp.Height - 1)
            };
            for (int i = 0; i < 4; i++)
            {
                int cornerMatched = 0;
                Color backColor = bmp.GetPixel(corners[i].X, corners[i].Y);
                for (int j = 0; j < 4; j++)
                {
                    Color cornerColor = bmp.GetPixel(corners[j].X, corners[j].Y);
                    if ((double)(int)cornerColor.R <= (double)(int)backColor.R * 1.1 && (double)(int)cornerColor.R >= (double)(int)backColor.R * 0.9 && (double)(int)cornerColor.G <= (double)(int)backColor.G * 1.1 && (double)(int)cornerColor.G >= (double)(int)backColor.G * 0.9 && (double)(int)cornerColor.B <= (double)(int)backColor.B * 1.1 && (double)(int)cornerColor.B >= (double)(int)backColor.B * 0.9)
                    {
                        cornerMatched++;
                    }
                }
                if (cornerMatched > 2)
                {
                    return backColor;
                }
            }
            return null;
        }
        /// <summary>
        /// Resource'dan gelen Bitmap'i güvenli bir şekilde Mat nesnesine dönüştürür.
        /// </summary>
        public static Mat ConvertBitmapToMat(Bitmap bmp)
        {

            //if (bmp == null)
            //    return null;

            //// Indexed formatları normalize et (çok kritik)
            //Bitmap workingBmp = bmp;

            //if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed ||
            //    bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format1bppIndexed ||
            //    bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed)
            //{
            //    workingBmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //    using (Graphics g = Graphics.FromImage(workingBmp))
            //    {
            //        g.DrawImage(bmp, 0, 0);
            //    }
            //}

            //// EmguCV dönüşüm (en stabil yol)
            //using (var img = new Image<Bgr, byte>(workingBmp))
            //{
            //    return img.Mat.Clone();
            //}
            if (bmp == null) return null;

            // 1. Her zaman 24bppRgb formatına zorla (Emgu Bgr tipi için en güvenlisi)
            Bitmap workingBmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(workingBmp))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, workingBmp.Width, workingBmp.Height));
            }

            // 2. Manuel Veri Kopyalama (Hata riskini minimize eder)
            var data = workingBmp.LockBits(new Rectangle(0, 0, workingBmp.Width, workingBmp.Height),
                                           System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                           workingBmp.PixelFormat);
            try
            {
                // Mat nesnesini doğrudan bellek adresinden oluştur
                Mat mat = new Mat(workingBmp.Height, workingBmp.Width, Emgu.CV.CvEnum.DepthType.Cv8U, 3, data.Scan0, data.Stride);
                return mat.Clone(); // Veriyi kopyalayıp orijinali serbest bırakıyoruz
            }
            finally
            {
                workingBmp.UnlockBits(data);
                workingBmp.Dispose();
            }
        }
        public static Mat GetMatFront(Enums.ScanObject ScanObjEnum)
        {
            Mat modelImage = null;

            // Not: Bitmap nesnelerini Image<Bgr, byte> içine atmadan önce 
            // bellek sızıntısını önlemek için ToImage extension'ı veya constructor kullanırız.

            switch (ScanObjEnum)
            {

                case Enums.ScanObject.YeniTcKimlik:
                    modelImage = ConvertBitmapToMat(Resources.YeniKimlikOn);
                    break;

                case Enums.ScanObject.Ehliyet:
                    modelImage = ConvertBitmapToMat(Resources.EhliyetOn);
                    break;

                case Enums.ScanObject.YeniEhliyet:
                    modelImage = ConvertBitmapToMat(Resources.YeniEhliyetOn);
                    break;

                case Enums.ScanObject.Pasaport:
                    modelImage = null;
                    break;

                default:
                    modelImage = null;
                    break;
            }

            return modelImage;
        }

        public static Mat GetMatBack(Enums.ScanObject ScanObjEnum)
        {
            Mat modelImage = null;
            switch (ScanObjEnum)
            {

                case Enums.ScanObject.YeniTcKimlik:
                    modelImage = ConvertBitmapToMat(Resources.YeniKimlikOn);
                    break;

                case Enums.ScanObject.Ehliyet:
                    modelImage = ConvertBitmapToMat(Resources.EhliyetOn);
                    break;

                case Enums.ScanObject.YeniEhliyet:
                    modelImage = ConvertBitmapToMat(Resources.YeniEhliyetOn);
                    break;

                case Enums.ScanObject.Pasaport:
                    modelImage = null;
                    break;

                default:
                    modelImage = null;
                    break;
            }
            return modelImage;
        }

        [STAThread]
        public static Image ImageHelperFront(string filename, Enums.ScanObject ScanObjEnum)
        {
            try
            {
                Application.EnableVisualStyles();
                using (Mat modelImage = GetMatFront(ScanObjEnum))
                {
                    using (Mat observedImage = CvInvoke.Imread(filename, LoadImageType.AnyColor))
                    {
                        long matchTime;
                        return FrontImage = DrawMatchess.Draw(modelImage, observedImage, out matchTime, ScanObjEnum);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Camera()
        {
            Capture capture = new Capture(1);
            Bitmap image = capture.QueryFrame().Bitmap;
            image.Save("C:\\Users\\Volant-Abdullah\\Desktop\\Kimlik\\Denemee.png");
        }

        public static Image CropImgSERI(Bitmap bmp1)
        {
            Image<Bgr, byte> cropImage = new Image<Bgr, byte>(bmp1);
            for (int i = 0; i < cropImage.Rows; i++)
            {
                for (int j = 0; j < cropImage.Cols; j++)
                {
                    Bgr currentColor = cropImage[i, j];
                    if (currentColor.Blue > 200.0 || currentColor.Green > 200.0 || currentColor.Red > 200.0)
                    {
                        cropImage[i, j] = new Bgr(255.0, 255.0, 255.0);
                    }
                    else if (currentColor.Blue > currentColor.Green + 20.0 && currentColor.Blue > currentColor.Red + 20.0)
                    {
                        cropImage[i, j] = new Bgr(255.0, 255.0, 255.0);
                    }
                }
            }
            cropImage.Erode(1);
            return cropImage.Bitmap;
        }

        [STAThread]
        public static eKimlik ImageHelperFront2(string filename, Enums.ScanObject ScanObjEnum)
        {
            if (rKimlik == null)
            {
                rKimlik = new eKimlik();
            }
            Application.EnableVisualStyles();
            using (Mat modelImage = GetMatFront(ScanObjEnum))
            {
                using (Mat observedImage = CvInvoke.Imread(filename, LoadImageType.AnyColor))
                {
                    long matchTime;
                    Bitmap cropImg = (Bitmap)(FrontImage = DrawMatchess.Draw(modelImage, observedImage, out matchTime, ScanObjEnum));
                    rKimlik.TCKIMLIKNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 523, 999, 99);
                    rKimlik.SERI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 447, 482, 92);
                    rKimlik.SERI = CropImgSERI((Bitmap)rKimlik.SERI);
                    rKimlik.NO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 557, 440, 443, 105);
                    rKimlik.SOYADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 601, 999, 97);
                    rKimlik.ADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 684, 999, 92);
                    rKimlik.BABAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 758, 999, 97);
                    rKimlik.ANAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 839, 999, 94);
                    rKimlik.DOGUMYERI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 911, 508, 89);
                    rKimlik.DOGUMTARIHI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 493, 915, 507, 85);
                    return rKimlik;
                }
            }
        }

        public static eKimlik ImageHelperFrontErp(Bitmap file, Enums.ScanObject ScanObjEnum)
        {
            try
            {
                rKimlik = new eKimlik();
                Mat modelImage = GetMatFront(ScanObjEnum);
                var observedImage = ConvertBitmapToMat(file);
                if (modelImage.Ptr == IntPtr.Zero)
                {
                    modelImage = GetMatFront(ScanObjEnum);
                }
                long matchTime;
                Bitmap cropImg = (Bitmap)(FrontImage = DrawMatchess.Draw(modelImage.Clone(), observedImage.Clone(), out matchTime, ScanObjEnum));

                //// modelImage nesnesini alıyoruz
                //using (Mat modelImage = GetMatFront(ScanObjEnum))
                //{
                //    // modelImage yüklenemezse veya boşsa işlemi durdur
                //    if (modelImage == null || modelImage.Ptr == IntPtr.Zero)
                //        return null;

                //    // Dosyadan resmi yükle ve using ile iş bittiğinde RAM'den at
                //    using (Image<Bgr, byte> observedImage = new Image<Bgr, byte>(file))
                //    {
                        //long matchTime;

                        // DrawMatchess.Draw içinde Clone kullanmak yerine doğrudan Mat gönderiyoruz 
                        // (Metodun iç yapısı Mat kabul ediyorsa kopyalamaya gerek yok, etmiyorsa .Clone() ekle)
                        //Image drawnResult = DrawMatchess.Draw(modelImage, observedImage.Mat, out matchTime, ScanObjEnum);

                        //if (drawnResult == null) return null;

                        //FrontImage = drawnResult;
                        //Bitmap cropImg = (Bitmap)drawnResult;

                        // Switch yapısı
                        switch (ScanObjEnum)
                        {
                            case Enums.ScanObject.TcKimlik:
                                rKimlik.TCKIMLIKNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 30, 520, 967, 90);
                                rKimlik.SERI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 30, 460, 499, 69);
                                if (rKimlik.SERI != null)
                                    rKimlik.SERI = CropImgSERI((Bitmap)rKimlik.SERI);
                                rKimlik.NO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 557, 449, 429, 80);
                                rKimlik.SOYADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 30, 596, 961, 90);
                                rKimlik.ADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 30, 675, 956, 85);
                                rKimlik.BABAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 30, 744, 956, 93);
                                rKimlik.ANAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 28, 821, 961, 91);
                                rKimlik.DOGUMYERI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 38, 894, 489, 106);
                                rKimlik.DOGUMTARIHI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 507, 892, 479, 106);
                                break;

                            case Enums.ScanObject.YeniTcKimlik:
                                rKimlik.TCKIMLIKNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 36, 192, 390, 62);
                                rKimlik.SOYADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 357, 316, 335, 62);
                                rKimlik.ADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 359, 400, 462, 62);
                                rKimlik.DOGUMTARIHI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 363, 483, 272, 57);
                                break;

                            case Enums.ScanObject.Ehliyet:
                                rKimlik.TCKIMLIKNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 363, 150, 652, 60);
                                rKimlik.ADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 369, 196, 655, 77);
                                rKimlik.SOYADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 363, 288, 661, 79);
                                break;

                            case Enums.ScanObject.YeniEhliyet:
                                rKimlik.SOYADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 372, 170, 535, 74);
                                rKimlik.ADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 372, 234, 644, 71);
                                rKimlik.DOGUMTARIHI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 372, 290, 235, 68);
                                rKimlik.DOGUMYERI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 602, 289, 417, 80);
                                rKimlik.TCKIMLIKNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 717, 418, 302, 64);
                                break;
                        }
                //    }
                //}
                return rKimlik;
            }
            catch
            {
                return null;
            }
        }

        [STAThread]
        public static Image ImageHelperBack(string filename, Enums.ScanObject ScanObjEnum)
        {
            try
            {
                Application.EnableVisualStyles();
                using (Mat modelImage = GetMatBack(ScanObjEnum))
                {
                    using (Mat observedImage = CvInvoke.Imread(filename, LoadImageType.AnyColor))
                    {
                        long matchTime;
                        Image img = DrawMatchess.Draw(modelImage, observedImage, out matchTime, ScanObjEnum);
                        if (ScanObjEnum == Enums.ScanObject.TcKimlik)
                        {
                            Mat modelImageArkaTamami = CvInvoke.Imread("Extensions/nufuscuzdaniArkaTamami.jpg", LoadImageType.AnyColor);
                            Image imgArkaTamami = DrawMatchess.Draw(modelImageArkaTamami, observedImage, out matchTime, ScanObjEnum);
                            BackImage = imgArkaTamami;
                        }
                        else
                        {
                            BackImage = img;
                        }
                        return img;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [STAThread]
        public static eKimlik ImageHelperBack2(string filename, Enums.ScanObject ScanObjEnum)
        {
            if (rKimlik == null)
            {
                rKimlik = new eKimlik();
            }
            Application.EnableVisualStyles();
            using (Mat modelImage = GetMatBack(ScanObjEnum))
            {
                using (Mat observedImage = CvInvoke.Imread(filename, LoadImageType.AnyColor))
                {
                    long matchTime;
                    Image img = (BackImage = DrawMatchess.Draw(modelImage, observedImage, out matchTime, ScanObjEnum));
                    Bitmap cropImg = (Bitmap)img;
                    rKimlik.MEDENIHAL = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 1, 360, 198);
                    rKimlik.DIN = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 331, 0, 365, 191);
                    rKimlik.KANGRUBU = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 663, 0, 337, 196);
                    rKimlik.IL = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 63, 178, 483, 185);
                    rKimlik.ILCE = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 513, 176, 487, 182);
                    rKimlik.MAHALLEKOY = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 68, 342, 932, 182);
                    rKimlik.CILTNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 65, 507, 325, 181);
                    rKimlik.AILESIRANO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 366, 500, 339, 186);
                    rKimlik.SIRANO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 682, 500, 318, 181);
                    rKimlik.VERILDIGIYER = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 68, 670, 473, 176);
                    rKimlik.VERILISNEDENI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 522, 664, 478, 179);
                    rKimlik.KAYITNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 67, 832, 474, 168);
                    rKimlik.VERILISTARIHI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 520, 823, 480, 177);
                    if (ScanObjEnum == Enums.ScanObject.TcKimlik)
                    {
                        Mat modelImageArkaTamami = CvInvoke.Imread("Extensions/nufuscuzdaniArkaTamami1.jpg", LoadImageType.AnyColor);
                        Image imgArkaTamami = DrawMatchess.Draw(modelImageArkaTamami, observedImage, out matchTime, ScanObjEnum);
                        BackImage = imgArkaTamami;
                    }
                    else
                    {
                        BackImage = img;
                    }
                    return rKimlik;
                }
            }
        }

        public static eKimlik ImageHelperBackErp(Bitmap file, Enums.ScanObject ScanObjEnum)
        {
            try
            {
                if (rKimlik == null)
                {
                    rKimlik = new eKimlik();
                }
                using (Mat modelImage = GetMatBack(ScanObjEnum))
                {
                    using (Image<Bgr, byte> observedImage = new Image<Bgr, byte>(file))
                    {
                        long matchTime;
                        Image img = (BackImage = DrawMatchess.Draw(modelImage, observedImage.Mat, out matchTime, ScanObjEnum, backSide: true));
                        Bitmap cropImg = (Bitmap)img;
                        switch (ScanObjEnum)
                        {
                            case Enums.ScanObject.TcKimlik:
                                {
                                    rKimlik.MEDENIHAL = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 1, 1, 360, 198);
                                    rKimlik.DIN = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 331, 0, 365, 191);
                                    rKimlik.KANGRUBU = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 663, 0, 337, 196);
                                    rKimlik.IL = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 63, 178, 483, 185);
                                    rKimlik.ILCE = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 513, 176, 487, 182);
                                    rKimlik.MAHALLEKOY = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 68, 342, 932, 182);
                                    rKimlik.CILTNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 65, 507, 325, 181);
                                    rKimlik.AILESIRANO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 366, 500, 339, 186);
                                    rKimlik.SIRANO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 682, 500, 318, 181);
                                    rKimlik.VERILDIGIYER = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 68, 670, 473, 176);
                                    rKimlik.VERILISNEDENI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 522, 664, 478, 179);
                                    rKimlik.KAYITNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 67, 832, 474, 168);
                                    rKimlik.VERILISTARIHI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 520, 823, 480, 177);
                                    //Image<Bgr, byte> myImageTCkimlikBack = new Image<Bgr, byte>(Resources.nufuscuzdaniArkaTamami1);
                                    //Mat modelImageArkaTamami = myImageTCkimlikBack.Mat;
                                    //Image imgArkaTamami = DrawMatchess.Draw(modelImageArkaTamami, observedImage.Mat, out matchTime, ScanObjEnum);
                                    //BackImage = imgArkaTamami;
                                    break;
                                }
                            case Enums.ScanObject.YeniTcKimlik:
                                rKimlik.ANAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 273, 188, 673, 68);
                                rKimlik.BABAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 276, 278, 686, 60);
                                break;
                            case Enums.ScanObject.Ehliyet:
                                rKimlik.IL = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 240, 242, 685, 84);
                                rKimlik.ILCE = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 240, 242, 685, 84);
                                rKimlik.MAHALLEKOY = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 17, 322, 991, 81);
                                rKimlik.CILTNO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 8, 386, 343, 83);
                                rKimlik.AILESIRANO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 562, 384, 375, 79);
                                rKimlik.SIRANO = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 319, 384, 277, 79);
                                rKimlik.KANGRUBU = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 6, 598, 260, 146);
                                rKimlik.BABAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 15, 459, 482, 81);
                                rKimlik.ANAADI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 463, 452, 483, 95);
                                rKimlik.DOGUMYERI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 15, 540, 918, 67);
                                rKimlik.DOGUMTARIHI = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 15, 540, 918, 67);
                                BackImage = img;
                                break;
                            case Enums.ScanObject.YeniEhliyet:
                                rKimlik.KANGRUBU = DrawMatchess.GetImagePoint(cropImg, ScanObjEnum, 55, 1, 183, 101);
                                break;
                        }
                        return rKimlik;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static void SaveImage(string TcKimlikNo, string FileSource)
        {
            try
            {
                Image newImg = new Bitmap(800, 600);
                using (Graphics g = Graphics.FromImage(newImg))
                {
                    g.Clear(Color.White);
                    if (FrontImage != null)
                    {
                        g.DrawImage(FrontImage, 0, 0, 395, 600);
                    }
                    if (BackImage != null)
                    {
                        g.DrawImage(BackImage, 405, 0, 395, 600);
                    }
                }
                newImg.Save(FileSource + "\\" + TcKimlikNo.Trim() + ".jpg", ImageFormat.Jpeg);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public static Image SendImage(bool FrontAndBack)
        {
            if (FrontAndBack)
            {
                return FrontImage;
            }
            return BackImage;
        }

        public static byte[][] GetRGB(Bitmap bmp)
        {
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmp_data.Scan0;
            int num_pixels = bmp.Width * bmp.Height;
            int num_bytes = bmp_data.Stride * bmp.Height;
            int padding = bmp_data.Stride - bmp.Width * 3;
            int i = 0;
            int ct = 1;
            byte[] r = new byte[num_pixels];
            byte[] g = new byte[num_pixels];
            byte[] b = new byte[num_pixels];
            byte[] rgb = new byte[num_bytes];
            Marshal.Copy(ptr, rgb, 0, num_bytes);
            for (int x = 0; x < num_bytes - 3; x += 3)
            {
                if (x == bmp_data.Stride * ct - padding)
                {
                    x += padding;
                    ct++;
                }
                r[i] = rgb[x];
                g[i] = rgb[x + 1];
                b[i] = rgb[x + 2];
                i++;
            }
            bmp.UnlockBits(bmp_data);
            return new byte[3][] { r, g, b };
        }

        public static Image AutoCrop(Bitmap bmp)
        {
            byte[][] pixels = GetRGB(bmp);
            int h = bmp.Height - 1;
            int w = bmp.Width;
            int top = 0;
            int bottom = h;
            int left = bmp.Width;
            int right = 0;
            int white = 0;
            int tolerance = 30;
            bool prev_color = false;
            for (int i = 0; i < pixels[0].Length; i++)
            {
                int x = i % w;
                int y = (int)Math.Floor((decimal)(i / w));
                int tol = 255 * tolerance / 100;
                if (pixels[0][i] >= tol && pixels[1][i] >= tol && pixels[2][i] >= tol)
                {
                    white++;
                    right = ((x > right && white == 1) ? x : right);
                }
                else
                {
                    left = ((x < left && white >= 1) ? x : left);
                    right = ((x == w - 1 && white == 0) ? (w - 1) : right);
                    white = 0;
                }
                if (white == w)
                {
                    top = ((y - top < 3) ? y : top);
                    bottom = ((prev_color && x == w - 1 && y > top + 1) ? y : bottom);
                }
                left = ((x != 0 || white != 0) ? left : 0);
                bottom = ((y == h && x == w - 1 && white != w && prev_color) ? (h + 1) : bottom);
                if (x == w - 1)
                {
                    prev_color = ((white < w) ? true : false);
                    white = 0;
                }
            }
            right = ((right == 0) ? w : right);
            left = ((left != w) ? left : 0);
            if (bottom - top > 0)
            {
                Bitmap bmpCrop = bmp.Clone(new Rectangle(left, top, right - left + 1, bottom - top), bmp.PixelFormat);
                bmpCrop = new Bitmap(bmpCrop, 1000, 1000);
                rKimlik.TCKIMLIKNO = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 1, 523, 999, 99);
                rKimlik.SERI = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 1, 447, 482, 92);
                rKimlik.SERI = CropImgSERI((Bitmap)rKimlik.SERI);
                rKimlik.NO = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 557, 440, 443, 105);
                rKimlik.SOYADI = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 1, 601, 999, 97);
                rKimlik.ADI = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 1, 684, 999, 92);
                rKimlik.BABAADI = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 1, 758, 999, 97);
                rKimlik.ANAADI = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 1, 839, 999, 94);
                rKimlik.DOGUMYERI = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 1, 911, 508, 89);
                rKimlik.DOGUMTARIHI = DrawMatchess.GetImagePoint(bmpCrop, ScanObjEnums, 493, 915, 507, 85);
                return bmpCrop;
            }
            return bmp;
        }
    }
}
