using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwainDotNet;
using TwainDotNet.WinFroms;
using ZXing;
using static EntegreFDLL.Class.Siniflar;

namespace EntegrefKrediOnay.Class
{

    public class DocumentScan
    {
        public int MaxScanLimit { get; set; } = 5; // Varsayılan 5 tarama
        private Twain _twain;
        private List<ScanResult> _scanResults;
        private IWindowsMessageHook _messageHook;
        private Form _ownerForm;
        private TaskCompletionSource<List<ScanResult>> _tcs;
        private bool _limitReached = false;
        public string ImagePath { get; set; }
        public string PdfPath { get; set; }
        public DocumentScan(Form ownerForm)
        {
            _ownerForm = ownerForm;
            _scanResults = new List<ScanResult>(); if (!IsTwainAvailableSafely(_ownerForm))
            {
                MessageBox.Show("Sistemde tarayıcı algılanamadı. Tarama özelliğini kullanmak için lütfen TWAIN sürücüsünü yükleyin.",
                                "Tarayıcı Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _messageHook = new WinFormsWindowMessageHook(_ownerForm);
                _twain = new Twain(_messageHook);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Twain başlatılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _twain.TransferImage += (s, e) =>
            {
                try
                {
                    if (e.Image != null)
                    {
                        if (_scanResults.Count >= MaxScanLimit)
                        {
                            _limitReached = true;
                            return;
                        }
                        else
                        {
                            _limitReached = false;
                            var image = new Bitmap(e.Image);
                            var Barkod = ReadBarcode(image);
                            var scanResult = new ScanResult
                            {
                                Image = image,
                                BarcodeValue = Barkod
                            };
                            _scanResults.Add(scanResult);
                            SaveImageAsPng(image, Barkod);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"TransferImage hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            };

            _twain.ScanningComplete += (s, e) =>
            {
                try
                {
                    _tcs?.TrySetResult(_scanResults);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ScanningComplete hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private bool IsTwainAvailableSafely(Form form)
        {
            try
            {
                using (Form dummyForm = new Form())
                {
                    var hook = new WinFormsWindowMessageHook(dummyForm);
                    var twain = new Twain(hook);
                    var sources = twain.SourceNames;

                    return sources != null && sources.Count > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TWAIN kontrolü sırasında hata: " + ex.Message);
                return false;
            }
        }
        public async Task<List<ScanResult>> StartScanAsync(bool Show, bool feeder)
        {
            try
            {
                _scanResults.Clear();
                _tcs = new TaskCompletionSource<List<ScanResult>>();

                _twain.SelectSource();

                var scanSettings = new ScanSettings
                {
                    UseDocumentFeeder = feeder,
                    ShowTwainUI = Show,
                    ShowProgressIndicatorUI = true,
                    UseDuplex = Properties.Settings.Default.ScanDublex,
                    Resolution = new ResolutionSettings
                    {
                        Dpi = Properties.Settings.Default.ScanDpi,
                        ColourSetting = ColourSetting.Colour
                    }
                };

                _twain.StartScanning(scanSettings);

                // Tarama bitene kadar bekle
                return await _tcs.Task;
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                //CustomMessageBox.ShowMessage("Hata Detayı", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }
        private string ReadBarcode(Bitmap image)
        {
            var reader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.QR_CODE,
                        BarcodeFormat.CODE_128
                    }
                }
            };

            var result = reader.Decode(image);
            return result?.Text ?? "[Barkod bulunamadı]";
        }
        private void SaveImageAsPng(Bitmap image, string _brkod)
        {
            var resim = image; // ResizeImageWithAspectRatio(image, 800, 1000);
            try
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scans");
                Directory.CreateDirectory(directory);

                string fileName = Path.Combine(directory, $"Scan_{_brkod}.png");
                resim.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + "Görüntü kaydedilirken hata oluştu: " + ex.Message + Environment.NewLine);
            }
        }
        private Image ResizeImageWithAspectRatio(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return resizedBitmap;
        }

    }
}
