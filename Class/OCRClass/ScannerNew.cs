using DevExpress.XtraSplashScreen;
using FluentFTP;
using NTwain;
using NTwain.Data;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
    public class eBarcodeInformation
    {
        public string DocumentType { get; set; }

        public string DocumentID { get; set; }

        public string Page { get; set; }

        public string TotalPage { get; set; }

        public Bitmap Image { get; set; }
    }
    public class ScannerNew
    {
        private ImageCodecInfo _jpegCodecInfo;

        private TwainSession _twain;

        private List<eBarcodeInformation> lBarcodeInformation = new List<eBarcodeInformation>();

        private List<Bitmap> images = new List<Bitmap>();

        public bool isFinished = false;

        private string curID = "";

        private FtpClient _ftpClient = null;

        public List<eBarcodeInformation> successfullyScannedDocuments = new List<eBarcodeInformation>();

        public List<eBarcodeInformation> unknownDocuments = new List<eBarcodeInformation>();

        public string OperationInformation = "";

        private bool forProduct = false;

        private List<string> scanners = new List<string>();
        public List<string> GetAllScanners()
        {
            try
            {
                TWIdentity appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetEntryAssembly());
                _twain = new TwainSession(appId);
                _twain.Open();
                if (_twain.State >= 3)
                {
                    scanners.Clear();
                    foreach (DataSource item in _twain)
                    {
                        if (!item.Name.Contains("WIA"))
                        {
                            scanners.Add(item.Name);
                        }
                    }
                }
                if (_twain.State == 4)
                {
                    _twain.CurrentSource.Close();
                }
                if (_twain.State == 3)
                {
                    _twain.Close();
                }
                if (_twain.State > 2)
                {
                    _twain.ForceStepDown(2);
                }
                return scanners;
            }
            catch (Exception)
            {
                if (_twain != null)
                {
                    _twain.Close();
                }
                return scanners;
            }
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
        public void DocumentScannerNew(FtpClient ftpClient, string curID, bool forProduct = false)
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
            this.curID = curID;
            this.forProduct = forProduct;
        }

        public void DocumentCreateCustomer()
        {
            try
            {
                if (!string.IsNullOrEmpty(curID))
                {
                    _ftpClient.UploadRateLimit = 0u;
                    _ftpClient.EnableThreadSafeDataConnections = true;
                    _ftpClient.ConnectTimeout = 60000;
                    _ftpClient.DataConnectionConnectTimeout = 60000;
                    _ftpClient.SocketPollInterval = 0;
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Araştırmalar/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Faturalar/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Sözleşmeler/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Teslimatlar/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/İrsaliye/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Sevk Belgeleri/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Arıza Belgeleri/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/KVKK Belgeleri/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Müşteriye Özel Belgeler/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Yüklenen Dosyalar/", force: true);
                    _ftpClient.Connect();
                    _ftpClient.CreateDirectory(curID + "/Teklifler/", force: true);
                    _ftpClient.Connect();
                }
            }
            catch (FtpException fe)
            {
                //MessageForm.Message("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                //ExceptionForm.Message(exp, this);
            }
            finally
            {
                _ftpClient.Disconnect();
            }
        }

        public void DocumentCreateCustomer(string curID, string folderName)
        {
            try
            {
                _ftpClient.UploadRateLimit = 0u;
                _ftpClient.EnableThreadSafeDataConnections = true;
                _ftpClient.ConnectTimeout = 60000;
                _ftpClient.DataConnectionConnectTimeout = 60000;
                _ftpClient.SocketPollInterval = 0;
                _ftpClient.Connect();
                _ftpClient.CreateDirectory(curID + "/" + folderName + "/",  true);
            }
            catch (FtpException fe)
            {
                //MessageForm.Message("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                //ExceptionForm.Message(exp, this);
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
                            if (_twain.CurrentSource.Capabilities.CapUIControllable.IsSupported && _twain.CurrentSource.Enable(SourceEnableMode.NoUI, false, IntPtr.Zero) == ReturnCode.Success)
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
                            //MessageForm.Message("Tarayıcıya erişilemiyor");
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
                src.Capabilities.ICapXResolution.SetValue(400F);
                src.Capabilities.ICapYResolution.SetValue(400F);
            }
        }

        private void Twain_ScanningComplete()
        {
            //SplashScreenManager.ShowForm(null, typeof(OpeningForm), useFadeIn: true, useFadeOut: true, throwExceptionIfAlreadyOpened: false);
            _ftpClient.UploadRateLimit = 0u;
            _ftpClient.EnableThreadSafeDataConnections = true;
            _ftpClient.ConnectTimeout = 60000;
            _ftpClient.DataConnectionConnectTimeout = 60000;
            _ftpClient.SocketPollInterval = 0;
            _ftpClient.Connect();
            try
            {
                List<eBarcodeInformation> distinctList = new List<eBarcodeInformation>();
                foreach (eBarcodeInformation item2 in lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType != null))
                {
                    if (distinctList.FirstOrDefault((eBarcodeInformation x) => x.DocumentID == item2.DocumentID && x.DocumentType == item2.DocumentType) == null)
                    {
                        distinctList.Add(item2);
                    }
                }
                foreach (eBarcodeInformation item3 in lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentID == null))
                {
                    distinctList.Add(item3);
                }
                foreach (eBarcodeInformation item in distinctList)
                {
                    string date = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond;
                    PdfDocument pdf = new PdfDocument();
                    if (item.DocumentID != null && item.DocumentType != null)
                    {
                        foreach (eBarcodeInformation document in from x in lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentID == item.DocumentID && x.DocumentType == item.DocumentType).ToList()
                                                                 orderby x.Page
                                                                 select x)
                        {
                            PdfPage newPage = new PdfPage();
                            pdf.Pages.Add(newPage);
                            newPage.Size = PageSize.A4;
                            XGraphics xgr = XGraphics.FromPdfPage(pdf.Pages[pdf.PageCount - 1]);
                            int newWidth = 768;
                            int newHeight = 1024;
                            Bitmap bitmap = ResizeImage(document.Image, newWidth, newHeight);
                            if (bitmap != null)
                            {
                                XImage img = XImage.FromGdiPlusImage(bitmap);
                                newPage.Width = XUnit.FromPoint(img.Size.Width);
                                newPage.Height = XUnit.FromPoint(img.Size.Height);
                                xgr.DrawImage(bitmap, 0.0, 0.0, img.Size.Width, img.Size.Height);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            PdfPage newPage2 = new PdfPage();
                            pdf.Pages.Add(newPage2);
                            newPage2.Size = PageSize.A4;
                            XGraphics xgr2 = XGraphics.FromPdfPage(pdf.Pages[pdf.PageCount - 1]);
                            int newWidth2 = 768;
                            int newHeight2 = 1024;
                            Bitmap bitmap2 = ResizeImage(item.Image, newWidth2, newHeight2);
                            if (bitmap2 != null)
                            {
                                XImage img2 = XImage.FromGdiPlusImage(bitmap2);
                                newPage2.Width = XUnit.FromPoint(img2.Size.Width);
                                newPage2.Height = XUnit.FromPoint(img2.Size.Height);
                                xgr2.DrawImage(bitmap2, 0.0, 0.0, img2.Size.Width, img2.Size.Height);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    using (MemoryStream fileStream = new MemoryStream())
                    {
                        pdf.Save(fileStream, closeStream: false);
                        string curId = null;
                        #region VolantKodu
                        //switch (item.DocumentType)
                        //{
                        //    case "10":
                        //        {
                        //            if (item.DocumentID == null)
                        //            {
                        //                break;
                        //            }
                        //            uDivSales data13 = new cSales().GetByPkForUI(Convert.ToInt64(item.DocumentID));
                        //            if (data13 != null)
                        //            {
                        //                curId = data13.curID.ToString();
                        //                DocumentCreateCustomer(curId, "Araştırmalar");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Araştırmalar/Araştırma " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data13.curName + ": Araştırma " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //            break;
                        //        }
                        //    case "11":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eSalesInvoice data12 = new cSalesInvoice().GetBySalIDForSelectOne(Convert.ToInt64(item.DocumentID));
                        //            if (data12 != null)
                        //            {
                        //                eSales sales = new cSales().GetByPk(data12.SALINVSALID);
                        //                if (sales != null)
                        //                {
                        //                    curId = sales.SALCURID.ToString();
                        //                    DocumentCreateCustomer(curId, "Faturalar");
                        //                    _ftpClient.Disconnect();
                        //                    _ftpClient.Connect();
                        //                    _ftpClient.Upload(fileStream, curId + "/Faturalar/SatışFaturası " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                    successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                    OperationInformation = OperationInformation + "\r\n" + sales.curAdi + ": SatışFaturası " + item.DocumentID + " " + date + ".pdf";
                        //                }
                        //                else
                        //                {
                        //                    unknownDocuments.Add(item);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "12":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eSales data11 = new cSales().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data11 == null)
                        //            {
                        //                data11 = new cSales().GetByPreFormNo(item.DocumentID);
                        //            }
                        //            if (data11 != null)
                        //            {
                        //                curId = data11.SALCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Sözleşmeler");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Sözleşmeler/SatışSözleşmesi " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data11.curAdi + ": SatışSözleşmesi " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "13":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eSales data10 = new cSales().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data10 != null)
                        //            {
                        //                curId = data10.SALCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Sözleşmeler");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Sözleşmeler/Senet " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data10.curAdi + ": Senet " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "14":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eDeeds data9 = new cDeeds().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data9 != null)
                        //            {
                        //                curId = data9.DEEDCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Teslimatlar");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Teslimatlar/Teslimat " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data9.cariAdi + ": Teslimat " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "15":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eDeeds data8 = new cDeeds().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data8 != null)
                        //            {
                        //                curId = data8.DEEDCURID.ToString();
                        //                DocumentCreateCustomer(curId, "İrsaliye");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/İrsaliye/İrsaliye " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data8.cariAdi + ": İrsaliye " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "16":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eCusDeliver data7 = new cCusDeliver().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data7 != null)
                        //            {
                        //                curId = data7.CDRCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Sevk Belgeleri");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Sevk Belgeleri/Sevk Belgesi " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data7.curName + ": SevkBelgelesi " + item.DocumentID + " " + date + ".pdf";
                        //                if (StartupExtension.rInitiations.rManagement.MTKURUMSTS == "01")
                        //                {
                        //                    new cCusDeliver().UpdateForAgreedByDeedId(new List<eCusDeliver> { data7 }, agree: true);
                        //                }
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "17":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eSsh data6 = new cSsh().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data6 != null)
                        //            {
                        //                curId = data6.SSHCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Arıza Belgeleri");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Arıza Belgeleri/Arıza Belgesi " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data6.musteriAdi + ": ArızaBelgelesi " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "18":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eSales data5 = new cSales().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data5 != null)
                        //            {
                        //                curId = data5.SALCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Sözleşmeler");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Sözleşmeler/TeslimEvragi " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data5.curAdi + ": TeslimEvragi " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "19":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eCurrents data4 = new cCurrents().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data4 != null)
                        //            {
                        //                curId = data4.CURID.ToString();
                        //                DocumentCreateCustomer(curId, "KVKK Belgeleri");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/KVKK Belgeleri/KVKK Belgesi " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data4.CURNAME + ": KVKKBelgesi " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "YON":
                        //        if (item.DocumentID != null)
                        //        {
                        //            eSales data3 = new cSales().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data3 == null)
                        //            {
                        //                data3 = new cSales().GetByPreFormNo("12 " + item.DocumentID);
                        //            }
                        //            if (data3 != null)
                        //            {
                        //                curId = data3.SALCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Sözleşmeler");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Sözleşmeler/SatışSözleşmesi " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data3.curAdi + ": SatışSözleşmesi " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    case "21":
                        //        if (item.DocumentID != null)
                        //        {
                        //            ePrdOffer data2 = new cPrdOffer().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data2 != null)
                        //            {
                        //                curId = data2.PRDOCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Teklifler");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Teklifler/Teklif " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data2.curName + ": Teklif " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            unknownDocuments.Add(item);
                        //        }
                        //        break;
                        //    default:
                        //        if ((StartupExtension.rInitiations.rManagement.MTWHO == "YONAVM" || StartupExtension.rInitiations.rManagement.MTWHO == "KAMALAR") && item.DocumentID != null)
                        //        {
                        //            eSales data = new cSales().GetByPk(Convert.ToInt64(item.DocumentID));
                        //            if (data == null)
                        //            {
                        //                data = new cSales().GetByPreFormNo(item.DocumentID);
                        //            }
                        //            if (data != null)
                        //            {
                        //                curId = data.SALCURID.ToString();
                        //                DocumentCreateCustomer(curId, "Sözleşmeler");
                        //                _ftpClient.Disconnect();
                        //                _ftpClient.Connect();
                        //                _ftpClient.Upload(fileStream, curId + "/Sözleşmeler/SatışSözleşmesi " + item.DocumentID + " " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                        //                successfullyScannedDocuments.AddRange(lBarcodeInformation.Where((eBarcodeInformation x) => x.DocumentType == item.DocumentType && x.DocumentID == item.DocumentID));
                        //                OperationInformation = OperationInformation + "\r\n" + data.curAdi + ": SatışSözleşmesi " + item.DocumentID + " " + date + ".pdf";
                        //            }
                        //            else
                        //            {
                        //                unknownDocuments.Add(item);
                        //            }
                        //        }                                
                        //        break;
                        //}
                        #endregion
                        if ((item.DocumentType != null || item.DocumentID != null) && curID == null && curId == null)
                        {
                            unknownDocuments.Add(item);
                        }
                        else if (curID != null && curID != "")
                        {
                            if (!forProduct)
                            {
                                DocumentCreateCustomer(curID, "Müşteriye Özel Belgeler");
                                _ftpClient.Disconnect();
                                _ftpClient.Connect();
                                _ftpClient.Upload(fileStream, curID + "/Müşteriye Özel Belgeler/Özel Belge " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                                item.DocumentType = "Döküman Algılanamadı,Müşteriye Özel Belgeler klasörüne atıldı";
                            }
                            else if (forProduct)
                            {
                                _ftpClient.Upload(fileStream, "Ürünler/" + curID + "/Taranan Dosyalar/Ürün Belgesi " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                            }
                            successfullyScannedDocuments.Add(item);
                            if (!forProduct)
                            {
                                unknownDocuments.Add(item);
                            }
                        }
                        else if (curId != null && curId != "")
                        {
                            DocumentCreateCustomer(curID, "Müşteriye Özel Belgeler");
                            _ftpClient.Disconnect();
                            _ftpClient.Connect();
                            _ftpClient.Upload(fileStream, curId + "/Müşteriye Özel Belgeler/Özel Belge " + date + ".pdf", FtpExists.Overwrite, createRemoteDir: true);
                            item.DocumentType = "Döküman Algılanamadı,Müşteriye Özel Belgeler klasörüne atıldı";
                            successfullyScannedDocuments.Add(item);
                            unknownDocuments.Add(item);
                        }
                        else
                        {
                            unknownDocuments.Add(item);
                        }
                        pdf.Close();
                    }
                }
            }
            catch (FtpException fe)
            {
                //MessageForm.Message("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + fe.Message);
            }
            catch (Exception exp)
            {
                //MessageForm.Message("Dosya yüklenirken bir hata ile karşılaşıldı, yükleme iptal edildi! " + exp.Message);
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
                if (images.Contains(bitmap))
                {
                    return;
                }
                IBarcodeReader reader = new BarcodeReader
                {
                    AutoRotate = true,
                    TryInverted = true,
                    Options = new DecodingOptions
                    {
                        TryHarder = true,
                        PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.QR_CODE,
                        BarcodeFormat.CODE_128
                    }
                    }
                };
                Result[] qrs = reader.DecodeMultiple(bitmap);
                List<Result> results = new List<Result>();
                if (qrs != null)
                {
                    results = qrs.ToList();
                }
                if (results != null && results.Count > 0)
                {
                    foreach (Result item in results)
                    {
                        try
                        {
                            string[] splited = item.Text.Split(' ');
                            if (!images.Contains(bitmap))
                            {
                                if (splited.Length == 1)
                                {
                                    lBarcodeInformation.Add(new eBarcodeInformation
                                    {
                                        DocumentType = "YON",
                                        DocumentID = splited[0],
                                        Page = null,
                                        TotalPage = null,
                                        Image = (bitmap.Clone() as Bitmap)
                                    });
                                }
                                else
                                {
                                    lBarcodeInformation.Add(new eBarcodeInformation
                                    {
                                        DocumentType = splited[0],
                                        DocumentID = splited[1],
                                        Page = splited[2],
                                        TotalPage = splited[3],
                                        Image = (bitmap.Clone() as Bitmap)
                                    });
                                }
                                images.Add(bitmap);
                            }
                        }
                        catch
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
                    return;
                }
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
    }
}
