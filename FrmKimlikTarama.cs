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

namespace EntegrefKrediOnay
{
    public partial class FrmKimlikTarama : DevExpress.XtraEditors.XtraForm
    {
        public FrmKimlikTarama()
        {
            InitializeComponent();
        }

        private Vol.Scanner.TwainScanner scanner;
        private void FrmKimlikTarama_Load(object sender, EventArgs e)
        {
            try
            {
                // Belge tipini seç
                Volant.Ocr.Dev.Extensions.Enums.ScanObject scanObject = Volant.Ocr.Dev.Extensions.Enums.ScanObject.TcKimlik;
                switch (comboBoxBelge.Text)
                {
                    case "Kimlik": scanObject = Volant.Ocr.Dev.Extensions.Enums.ScanObject.TcKimlik; break;
                    case "Yeni Kimlik": scanObject = Volant.Ocr.Dev.Extensions.Enums.ScanObject.YeniTcKimlik; break;
                    case "Ehliyet": scanObject = Volant.Ocr.Dev.Extensions.Enums.ScanObject.YeniEhliyet; break;
                    case "Pasaport": scanObject = Volant.Ocr.Dev.Extensions.Enums.ScanObject.Pasaport; break;
                }

                // Tarayıcı başlat
                scanner = new Vol.Scanner.TwainScanner();
                scanner.ScanIdentity("TarayıcıAdı", scanObject);

                // Tarama tamamlanana kadar bekle
                while (!scanner.isFinished)
                {
                    Application.DoEvents();
                }

                if (scanner.isCompleted)
                {
                    // Görselleri bas
                    if (scanner.Identity != null)
                        pictureKimlik.Image = scanner.Identity;

                    if (scanner.Head != null)
                        pictureFoto.Image = scanner.Head;

                    // OCR verilerini doldur
                    if (scanner.rCusIdentity != null)
                    {
                        //txtAd.Text = scanner.rCusIdentity.CUSIDNAME;
                        //txtSoyad.Text = scanner.rCusIdentity.CUSIDSIRNAME;
                        //txtTcNo.Text = scanner.rCusIdentity.CUSIDTCNO?.ToString();
                        //txtDogumTarihi.Text = scanner.rCusIdentity.CUSIDBIRTHDAY?.ToShortDateString();
                        //txtAnne.Text = scanner.rCusIdentity.CUSIDMOTHER;
                        //txtBaba.Text = scanner.rCusIdentity.CUSIDFATHER;
                    }
                }
                else
                {
                    XtraMessageBox.Show("Tarama tamamlanamadı!");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Hata: " + ex.Message);
            }
        }
    }    
}