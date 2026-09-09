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
using Newtonsoft.Json;
using DevExpress.XtraTab;
using DevExpress.XtraGrid;
using System.Net;
using System.Threading;
using DevExpress.XtraLayout;
using DevExpress.LookAndFeel;
using DevExpress.XtraSplashScreen;
using Timer = System.Windows.Forms.Timer;
using DevExpress.XtraEditors.Controls;
using System.Data.SqlClient;
using System.IO;
using EntegreFDLL.Class;
using EntegrefKrediOnay.Class;
using EntegreFDLL;
using static EntegreFDLL.Class.VolantApiClass;
//using Volant.Ocr.Dev.Extensions;
using System.Reflection;
using EntegreFDLL.Main;

namespace EntegrefKrediOnay
{
    public partial class frmMusteri : DevExpress.XtraEditors.XtraForm
    {
        EntegreFDLL.Class.BGClass BGClass = new BGClass();
        public List<Musteriler> musterilers = new List<Musteriler>();
        public frmMusteri()
        {   try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.Company, "Satış Onay Açılıyor");
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    // Hata veren DLL'in adını al
                    string assemblyName = new AssemblyName(args.Name).Name;

                    // Eğer NTwain veya DevExpress 21.2 isteniyorsa, klasördeki mevcut olanı ver
                    if (assemblyName == "NTwain" || assemblyName.StartsWith("DevExpress"))
                    {
                        string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName + ".dll");
                        if (File.Exists(dllPath))
                        {
                            return Assembly.LoadFrom(dllPath);
                        }
                    }
                    return null;
                };
                //InitUI();
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
        }
        EntegreFDLL.Class.BGClass GetBGClass = new EntegreFDLL.Class.BGClass();
        SqlConnectionObject conn = new SqlConnectionObject();
        private Image kimlikresmi;
        private Image Portreresmi;
        public static string CURVAL = "";
        public class Musteriler
        {
            public string CURID { get; set; }
            public string SALID { get; set; }
            public string CURNAME { get; set; }
        }
        private BackgroundWorker _backgroundWorker;
        private ManualResetEvent _workerCompletedEvent = new ManualResetEvent(false);
        private void executeBackground(Action doWorkAction, Action progressAction = null, Action completedAction = null)
        {
            try
            {
                if (_backgroundWorker != null)
                {
                    if (_backgroundWorker.IsBusy)
                    {
                        return;
                    }
                }
                _backgroundWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _backgroundWorker.DoWork += (x, y) =>
                {
                    try
                    {
                        doWorkAction.Invoke();
                    }
                    catch (Exception ex)
                    {
                        y.Cancel = true;
                        XtraMessageBox.Show("Bilinmeyen Hata. Detay : " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // throw;
                    }
                };
                if (progressAction != null)
                {
                    _backgroundWorker.ProgressChanged += (x, y) =>
                    {
                        progressAction.Invoke();
                    };
                }
                if (completedAction != null)
                {
                    _backgroundWorker.RunWorkerCompleted += (x, y) =>
                    {
                        completedAction.Invoke();
                    };
                }
                this.Enabled = false;
                _backgroundWorker.RunWorkerAsync();
            }
            catch (Exception)
            {

            }

        }
        private void completeProgress()
        {
            try
            {
                _backgroundWorker.Dispose();
                _backgroundWorker = null;
                if (!this.Enabled)
                {
                    this.Enabled = true;
                }

            }
            finally
            {
                //this.Cursor = Cursors.Default;
                _workerCompletedEvent.Set();

            }
        }
        private void frmSatisOnay_Load(object sender, EventArgs e)
        {
            //navBarDetay.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit1.MouseEnter += PictureEdit1_MouseEnter;
            pictureEdit1.MouseMove += PictureEdit1_MouseMove;
            pictureEdit1.MouseLeave += PictureEdit1_MouseLeave;

            pictureEdit2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            var pdfsize = navigationFrame1.Size;
            flyoutPanel1.OwnerControl = this;
            flyoutPanel1.Size = pdfsize;// new Size(700, 700);
            var pdfloc = navigationFrame1.Location;
            flyoutPanel1.Options.Location = new Point(250,250); //pdfloc; //
            flyoutPanel1.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
            flyoutPanel1.Options.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Fade;
            //Bilgiler();
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
        string CURID;
        string SALID;
        string raporSalids;
        string WARANTER = "";
        string RiskYuzdesi = "";
        double Geciken = 0;
        bool SatisAcik = true;
        private async void Bilgiler()
        {
            DataTable urunler = new DataTable();
            DataTable ekstre = new DataTable();
            DataTable gecikmedetay = new DataTable();
            DataTable Taksitler = new DataTable();
            DataTable TaksitDetayi = new DataTable();
            DataTable kimlik = new DataTable();
            DataTable IsBilgisi = new DataTable();
            double Pesin = 0;
            double Taksitli = 0;
            double Odenen = 0;
            double Kalan = 0;
            string GecikmeTarihi = "";
            string Ortalama = "";
            bool Ftpvar;
            SatisAcik = true;
            RiskYuzdesi = "";
            Geciken = 0;

            Entegref.SplashScreen(this, "", Properties.Settings.Default.CompanyName, "");
            try
            {
                raporSalids = string.Join(",", musterilers
                .Where(m => m.CURID == CURID)
                .Select(m => m.SALID));

                var qs = string.Format("select CUPIDENTITY,CUPPORTRAIT from CUSTOMERPICTURE where CUPCURID = '{0}'", CURID);// sabit "1993863"
                kimlik = conn.GetData(qs, Properties.Settings.Default.connectionstring);

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
                        SatisAcik = false;
                    }
                }
                else
                {
                    var tarama = conn.GetData($@"select IPPORTRAIT,IPFULLSIDE from EntegreF..IDENTYPICTURE
                        left outer join CUSIDENTITY on CUSIDTCNO = IPIDENTY
                        where CUSIDCURID = {CURID}", Properties.Settings.Default.connectionstring);
                    //if (tarama != null)
                    //{
                    //    var CUPIDENTITY = kimlik.Rows[0]["IPPORTRAIT"].ToString();
                    //    var CUPPORTRAIT = kimlik.Rows[0]["IPFULLSIDE"].ToString();
                    //    kimlikresmi = EntegreFDLL.Class.VolantConvert.FromBase64String(CUPIDENTITY).byteArrayToImage();
                    //    Portreresmi = EntegreFDLL.Class.VolantConvert.FromBase64String(CUPPORTRAIT).byteArrayToImage();
                    //    pictureEdit1.Image = Portreresmi;
                    //}
                    //else
                    //{
                    kimlikresmi = EntegreFDLL.Class.VolantConvert.FromBase64String("").byteArrayToImage();
                    Portreresmi = EntegreFDLL.Class.VolantConvert.FromBase64String("").byteArrayToImage();
                    pictureEdit1.Image = null;
                    string rtfMessage = @"{\rtf1\ansi{\colortbl ;\red255\green0\blue0;}\fs16 " +
                      "Müşteri Kimlik Taraması Eklenmemiş\\line" +
                      "\\b\\ul\\cf1 Sonki Alışverişler Kimlik Yüklenen Kadar Kapatılacak.\\b0\\ulnone\\cf0 }";
                    //XtraMessageBox.Show(rtfMessage);
                    //CustomMessageBox.ShowMessage(rtfMessage, "", this, "UYARI", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SatisAcik = false;
                    //}
                }
                //Image resim = Class.Convert.FromBase64String(kimlikresmi).byteArrayToImage();
                //string kaydetmeYolu = "resmi.png";
                //resim.Save(kaydetmeYolu);

                urunler = conn.GetData($@"select PROVAL as [Stok Kodu],PRONAME as [Stok Adı],ORDCHQUAN as [Satış Adeti],ORDCHBALANCE as [Satış Tutarı],
                        case when (select SALAMOUNT from SALES where SALID = ORDSALID) <= Risk_TutarMax then Risk_id else Risk_id+1 end as Risktutar,
                        case when ORDCHBALANCE >= Risk_TutarMax then Risk_id+1 else Risk_id end as Riskid ,
                        SMENNAME as [Satış Yapan Satıcı]
			            from ORDERS 
                        left outer join DIVISON on DIVVAL = ORDDIVISON
                        left outer join ORDERSCHILD on ORDCHORDID = ORDID
                        left outer join PRODUCTS on PROID = ORDCHPROID
                        left outer join SALESMEN on SMENID = ORDCHSMENID
                        left outer join EntegreF.dbo.KrediPuan_RiskUrunGurup on VOLUID = PROPROUID
                        left outer join EntegreF.dbo.KrediPuan_RiskUrunGurupPuan on id = Risk_id
                        where ORDSALID = {SALID}
                        union 
                        select '' as [Stok Kodu],'' as [Stok Adı],sum(ORDCHQUAN) as [Satış Adeti],sum(ORDCHBALANCE) as [Satış Tutarı],
                        p.id as Risktutar,
                        p.id as Riskid ,
                        '' as [Satış Yapan Satıcı]
			            from ORDERS 
                        left outer join DIVISON on DIVVAL = ORDDIVISON
                        left outer join ORDERSCHILD on ORDCHORDID = ORDID
                        left outer join PRODUCTS on PROID = ORDCHPROID
                        outer apply(select * from EntegreF.dbo.KrediPuan_RiskSatısGurupPuan p where ROUND(ORDBALANCE,0) between p.Risk_TutarMin and p.Risk_TutarMax) p
                        where ORDSALID = {SALID}
                        group by DIVNAME,Risk_Adi,p.id
                        order by 1 desc", Properties.Settings.Default.connectionstring);

                ekstre = conn.GetData($@"select * from(
                        select 2 as sira,SALID as ID,SALDATE as tarih,DIVNAME as MagazaAdı,
                        case when SALID > 0 then 'Alışveriş Toplamı' else 'İade Toplamı' end as Tip,
                        case when SALID > 0 then 'Alışveriş Toplamı' else 'İade Toplamı' end as [Ürün Kodu],
                        case when SALID > 0 then 'Alışveriş Toplamı' else 'İade Toplamı' end as [Ürün Adı],
                        isnull(case when ORDCHQUAN != 0 then case when SALID < 0 then -1*ORDCHQUAN else ORDCHQUAN end else case when SALID < 0 then -1*INVCHQUAN else INVCHQUAN end end, 0) as [Satılan Adet], 
                        isnull(case when SALID < 0 then -1*ORDCHBALANCE else ORDCHBALANCE end, 0) as [Teslimat Bekleyen Adet],
                        case when SALID < 0 then -1*SALAMOUNT else SALAMOUNT end as [Alisveriş Tutar],
                        cast(TaksitToplam as Char(2)) + '/' + cast(TaksitKalan as Char(2)) as [Kalan Taksit Sayısı], '' as Satici
                        from SALES
                        left outer join DIVISON on DIVVAL = SALDIVISON
                        outer apply(select SUM(isnull(ORDCHQUAN, 0)) as ORDCHQUAN from ORDERS
                                    left outer join ORDERSCHILD on ORDID = ORDCHORDID
                                    where SALID = ORDSALID) ORDERSCHILD
                                    outer apply(select SUM(isnull(ORDCHBALANCEQUAN, 0)) as ORDCHBALANCE from ORDERS
                                    left outer join ORDERSCHILD on ORDID = ORDCHORDID
                                    where SALID = ORDSALID) ORDERSCHILDBALANCE
                        outer apply(select SUM(isnull(PROBHQUAN, 0)) as INVCHQUAN from INVOICE
                                    left outer join INVOICECHILD on INVID = INVCHINVID
                                    left outer join INVOICECHILDPROBH on INVCHPBHID = INVCHID
                                    left outer join PRODUCTSBEHAVE on PROBHID = INVCHPBHPROBHID
                                    where SALID = INVSALID) INVOICECHILD
                        outer apply (select count(*) as TaksitToplam from INSTALMENT where INSSALID = SALID) INSTALMENTCount
                        outer apply (select count(*) as TaksitKalan from INSTALMENT where INSSALID = SALID and INSBALANCE > 0) INSTALMENT
                        where SALCURID = {CURID}
                        union all
                        select 1 as sira, ORDSALID as ID,ORDDATE as tarih,DIVNAME as MagazaAdı,
                        case when SALID > 0 and SALAMOUNT != 0 then 'Ürünler' else 'İade Ürünler' end,PROVAL,PRONAME,
                        case when SALID < 0 then -1*ORDCHQUAN else ORDCHQUAN end,
                        case when SALID < 0 then -1*ORDCHBALANCEQUAN else ORDCHBALANCEQUAN end,
                        case when SALID < 0 then -1*ORDCHBALANCE else ORDCHBALANCE end AlisverisTutar,'', SMENNAME from ORDERS
                        left outer join SALES on SALID = ORDSALID
                        left outer join ORDERSCHILD on ORDID = ORDCHORDID
                        left outer join PRODUCTS on PROID = ORDCHPROID
                        left outer join SALESMEN on SMENID = ORDCHSMENID
                        left outer join DIVISON on DIVVAL = ORDDIVISON
                        where ORDCURID = {CURID}
                        and SALSHIPKIND = 'S'
                        union all
                        select 1 as sira, SALID as ID,INVDATE as tarih,DIVNAME as MagazaAdı,
                        case when SALID > 0 and SALAMOUNT = 0 then 'VADE FARKI'
                        when SALID > 0 and SALAMOUNT != 0 then 'Ürünler' else 'İade Ürünler' end,PROVAL,PRONAME,
                        case when SALID < 0 then -1*PROBHQUAN else PROBHQUAN end,
                        case when SALID < 0 then -1*PROBHQUAN else PROBHQUAN end,
                        case when SALID < 0 then -1*INVCHBALANCE else INVCHBALANCE end AlisverisTutar,'', SMENNAME from INVOICE
                        left outer join SALES on SALID = INVSALID 
                        left outer join INVOICECHILD on INVID = INVCHINVID
                        left outer join INVOICECHILDPROBH on INVCHPBHID = INVCHID
                        left outer join PRODUCTSBEHAVE on PROBHID = INVCHPBHPROBHID
                        left outer join PRODUCTS on PROID = PROBHPROID
                        left outer join SALESMEN on SMENID = INVCHSMENID
                        left outer join DIVISON on DIVVAL = INVDIVISON
                        where SALCURID = {CURID}
                        and SALSHIPKIND = 'H'
                        union all
                        select 1 as sira, 99999999 as ID,PCDSDATE as tarih,DIVNAME as MagazaAdı,
                        case when PCDSDC = 0 then 'Ödeme' else 'Ödeme İadesi' end,'','',0,0,
                        case when PCDSDC = 0 then PCDSAMOUNT else PCDSAMOUNT *-1 end AlisverisTutar,'', SONAME +' ' + SOSURNAME from PROCEEDS
                        left outer join CASHIER on CHVAL = PCDSCASHIER
                        left outer join SOCIAL on SOCODE = CHSOCODE
                        left outer join DIVISON on DIVVAL = PCDSDIVISON
                        where PCDSCURID = {CURID}
                        ) net
                        order by 3,2,1", Properties.Settings.Default.connectionstring);

                Pesin = double.Parse(conn.GetValueConnection($@"
                        select isnull(sum(SALAMOUNT),0) from SALES
                        where SALCURID = {CURID}
                        --AND SALID not in ({raporSalids})
                        AND SALSALEKIND = 'P'", Properties.Settings.Default.connectionstring));

                Taksitli = double.Parse(conn.GetValueConnection($@"
                        select isnull(sum(SALAMOUNT),0)  from SALES s
                        where SALCURID = {CURID}
                        and SALID > 0
                        and not exists (select * from SALES i where i.SALCANSALID = s.SALID) 
                        --AND SALID not in ({raporSalids})
                        AND SALSALEKIND = 'T'", Properties.Settings.Default.connectionstring));

                Odenen = double.Parse(conn.GetValueConnection($@"
                        select 
                        isnull(sum(case when PCDSDC = 0 then (PCDSAMOUNT-PCDSEARLYPAYDISC) else (PCDSAMOUNT-PCDSEARLYPAYDISC)*-1 end +PCDSEARLYPAYDISC+PCDSLATEINCOME),0)
                        from PROCEEDS 
                        where PCDSCURID = {CURID} and (PCDSKIND != 2 and PCDSKIND > 0)", Properties.Settings.Default.connectionstring));

                Kalan = double.Parse(conn.GetValueConnection($@"
                        select isnull(sum(INSBALANCE),0)  from INSTALMENT
                        where INSCURID = {CURID}
                        --AND INSSALID not in ({raporSalids})
                        AND INSBALANCE > 0", Properties.Settings.Default.connectionstring));

                gecikmedetay = conn.GetData($@"select count(*) ,isnull(sum(PCDSLATEINCOME), 0) from PROCEEDS where PCDSCURID = {CURID} and isnull(PCDSLATEINCOME,0) != 0", Properties.Settings.Default.connectionstring);
                Ortalama = conn.GetValueConnection($@"
                        select isnull(sum(DATEDIFF(Day,INSFIXDATE,PCDSDATE))/count(*),0) from INSTALMENT
                        outer apply(select PCDSDATE from INSTALMENTPROCEEDS 
			                        left outer join PROCEEDS on PCDSID = INSPCDPCDID
			                        where INSPCDINSID = INSID 
			                        and INSPCDLATEINCOME != 0) odeme
                         where INSCURID = {CURID}  and odeme.PCDSDATE is not NULL", Properties.Settings.Default.connectionstring);

                var gc = conn.GetData($@"
                        select isnull(sum(isnull(INSBALANCE,0)),0) as Tutar, isnull(min(INSFIXDATE),0) as Tarih from INSTALMENT
                        where INSCURID = {CURID}
                        --AND INSSALID not in ({raporSalids})
                        AND INSBALANCE > 0
                        AND INSFIXDATE between '2000-01-01' and DATEADD(day,-60, GETDATE())", Properties.Settings.Default.connectionstring);
                if (gc != null)
                {
                    Geciken = double.Parse(gc.Rows[0][0].ToString());
                    GecikmeTarihi = DateTime.Parse(gc.Rows[0][1].ToString()).ToString("yyyy-MM-dd");
                }
                RiskYuzdesi = conn.GetValueConnection($@"
                        select ROUND(RiskYuzdesi,0) from (
                        select ROUND(sum(SALAMOUNT),0) as SALAMOUNT from SALES 
                        where SALID in ({raporSalids})
                        and SALCURID = {CURID}) SALES
                        outer apply (
                        SELECT TOP 1 
                            id,
                            Risk_Adi,
                            Risk_TutarMin,
                            Risk_TutarMax,
                            CAST(
                                25 * (id) - 25 + 
                                (CAST( (SALAMOUNT+{Kalan}) - Risk_TutarMin AS FLOAT) / NULLIF(Risk_TutarMax - Risk_TutarMin, 0)) * 25
                            AS DECIMAL(5,2)) AS RiskYuzdesi
                        FROM EntegreF..KrediPuan_RiskSatısGurupPuan
                        WHERE (SALAMOUNT+{Kalan}) BETWEEN Risk_TutarMin AND Risk_TutarMax) oran", Properties.Settings.Default.connectionstring);

                WARANTER = conn.GetValueConnection($@"select SALWWRTRID from SALESWARRANTERS where SALWSALID = {SALID}", Properties.Settings.Default.connectionstring);

                TaksitDetayi = conn.GetData($@"
                        select Convert(char(10),INSFIXDATE,121) as [Tarksit Tarihi],INSAMOUNT as [Taksit Tutarı],INSBALANCE as [Ödenecek Bakiye],
                            Convert(numeric(18,2),(
                              SELECT 
                                SUM(
                                  CASE WHEN DATEDIFF(DAY, vade.INSFIXDATE, getdate()) <= 59 
                                  THEN 
                                    0 
                                  ELSE (
                                    vade.INSBALANCE * 3.00 * DATEDIFF(DAY, (vade.INSFIXDATE),getdate()) / 3000) 
                                  END
                                ) 
                              FROM 
                                INSTALMENT vade WITH (NOLOCK) 
                              WHERE 
                                INSCOMPANY = INSCOMPANY 
                                AND INSID = taksit.INSID
                                AND INSBALANCE > 0                             
                            )) VADEFARKI 
                        from INSTALMENT taksit
                        where taksit.INSCURID = {CURID}
                        AND taksit.INSBALANCE > 0
                        --AND taksit.INSSALID not in ({raporSalids})
                        ORDER BY 1", Properties.Settings.Default.connectionstring);
                Taksitler = conn.GetData($@"
                    SET language turkish
                    select INFIXDATE as [Tarksit Tarihi], sum(INSAMOUNT) as [Taksit Toplamı],sum(INSBALANCE) as [Ödenecek Bakiye], sum(VADEFARKI) as [Vade Farkı] from (
                    select 
                    DATENAME(YEAR,INSFIXDATE) +' '+ upper(DATENAME(MONTH,INSFIXDATE)) as INFIXDATE,
                    INSAMOUNT,
                    INSBALANCE,
                        Convert(numeric(18,2),(
                          SELECT 
                            SUM(
                              CASE WHEN DATEDIFF(DAY, vade.INSFIXDATE, getdate()) <= 59 
                              THEN 
                                0 
                              ELSE (
                                vade.INSBALANCE * 3.00 * DATEDIFF(DAY, (vade.INSFIXDATE),getdate()) / 3000) 
                              END
                            ) 
                          FROM 
                            INSTALMENT vade WITH (NOLOCK) 
                          WHERE 
                            INSCOMPANY = INSCOMPANY 
                            AND INSID = taksit.INSID
                            AND INSBALANCE > 0                             
                        )) VADEFARKI 
                    from INSTALMENT taksit
                    where taksit.INSCURID = {CURID}
                    AND taksit.INSBALANCE > 0) toplam
                    group by INFIXDATE
                        ORDER BY 1", Properties.Settings.Default.connectionstring);
                IsBilgisi = conn.GetData($@"
                        select 
                        CUSIDWORKNAME,
                        CUSIDWORKADR1,
                        CUSIDWORKADR2,
                        CUSIDWORKCOUNTY,
                        CUSIDWORKCITY,
                        CUSIDWORKPOSTALCODE,
                        CUSIDWORKPHONE1,
                        CUSIDWORKSGKNO
                        from CUSIDENTITY where CUSIDCURID = {CURID}
                        and (
                        CUSIDWORKNAME != '' or CUSIDWORKADR1 != '' or
                        CUSIDWORKADR2 != '' or CUSIDWORKCOUNTY != '' or 
                        CUSIDWORKCITY != '' or CUSIDWORKPOSTALCODE  != '' or
                        CUSIDWORKPHONE1 != '' or CUSIDWORKSGKNO != '')", Properties.Settings.Default.connectionstring);

            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            await SplashScrenn.RunWithSplashAsync(this, false, 0, this.Text,
                async (progress, token) =>
                {
                    await Task.Delay(100, token);
                    token.ThrowIfCancellationRequested();
                    progress.Report((0, $"Yükleniyor... "));


                    if (!string.IsNullOrWhiteSpace(WARANTER))
                    {
                        Kefil();
                    }
                    await Musteri();
                    navBarDetay.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                    navigationFrame1.SelectedPage = navigationPage1;
                    Invoke((MethodInvoker)delegate
                    {
                        //Ftpvar = KlasordeDosyaVarMi(CURID + "/E-Devlet");
                        //if (!Ftpvar)
                        //{
                        //    Ftpvar = KlasordeDosyaVarMi(CURID + "/Yüklenen Dosyalar");
                        //}
                        //if (Ftpvar)
                        //{
                        //    chkEDevlet.Checked = true;
                        //    chkEDevlet.Text = "Yüklü";
                        //}
                        //else
                        //{
                        //    chkEDevlet.Checked = false;
                        //    chkEDevlet.Text = "Yok";
                        //}
                        //if (WARANTER != "")
                        //{
                        //    tileBarItem6.Enabled = true;
                        //    tileBarItem6.Tag = WARANTER;
                        //    chkHesap.Checked = true;
                        //    chkHesap.Text = "Kefil Var";
                        //}
                        //else
                        //{
                        //    tileBarItem6.Enabled = false;
                        //    tileBarItem6.Tag = "";
                        //    chkHesap.Checked = false;
                        //    chkHesap.Text = "Yok";
                        //}
                        Notlar();
                        txtTaksitli.EditValue = Taksitli;
                        txtOdenen.EditValue = Odenen;
                        txtKalan.EditValue = Kalan;
                        if (gecikmedetay.Rows.Count > 0)
                        {
                            txtVadeFarkiSayisi.EditValue = gecikmedetay.Rows[0][0].ToString();
                            txtVadeFarkiTtuari.EditValue = gecikmedetay.Rows[0][1].ToString();
                        }
                        else
                        {
                            txtVadeFarkiSayisi.EditValue = 0;
                            txtVadeFarkiTtuari.EditValue = 0;
                        }
                        txtVadeFarkiGun.EditValue = Ortalama;
                        if (Geciken > 10)
                        {
                            blinkTimer.Start();
                            txtGeciken.EditValue = Geciken;
                            txtGecikenTarih.EditValue = GecikmeTarihi;
                        }
                        else
                        {
                            blinkTimer.Stop();
                            txtGeciken.EditValue = 0;
                            txtGecikenTarih.EditValue = "";
                        }
                        //UpdateRiskBar(int.Parse(RiskYuzdesi.Replace(".00", "")));
                        pictureEdit1.Image = Portreresmi;//Class.Convert.FromBase64String(kimlikresmi).byteArrayToImage();
                        gridEkstre.DataSource = ekstre;
                        gridTaksitDetay.DataSource = TaksitDetayi;
                        gridTaksit.DataSource = Taksitler;
                        ViewEkstre.OptionsBehavior.Editable = false;
                        ViewEkstre.OptionsBehavior.ReadOnly = true;
                        ViewEkstre.OptionsBehavior.ReadOnly = true;
                        ViewEkstre.Columns["tarih"].GroupIndex = 0;
                        ViewEkstre.Columns["sira"].Visible = false;
                        ViewEkstre.Columns["ID"].Visible = false;
                        ViewEkstre.OptionsSelection.EnableAppearanceFocusedRow = false;
                        ViewEkstre.Appearance.FocusedRow.Options.UseBackColor = false;
                        ViewEkstre.Appearance.SelectedRow.Options.UseBackColor = false;
                        ViewEkstre.OptionsView.ColumnAutoWidth = false;
                        ViewEkstre.ExpandAllGroups();
                        ViewEkstre.OptionsView.BestFitMaxRowCount = -1;
                        ViewEkstre.BestFitColumns(true);
                        ViewEkstre.Appearance.FocusedCell.Options.UseBackColor = false;
                        ViewEkstre.Appearance.FocusedCell.Options.UseForeColor = false;
                        ViewTaksitDetay.FocusedRowHandle = GridControl.InvalidRowHandle;
                        ViewTaksitDetay.OptionsSelection.MultiSelect = false;
                        ViewTaksitDetay.OptionsBehavior.Editable = false;
                        ViewTaksitDetay.OptionsBehavior.ReadOnly = true;
                        ViewTaksitDetay.OptionsSelection.EnableAppearanceFocusedRow = false;
                        ViewTaksitDetay.Appearance.FocusedRow.Options.UseBackColor = false;
                        ViewTaksitDetay.Appearance.SelectedRow.Options.UseBackColor = false;
                        ViewTaksitDetay.OptionsView.ColumnAutoWidth = false;
                        ViewTaksitDetay.ExpandAllGroups();
                        ViewTaksitDetay.OptionsView.BestFitMaxRowCount = -1;
                        ViewTaksitDetay.BestFitColumns(true);
                        ViewTaksitDetay.Appearance.FocusedCell.Options.UseBackColor = false;
                        ViewTaksitDetay.Appearance.FocusedCell.Options.UseForeColor = false;
                        ViewNotes.OptionsView.ColumnAutoWidth = false;
                        ViewNotes.OptionsView.BestFitMaxRowCount = -1;
                        ViewNotes.BestFitColumns(true);


                        completeProgress();
                        navBarDetay.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;


                    });
                });

        }

        private bool isRed = true;
        private void blinkTimer_Tick(object sender, EventArgs e)
        {
            if (isRed)
            {
                txtGeciken.BackColor = Color.White;
                txtGeciken.ForeColor = Color.Red;
                txtGeciken.Font = new Font(txtGeciken.Font.FontFamily, 12, FontStyle.Bold);


                txtGecikenTarih.BackColor = Color.White;
                txtGecikenTarih.ForeColor = Color.Red;
                txtGecikenTarih.Font = new Font(txtGecikenTarih.Font.FontFamily, 12, FontStyle.Bold);

                isRed = false;
            }
            else
            {
                txtGeciken.BackColor = Color.White;
                txtGeciken.ForeColor = Color.Black;
                txtGeciken.Font = new Font(txtGeciken.Font.FontFamily, 8, FontStyle.Regular);


                txtGecikenTarih.BackColor = Color.White;
                txtGecikenTarih.ForeColor = Color.Black;
                txtGecikenTarih.Font = new Font(txtGecikenTarih.Font.FontFamily, 8, FontStyle.Regular);
                isRed = true;
            }
        }
        public bool KlasordeDosyaVarMi(string folderPath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(Entegref.GetLogins.FTPURL + "/" + folderPath));
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(Entegref.GetLogins.FTPUSER, Entegref.GetLogins.FTPPASS);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    // Klasördeki tüm öğeleri al (dosya + klasör isimleri)
                    List<string> entries = new List<string>();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                            entries.Add(line);
                    }

                    // Eğer hiç sonuç yoksa klasör boş
                    return entries.Count > 0;  // true = dosya var, false = dosya yok
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = ex.Response as FtpWebResponse;

                if (response != null &&
                    response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false; // klasör yok
                }

                return false;
            }
        }
        private bool CheckFtpFolderExists(string folderPath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(Entegref.GetLogins.FTPURL + "/" + folderPath));
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(Entegref.GetLogins.FTPUSER, Entegref.GetLogins.FTPPASS);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {

                    return true; // Klasör var
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false; // Klasör bulunamadı
                }
                else
                {
                    return false;
                }
            }
        }
        private void frmSatisOnay_SizeChanged(object sender, EventArgs e)
        {
            navBarDetay.Width = this.Size.Width;
        }
        private void UpdateRiskBar(int value)
        {
            //int barHeight = panelBar.Height;
            //int barWidth = panelBar.Width;

            //Bitmap bmp = new Bitmap(barWidth, barHeight);
            //using (Graphics g = Graphics.FromImage(bmp))
            //{
            //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //    // Bölümlerin genişlikleri
            //    int greenWidth = (int)(barWidth * 0.24);
            //    int yellowWidth = (int)(barWidth * 0.25);
            //    int orangeWidth = (int)(barWidth * 0.25);
            //    int redWidth = barWidth - greenWidth - yellowWidth - orangeWidth;

            //    // Soldan sağa renkler
            //    int x = 0;
            //    g.FillRectangle(Brushes.Green, x, 0, greenWidth, barHeight); x += greenWidth;
            //    g.FillRectangle(Brushes.Yellow, x, 0, yellowWidth, barHeight); x += yellowWidth;
            //    g.FillRectangle(Brushes.Orange, x, 0, orangeWidth, barHeight); x += orangeWidth;
            //    g.FillRectangle(Brushes.Red, x, 0, redWidth, barHeight);

            //    // Yüzde + kategori yazısı
            //    string category = GetCategoryName(value);
            //    string text = $"{value}% - {category}";
            //    using (Font f = new Font("Segoe UI", 10, FontStyle.Bold))
            //    using (Brush b = new SolidBrush(Color.Black))
            //    {
            //        var textSize = g.MeasureString(text, f);
            //        g.DrawString(text, f, b, (barWidth - textSize.Width) / 2, (barHeight - textSize.Height) / 2);
            //    }
            //}

            //panelBar.BackgroundImage = bmp;
            //panelBar.BackgroundImageLayout = ImageLayout.Stretch;

            //// Ok pozisyonu (0 solda, 100 sağda)
            //int positionX = (int)(value / 100.0 * barWidth) - lblArrow.Width / 2;
            //lblArrow.Left = panelBar.Left + Math.Max(0, Math.Min(barWidth - lblArrow.Width, positionX));
            //lblArrow.Top = panelBar.Bottom + 2; // Ok barın altında
            //lblArrow.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            //lblArrow.ForeColor = Color.Black;
            //lblArrow.Text = "▲"; // Yatayda yukarı bakan ok
        }    
        private string GetCategoryName(int value)
        {
            if (value <= 24) return "Güvenli";
            if (value <= 49) return "Orta";
            if (value <= 74) return "Riskli";
            return "Kritik";
        }
        private void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage1;
        }
        private void tileBarItem2_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage2;
        }
        private void tileBarItem4_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage4;
            //Notlar();
        }
        private void tileBarItem5_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage6;
        }
        private void tileBarItem6_ItemClick(object sender, TileItemEventArgs e)
        {
            var Kefil = conn.GetData($@"select SALWWRTRID,WRTRNAME + ' ' + WRTRSURNAME as WRTRNAME,SALID,SALCURID from SALESWARRANTERS 
            join WARRANTERS on WRTRID = SALWWRTRID
            join SALES on SALID = SALWSALID
            where SALWSALID in ({SALID})", Properties.Settings.Default.connectionstring);
            List<Class.BGClass.BGKefil.Kefil> kefils = new List<Class.BGClass.BGKefil.Kefil>();
            for (int i = 0; i < Kefil.Rows.Count; i++)
            {
                kefils.Add(new Class.BGClass.BGKefil.Kefil
                {
                    WRTRID = Kefil.Rows[i]["SALWWRTRID"].ToString(),
                    WRTRNAME = Kefil.Rows[i]["WRTRNAME"].ToString(),
                    CURID = Kefil.Rows[i]["SALCURID"].ToString(),
                    SALID = Kefil.Rows[i]["SALID"].ToString(),
                });
            }
            pnlMain.OpenForm<frmKefilCoklu>(kefils);
            //frmKefilCoklu kefilCoklu = new frmKefilCoklu(kefils);
            //kefilCoklu.ShowDialog();
            //frmBGKefil kefil = new frmBGKefil(tileBarItem6.Tag.ToString(), raporSalids, CURID);
            //    kefil.ShowDialog();
        }
        private void tileBarItem7_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage5;
        }
        public async Task Musteri()
        {
            Currents myDeserializedClass = new Currents();
            Program.FBGConfigProvider.Servis = "GetCustomerForProfile";
            Program.FBGConfigProvider.filter.curId = long.Parse(CURID);
            await Task.Run(async () =>
            {
                var sonuc = await GetBGClass.VolantServisAsync(Program.FBGConfigProvider);
                myDeserializedClass = JsonConvert.DeserializeObject<Currents>(sonuc);
            });
            if (myDeserializedClass.success)
            {
                //Kişisel Bilgiler
                txtMagaza.EditValue = conn.GetValueConnection($@"select DIVNAME from DIVISON where DIVVAL = '{myDeserializedClass.rCurrents.CURDIVISONk__BackingField}'", Properties.Settings.Default.connectionstring);
                txtKodu.EditValue = myDeserializedClass.rCurrents.CURVALk__BackingField;
                txtAdi.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDNAMEk__BackingField;
                txtSoyadi.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSIRNAMEk__BackingField;
                txtTC.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDTCNOk__BackingField;
                txtVknName.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHWATPk__BackingField;
                txtVknNo.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHWATNOk__BackingField;
                txtSgkNoı.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKSGKNOk__BackingField;
                if (myDeserializedClass.rCurrents.CURSTSk__BackingField)
                {
                    togAktif.IsOn = true;
                }
                string input = myDeserializedClass.rCurrents.kirilimAckk__BackingField;
                // </br> ile ayır ve boş olanları filtrele
                string[] lines = input.Split(new string[] { "</br>" }, StringSplitOptions.RemoveEmptyEntries);
                // DataTable oluştur
                DataTable dt = new DataTable();
                dt.Columns.Add("Sinif Adı");
                dt.Columns.Add("Seçili Değer");
                bool sinif_Var = false;
                foreach (string line in lines)
                {
                    if (line.Contains("ALIŞVERİŞ KREDİ YAPISI"))
                    {
                        sinif_Var = true;
                    }
                    // ':' ile ayır ve sol/sağ kırp
                    string[] parts = line.Split(new char[] { ':' }, 2); // Sadece ilk ':''dan ayır
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        var SatisYapılamaz = int.Parse(conn.GetValueConnection($@"select count(*) from WAVECUSTREE where WCTRECANSALE = 0 and WCTRENAME = '{value}'", Properties.Settings.Default.connectionstring));
                        if (SatisYapılamaz != 0)
                        {
                            string rtfMessage = @"{\rtf1\ansi{\colortbl ;\red255\green0\blue0;}\fs20 " +
                              "Müşteriye Satış Yapılamaz Eklenmiş\\line" +
                              "\\b\\ul\\cf1 Müşteri Onayı Yaparken Dikkat Ediniz.\\b0\\ulnone\\cf0 }";

                            CustomMessageBox.ShowMessage(rtfMessage, "", this, "UYARI", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        if (key == "EV SAHIBI")
                        {
                            if (value == "EV SAHİBİ")
                            {
                                chkEvSahibi.Checked = true;
                            }
                            chkEvSahibi.Text = value;

                        }
                        dt.Rows.Add(key, value);
                    }
                }
                if (!sinif_Var)
                {
                    var varyok = conn.GetData($@"select * from WAVECUSTOMER where WCUSCURID = {CURID} and WCUSUNIQ = 9", Properties.Settings.Default.connectionstring);
                    if (varyok == null)
                    {
                        var sinifekleme = conn.InsertValue($@"insert into WAVECUSTOMER values ({CURID},9,'YON')", Properties.Settings.Default.connectionstring);
                        dt.Rows.Add("ALIŞVERİŞ KREDİ YAPISI", "Satışa Açık");
                    }
                }
                gridSiniflar.DataSource = dt;
                dteDogumTarihi.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDBIRTHDAYk__BackingField;
                txtYas.EditValue = DateTime.Now.Year - DateTime.Parse(myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDBIRTHDAYk__BackingField.ToString()).Year;

                //Kimlik Bilgileri
                txtCuzdanNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSERIALNOk__BackingField;
                txtBaba.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDFATHERk__BackingField;
                txtAnne.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDMOTHERk__BackingField;
                txtDogumYeri.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDBIRTHPLACEk__BackingField;
                txtDogumIl.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGCITYk__BackingField;
                txtDogumIlce.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGCOUNTYk__BackingField;
                txtMahlle.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGREGIONk__BackingField;
                txtCiltNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDVOLNOk__BackingField;
                txtAileSiraNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDFAMILYNOk__BackingField;
                txtSiraNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSORTNOk__BackingField;
                txtKayitNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGNOk__BackingField;                
                txtVerildigiYer1.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDGIVENPLACEk__BackingField;
                dteVerildiTarih1.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDGIVENDATEk__BackingField;
                int vierilisnedeni = int.Parse(myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDGIVENREASONk__BackingField.ToString());
                foreach (CheckedListBoxItem item in chekVerilisNedeni.Items)
                {
                    if (int.Parse(item.Value.ToString()) == vierilisnedeni)
                    {
                        item.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        item.CheckState = CheckState.Unchecked;
                    }
                }
                string sex = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSEXk__BackingField;
                string maried = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDMARRIEDk__BackingField;
                for (int i = 0; i < rdCinsiyet.Properties.Items.Count; i++)
                {
                    if (rdCinsiyet.Properties.Items[i].Value.ToString() == sex)
                    {
                        rdCinsiyet.SelectedIndex = i;
                        break; // eşleşmeyi bulduysan döngüden çık
                    }
                }
                for (int i = 0; i < rdMedeniHal.Properties.Items.Count; i++)
                {
                    if (rdMedeniHal.Properties.Items[i].Value.ToString() == maried)
                    {
                        rdMedeniHal.SelectedIndex = i;
                        break; // eşleşmeyi bulduysan döngüden çık
                    }
                }
                txtEhilyetNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDDRVNOk__BackingField;
                txtVerildiYer2.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDDRVGIVENPLACEk__BackingField;
                dteVerildiTarih2.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDDRVGIVENDATEk__BackingField;

                txtEvIl.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHCITYk__BackingField;
                txtEvIlce.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHCOUNTYk__BackingField;
                txtEvMahalle.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHADR1k__BackingField;
                txtEvAdres.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHADR2k__BackingField;
                txtPKod.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPOSTALCODEk__BackingField;
                txtMail.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHEMAILk__BackingField;

                txtIsIl.EditValue   = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKCITYk__BackingField;
                txtIsIlce.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKCOUNTYk__BackingField;
                txtIsAdi.EditValue  = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKNAMEk__BackingField;
                txtIsTel.EditValue  = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKPHONE1k__BackingField;
                txtIsadres1.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKADR1k__BackingField;
                txtIsAdres2.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKADR2k__BackingField;

                txtGsm1.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHGSM1k__BackingField;
                txtGsm2.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHGSM2k__BackingField;
                txtGsm3.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHGSM3k__BackingField;

                txtTel1.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPHONE1k__BackingField;
                txtTel2.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPHONE2k__BackingField;
                txtTel3.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPHONE3k__BackingField;
            }
        }
        public async void Kefil()
        {
            VolantApiClass.Filter filter = new VolantApiClass.Filter
            {
                username = Entegref.GetLogins.userID,
                password = EntegreFDLL.Class.Entegref.GetLogins.userPass,
                soCode = Entegref.GetLogins.userID,
                curId = long.Parse(CURID),
            };
            Currents myDeserializedClass = new Currents();
            await Task.Run(async () =>
            {
                var sonuc = await BGClass.VolantServisAsync(Program.FBGConfigProvider);
                myDeserializedClass = JsonConvert.DeserializeObject<Currents>(sonuc);
            });
            if (myDeserializedClass.success)
            {
                //Kişisel Bilgiler
                txtMagaza.EditValue = conn.GetValueConnection($@"select DIVNAME from DIVISON where DIVVAL = '{myDeserializedClass.rCurrents.CURDIVISONk__BackingField}'", Properties.Settings.Default.connectionstring);
                txtKodu.EditValue = myDeserializedClass.rCurrents.CURVALk__BackingField;
                txtAdi.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDNAMEk__BackingField;
                txtSoyadi.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSIRNAMEk__BackingField;
                txtTC.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDTCNOk__BackingField;
                txtVknName.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHWATPk__BackingField;
                txtVknNo.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHWATNOk__BackingField;
                txtSgkNoı.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKSGKNOk__BackingField;
                if (myDeserializedClass.rCurrents.CURSTSk__BackingField)
                {
                    togAktif.IsOn = true;
                }
                string input = myDeserializedClass.rCurrents.kirilimAckk__BackingField;
                // </br> ile ayır ve boş olanları filtrele
                string[] lines = input.Split(new string[] { "</br>" }, StringSplitOptions.RemoveEmptyEntries);

                // DataTable oluştur
                DataTable dt = new DataTable();
                dt.Columns.Add("Sinif Adı");
                dt.Columns.Add("Seçili Değer");

                foreach (string line in lines)
                {
                    // ':' ile ayır ve sol/sağ kırp
                    string[] parts = line.Split(new char[] { ':' }, 2); // Sadece ilk ':''dan ayır
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        dt.Rows.Add(key, value);
                    }
                }
                gridSiniflar.DataSource = dt;
                dteDogumTarihi.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDBIRTHDAYk__BackingField;


                //Kimlik Bilgileri
                txtCuzdanNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSERIALNOk__BackingField;
                txtBaba.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDFATHERk__BackingField;
                txtAnne.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDMOTHERk__BackingField;
                txtDogumYeri.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDBIRTHPLACEk__BackingField;
                txtDogumIl.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGCITYk__BackingField;
                txtDogumIlce.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGCOUNTYk__BackingField;
                txtMahlle.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGREGIONk__BackingField;
                txtCiltNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDVOLNOk__BackingField;
                txtAileSiraNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDFAMILYNOk__BackingField;
                txtSiraNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSORTNOk__BackingField;
                txtKayitNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDREGNOk__BackingField;
                txtVerildigiYer1.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDGIVENPLACEk__BackingField;
                dteVerildiTarih1.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDGIVENDATEk__BackingField;
                int vierilisnedeni = int.Parse(myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDGIVENREASONk__BackingField.ToString());
                foreach (CheckedListBoxItem item in chekVerilisNedeni.Items)
                {
                    if (int.Parse(item.Value.ToString()) == vierilisnedeni)
                    {
                        item.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        item.CheckState = CheckState.Unchecked;
                    }
                }
                string sex = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDSEXk__BackingField;
                string maried = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDMARRIEDk__BackingField;

                for (int i = 0; i < rdCinsiyet.Properties.Items.Count; i++)
                {
                    if (rdCinsiyet.Properties.Items[i].Value.ToString() == sex)
                    {
                        rdCinsiyet.SelectedIndex = i;
                        break; // eşleşmeyi bulduysan döngüden çık
                    }
                }
                for (int i = 0; i < rdMedeniHal.Properties.Items.Count; i++)
                {
                    if (rdMedeniHal.Properties.Items[i].Value.ToString() == maried)
                    {
                        rdMedeniHal.SelectedIndex = i;
                        break; // eşleşmeyi bulduysan döngüden çık
                    }
                }
                txtEhilyetNo.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDDRVNOk__BackingField;
                txtVerildiYer2.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDDRVGIVENPLACEk__BackingField;
                dteVerildiTarih2.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDDRVGIVENDATEk__BackingField;

                txtEvIl.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHCITYk__BackingField;
                txtEvIlce.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHCOUNTYk__BackingField;
                txtEvMahalle.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHADR1k__BackingField;
                txtEvAdres.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHADR2k__BackingField;
                txtPKod.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPOSTALCODEk__BackingField;
                txtMail.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHEMAILk__BackingField;

                txtIsIl.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKCITYk__BackingField;
                txtIsIlce.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKCOUNTYk__BackingField;
                txtIsAdi.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKNAMEk__BackingField;
                txtIsTel.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKPHONE1k__BackingField;
                txtIsadres1.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKADR1k__BackingField;
                txtIsAdres2.EditValue = myDeserializedClass.rCurrents.rCusIdentityk__BackingField.CUSIDWORKADR2k__BackingField;

                txtGsm1.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHGSM1k__BackingField;
                txtGsm2.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHGSM2k__BackingField;
                txtGsm3.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHGSM3k__BackingField;

                txtTel1.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPHONE1k__BackingField;
                txtTel2.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPHONE2k__BackingField;
                txtTel3.EditValue = myDeserializedClass.rCurrents.rCurrentsChildk__BackingField.CURCHPHONE3k__BackingField;
            }
        }
        public async void Notlar()
        {
            Program.FBGConfigProvider.filter.curId = long.Parse(CURID);
            CurNotes myDeserializedClass = new CurNotes();
            List<Notes> curNotes = new List<Notes>();
            await Task.Run(async () =>
            {
                var sonuc = await GetBGClass.VolantServisAsync(Program.FBGConfigProvider);
                myDeserializedClass = JsonConvert.DeserializeObject<CurNotes>(sonuc);
            });
            foreach (var item in myDeserializedClass.lCurNotes)
            {
                curNotes.Add(new Notes
                {
                    NOT_TIPI = item.notTipForServicek__BackingField,
                    NOT = item.CURNTNOTESk__BackingField,
                    KAYDEDEN = item.CURNTSOCODEk__BackingField,
                    KAYITZAMANI = item.CURNTDATETIMEk__BackingField
                });
            }
            
            EntegreFDLL.Main.ListtoDataTableConverter converter = new EntegreFDLL.Main.ListtoDataTableConverter();
            var dt = converter.ToDataTable(curNotes);
            gridNotes.DataSource = dt;
        }
        public static string[] GetMailListBySALID(int salid)
        {
            List<string> mailListesi = new List<string>();

            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.connectionstring))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand($@"
                SELECT DISTINCT DIVEMAIL FROM (
                                SELECT ISNULL(DIVEMAIL,'') AS DIVEMAIL FROM CUSDELIVER 
                                LEFT OUTER JOIN DIVISON ON DIVVAL = CDRSALEDIV
                                WHERE CDRSALID = @salid 
                                UNION
                                SELECT ISNULL(DIVEMAIL,'') AS DIVEMAIL FROM CUSDELIVER
                                LEFT OUTER JOIN DEFSTORAGE ON DSTORID = CDRSTORID
                                LEFT OUTER JOIN DIVISON depo ON depo.DIVVAL = DSTORVAL
                                WHERE CDRSALID = @salid
                                UNION
                                SELECT ISNULL(DIVEMAIL,'') AS DIVEMAIL FROM SALES
                                LEFT OUTER JOIN DIVISON ON DIVVAL = SALDIVISON
                                WHERE SALID = @salid
                )SON
                WHERE ISNULL(DIVEMAIL,'') != ''", conn))
                {
                    cmd.Parameters.AddWithValue("@salid", salid);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string mail = reader["DIVEMAIL"].ToString();
                            if (!string.IsNullOrWhiteSpace(mail))
                                mailListesi.Add(mail);
                        }
                    }
                }
            }

            return mailListesi.ToArray();
        }
        private void ViewUrunler_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var riskid = gv.GetRowCellValue(e.RowHandle, "Riskid")?.ToString();
            var risktutar = gv.GetRowCellValue(e.RowHandle, "Risktutar")?.ToString();

            if (riskid == null) return;

            if (riskid == "3" || int.Parse(riskid) > 3)
            {
                e.Appearance.BackColor = Color.Red;
                e.Appearance.ForeColor = Color.White;
            }
            else if (riskid == "2")
            {
                if (int.Parse(risktutar) > int.Parse(riskid))
                {
                    e.Appearance.BackColor = Color.Yellow;
                    e.Appearance.BackColor2 = Color.Red;
                    e.Appearance.ForeColor = Color.Black;
                }
                else
                {
                    e.Appearance.BackColor = Color.Yellow;
                    e.Appearance.ForeColor = Color.Black;
                }
            }
            else if (riskid == "1")
            {
                if (int.Parse(risktutar) > int.Parse(riskid))
                {
                    e.Appearance.BackColor = Color.Green;
                    e.Appearance.BackColor2 = Color.Yellow;
                    e.Appearance.ForeColor = Color.Black;
                }
                else
                {
                    e.Appearance.BackColor = Color.Green;
                    e.Appearance.ForeColor = Color.White;
                }
            }

            e.Appearance.Options.UseBackColor = true;
            e.Appearance.Options.UseForeColor = true;

            // ÇOK ÖNEMLİ: RowStyle'ın seçili/odak stilinden sonra uygulanmasını sağlayarak üzerine yazılmasını engelle
            e.HighPriority = true;
        }
        private void ViewEkstre_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            // Grid'deki ID alanını al
            var idValue = gv.GetRowCellValue(e.RowHandle, "ID");
            var Tip = gv.GetRowCellValue(e.RowHandle, "Tip").ToString();

            if (idValue != null)
            {
                string idStr = idValue.ToString();

                // SALID ile eşleşme kontrolü
                bool match = musterilers.Any(m => m.SALID == idStr);

                if (match)
                {
                    e.Appearance.BackColor = Color.Turquoise;
                    e.Appearance.BackColor2 = Color.Red;
                    e.HighPriority = true;
                }
                else
                {
                    if (Tip == "Ödeme")
                    {
                        e.Appearance.BackColor = Color.LightYellow;
                        e.Appearance.BackColor2 = Color.Gold;
                        e.HighPriority = true;
                    }
                    else if (Tip == "Alışveriş Toplamı")
                    {
                        e.Appearance.BackColor = Color.Green;
                        e.Appearance.BackColor2 = Color.Gold;
                        e.HighPriority = true;
                    }
                    else if (Tip == "İade Toplamı")
                    {
                        e.Appearance.BackColor = Color.DarkGoldenrod;
                        e.Appearance.BackColor2 = Color.Green;
                        e.HighPriority = true;
                    }
                }
            }
        }
        private void ViewTaksit_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            object cellValue = ViewTaksit.GetRowCellValue(e.RowHandle, "Tarksit Tarihi");
            if (cellValue == null || cellValue == DBNull.Value) return;

            if (DateTime.TryParse(cellValue.ToString(), out DateTime taksitTarihi))
            {
                DateTime gecikenTarih = DateTime.Today.AddDays(-59);
                DateTime bugun = DateTime.Today;

                if (taksitTarihi.Date <= gecikenTarih)
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.White;
                    e.Appearance.ForeColor = Color.Black;
                    e.HighPriority = true;
                }
            }
        }
        private void ViewSiniflar_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
        }
        private VolantScannerAdapter scanner;
        private void btnTara_Click(object sender, EventArgs e)
        {
            #region Manus Code

            //try
            //{
            //    if (scanner == null)
            //        scanner = new VolantScannerAdapter(AppDomain.CurrentDomain.BaseDirectory);

            //    // Kendi projenizdeki ConnectionString'i buraya yazın
            //    string myConnStr = Properties.Settings.Default.connectionstring;
            //    scanner.SetConnectionString(myConnStr);
            //    scanner.InitializeVolantStartup();
            //    scanner.InitializeManagement();
            //    var tarayicilar = scanner.GetScannerSources();
            //    string secilenTarayici = "";

            //    if (tarayicilar.Count == 0)
            //    {
            //        // Eğer liste boşsa manuel giriş iste veya SetupTwain'i tekrar dene
            //        secilenTarayici = DevExpress.XtraEditors.XtraInputBox.Show("Sistemde otomatik tarayıcı bulunamadı. Lütfen tarayıcı adını el ile girin:", "Tarayıcı Bulunamadı", "");
            //    }
            //    else
            //    {
            //        // Kullanıcıya listeden seçtir (DevExpress RadioGroup veya basit bir seçim penceresi)
            //        // Örnek olarak listenin ilkini alıyoruz veya kullanıcıya seçtiriyoruz:
            //        DevExpress.XtraEditors.XtraInputBoxArgs args = new DevExpress.XtraEditors.XtraInputBoxArgs();
            //        args.Caption = "Tarayıcı Seçimi";
            //        args.Prompt = "Lütfen kullanmak istediğiniz tarayıcıyı seçin:";
            //        args.DefaultButtonIndex = 0;

            //        // ComboBox Editörünü Tanımla
            //        DevExpress.XtraEditors.ComboBoxEdit editor = new DevExpress.XtraEditors.ComboBoxEdit();
            //        foreach (var tarayici in tarayicilar)
            //        {
            //            editor.Properties.Items.Add(tarayici);
            //        }
            //        editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor; // Elle yazmayı kapat

            //        args.Editor = editor;
            //        args.DefaultResponse = tarayicilar[0]; // İlk tarayıcıyı varsayılan seç

            //        // Seçimi Göster
            //        var result = DevExpress.XtraEditors.XtraInputBox.Show(args);
            //        secilenTarayici = result?.ToString();
            //    }

            //    if (string.IsNullOrEmpty(secilenTarayici)) return;

            //    int scanObjectValue = 0;

            //    // 2. Belge tipini belirle
            //    string belgeTipi = comboBoxBelge.Text;
            //    switch (belgeTipi)
            //    {
            //        case "Kimlik": scanObjectValue = 0; break;
            //        case "Yeni Kimlik": scanObjectValue = 1; break;
            //        case "Ehliyet": scanObjectValue = 2; break;
            //        case "Pasaport": scanObjectValue = 3; break;
            //        default: scanObjectValue = 0; break;
            //    }
            //    // Taramayı başlat
            //    scanner.ScanIdentity(secilenTarayici, scanObjectValue);

            //    // Tarama tamamlanana kadar bekle
            //    while (!scanner.IsFinished)
            //    {
            //        System.Windows.Forms.Application.DoEvents();
            //        System.Threading.Thread.Sleep(200);
            //    }
            //    System.Threading.Thread.Sleep(1000);
            //    if (scanner.IsCompleted)
            //    {
            //        // Görselleri bas
            //        pictureKimlik.Image = scanner.Identity;
            //        pictureEdit1.Image = scanner.Head;

            //        // OCR verilerini doldur (dynamic kullanarak doğrudan property isimleriyle erişiyoruz)
            //        var res = scanner.RCusIdentity;
            //        if (res != null)
            //        {
            //            txtAdi.Text = res.CUSIDNAME?.ToString();
            //            txtSoyadi.Text = res.CUSIDSIRNAME?.ToString();
            //            txtTC.Text = res.CUSIDTCNO?.ToString();
            //            dteDogumTarihi.EditValue = res.CUSIDBIRTHDAY?.ToShortDateString();
            //            txtAnne.Text = res.CUSIDMOTHER?.ToString();
            //            txtBaba.Text = res.CUSIDFATHER?.ToString();
            //        }
            //    }
            //    else
            //    {
            //        DevExpress.XtraEditors.XtraMessageBox.Show("Tarama tamamlanamadı!");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DevExpress.XtraEditors.XtraMessageBox.Show("Hata: " + ex.Message);
            //}
            #endregion
        }

        private void musteriKoduTbx_TextChanged(object sender, EventArgs e)
        {
            if (musteriKoduTbx.Text == "")
            {
                musteriKoduTbx.Text = null;
                musteriKoduTbx.Properties.NullText = "Müşteri Numarası Giriniz..";
            }
        }

        private void musteriKoduTbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Bilgiler();
                ViewEkstre.Focus();
                musteriKoduTbx.Focus();
            }
        }

        private void musteriKoduTbx_Validated(object sender, EventArgs e)
        {
            try
            {
                if (musteriKoduTbx.IsModified)
                {
                    //isCurrentsRefresh = true;
                    //RefreshAll();
                    //ShowWarnings();
                    musteriKoduTbx.IsModified = false;
                    
                }
            }
            catch (Exception exp)
            {
                CustomMessageBox.ShowMessage(exp.Message,"", this,"",false,MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void musteriKoduTbx_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            Magaza.frmMusteriSec sec = new Magaza.frmMusteriSec(this);
            sec.ShowDialog();
            musteriKoduTbx.Text = CURVAL;
            CURID = conn.GetValueConnection($"select CURID from CURRENTS where CURVAL = '{CURVAL}'", Properties.Settings.Default.connectionstring);
            musteriKoduTbx.IsModified = true;
        }
    }
}