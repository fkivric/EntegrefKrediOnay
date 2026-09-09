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
using FluentFTP;
using System.Net;
using System.IO;
using System.Management;
using System.Drawing.Imaging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevExpress.XtraSplashScreen;
using NTwain;
using System.Threading;
using PdfSharp.Drawing;
using EntegrefKrediOnay.Class;
using EntegrefKrediOnay.Class.OCRClass;
using EntegreFDLL;
using EntegreFDLL.Main;
using EntegreFDLL.Class;
using static EntegreFDLL.Class.Siniflar;

namespace EntegrefKrediOnay.Magaza
{
    public partial class frmDijitalArsiv : DevExpress.XtraEditors.XtraForm
    {
        public frmDijitalArsiv()
        {
            InitializeComponent();
        }
        class DocumentItem
        {
            public byte[] Data { get; set; }
            public string FileType { get; set; } // ".pdf", ".png"
            public Image image { get; set; }
        }
        class Documents
        {
            public DocumentItem adres { get; set; }
            public DocumentItem arac { get; set; }
            public DocumentItem dava { get; set; }
            public DocumentItem icra { get; set; }
            public DocumentItem sgk { get; set; }
            public DocumentItem tapu { get; set; }
        }
        static Documents Belgeler = new Documents();
        string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"EntegreF", "Entegref Taranan Evraklar");
        string CurFilePath;
        string url = "ftp://192.168.4.20//";
        string pass = "Yon";
        string uys = "Yonavm123!";
        public static string CURID;
        public static string CURVAL;
        public static string CURNAME;
        FtpClient client = new FtpClient();
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        private Image kimlikresmi;
        private Image Portreresmi;
        ListtoDataTableConverter converter = new ListtoDataTableConverter();
        private void frmKasiyerEdevlet_Load(object sender, EventArgs e)
        {
            url = Entegref.GetLogins.FTPURL;
            pass = Entegref.GetLogins.FTPPASS;
            uys = Entegref.GetLogins.FTPUSER;

            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit1.MouseEnter += PictureEdit1_MouseEnter;
            pictureEdit1.MouseMove += PictureEdit1_MouseMove;
            pictureEdit1.MouseLeave += PictureEdit1_MouseLeave;            

            pictureEdit2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            var pdfsize = panelControl2.Size;
            flyoutPanel1.OwnerControl = this;
            flyoutPanel1.Size = new Size(700, 600);
            var pdfloc = tileBar1.Location;
            flyoutPanel1.Options.Location = new Point(350, 150); //pdfloc; //
            flyoutPanel1.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
            flyoutPanel1.Options.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Fade;
            pictureEdit3.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit4.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit5.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit6.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit7.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit8.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
        }
        private void PictureEdit1_MouseLeave(object sender, EventArgs e)
        {
            pictureEdit1.Image = Portreresmi;
            flyoutPanel1.HidePopup();
        }
        int zoomFacet = 400;
        private void PictureEdit1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureEdit1.Image != null)
            {
                if (!flyoutPanel1.IsPopupOpen)
                    flyoutPanel1.ShowPopup();
                Point offsetLocation = pictureEdit1.ViewportToImage(e.Location);

                offsetLocation.X -= zoomFacet / 2;
                offsetLocation.Y -= zoomFacet / 3;

                offsetLocation.X = offsetLocation.X + zoomFacet > pictureEdit1.Image.Width
                    ? pictureEdit1.Image.Width - zoomFacet : offsetLocation.X;
                offsetLocation.X = offsetLocation.X < 0 ? 0 : offsetLocation.X;
                offsetLocation.Y = offsetLocation.Y + zoomFacet > pictureEdit1.Image.Height
                    ? pictureEdit1.Image.Height - zoomFacet : offsetLocation.Y;
                offsetLocation.Y = offsetLocation.Y < 0 ? 0 : offsetLocation.Y;

                pictureEdit2.Image = cropImage(pictureEdit1.Image,
                    new Rectangle(offsetLocation, new Size(zoomFacet, zoomFacet)));
            }
            else
            {
                pictureEdit2.Image = null;
            }
        }
        private void PictureEdit1_MouseEnter(object sender, EventArgs e)
        {
            pictureEdit1.Image = kimlikresmi;
            flyoutPanel1.ShowPopup();
        }
        private static Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics g = Graphics.FromImage(bmpImage))
            {
                g.DrawImage(img, new Rectangle(0, 0, bmpImage.Width, bmpImage.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmpImage;
        }
        private void btnCurrents_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
//            Sozlesme.frmMusteriSec musteriSec = new Sozlesme.frmMusteriSec(null, null);
//            DialogResult result = musteriSec.ShowDialog();
//            switch (result)
//            {
//                case DialogResult.OK:
//                    lblMusteri.Location = new Point(5, 10);
//                    lblMusteri.Text = $@"Müşteri Seçimi : 
//{CURNAME}";
//                    btnCurrents.Tag = CURID;
//                    btnCurrents.Text = CURVAL;
//                    break;
//            }
        }
        private void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            if (btnCurrents.EditValue != null)
            {
                if (btnCurrents.Tag != null)
                {
                    btnCurrents.Enabled = false;
                    tablePanel1.Enabled = true;
                }
                else
                {
                    var dt = conn.GetData($@"Select CURID,CURVAL as SALID,CURNAME from CURRENTS 
                    left outer join CUSTOMER on CUSCURID = CURID
                    where CURCUSTOMER = 1 and CURVAL = '{btnCurrents.EditValue}'
                    order by CUSDATETIME desc", Properties.Settings.Default.connectionstring);
                    lblMusteri.Location = new Point(5, 10);
                    lblMusteri.Text = $@"Müşteri Seçimi : 
{dt.Rows[0][2].ToString()}";
                    btnCurrents.Tag = dt.Rows[0][0].ToString();
                    btnCurrents.Text = dt.Rows[0][1].ToString();
                    CURID = dt.Rows[0][0].ToString();
                    CURVAL = dt.Rows[0][1].ToString();
                    CURNAME = dt.Rows[0][2].ToString();
                    btnCurrents.Enabled = false;
                    tablePanel1.Enabled = true;
                }

                var qs = string.Format("select CUPIDENTITY,CUPPORTRAIT from CUSTOMERPICTURE where CUPCURID = '{0}'", CURID);// sabit "1993863"
                var kimlik = conn.GetData(qs, Properties.Settings.Default.connectionstring);

                if (kimlik != null)
                {
                    var datakimlik = kimlik.Rows[0]["CUPIDENTITY"].ToString();
                    if (kimlik.Rows[0]["CUPIDENTITY"].ToString() != "")
                    {
                        kimlikresmi = EntegreFDLL.Class.VolantConvert.FromBase64String(kimlik.Rows[0]["CUPIDENTITY"].ToString()).byteArrayToImage();
                        Portreresmi = EntegreFDLL.Class.VolantConvert.FromBase64String(kimlik.Rows[0]["CUPPORTRAIT"].ToString()).byteArrayToImage();
                        pictureEdit1.Image = Portreresmi;
                    }
                    else
                    {
                        kimlikresmi = EntegreFDLL.Class.VolantConvert.FromBase64String("").byteArrayToImage();
                        Portreresmi = EntegreFDLL.Class.VolantConvert.FromBase64String("").byteArrayToImage();
                        pictureEdit1.Image = null;
                        string rtfMessage = @"{\rtf1\ansi{\colortbl ;\red255\green0\blue0;}\fs16 " +
                          "Müşteri Kimlik Taraması Eklenmemiş\\line" +
                          "\\b\\ul\\cf1 Sonki Alışverişler Kimlik Yüklenen Kadar Kapatılacak.\\b0\\ulnone\\cf0 }";
                        CustomMessageBox.ShowMessage(rtfMessage, "", this, "UYARI", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    kimlikresmi = EntegreFDLL.Class.VolantConvert.FromBase64String("").byteArrayToImage();
                    Portreresmi = EntegreFDLL.Class.VolantConvert.FromBase64String("").byteArrayToImage();
                    pictureEdit1.Image = null;
                    string rtfMessage = @"{\rtf1\ansi{\colortbl ;\red255\green0\blue0;}\fs16 " +
                      "Müşteri Kimlik Taraması Eklenmemiş\\line" +
                      "\\b\\ul\\cf1 Sonki Alışverişler Kimlik Yüklenen Kadar Kapatılacak.\\b0\\ulnone\\cf0 }";
                    CustomMessageBox.ShowMessage(rtfMessage, "", this, "UYARI", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Belgeler = new Documents();
                Belgeler.adres = new DocumentItem();
                Belgeler.arac = new DocumentItem();
                Belgeler.dava = new DocumentItem();
                Belgeler.icra = new DocumentItem();
                Belgeler.sgk = new DocumentItem();
                Belgeler.tapu = new DocumentItem();
                CurFilePath = Path.Combine(FilePath, CURID);
                Entegref.CreateDirectoryIfNotExists(CurFilePath);
            }
            else
            {
                CustomMessageBox.ShowMessage("Müşteri Seçimi Yapınız.", "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private async void btnTaramaAdres_Click(object sender, EventArgs e)
        {            
            var Sonuc = await Tarama("Adres_" + btnCurrents.Tag.ToString(), btnCurrents.Tag.ToString(),pictureEdit3,txtAdres,Belgeler.adres);
            if (Sonuc != true)
            {
                btnTaramaAdres.Enabled = false;
                txtAdres.Properties.ReadOnly = true;
                btnDosyaAdres.Enabled = false;
            }
        }
        private async void btnTaramaArac_Click(object sender, EventArgs e)
        {
            var Sonuc = await Tarama("Arac_" + btnCurrents.Tag.ToString(), btnCurrents.Tag.ToString(), pictureEdit4,txtArac, Belgeler.arac);
            if (Sonuc != true)
            {
                btnTaramaArac.Enabled = false;
                txtArac.Properties.ReadOnly = true;
                btnDosyaArac.Enabled = false;
            }
        }
        private async void btnTaramaDava_Click(object sender, EventArgs e)
        {
            var Sonuc = await Tarama("Davalar_" + btnCurrents.Tag.ToString(), btnCurrents.Tag.ToString(), pictureEdit5,txtDava, Belgeler.dava);
        }
        private async void btnTaramaIcra_Click(object sender, EventArgs e)
        {
            var Sonuc = await Tarama("Icra_" + btnCurrents.Tag.ToString(), btnCurrents.Tag.ToString(), pictureEdit6,txtIcra, Belgeler.icra);
        }
        private async void btnTaramaSGKBildirge_Click(object sender, EventArgs e)
        {
            var Sonuc = await Tarama("SGKBildirge_" + btnCurrents.Tag.ToString(), btnCurrents.Tag.ToString(), pictureEdit7,txtSGKBildirge,Belgeler.sgk);
        }
        private async void btnTaramaTapu_Click(object sender, EventArgs e)
        {
            var Sonuc = await Tarama("SGKBildirge_" + btnCurrents.Tag.ToString(), btnCurrents.Tag.ToString(), pictureEdit8, txtTapu,Belgeler.tapu);
        }
        private async void btnDosyaAdres_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            // Set filters so users only see image files
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp;*.pdf)|*.jpg; *.jpeg; *.gif; *.bmp;*.pdf";
            if (open.ShowDialog() == DialogResult.OK)
            {
                var sonuc = await Dosya(open.FileName, "Arac_" + btnCurrents.Tag.ToString(), Belgeler.adres);
                pictureEdit3.Image = Belgeler.adres.image;

            }
        }
        public eIdCard rIdCard;
        public static string scannerName;
        public Class.OCRClass.eKimlik rKimlik;
        public Enums.ScanObject ScanObjEnum = Enums.ScanObject.YeniTcKimlik;
        private YandexTranslate trnslt = new YandexTranslate();
        private TwainScanner scanner = null;
        public string ImagePath { get; set; }
        public eCurrents rCurrents;
        TwainHelper GetTwainList = new TwainHelper();
        private async void btnDosyaArac_Click(object sender, EventArgs e)
        {
            Enums.ScanObject scanObject = Enums.ScanObject.YeniTcKimlik;
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                rCurrents = new eCurrents();
                rCurrents = CurrentsGet(long.Parse(CURID));
                rCurrents.rCurrentsChild = eCurrentsChildGet(long.Parse(CURID));
                rCurrents.rCustomer = CustomerGet(long.Parse(CURID));
                rCurrents.rCustomer.rCusIdentity = CusIdentityGet(long.Parse(CURID));
                var scanlist = TwainHelper.GetScannerList();
                // Kullanıcıya listeden seçtir (DevExpress RadioGroup veya basit bir seçim penceresi)
                // Örnek olarak listenin ilkini alıyoruz veya kullanıcıya seçtiriyoruz:
                DevExpress.XtraEditors.XtraInputBoxArgs args = new DevExpress.XtraEditors.XtraInputBoxArgs();
                args.Caption = "Tarayıcı Seçimi";
                args.Prompt = "Lütfen kullanmak istediğiniz tarayıcıyı seçin:";
                args.DefaultButtonIndex = 0;

                // ComboBox Editörünü Tanımla
                DevExpress.XtraEditors.ComboBoxEdit editor = new DevExpress.XtraEditors.ComboBoxEdit();
                foreach (var tarayici in scanlist)
                {
                    editor.Properties.Items.Add(tarayici);
                }
                editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor; // Elle yazmayı kapat

                args.Editor = editor;
                args.DefaultResponse = scanlist[0]; // İlk tarayıcıyı varsayılan seç

                // Seçimi Göster
                var result = DevExpress.XtraEditors.XtraInputBox.Show(args);
                scannerName = result?.ToString();
                //Sozlesme.Vol_FK.frmScannerSec scannerSec = new Sozlesme.Vol_FK.frmScannerSec(scanlist);
                //scannerSec.ShowDialog();
                //scannerName = Sozlesme.Vol_FK.frmScannerSec.ScanName;
                if (scanner == null)
                {
                    scanner = new TwainScanner();
                }
                scanner.rCusIdentity = new eCusIdentity();
                scanner.ScanIdentity(scannerName, scanObject);
                Thread thread = new Thread((ThreadStart)delegate
                {
                    try
                    {
                        while (!scanner.isFinished)
                        {
                            Thread.Sleep(100);
                        }
                        if (!scanner.isCompleted)
                        {
                            scanner.isFinished = false;
                            if (scanner.scannedImages == null || scanner.scannedImages.Count == 0)
                            {
                                MessageBox.Show("Kimlik taranırken bir problemle karşılaşıldı!");
                            }
                            else if (scanner.scannedImages.Count > 0)
                            {
                                MessageBox.Show("Kimliğin diğer yüzünü çevirin ve Tekrar seçiniz");
                            }
                        }
                        else
                        {
                            Image head = scanner.Head;
                            Image ıdentity = scanner.Identity;
                            if (head != null)
                            {
                                pictureEdit4.Image = head;
                            }
                            else
                            {
                                MessageBox.Show("Kimlikteki Resim Algılanamadı!");
                            }
                            if (ıdentity != null)
                            {
                                //Thread.Sleep(1000);
                                //if (scanObject == Enums.ScanObject.YeniEhliyet || scanObject == Enums.ScanObject.YeniTcKimlik)
                                //{
                                //    pictureEdit5.Size = new Size(640, 200);
                                //}
                                //else
                                //{
                                //    pictureEdit5.Size = new Size(640, 480);
                                //}
                                pictureEdit5.Image = ıdentity;
                                if (1 == 1)
                                {
                                    rCurrents.rCustomer.CUSCONFIRMTCID = false;
                                    if (scanObject == Enums.ScanObject.TcKimlik)
                                    {
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDNAME))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDNAME = scanner.rCusIdentity.CUSIDNAME;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDBIRTHPLACE))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDBIRTHPLACE = scanner.rCusIdentity.CUSIDBIRTHPLACE;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME = scanner.rCusIdentity.CUSIDSIRNAME;
                                        }
                                        if (!rCurrents.rCustomer.rCusIdentity.CUSIDBIRTHDAY.HasValue)
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDBIRTHDAY = scanner.rCusIdentity.CUSIDBIRTHDAY;
                                        }
                                        if (!rCurrents.rCustomer.rCusIdentity.CUSIDTCNO.HasValue)
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDTCNO = scanner.rCusIdentity.CUSIDTCNO;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDSERIALNO))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDSERIALNO = scanner.rCusIdentity.CUSIDSERIALNO;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDMOTHER))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDMOTHER = scanner.rCusIdentity.CUSIDMOTHER;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDFATHER))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDFATHER = scanner.rCusIdentity.CUSIDFATHER;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDSERIALVAL))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDSERIALVAL = scanner.rCusIdentity.CUSIDSERIALVAL;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDREGCITY))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDREGCITY = scanner.rCusIdentity.CUSIDREGCITY;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDREGCOUNTY))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDREGCOUNTY = scanner.rCusIdentity.CUSIDREGCOUNTY;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDREGREGION))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDREGREGION = scanner.rCusIdentity.CUSIDREGREGION;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDVOLNO))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDVOLNO = scanner.rCusIdentity.CUSIDVOLNO;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDFAMILYNO))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDFAMILYNO = scanner.rCusIdentity.CUSIDFAMILYNO;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDSORTNO))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDSORTNO = scanner.rCusIdentity.CUSIDSORTNO;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDGIVENPLACE))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDGIVENPLACE = scanner.rCusIdentity.CUSIDGIVENPLACE;
                                        }
                                        if (!rCurrents.rCustomer.rCusIdentity.CUSIDGIVENDATE.HasValue)
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDGIVENDATE = scanner.rCusIdentity.CUSIDGIVENDATE;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDREGNO))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDREGNO = scanner.rCusIdentity.CUSIDREGNO;
                                        }
                                    }
                                    else if (scanObject == Enums.ScanObject.YeniTcKimlik)
                                    {
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDNAME))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDNAME = scanner.rCusIdentity.CUSIDNAME;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME = scanner.rCusIdentity.CUSIDSIRNAME;
                                        }
                                        if (!rCurrents.rCustomer.rCusIdentity.CUSIDBIRTHDAY.HasValue)
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDBIRTHDAY = scanner.rCusIdentity.CUSIDBIRTHDAY;
                                        }
                                        if (!rCurrents.rCustomer.rCusIdentity.CUSIDTCNO.HasValue)
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDTCNO = scanner.rCusIdentity.CUSIDTCNO;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDMOTHER))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDMOTHER = scanner.rCusIdentity.CUSIDMOTHER;
                                        }
                                        if (string.IsNullOrEmpty(rCurrents.rCustomer.rCusIdentity.CUSIDFATHER))
                                        {
                                            rCurrents.rCustomer.rCusIdentity.CUSIDFATHER = scanner.rCusIdentity.CUSIDFATHER;
                                        }
                                    }
                                    else
                                    {
                                        rCurrents.rCustomer.rCusIdentity = scanner.rCusIdentity;
                                    }
                                    if (rCurrents != null && rCurrents.rCustomer != null && rCurrents.rCustomer.rCusIdentity != null && rCurrents.rCustomer.rCusIdentity.CUSIDNAME != null)
                                    {
                                        rCurrents.rCustomer.rCusIdentity.CUSIDNAME = rCurrents.rCustomer.rCusIdentity.CUSIDNAME.ToUpper();
                                    }
                                    if (rCurrents != null && rCurrents.rCustomer != null && rCurrents.rCustomer.rCusIdentity != null && rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME != null)
                                    {
                                        rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME = rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME.ToUpper();
                                    }
                                    if (rCurrents != null && rCurrents.rCustomer != null && rCurrents.rCustomer.rCusIdentity != null)
                                    {
                                        rCurrents.CURNAME = (rCurrents.rCustomer.rCusIdentity.CUSIDNAME + " " + rCurrents.rCustomer.rCusIdentity.CUSIDSIRNAME).ToUpper();
                                    }
                                    //eCusIdentityBds.DataSource = rCurrents.rCustomer.rCusIdentity;
                                    //eCusIdentityBds.ResetBindings(metadataChanged: true);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Kimlik algılanamadı!");
                            }
                            scanner = null;
                        }
                    }
                    catch (Exception)
                    {
                    }
                });
                thread.Start();

            }
            catch (TwainException te)
            {
                if (te.Message != "Error opening data source")
                {
                    throw new Exception("Bilgisayara Bağlı Tarayıcıya bulunamadı!");
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }
            finally
            {
            }
        }
        public eCurrents CurrentsGet(long? CURID)
        {
            if (CURID == null)
                return null;

            // SQL sorgunu burada doğru şekilde yazmalısın
            string sql = $"SELECT * FROM CURRENTS WHERE CURID = {CURID}";
            DataTable dt = conn.GetData(sql, sql1);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            // İlk satırı al ve dönüştür
            DataRow row = dt.Rows[0];
            eCurrents record = this.MoveFromDataRowToeCurrents(row);

            return record;

        }
        public eCurrentsChild eCurrentsChildGet(long? CURID)
        {
            if (CURID == null)
                return null;

            // SQL sorgunu burada doğru şekilde yazmalısın
            string sql = $"select * from CURRENTSCHILD where CURCHID = {CURID}";
            DataTable dt = conn.GetData(sql, sql1);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            // İlk satırı al ve dönüştür
            DataRow row = dt.Rows[0];
            eCurrentsChild record = MoveFromDataRowToeCurrentsChild(row);

            return record;

        }
        public eCustomer CustomerGet(long? CURID)
        {
            if (CURID == null)
                return null;

            // SQL sorgunu burada doğru şekilde yazmalısın
            string sql = $"select * from CUSTOMER where CUSCURID = {CURID}";
            DataTable dt = conn.GetData(sql, sql1);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            // İlk satırı al ve dönüştür
            DataRow row = dt.Rows[0];
            eCustomer record = MoveFromDataRowToeCustomer(row);

            return record;
        }
        public eCusIdentity CusIdentityGet(long? CURID)
        {
            if (CURID == null)
                return null;

            // SQL sorgunu burada doğru şekilde yazmalısın
            string sql = $"select * from CUSIDENTITY where CUSIDCURID = {CURID}";
            DataTable dt = conn.GetData(sql, sql1);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            // İlk satırı al ve dönüştür
            DataRow row = dt.Rows[0];
            eCusIdentity record = MoveFromDataRowToeCurIdenty(row);

            return record;
        }
        private eCurrents MoveFromDataRowToeCurrents(DataRow row)
        {
            return new eCurrents
            {
                CURID = ((row["CURID"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CURID"]))),
                CURVAL = ((row["CURVAL"] == DBNull.Value) ? null : Convert.ToString(row["CURVAL"]).TrimEnd(Array.Empty<char>())),
                CURNAME = ((row["CURNAME"] == DBNull.Value) ? null : Convert.ToString(row["CURNAME"]).TrimEnd(Array.Empty<char>())),
                CURCUSTOMER = ((row["CURCUSTOMER"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCUSTOMER"]))),
                CURSUPPLIER = ((row["CURSUPPLIER"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURSUPPLIER"]))),
                CURSTAFF = ((row["CURSTAFF"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURSTAFF"]))),
                CURBANK = ((row["CURBANK"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURBANK"]))),
                CURCREDITCARD = ((row["CURCREDITCARD"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCREDITCARD"]))),
                CURCOCARD = ((row["CURCOCARD"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCOCARD"]))),
                CURBANKCREDIT = ((row["CURBANKCREDIT"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURBANKCREDIT"]))),
                CUROTHER = ((row["CUROTHER"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CUROTHER"]))),
                CURCOMPANY = ((row["CURCOMPANY"] == DBNull.Value) ? null : Convert.ToString(row["CURCOMPANY"]).TrimEnd(Array.Empty<char>())),
                CURDIVISON = ((row["CURDIVISON"] == DBNull.Value) ? null : Convert.ToString(row["CURDIVISON"]).TrimEnd(Array.Empty<char>())),
                CURSTS = ((row["CURSTS"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURSTS"]))),
                CURUSEFIELD1 = ((row["CURUSEFIELD1"] == DBNull.Value) ? null : Convert.ToString(row["CURUSEFIELD1"]).TrimEnd(Array.Empty<char>())),
                CURUSEFIELD2 = ((row["CURUSEFIELD2"] == DBNull.Value) ? null : Convert.ToString(row["CURUSEFIELD2"]).TrimEnd(Array.Empty<char>())),
                CURUSEFIELD3 = ((row["CURUSEFIELD3"] == DBNull.Value) ? null : Convert.ToString(row["CURUSEFIELD3"]).TrimEnd(Array.Empty<char>()))
            };
        }
        public eCurrentsChild MoveFromDataRowToeCurrentsChild(DataRow row)
        {
            return new eCurrentsChild
            {
                CURCHID = ((row["CURCHID"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CURCHID"]))),
                CURCHTITLE = ((row["CURCHTITLE"] == DBNull.Value) ? null : Convert.ToString(row["CURCHTITLE"]).TrimEnd(Array.Empty<char>())),
                CURCHSELECTIVEADR = ((row["CURCHSELECTIVEADR"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHSELECTIVEADR"]))),
                CURCHADR1 = ((row["CURCHADR1"] == DBNull.Value) ? null : Convert.ToString(row["CURCHADR1"]).TrimEnd(Array.Empty<char>())),
                CURCHADR2 = ((row["CURCHADR2"] == DBNull.Value) ? null : Convert.ToString(row["CURCHADR2"]).TrimEnd(Array.Empty<char>())),
                CURCHCOUNTY = ((row["CURCHCOUNTY"] == DBNull.Value) ? null : Convert.ToString(row["CURCHCOUNTY"]).TrimEnd(Array.Empty<char>())),
                CURCHCITY = ((row["CURCHCITY"] == DBNull.Value) ? null : Convert.ToString(row["CURCHCITY"]).TrimEnd(Array.Empty<char>())),
                CURCHPOSTALCODE = ((row["CURCHPOSTALCODE"] == DBNull.Value) ? null : Convert.ToString(row["CURCHPOSTALCODE"]).TrimEnd(Array.Empty<char>())),
                CURCHWATP = ((row["CURCHWATP"] == DBNull.Value) ? null : Convert.ToString(row["CURCHWATP"]).TrimEnd(Array.Empty<char>())),
                CURCHWATNO = ((row["CURCHWATNO"] == DBNull.Value) ? null : Convert.ToString(row["CURCHWATNO"]).TrimEnd(Array.Empty<char>())),
                CURCHPHONE1 = ((row["CURCHPHONE1"] == DBNull.Value) ? null : Convert.ToString(row["CURCHPHONE1"]).TrimEnd(Array.Empty<char>())),
                CURCHPHONE2 = ((row["CURCHPHONE2"] == DBNull.Value) ? null : Convert.ToString(row["CURCHPHONE2"]).TrimEnd(Array.Empty<char>())),
                CURCHPHONE3 = ((row["CURCHPHONE3"] == DBNull.Value) ? null : Convert.ToString(row["CURCHPHONE3"]).TrimEnd(Array.Empty<char>())),
                CURCHGSM1 = ((row["CURCHGSM1"] == DBNull.Value) ? null : Convert.ToString(row["CURCHGSM1"]).TrimEnd(Array.Empty<char>())),
                CURCHGSM2 = ((row["CURCHGSM2"] == DBNull.Value) ? null : Convert.ToString(row["CURCHGSM2"]).TrimEnd(Array.Empty<char>())),
                CURCHGSM3 = ((row["CURCHGSM3"] == DBNull.Value) ? null : Convert.ToString(row["CURCHGSM3"]).TrimEnd(Array.Empty<char>())),
                CURCHFAX = ((row["CURCHFAX"] == DBNull.Value) ? null : Convert.ToString(row["CURCHFAX"]).TrimEnd(Array.Empty<char>())),
                CURCHEMAIL = ((row["CURCHEMAIL"] == DBNull.Value) ? null : Convert.ToString(row["CURCHEMAIL"]).TrimEnd(Array.Empty<char>())),
                CURCHWEB = ((row["CURCHWEB"] == DBNull.Value) ? null : Convert.ToString(row["CURCHWEB"]).TrimEnd(Array.Empty<char>())),
                CURCHTSN = ((row["CURCHTSN"] == DBNull.Value) ? null : Convert.ToString(row["CURCHTSN"]).TrimEnd(Array.Empty<char>())),
                CURCHSHIPREGION = ((row["CURCHSHIPREGION"] == DBNull.Value) ? null : Convert.ToString(row["CURCHSHIPREGION"]).TrimEnd(Array.Empty<char>())),
                CURCHARTVAL = ((row["CURCHARTVAL"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHARTVAL"]))),
                CURCHEINVOICE = ((row["CURCHEINVOICE"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHEINVOICE"]))),
                CURCHEINVOICEUNIQUEVAL = ((row["CURCHEINVOICEUNIQUEVAL"] == DBNull.Value) ? null : Convert.ToString(row["CURCHEINVOICEUNIQUEVAL"]).TrimEnd(Array.Empty<char>())),
                CURCHEINVOICECANPRINT = ((row["CURCHEINVOICECANPRINT"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHEINVOICECANPRINT"]))),
                CURCHEINVOICECOWORKGIB = ((row["CURCHEINVOICECOWORKGIB"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHEINVOICECOWORKGIB"]))),
                CURCHEDESPATCH = ((row["CURCHEDESPATCH"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHEDESPATCH"]))),
                CURCHREFERNAME = ((row["CURCHREFERNAME"] == DBNull.Value) ? null : Convert.ToString(row["CURCHREFERNAME"]).TrimEnd(Array.Empty<char>())),
                CURCHREFERADR1 = ((row["CURCHREFERADR1"] == DBNull.Value) ? null : Convert.ToString(row["CURCHREFERADR1"]).TrimEnd(Array.Empty<char>())),
                CURCHREFERPHONE1 = ((row["CURCHREFERPHONE1"] == DBNull.Value) ? null : Convert.ToString(row["CURCHREFERPHONE1"]).TrimEnd(Array.Empty<char>())),
                CURCHREFERGSM1 = ((row["CURCHREFERGSM1"] == DBNull.Value) ? null : Convert.ToString(row["CURCHREFERGSM1"]).TrimEnd(Array.Empty<char>())),
                CURCHNOSMS = ((row["CURCHNOSMS"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHNOSMS"]))),
                CURCHCHECKGSM = ((row["CURCHCHECKGSM"] == DBNull.Value) ? null : Convert.ToString(row["CURCHCHECKGSM"]).TrimEnd(Array.Empty<char>())),
                CURCHDPRID = ((row["CURCHDPRID"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CURCHDPRID"]))),
                CURCHSMENID = ((row["CURCHSMENID"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CURCHSMENID"]))),
                CURCHDISCRATE = ((row["CURCHDISCRATE"] == DBNull.Value) ? null : new decimal?(Convert.ToDecimal(row["CURCHDISCRATE"]))),
                CURCHMERSIS = ((row["CURCHMERSIS"] == DBNull.Value) ? null : Convert.ToString(row["CURCHMERSIS"]).TrimEnd(Array.Empty<char>())),
                CURCHWHOLENAME = ((row["CURCHWHOLENAME"] == DBNull.Value) ? null : Convert.ToString(row["CURCHWHOLENAME"]).TrimEnd(Array.Empty<char>())),
                CURCHWHOLEPASSWORD = ((row["CURCHWHOLEPASSWORD"] == DBNull.Value) ? null : Convert.ToString(row["CURCHWHOLEPASSWORD"]).TrimEnd(Array.Empty<char>())),
                CURCHWHOLESTS = ((row["CURCHWHOLESTS"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHWHOLESTS"]))),
                CURCHKVKKOK = ((row["CURCHKVKKOK"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHKVKKOK"]))),
                CURCHIYS = ((row["CURCHIYS"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHIYS"]))),
                CURCHVOLTAKER = ((row["CURCHVOLTAKER"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHVOLTAKER"]))),
                CURCHVOLTAKERDATETIME = ((row["CURCHVOLTAKERDATETIME"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CURCHVOLTAKERDATETIME"]))),
                CURCHVOLTAKERLASTLOGIN = ((row["CURCHVOLTAKERLASTLOGIN"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CURCHVOLTAKERLASTLOGIN"]))),
                CURCHMUHTASARKODU = ((row["CURCHMUHTASARKODU"] == DBNull.Value) ? null : Convert.ToString(row["CURCHMUHTASARKODU"]).TrimEnd(Array.Empty<char>())),
                CURCHSTOPAJSTS = ((row["CURCHSTOPAJSTS"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CURCHSTOPAJSTS"]))),
                CURCHNAME = ((row["CURCHNAME"] == DBNull.Value) ? null : Convert.ToString(row["CURCHNAME"]).TrimEnd(Array.Empty<char>())),
                CURCHSIRNAME = ((row["CURCHSIRNAME"] == DBNull.Value) ? null : Convert.ToString(row["CURCHSIRNAME"]).TrimEnd(Array.Empty<char>()))
            };
        }
        public eCustomer MoveFromDataRowToeCustomer(DataRow row)
        {
            return new eCustomer
            {
                CUSCURID = ((row["CUSCURID"] == DBNull.Value) ? 0 : Convert.ToInt64(row["CUSCURID"])),
                CUSFROM = ((row["CUSFROM"] == DBNull.Value) ? null : Convert.ToString(row["CUSFROM"]).TrimEnd(Array.Empty<char>())),
                CUSCAST = ((row["CUSCAST"] == DBNull.Value) ? null : Convert.ToString(row["CUSCAST"]).TrimEnd(Array.Empty<char>())),
                CUSCP = ((row["CUSCP"] == DBNull.Value) ? null : Convert.ToString(row["CUSCP"]).TrimEnd(Array.Empty<char>())),
                CUSCURKIND = ((row["CUSCURKIND"] == DBNull.Value) ? null : Convert.ToString(row["CUSCURKIND"]).TrimEnd(Array.Empty<char>())),
                CUSLETTER = ((row["CUSLETTER"] == DBNull.Value) ? null : Convert.ToString(row["CUSLETTER"]).TrimEnd(Array.Empty<char>())),
                CUSCORPARATE = ((row["CUSCORPARATE"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CUSCORPARATE"]))),
                CUSPERSON = ((row["CUSPERSON"] == DBNull.Value) ? null : new bool?(Convert.ToBoolean(row["CUSPERSON"]))),
                CUSLATEDAY = ((row["CUSLATEDAY"] == DBNull.Value) ? null : new short?(Convert.ToInt16(row["CUSLATEDAY"]))),
                CUSCREDIT = ((row["CUSCREDIT"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["CUSCREDIT"])),
                CUSCOCREDIT = ((row["CUSCOCREDIT"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["CUSCOCREDIT"])),
                CUSCREDITSTARTDATE = ((row["CUSCREDITSTARTDATE"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CUSCREDITSTARTDATE"]))),
                CUSCREDITENDDATE = ((row["CUSCREDITENDDATE"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CUSCREDITENDDATE"]))),
                CUSAUTOCREDIT = ((row["CUSAUTOCREDIT"] == DBNull.Value) ? false : Convert.ToBoolean(row["CUSAUTOCREDIT"])),
                CUSACCID = ((row["CUSACCID"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CUSACCID"]))),
                CUSEXSOID = ((row["CUSEXSOID"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CUSEXSOID"]))),
                CUSACCCANVAL = ((row["CUSACCCANVAL"] == DBNull.Value) ? null : Convert.ToString(row["CUSACCCANVAL"]).TrimEnd(Array.Empty<char>())),
                CUSINVRATEVAL = ((row["CUSINVRATEVAL"] == DBNull.Value) ? 0 : Convert.ToInt32(row["CUSINVRATEVAL"])),
                CUSCONFIRMTCID = ((row["CUSCONFIRMTCID"] == DBNull.Value) ? false : Convert.ToBoolean(row["CUSCONFIRMTCID"])),
                CUSSOCODE = ((row["CUSSOCODE"] == DBNull.Value) ? null : Convert.ToString(row["CUSSOCODE"]).TrimEnd(Array.Empty<char>())),
                CUSDATETIME = ((row["CUSDATETIME"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(row["CUSDATETIME"])),
                CUSCANPROCEEDWITHNOLATEINCOME = ((row["CUSCANPROCEEDWITHNOLATEINCOME"] == DBNull.Value) ? false : Convert.ToBoolean(row["CUSCANPROCEEDWITHNOLATEINCOME"])),
                CUSCREATESALETYPE = ((row["CUSCREATESALETYPE"] == DBNull.Value) ? null : new short?(Convert.ToInt16(row["CUSCREATESALETYPE"]))),
                CUSCRCARDID = ((row["CUSCRCARDID"] == DBNull.Value) ? null : Convert.ToString(row["CUSCRCARDID"]).TrimEnd(Array.Empty<char>())),
                CUSCRCARDENDDATE = ((row["CUSCRCARDENDDATE"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CUSCRCARDENDDATE"]))),
                CUSCRNOTKEY = ((row["CUSCRNOTKEY"] == DBNull.Value) ? null : Convert.ToString(row["CUSCRNOTKEY"]).TrimEnd(Array.Empty<char>())),
                CUSCRCOIN = ((row["CUSCRCOIN"] == DBNull.Value) ? null : new decimal?(Convert.ToDecimal(row["CUSCRCOIN"]))),
                CUSCRCOINKEY = ((row["CUSCRCOINKEY"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CUSCRCOINKEY"])))
            };

        }
        public eCusIdentity MoveFromDataRowToeCurIdenty(DataRow row)
        {
            return new eCusIdentity
            {
                CUSIDCURID = ((row["CUSIDCURID"] == DBNull.Value) ? 0 : Convert.ToInt64(row["CUSIDCURID"])),
                CUSIDTCNO = ((row["CUSIDTCNO"] == DBNull.Value) ? null : new long?(Convert.ToInt64(row["CUSIDTCNO"]))),
                CUSIDNAME = ((row["CUSIDNAME"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDNAME"]).TrimEnd(Array.Empty<char>())),
                CUSIDSIRNAME = ((row["CUSIDSIRNAME"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDSIRNAME"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKNAME = ((row["CUSIDWORKNAME"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKNAME"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKADR1 = ((row["CUSIDWORKADR1"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKADR1"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKADR2 = ((row["CUSIDWORKADR2"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKADR2"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKCOUNTY = ((row["CUSIDWORKCOUNTY"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKCOUNTY"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKCITY = ((row["CUSIDWORKCITY"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKCITY"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKPOSTALCODE = ((row["CUSIDWORKPOSTALCODE"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKPOSTALCODE"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKPHONE1 = ((row["CUSIDWORKPHONE1"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKPHONE1"]).TrimEnd(Array.Empty<char>())),
                CUSIDWORKSGKNO = ((row["CUSIDWORKSGKNO"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDWORKSGKNO"]).TrimEnd(Array.Empty<char>())),
                CUSIDMOTHER = ((row["CUSIDMOTHER"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDMOTHER"]).TrimEnd(Array.Empty<char>())),
                CUSIDFATHER = ((row["CUSIDFATHER"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDFATHER"]).TrimEnd(Array.Empty<char>())),
                CUSIDBIRTHPLACE = ((row["CUSIDBIRTHPLACE"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDBIRTHPLACE"]).TrimEnd(Array.Empty<char>())),
                CUSIDBIRTHDAY = ((row["CUSIDBIRTHDAY"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CUSIDBIRTHDAY"]))),
                CUSIDSERIALNO = ((row["CUSIDSERIALNO"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDSERIALNO"]).TrimEnd(Array.Empty<char>())),
                CUSIDREGCITY = ((row["CUSIDREGCITY"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDREGCITY"]).TrimEnd(Array.Empty<char>())),
                CUSIDREGCOUNTY = ((row["CUSIDREGCOUNTY"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDREGCOUNTY"]).TrimEnd(Array.Empty<char>())),
                CUSIDREGREGION = ((row["CUSIDREGREGION"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDREGREGION"]).TrimEnd(Array.Empty<char>())),
                CUSIDVOLNO = ((row["CUSIDVOLNO"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDVOLNO"]).TrimEnd(Array.Empty<char>())),
                CUSIDFAMILYNO = ((row["CUSIDFAMILYNO"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDFAMILYNO"]).TrimEnd(Array.Empty<char>())),
                CUSIDSORTNO = ((row["CUSIDSORTNO"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDSORTNO"]).TrimEnd(Array.Empty<char>())),
                CUSIDGIVENPLACE = ((row["CUSIDGIVENPLACE"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDGIVENPLACE"]).TrimEnd(Array.Empty<char>())),
                CUSIDGIVENDATE = ((row["CUSIDGIVENDATE"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CUSIDGIVENDATE"]))),
                CUSIDGIVENREASON = ((row["CUSIDGIVENREASON"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDGIVENREASON"]).TrimEnd(Array.Empty<char>())),
                CUSIDREGNO = ((row["CUSIDREGNO"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDREGNO"]).TrimEnd(Array.Empty<char>())),
                CUSIDDRVNO = ((row["CUSIDDRVNO"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDDRVNO"]).TrimEnd(Array.Empty<char>())),
                CUSIDDRVGIVENDATE = ((row["CUSIDDRVGIVENDATE"] == DBNull.Value) ? null : new DateTime?(Convert.ToDateTime(row["CUSIDDRVGIVENDATE"]))),
                CUSIDDRVGIVENPLACE = ((row["CUSIDDRVGIVENPLACE"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDDRVGIVENPLACE"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERNAME = ((row["CUSIDPEERNAME"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERNAME"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERSURNAME = ((row["CUSIDPEERSURNAME"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERSURNAME"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERWORKNAME = ((row["CUSIDPEERWORKNAME"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERWORKNAME"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERWORKADR1 = ((row["CUSIDPEERWORKADR1"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERWORKADR1"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERWORKADR2 = ((row["CUSIDPEERWORKADR2"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERWORKADR2"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERWORKCOUNTY = ((row["CUSIDPEERWORKCOUNTY"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERWORKCOUNTY"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERWORKCITY = ((row["CUSIDPEERWORKCITY"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERWORKCITY"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERWORKPOSTALCODE = ((row["CUSIDPEERWORKPOSTALCODE"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERWORKPOSTALCODE"]).TrimEnd(Array.Empty<char>())),
                CUSIDPEERWORKPHONE1 = ((row["CUSIDPEERWORKPHONE1"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDPEERWORKPHONE1"]).TrimEnd(Array.Empty<char>())),
                CUSIDSEX = ((row["CUSIDSEX"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDSEX"]).TrimEnd(Array.Empty<char>())),
                CUSIDMARRIED = ((row["CUSIDMARRIED"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDMARRIED"]).TrimEnd(Array.Empty<char>())),
                CUSIDFAMILYCOUNT = ((row["CUSIDFAMILYCOUNT"] == DBNull.Value) ? 0 : Convert.ToInt32(row["CUSIDFAMILYCOUNT"])),
                CUSIDCHILDCOUNT = ((row["CUSIDCHILDCOUNT"] == DBNull.Value) ? 0 : Convert.ToInt32(row["CUSIDCHILDCOUNT"])),
                CUSMOTHERMAIDEN = ((row["CUSMOTHERMAIDEN"] == DBNull.Value) ? null : Convert.ToString(row["CUSMOTHERMAIDEN"]).TrimEnd(Array.Empty<char>())),
                CUSIDSERIALVAL = ((row["CUSIDSERIALVAL"] == DBNull.Value) ? null : Convert.ToString(row["CUSIDSERIALVAL"]).TrimEnd(Array.Empty<char>())),
                CUSIDIDKIND = ((row["CUSIDIDKIND"] == DBNull.Value) ? false : Convert.ToBoolean(row["CUSIDIDKIND"])),
                CUSIDFOREIGN = ((row["CUSIDFOREIGN"] == DBNull.Value) ? false : Convert.ToBoolean(row["CUSIDFOREIGN"]))
            };


        }

        private void btnDosyaDava_Click(object sender, EventArgs e)
        {

        }

        private void btnDosyaIcra_Click(object sender, EventArgs e)
        {
        }

        private void btnDosyaSGKBildirge_Click(object sender, EventArgs e)
        {

        }
        private void btnDosyaTapu_Click(object sender, EventArgs e)
        {

        }
        public static int GetAutomaticScanLimit()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong totalMemory = (ulong)obj["TotalPhysicalMemory"]; // Byte cinsinden
                    ulong usableMemory = totalMemory / 5; // %20'sini kullanalım
                    int usableMB = (int)(usableMemory / (1024 * 1024));

                    int estimatedLimit = usableMB / 4; // Her belge 4MB varsayımıyla
                    return Math.Max(5, Math.Min(estimatedLimit, 100)); // 5–100 arası sınırla
                }
            }
            catch { }

            return 10; // Hata durumunda varsayılan
        }
        void DeleteFile(string tempPdfPath)
        {
            if (Directory.Exists(tempPdfPath))
            {
                try
                {
                    // Dosyaları tek tek sil
                    foreach (string file in Directory.GetFiles(tempPdfPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.SetAttributes(file, FileAttributes.Normal); // Salt okunur gibi engelleri kaldır
                            File.Delete(file);
                        }
                        catch { /* Gerekirse logla */ }
                    }

                    // Alt klasörleri sil
                    foreach (string dir in Directory.GetDirectories(tempPdfPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch { /* Gerekirse logla */ }
                    }

                    // En son klasörün kendisini sil
                    Directory.Delete(tempPdfPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Klasör silinirken hata oluştu: " + ex.Message);
                }
            }
        }
        private static void SavePDFtoImage()
        {

        }
        public async Task SaveImageAsPdfWithLimitAsync(Image image, string filePath, int maxSizeInBytes = 1_000_000)
        {
            await Task.Run(() =>
            {
                long quality = 80;
                int attempt = 0;

                while (attempt < 5)
                {
                    SaveImageAsPdf2(image, filePath, quality);
                    var fileSize = new FileInfo(filePath).Length;

                    if (fileSize <= maxSizeInBytes)
                        break;

                    quality -= 10;
                    if (quality < 20) break;

                    attempt++;
                }
            });
        }
        private static void SaveImageAsPdf2(Image image, string filePath, long jpegQuality = 50L)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var doc = new PdfSharp.Pdf.PdfDocument())
            {
                var page = doc.AddPage();
                page.Width = XUnit.FromMillimeter(210); //image.Width;
                page.Height = XUnit.FromMillimeter(310); //image.Height;

                using (var gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page))
                using (var ms = new MemoryStream())
                {
                    using (var reduced = ReduceDpi(image))
                    {
                        var encoder = GetEncoder(ImageFormat.Jpeg);
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality); // 1-100

                        reduced.Save(ms, encoder, encoderParams);
                    }
                    ms.Position = 0;

                    using (var img = System.Drawing.Image.FromStream(ms))
                    {
                        using (var xImage = PdfSharp.Drawing.XImage.FromGdiPlusImage(img))
                        {
                            gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                        }
                    }

                }

                doc.Save(filePath);
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
        }
        private static Bitmap ReduceDpi(Image originalImage, int targetDpi = 150)
        {
            Bitmap bitmap = new Bitmap(originalImage);
            bitmap.SetResolution(targetDpi, targetDpi);
            return bitmap;
        }
        private void EnsureFtpDirectoriesExist(string ftpBaseUrl, string ftpUser, string ftpPass, string curID)
        {
            if (string.IsNullOrWhiteSpace(curID)) return;

            string[] altKlasorler = new[]
            {
                "E-Devlet",
            };
            client.ConnectTimeout = 5000;
            client.ReadTimeout = 5000;
            client.DataConnectionConnectTimeout = 5000;
            client.DataConnectionReadTimeout = 5000;

            try
            {
                if (!client.IsConnected)
                {
                    client.Connect();
                }

                string anaKlasor = $"/{curID}";
                if (!client.DirectoryExists(anaKlasor))
                {
                    client.CreateDirectory(anaKlasor, true);
                }

                foreach (var folder in altKlasorler)
                {
                    string fullPath = $"{anaKlasor}/{folder}";
                    if (!client.DirectoryExists(fullPath))
                    {
                        client.CreateDirectory(fullPath, true);
                    }
                }
            }
            catch (FtpException ftpEx)
            {
                Console.WriteLine("FTP hatası: " + ftpEx.Message);
            }
        }
        public async Task<string> UploadFileToFtpIfNotExists(string ftpBaseUrl, string curID, string remoteFolder, string remoteFileName, string filePath, string ftpUser, string ftpPass)
        {
            string fullRemotePath = $"/{curID}/{remoteFolder}/{remoteFileName}";

            try
            {
                client.ConnectTimeout = 5000;
                client.ReadTimeout = 5000;
                client.DataConnectionConnectTimeout = 5000;
                client.DataConnectionReadTimeout = 5000;
                string directoryPath = $"/{curID}/{remoteFolder}";
                if (!await client.DirectoryExistsAsync(directoryPath))
                    await client.CreateDirectoryAsync(directoryPath, true);

                if (await client.FileExistsAsync(fullRemotePath))
                {
                    return "Aynı isimde dosya FTP'de zaten var. Yükleme yapılmadı.";
                }

                var status = await client.UploadFileAsync(filePath, fullRemotePath, FtpExists.Overwrite, true);

                if (status == true)
                {
                    return "Dosya başarıyla yüklendi.";
                }
                else
                {
                    return "Dosya yüklenemedi.";
                }
            }
            catch (Exception ex)
            {
                return "FTP işleminde hata: " + ex.Message;
            }
        }
        async Task<string> FtpFilePDF(Image image, string FileName, string TempPath)
        {
            try
            {
                client = new FtpClient(url, new NetworkCredential(uys, pass));
                await client.ConnectAsync();
            }
            catch (Exception)
            {

            }
            try
            {
                string filename = $"{FileName}_{DateTime.Today.ToString("yyyy-MM-dd")}.pdf";
                string tempFullPath = Path.Combine(TempPath, filename);
                await SaveImageAsPdfWithLimitAsync(image, tempFullPath);
                EnsureFtpDirectoriesExist(url, uys, pass, CURID);
                var FtpSonuc = await UploadFileToFtpIfNotExists(url, CURID, "E-Devlet", filename, tempFullPath, uys, pass);
                return FtpSonuc;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        async Task<bool> Tarama(string FileName, string CURID, PictureEdit pictureEdit,TextEdit textEdit, DocumentItem  document)
        {
            string tempPdfPath = "";
            List<ScanResult> results = new List<ScanResult>();
            try
            {
                var scanner = new DocumentScan(this);
                scanner.MaxScanLimit = GetAutomaticScanLimit();
                results = await scanner.StartScanAsync(false,true);
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("Hata Detayı", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            tempPdfPath = Path.Combine(CurFilePath, FileName);
            if (results != null && results.Count > 0)
            {
                try
                {
                    //await FtpFilePDF(results[0].Image, FileName, tempPdfPath);
                    string filename = $"{FileName}_{DateTime.Today.ToString("yyyy-MM-dd")}.pdf";
                    string filePath = Path.Combine(tempPdfPath, filename);
                    textEdit.Text = "Tarama Başarılı";
                    pictureEdit.Image = results[0].Image;
                    document.image = results[0].Image;
                    await SaveImageAsPdfWithLimitAsync(results[0].Image, filePath);
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    string ext = Path.GetExtension(filePath).ToLower();
                    document.Data = fileBytes;
                    document.FileType = ext; // ".pdf", ".jpg"
                    return true;
                }
                catch (Exception ex)
                {
                    string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                    CustomMessageBox.ShowMessage("Hata Detayı", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        async Task<bool> Dosya(string FileName,string NewName, DocumentItem documentItem)
        {
            try
            {
                if (documentItem == null)
                {
                    documentItem = new DocumentItem();
                }
                if (Path.GetExtension(FileName).ToLower() == ".pdf")
                {
                    //using (var document = PdfDocument.Load(FileName))
                    //{
                    //    for (int i = 0; i < document.PageCount; i++)
                    //    {
                    //        // 👇 PDF sayfasını image çevir
                    //        Image img = document.Render(i, 300, 300, true);

                    //        var outputPath = Path.Combine(
                    //            CurFilePath,
                    //            $"{NewName}_{DateTime.Today.ToString("yyyy-MM-dd")}_{i + 1}.png");
                    //        img.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
                    //        if (i == 0)
                    //        {
                    //            documentItem.image = img;
                    //        }
                    //    }
                    //}
                    //documentItem.image = img;
                    byte[] fileBytes = File.ReadAllBytes(FileName);
                    string ext = Path.GetExtension(FileName).ToLower();
                    documentItem.Data = fileBytes;
                    documentItem.FileType = ext; // ".pdf", ".jpg"
                }
                else
                {
                    // dosyayı byte[] olarak al
                    byte[] fileBytes = File.ReadAllBytes(FileName);
                    string ext = Path.GetExtension(FileName).ToLower();


                    // 👇 Image oluştur (preview için)
                    using (var ms = new MemoryStream(fileBytes))
                    {
                        Image img = Image.FromStream(ms);

                        // ilk image'ı sakla (senin yapına göre)
                        documentItem.image = (Image)img.Clone();

                        // diske kayıt (PDF'teki gibi)
                        var outputPath = Path.Combine(
                            CurFilePath,
                            $"{NewName}_{DateTime.Today:yyyy-MM-dd}.png");
                        img.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

                        var outputPath1 = Path.Combine(
                            CurFilePath,
                            $"{NewName}_{DateTime.Today:yyyy-MM-dd}.pdf");
                        await SaveImageAsPdfWithLimitAsync(documentItem.image, outputPath1);
                        byte[] fileBytes2 = File.ReadAllBytes(outputPath1);
                        string ext2 = Path.GetExtension(outputPath1).ToLower();
                        documentItem.Data = fileBytes;
                        documentItem.FileType = ext2;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void pictureEdit3_Click(object sender, EventArgs e)
        {
            Form previewForm = new Form();
            previewForm.StartPosition = FormStartPosition.CenterScreen;
            previewForm.Size = new Size(800, 800);
            if (Belgeler.adres.FileType == ".pdf")
            {
                DevExpress.XtraPdfViewer.PdfViewer pdfViewer = new DevExpress.XtraPdfViewer.PdfViewer();
                pdfViewer.Dock = DockStyle.Fill;
                pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
                //pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfViewerZoomMode.FitWidth;

                // 👇 PDF'i yükle (senin eksik olan kısım burası)
                MemoryStream ms = new MemoryStream(Belgeler.adres.Data);
                pdfViewer.LoadDocument(ms);

                previewForm.Controls.Add(pdfViewer);
            }
            else
            {
                PictureBox pb = new PictureBox();
                pb.Dock = DockStyle.Fill;
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Image = pictureEdit3.Image;
                previewForm.Controls.Add(pb);
            }

            previewForm.ShowDialog();
        }
        private void pictureEdit4_Click(object sender, EventArgs e)
        {
            Form previewForm = new Form();
            previewForm.StartPosition = FormStartPosition.CenterScreen;
            previewForm.Size = new Size(800, 800);

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = pictureEdit4.Image;

            previewForm.Controls.Add(pb);
            previewForm.ShowDialog();
        }
        private void pictureEdit5_Click(object sender, EventArgs e)
        {
            Form previewForm = new Form();
            previewForm.StartPosition = FormStartPosition.CenterScreen;
            previewForm.Size = new Size(800, 800);

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = pictureEdit5.Image;

            previewForm.Controls.Add(pb);
            previewForm.ShowDialog();
        }
        private void pictureEdit6_Click(object sender, EventArgs e)
        {
            Form previewForm = new Form();
            previewForm.StartPosition = FormStartPosition.CenterScreen;
            previewForm.Size = new Size(800, 800);

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = pictureEdit6.Image;

            previewForm.Controls.Add(pb);
            previewForm.ShowDialog();
        }
        private void pictureEdit7_Click(object sender, EventArgs e)
        {
            Form previewForm = new Form();
            previewForm.StartPosition = FormStartPosition.CenterScreen;
            previewForm.Size = new Size(800, 800);

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = pictureEdit7.Image;

            previewForm.Controls.Add(pb);
            previewForm.ShowDialog();
        }
        private void pictureEdit8_Click(object sender, EventArgs e)
        {
            Form previewForm = new Form();
            previewForm.StartPosition = FormStartPosition.CenterScreen;
            previewForm.Size = new Size(800, 800);

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = pictureEdit8.Image;

            previewForm.Controls.Add(pb);
            previewForm.ShowDialog();
        }
        
        private byte[] BitmapToByte(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private byte[] Combine(Bitmap front, Bitmap back)
        {
            if (back == null)
                return BitmapToByte(front);

            int w = front.Width + back.Width;
            int h = Math.Max(front.Height, back.Height);

            using (Bitmap final = new Bitmap(w, h))
            using (Graphics g = Graphics.FromImage(final))
            {
                g.DrawImage(front, 0, 0);
                g.DrawImage(back, front.Width, 0);

                return BitmapToByte(final);
            }
        }

        private byte[] CropFaceBitmap(Bitmap bmp)
        {
            Rectangle r = new Rectangle(20, 100, 220, 260);

            using (Bitmap face = bmp.Clone(r, bmp.PixelFormat))
            {
                return BitmapToByte(face);
            }
        }

        private async Task<string> OCR(string pdfPath)
        {
            //HttpClient httpClient = new HttpClient
            //{
            //    Timeout = new TimeSpan(1, 1, 1)
            //};
            //MultipartFormDataContent form = new MultipartFormDataContent
            //    {
            //        {
            //            new StringContent("7b0ede3ee988957"),
            //            "apikey"
            //        },
            //        {
            //            new StringContent("tur"),
            //            "language"
            //        }
            //    };
            //if (string.IsNullOrEmpty(ImagePath) == false)
            //{
            //    byte[] imageData = File.ReadAllBytes(ImagePath);
            //    //byte[] imageData = File.ReadAllBytes(ImagePath);
            //    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
            //}
            //else if (string.IsNullOrEmpty(PdfPath) == false)
            //{
            //    byte[] imageData = File.ReadAllBytes(PdfPath);
            //    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "PDF", "pdf.pdf");
            //}
            //HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);

            //string strContent = await response.Content.ReadAsStringAsync();

            //Rootobject ocrResult = JsonConvert.DeserializeObject<Rootobject>(strContent);

            //string result = "";
            //if (ocrResult.OCRExitCode == 1)
            //{
            //    for (int i = 0; i < ocrResult.ParsedResults.Length; i++)
            //    {
            //        result += ocrResult.ParsedResults[i].ParsedText;
            //    }
            //}

            string result = "";
            AiAnalysisService AiAnalysisService = new AiAnalysisService();
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            var base64Data = Convert.ToBase64String(pdfBytes);
            await SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
                    async (progress, token) =>
                    {
                        progress.Report((0, $"Yapay Zeka Danışmanına Soruluyor"));
                        await Task.Delay(10, token);
                        token.ThrowIfCancellationRequested();                        
                        string myPrompt2 = @"
                    metin gibi tüm sayfayı oku aşagıdaki formatta ver, gelen sonucu class içine sorunsuz JsonConvert.DeserializeObject ile alabilmeliyim. yani cevabın josun formatında olsun
                    public class Kimlik
                    {
                        public string Identity {get ; set;}
                        public string Surname  {get ; set;}
                        public string GivenName  {get ; set;}
                        public string DateofBirth  {get ; set;}
                        public string Gender {get ; set;}
                        public string DocumentNo  {get ; set;}
                        public string Nationality  {get ; set;}
                        public string ValidUntil  {get ; set;}
                    }";

                        result = await AiAnalysisService.AnalyzePdfWithGemini(myPrompt2, base64Data);
                    });

            return result;
        }
        private string SaveImageAsPng(Bitmap image, string _brkod)
        {
            var resim = image; // ResizeImageWithAspectRatio(image, 800, 1000);
            try
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scans");
                Directory.CreateDirectory(directory);

                string fileName = Path.Combine(directory, $"Scan_{_brkod}.png");
                resim.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                return fileName;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemLog.txt"), DateTime.Now + " => " + "Görüntü kaydedilirken hata oluştu: " + ex.Message + Environment.NewLine);
                return "";
            }
        }
    }
}