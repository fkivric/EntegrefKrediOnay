using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public static class DrawMatchess
    {
        public static Image img;

        public static Bitmap CroppedImage;

        public static void FindMatch(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {
            int i = 2;
            double uniquenessThreshold = 0.8;
            double hessianThresh = 300.0;
            homography = null;
            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();
            Stopwatch watch;
            if (CudaInvoke.HasCuda)
            {
                CudaSURF surfCuda = new CudaSURF((float)hessianThresh);
                using (GpuMat gpuModelImage = new GpuMat(modelImage))
                {
                    using (GpuMat gpuModelKeyPoints = surfCuda.DetectKeyPointsRaw(gpuModelImage))
                    {
                        using (GpuMat gpuModelDescriptors = surfCuda.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                        {
                            using (CudaBFMatcher matcher = new CudaBFMatcher(DistanceType.L2))
                            {
                                surfCuda.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                                watch = Stopwatch.StartNew();
                                using (GpuMat gpuObservedImage = new GpuMat(observedImage))
                                {
                                    using (GpuMat gpuObservedKeyPoints = surfCuda.DetectKeyPointsRaw(gpuObservedImage))
                                    {
                                        using (GpuMat gpuObservedDescriptors = surfCuda.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                                        {
                                            matcher.KnnMatch(gpuObservedDescriptors, gpuModelDescriptors, matches, i);
                                            surfCuda.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);
                                            mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                                            mask.SetTo(new MCvScalar(255.0));
                                            Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);
                                            int nonZeroCount = CvInvoke.CountNonZero(mask);
                                            if (nonZeroCount >= 4)
                                            {
                                                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);
                                                if (nonZeroCount >= 4)
                                                {
                                                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2.0);
                                                }
                                            }
                                        }
                                    }
                                }
                                watch.Stop();
                            }
                        }
                    }
                }
            }
            else
            {
                UMat uModelImage = modelImage.ToUMat(AccessType.Fast);
                UMat uObservedImage = observedImage.ToUMat(AccessType.Fast);
                SURF surfCPU = new SURF(hessianThresh);
                UMat modelDescriptors = new UMat();
                surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, useProvidedKeyPoints: false);
                watch = Stopwatch.StartNew();
                UMat observedDescriptors = new UMat();
                surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, useProvidedKeyPoints: false);
                BFMatcher matcher2 = new BFMatcher(DistanceType.L2);
                matcher2.Add(modelDescriptors);
                matcher2.KnnMatch(observedDescriptors, matches, i, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255.0));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);
                int nonZeroCount2 = CvInvoke.CountNonZero(mask);
                if (nonZeroCount2 >= 4)
                {
                    nonZeroCount2 = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);
                    if (nonZeroCount2 >= 4)
                    {
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2.0);
                    }
                }
                watch.Stop();
            }
            matchTime = watch.ElapsedMilliseconds;
        }

        public static Image Draw(Mat modelImage, Mat observedImage, out long matchTime, Enums.ScanObject ScanObjEnums, bool backSide = false)
        {
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                FindMatch(modelImage.Clone(), observedImage.Clone(), out matchTime, out var modelKeyPoints, out var observedKeyPoints, matches, out var mask, out var homography);
                Mat result = new Mat();
                try
                {
                    if (!(modelImage.Ptr != IntPtr.Zero))
                    {
                        throw new AccessViolationException();
                    }
                    Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints, matches, result, new MCvScalar(255.0, 255.0, 255.0), new MCvScalar(255.0, 255.0, 255.0), mask);
                }
                catch
                {
                    modelImage = (backSide ? ImageHelper.GetMatBack(ScanObjEnums) : ImageHelper.GetMatFront(ScanObjEnums));
                    Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints, matches, result, new MCvScalar(255.0, 255.0, 255.0), new MCvScalar(255.0, 255.0, 255.0), mask);
                }
                if (homography != null)
                {
                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
                    PointF[] pts = new PointF[4]
                    {
                    new PointF(rect.Left, rect.Bottom),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.Right, rect.Top),
                    new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);
                    Point[] points = Array.ConvertAll(pts, Point.Round);
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);

                        //CvInvoke.Polylines(result, vp, isClosed: true, new MCvScalar(255,0,0,255), 5);
                    }
                    Bitmap bmp = observedImage.Bitmap;
                    img = GetImage(points, bmp, ScanObjEnums);
                    Bitmap cropImg = (Bitmap)img;
                }
                return img;
            }
        }

        public static Image GetImage(Point[] points, Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = Math.Min(points[1].X, points[3].X);
            int y = Math.Min(points[1].Y, points[3].Y);
            int width = Math.Abs(points[1].X - points[3].X);
            int height = Math.Abs(points[1].Y - points[3].Y);
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    {
                        Bitmap bmp = new Bitmap(img, 1000, 1000);
                        bmp.SetResolution(300f, 300f);
                        img2 = bmp;
                        break;
                    }
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointTCKIMLIK(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 1;
            int y = 523;
            int width = 999;
            int height = 99;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 999, 99);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointSERI(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 305;
            int y = 463;
            int width = 174;
            int height = 75;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 300, 92);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointSERINO(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 557;
            int y = 440;
            int width = 443;
            int height = 105;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 443, 105);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointSOYADI(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 1;
            int y = 601;
            int width = 999;
            int height = 97;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 999, 97);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointADI(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 1;
            int y = 684;
            int width = 999;
            int height = 92;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 999, 92);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointBABA(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 1;
            int y = 758;
            int width = 999;
            int height = 97;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 999, 97);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointANA(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 1;
            int y = 839;
            int width = 999;
            int height = 94;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 999, 94);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointDOGUMYERI(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 1;
            int y = 911;
            int width = 508;
            int height = 89;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 508, 89);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePointDOGUMTARIHI(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            int x = 493;
            int y = 915;
            int width = 507;
            int height = 85;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    img2 = new Bitmap(img, 507, 85);
                    break;
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, 1024, 768);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }

        public static Image GetImagePoint(Bitmap DisplayImage, Enums.ScanObject ScanObjEnums, int x, int y, int width, int height)
        {
            CroppedImage = DisplayImage.Clone() as Bitmap;
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);
            DisplayImage = new Bitmap(width, height);
            Graphics DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            Image img = DisplayImage;
            Image img2 = null;
            switch (ScanObjEnums)
            {
                case Enums.ScanObject.TcKimlik:
                    {
                        Bitmap bmp = new Bitmap(img, width, height);
                        bmp.SetResolution(300f, 300f);
                        img2 = bmp;
                        break;
                    }
                case Enums.ScanObject.YeniTcKimlik:
                    img2 = new Bitmap(img, width + 50, height);
                    break;
                case Enums.ScanObject.Ehliyet:
                    img2 = new Bitmap(img, width, height);
                    break;
                case Enums.ScanObject.YeniEhliyet:
                    img2 = new Bitmap(img, width, height);
                    break;
                case Enums.ScanObject.Pasaport:
                    img2 = new Bitmap(img, 1000, 1000);
                    break;
            }
            return img2;
        }
    }
}
