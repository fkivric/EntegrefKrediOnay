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
using System.IO;
using System.Diagnostics;
using EntegreFDLL.Class.AI;
using EntegreFDLL.Class;
using EntegrefKrediOnay.Class;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmBGAISonuc : DevExpress.XtraEditors.XtraForm
    {
        public static string Sonuc;
        public static string Musteri;
        decimal kefilTutar = 0;
        string Risk = "";
        eSalesConfirm data;
        public frmBGAISonuc(string _sonuc, string _mus, eSalesConfirm _data, decimal _kf, string _rs)
        {
            InitializeComponent();
            Sonuc = _sonuc;
            Musteri = _mus;
            data = _data;
            Risk = _rs;
            kefilTutar = _kf;
        }

        private void frmBGAISonuc_Load(object sender, EventArgs e)
        {
            DevExpress.XtraRichEdit.API.Native.Document document;
            Program.EntegreFIAConfigProvider.Doc = richEditControl1.Document;
            Program.EntegreFIAConfigProvider.SalesConfirm = data;
            Program.EntegreFIAConfigProvider.aiOutput = Sonuc;
            Program.EntegreFIAConfigProvider.KefilTutar = kefilTutar;
            Program.EntegreFIAConfigProvider.Risk = Risk;
            var rebder = SalesConfirmRichEditRenderer.Render2(Program.EntegreFIAConfigProvider);
            document = rebder;
        }
        static int GetFileCountInFolder(string folderPath)
        {
            int adet = 0;
            try
            {
                // Belirtilen klasördeki dosyaları al
                string[] files = Directory.GetFiles(folderPath);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains("EntegreF AI_"))
                    {
                        adet++;
                    }
                }
                // Dosya sayısını döndür
                return adet;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
                return 0; // Hata durumunda -1 döndür
            }
        }
        private async void tileBarItem8_ItemClick(object sender, TileItemEventArgs e)
        {
            string yapi = "";

            yapi = "1";
            await Class.SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
            async (progress, token) =>
            {
                progress.Report((0, $"Yapay zeka sonucu kaydediliyor"));
                await Task.Delay(10, token);
                token.ThrowIfCancellationRequested();
                try
                {
                    Entegref.CreateDirectoryIfNotExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Satis Kredi Onay İşlemleri", data.musteriKodu));
                    string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Satis Kredi Onay İşlemleri", data.musteriKodu);
                    var sira = GetFileCountInFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EntegreF", "Satis Kredi Onay İşlemleri", data.musteriKodu));
                    string filename = "EntegreF AI_" + Musteri + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + yapi + "_Analiz Sonucu_" + sira + ".pdf";
                    string filePath = Path.Combine(FolderPath, filename);
                    richEditControl1.ExportToPdf(filePath);
                }
                catch (Exception ex)
                {
                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
            //try
            //{
            //    Process.Start(filePath);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("PDF açılamadı: " + ex.Message);
            //}
        }
    }
}