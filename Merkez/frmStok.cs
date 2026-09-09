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
using DevExpress.LookAndFeel;
using System.Threading;
using EntegrefKrediOnay.Class;
using System.Reflection;
using System.IO;
using EntegreFDLL;
using System.Data.SqlClient;
using static DataTableExtensions;
using DevExpress.Spreadsheet;
using static EntegreFDLL.Class.DataTableClass;
using EntegreFDLL.Main;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static EntegreFDLL.Class.MainExtensionsVolant;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmStok : DevExpress.XtraEditors.XtraForm
    {
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        static Convertler convertler = new Convertler();
        public class PRODUCTS
        {
            public long PROID { get; set; }

            public string PROVAL { get; set; }

            public string PRONAME { get; set; }

            public long PROPROUID { get; set; } = 1;

            public string PROKIND { get; set; } = "5";

            public decimal PROSALEVATVAL { get; set; } = 20;

            public decimal? PROPURCHVATVAL { get; set; } = 20;

            public decimal PROPURCHOTVRATE { get; set; } = 0;

            public decimal PROSALEOTVRATE { get; set; } = 0;

            public decimal PROOTVPRICE { get; set; } = 0;

            public bool PROSTS { get; set; } = true;

            public long? PROTRFPROID { get; set; }

            public short PROPRODUCTION { get; set; } = 0;

            public string PROMAINVAL { get; set; } = null;

            public string PROCOLORVAL { get; set; } = null;

            public string PROBODYKIND { get; set; } = null;

            public string PROBODY { get; set; } = null;

            public short PROPURCHKIND { get; set; } = 0;

            public short PROSALEKIND { get; set; } = 0;

            public short PROSHIPDURATION { get; set; } = 0;

            public short? PROSHIPMAXDAY { get; set; }

            public bool? PRONEVERSALEFORHT { get; set; }

            public short PROSHIPSOURCE { get; set; } = 0;

            public string PROSOCODE { get; set; }

            public DateTime PRODATETIME { get; set; } = DateTime.Now;

            public string PROLOOSEUNIT { get; set; } = null;

            public bool PROFREEPURCH { get; set; } = false;

            public bool PROHAND { get; set; } = false;

            public string PROSHIPDIVVAL { get; set; } = "";

            public bool? PROWITHSERI { get; set; } = false;

            public string PROSUPNAME { get; set; } = "";

            public string PRONOTES { get; set; } = "";

            public string PROWIDTH { get; set; } = null;

            public string PRODEPTH { get; set; } = null;

            public string PROHEIGHT { get; set; } = null;

            public string PROWEIGHT { get; set; } = null;

            public bool PRONOSUMDISC { get; set; } = false;

            public long? PROUSEFIELDINT { get; set; }

            public decimal? PROVOLUME { get; set; } = 0;

            public short? PROBUILDTIME { get; set; }

            public bool? PROMAIN { get; set; } = false;

            public decimal? PROBOX { get; set; }

        }
        public class PRODUCTSUNITED
        {
            public long PROUID { get; set; }

            public string PROUVAL { get; set; }

            public string PROUKIND { get; set; }

            public string PROUNAME { get; set; }

            public string PROUPROVALNICK { get; set; }

            public string PROULEVEL { get; set; }

            public bool PROUMARKA { get; set; }

        }
        public class PROLIST
        {
            public long PROID { get; set; }
            public long PROPROUID { get; set; }
            public string PROVAL { get; set; }
            public string PRONAME { get; set; }
            public bool PROSTS { get; set; }
            public string Marka { get; set; }
            public string ÜrünSınıfı { get; set; }
            public string ÜrünGrubu { get; set; }
            public string KotaGrubu { get; set; }
            public string UrunDurumu { get; set; }

        }
        public class WAVEPROTREE
        {
            public short WPTREUNIQ { get; set; }

            public string WPTREVAL { get; set; }

            public string WPTRENAME { get; set; }

        }
        public class PRICELIST
        {
            public long PRLDPRID { get; set; }

            public long PRLPROID { get; set; }

            public decimal PRLPRICE { get; set; }

            public DateTime PRLDATETIME { get; set; } = DateTime.Now;

            public string PRLSOCODEE { get; set; }

        }
        public class PRODUCTPICTURE
        {
            public long PRPID { get; set; }

            public long PRPPROID { get; set; }

            public bool? PRPISDEFAULT { get; set; }

            public string PRPIMAGE { get; set; }

        }
        public class DEFPRICE
        {
            public long DPRID { get; set; }

            public string DPRCOMPANY { get; set; }

            public string DPRVAL { get; set; }

            public string DPRNAME { get; set; }

            public string DPRKIND { get; set; }

            public string DPRVATKIND { get; set; }

            public long? DPRCURID { get; set; }

            public string DPRDIVISON { get; set; }

            public DateTime DPRSTARTDATE { get; set; }

            public DateTime DPRENDDATE { get; set; }

            public string DPREXCH { get; set; }

            public short DPRSORT { get; set; }

            public short? DPRPAYMENTVALUE { get; set; }

            public bool DPRSTS { get; set; }

            public bool DPRPRODUCTDISC { get; set; }

            public bool DPRSUMDISC { get; set; }

            public bool DPRCANUP { get; set; }

            public int? DPRCREDITCARDMAXLIMIT { get; set; }

            public bool DPRCANTAGPRINT { get; set; }

            public bool? DPROTOSELECT { get; set; }

            public decimal? DPRKRATE { get; set; }

            public bool? DPRMAIN { get; set; }

        }
        public class PROUNIT
        {
            public long PUNIPROID { get; set; }

            public string PUNIUNIT { get; set; } = "AD";

            public short PUNISORT { get; set; } = 0;

            public decimal PUNIMULTIPLIER { get; set; }

        }
        public class WAVEPRODUCTS
        {
            public long WPROID { get; set; }

            public short WPROUNIQ { get; set; }

            public string WPROVAL { get; set; }

        }
        public class PROMAIN
        {
            public long PROID { get; set; }
            public List<PRODUCTS> pRODUCTs { get; set; }
            public List<WAVEPRODUCTS> wAVEPRODUCTs { get; set; }
            public List<PRICELIST> pRICELISTs { get; set; }
            public List<PROUNIT> pROUNITs { get; set; }
        }
        public class PICTURE
        {
            public long PRPID {get;set;}
            public Image GetImage { get; set; }
        }
        public frmStok()
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Stok İşlemleri Açılıyor");
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
                navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
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
        bool NewUser = false;
        public List<PROLIST> PRODUCTSLIST = new List<PROLIST>();
        private void frmKullanici_Load(object sender, EventArgs e)
        {
            Stoklar();
            layoutControl1.Enabled = false;
            layoutControl5.Enabled = false;
            var dt = conn.GetData("select * from PRODUCTSUNITED", sql1).ToList<PRODUCTSUNITED>();
            srcPROUNITED.Properties.DataSource = dt;
            srcPROUNITED.Properties.ValueMember = "PROUID";
            srcPROUNITED.Properties.DisplayMember = "PROUNAME";

        }
        private void Stoklar()
        {
            var dt = conn.GetData(@"
select PROID,PROVAL,PROPROUID,PRONAME,PROSTS,Marka,ÜrünSınıfı,ÜrünGrubu,KotaGrubu,UrunDurumu from PRODUCTS
outer apply (select WPTRENAME as Marka from WAVEPRODUCTS 
			 left outer join WAVEPROTREE on WPROUNIQ = WPTREUNIQ and WPROVAL = WPTREVAL
			 where WPROUNIQ = 1 and WPROID = PROID) WAVE1
outer apply (select WPTRENAME as ÜrünSınıfı from WAVEPRODUCTS 
			 left outer join WAVEPROTREE on WPROUNIQ = WPTREUNIQ and WPROVAL = WPTREVAL
			 where WPROUNIQ = 2 and WPROID = PROID) WAVE2
outer apply (select WPTRENAME as ÜrünGrubu from WAVEPRODUCTS 
			 left outer join WAVEPROTREE on WPROUNIQ = WPTREUNIQ and WPROVAL = WPTREVAL
			 where WPROUNIQ = 3 and WPROID = PROID) WAVE3
outer apply (select WPTRENAME as KotaGrubu from WAVEPRODUCTS 
			 left outer join WAVEPROTREE on WPROUNIQ = WPTREUNIQ and WPROVAL = WPTREVAL
			 where WPROUNIQ = 4 and WPROID = PROID) WAVE4
outer apply (select WPTRENAME as UrunDurumu from WAVEPRODUCTS 
			 left outer join WAVEPROTREE on WPROUNIQ = WPTREUNIQ and WPROVAL = WPTREVAL
			 where WPROUNIQ = 5 and WPROID = PROID) WAVE5", sql1);
            if (dt != null)
            {
                PRODUCTSLIST = dt.ToList<PROLIST>();
                gridStoklar.DataSource = PRODUCTSLIST;
                ViewStoklar.OptionsView.BestFitMaxRowCount = -1;
                ViewStoklar.BestFitColumns(true);
            }
        }
        List<PICTURE> sss = new List<PICTURE>();
        int currentIndex = 0;

        private void musteriResmiPce_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Resim Dosyası (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image image = Image.FromFile(ofd.FileName);
                sss.Add(new PICTURE
                {
                    PRPID = 0,
                    GetImage = ResizeImage(image, 1024, 768)
                });
                currentIndex = sss.Count-1;
                ShowImage();
            }
        }
        private void ShowImage()
        {
            if (sss.Count > 0 && currentIndex >= 0 && currentIndex < sss.Count)
            {
                musteriResmiPce.Image = sss[currentIndex].GetImage;
                layoutControlItem3.Text = $"{currentIndex + 1}/{sss.Count}";
            }
            else
            {
                layoutControlItem3.Text = "0/0";
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentIndex < sss.Count - 1)
            {
                currentIndex++;
                ShowImage();
                simpleButton1.Enabled = true;
                simpleButton2.Enabled = true;
            }
            if (currentIndex == sss.Count - 1)
            {
                simpleButton2.Enabled = false;
                simpleButton1.Enabled = true;
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                ShowImage();
                simpleButton1.Enabled = true;
                simpleButton2.Enabled = true;
            }
            if (currentIndex == 0)
            {
                simpleButton2.Enabled = true;
                simpleButton1.Enabled = false;
            }
        }
        public async static Task<long> REGISTER(string _RGKIND, string _ConnectionString)
        {
            long RGID = 0;
            using (var sqlConnection = new SqlConnection(_ConnectionString))
            {
                await sqlConnection.OpenAsync();

                using (var tran = sqlConnection.BeginTransaction())
                {
                    var db = new DbTrans(sqlConnection, tran);
                    try
                    {
                        var ID = db.GetValue($@"
                                UPDATE REGISTER
                                SET RGID = RGID + 1
                                OUTPUT INSERTED.RGID
                                WHERE RGCOMPANY = ''
                                  AND RGKIND = {_RGKIND}
                                  AND RGVAL1 = ''
                                  AND RGVAL2 = ''
                                  AND RGDATE = 0");
                        RGID = long.Parse(ID);
                        tran.Commit(); // Başarılıysa commit
                    }
                    catch (Exception)
                    {
                        tran.Rollback(); // Hata olursa rollback
                    }
                }
            }
            return RGID;
        }

        private void ViewKullanicilar_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.RowHandle > -1 && e.Clicks >= 2 && e.Button == MouseButtons.Left )
            {
                NewUser = false;
                var PROID = long.Parse(ViewStoklar.GetRowCellValue(e.RowHandle, "PROID").ToString());
                var PROUID = long.Parse(ViewStoklar.GetRowCellValue(e.RowHandle, "PROPROUID").ToString());
                var PROVAL = ViewStoklar.GetRowCellValue(e.RowHandle, "PROVAL").ToString();
                var PRONAME = ViewStoklar.GetRowCellValue(e.RowHandle, "PRONAME").ToString();
                var PROSTS = bool.Parse(ViewStoklar.GetRowCellValue(e.RowHandle, "PROSTS").ToString());
                if (PROSTS)
                {
                    cmbPROSTS.SelectedIndex = 1;
                }
                else
                {
                    cmbPROSTS.SelectedIndex = 0;
                }
                txtPROVAL.Text = PROVAL;
                txtPROVAL.Tag = PROID;
                txtPRONAME.Text = PRONAME;
                srcPROUNITED.EditValue = PROUID;
                var kirilim = conn.GetData($@"
                                select DWPROUNIQ,DWPROTITLE,WPROVAL as WPTREVAL,WPTRENAME,WPROVAL as LASTPROVAL from DEFWAVEPRO
                outer apply(select WPROVAL,WPTRENAME from WAVEPRODUCTS 
                            left outer join WAVEPROTREE on WPTREUNIQ = WPROUNIQ and WPTREVAL = WPROVAL
                            where WPROUNIQ = DWPROUNIQ and WPROID = {PROID}) PRODUC
                where DWPROSTS = 1", sql1);
                var fiyat = conn.GetData($@"
                select DPRID,DPRVAL,DPRNAME,PRLPRICE from DEFPRICE
                outer apply(select * from PRICELIST where PRLDPRID = DPRID
                            and PRLPROID = {PROID}) PRICE
                where DPRSTS = 1", sql1);
                var tarihce = conn.GetData($@"
                select DPRVAL, DPRNAME ,HPRLDATETIME,SOFULLNAME,OLDPRICE,HPRLPRICE from (
                SELECT H2.*, DPRVAL, DPRNAME , PROVAL , PRONAME,SONAME + ' ' + SOSURNAME as SOFULLNAME,
                (SELECT TOP 1 H1.HPRLPRICE FROM HISTORYPRICELIST H1 WHERE H1.HPRLPROID = H2.HPRLPROID AND H1.HPRLDPRID = H2.HPRLDPRID AND H1.HPRLDATETIME< H2.HPRLDATETIME ORDER BY H1.HPRLDATETIME DESC) OLDPRICE 
                FROM  HISTORYPRICELIST H2 
                LEFT OUTER JOIN DEFPRICE ON DPRID = H2.HPRLDPRID   
                LEFT OUTER JOIN PRODUCTS ON PRODUCTS.PROID = H2.HPRLPROID  
                LEFT OUTER JOIN PRODUCTSUNITED ON PRODUCTS.PROPROUID = PROUID 
                LEFT OUTER JOIN SOCIAL on SOCODE = HPRLSOCODEE
                WHERE	H2.HPRLPROID = {PROID} AND 
                H2.HPRLDATETIME BETWEEN '{new DateTime(DateTime.Today.Year, 1,1).ToString("yyyy-MM-dd")}' and '{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}'
                )son
                where OLDPRICE != HPRLPRICE", sql1);
                currentIndex = 0;
                var pic = conn.GetData($@"select * from PRODUCTPICTURE where PRPPROID = {PROID}", sql1);
                if (pic != null)
                {
                    var resim = pic.ToList<PRODUCTPICTURE>();
                    foreach (PRODUCTPICTURE item in resim)
                    {
                        sss.Add(new PICTURE
                        {
                            PRPID = item.PRPID,
                            GetImage = EntegreFDLL.Class.ConvertVolant.FromBase64String(item.PRPIMAGE).byteArrayToImage()
                        });
                    }
                    ShowImage();
                }
                else
                {
                    sss.Clear();
                    musteriResmiPce.Image = null;
                    ShowImage();
                }
                gridKirilim.DataSource = kirilim;
                ViewKirilim.OptionsView.BestFitMaxRowCount = -1;
                ViewKirilim.BestFitColumns(true);
                gridFiyat.DataSource = fiyat;
                ViewFiyat.OptionsView.BestFitMaxRowCount = -1;
                ViewFiyat.BestFitColumns(true);
                navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                layoutControl1.Enabled = false;
                layoutControl5.Enabled = false;
            }
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Kullanıcı Hesapları Listeleniyor");
                Stoklar();
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
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtPROVAL.Text = null;
            txtPRONAME.Text = null;
            txtPROVAL.Tag = null;
            gridKirilim.DataSource = null;
            gridFiyat.DataSource = null;
            gridTarihce.DataSource = null;
            navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
            layoutControl1.Enabled = false;
            layoutControl5.Enabled = false;
            musteriResmiPce.Enabled = false;
            NewUser = false;
            sss.Clear();
        }
        private async void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {            
            bool tamam = false;
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Lütfen Bekleyin");

                if (NewUser)
                {
                    if (string.IsNullOrWhiteSpace(txtPROVAL.Text))
                    {
                        throw new Exception("Stok Kodu Boş Olamaz");
                    }
                    else if (string.IsNullOrWhiteSpace(txtPRONAME.Text))
                    {
                        throw new Exception("Stok Adı Boş Olamaz");
                    }
                    else
                    {
                        string BulkInsertReturn = "";
                        List<PRODUCTS> pRODUCTs = new List<PRODUCTS>();
                        using (var sqlConnection = new SqlConnection(sql1))
                        {
                            await sqlConnection.OpenAsync();
                            using (var tran = sqlConnection.BeginTransaction())
                            {
                                var db = new DbTrans(sqlConnection, tran);
                                try
                                {
                                    var PRODUCTS = convertler.ToDataTable(pRODUCTs);
                                    BulkInsertReturn = db.BulkInsertRetorn(PRODUCTS, "PRODUCTS");
                                    if (BulkInsertReturn == "Aktarım Tamamlandı")
                                    {
                                        List<WAVEPRODUCTS> wAVEPRODUCTs = new List<WAVEPRODUCTS>();
                                        for (int i = 0; i < ViewKirilim.RowCount; i++)
                                        {
                                            var DWPROUNIQ = ViewKirilim.GetRowCellValue(i, "DWPROUNIQ").ToString();
                                            var WPTREVAL = ViewKirilim.GetRowCellValue(i, "WPTREVAL").ToString();
                                            wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                            {
                                                WPROID= pRODUCTs[0].PROID,
                                                WPROUNIQ = short.Parse(DWPROUNIQ),
                                                WPROVAL = WPTREVAL
                                            });
                                        }

                                        var WAVEPRODUCTS = convertler.ToDataTable(wAVEPRODUCTs);
                                        BulkInsertReturn = db.BulkInsertRetorn(WAVEPRODUCTS, "WAVEPRODUCTS");
                                        if (BulkInsertReturn == "Aktarım Tamamlandı")
                                        {
                                            List<PRICELIST> pRICELISTs = new List<PRICELIST>();
                                            for (int i = 0; i < ViewFiyat.RowCount; i++)
                                            {
                                                var DPRID = ViewFiyat.GetRowCellValue(i, "DPRID").ToString();
                                                var fiyat = ViewFiyat.GetRowCellValue(i, "PRLPRICE").ToString();
                                                pRICELISTs.Add(new PRICELIST
                                                {
                                                    PRLPROID = pRODUCTs[0].PROID,
                                                    PRLDPRID = long.Parse(DPRID),
                                                    PRLPRICE = decimal.Parse(fiyat),
                                                    PRLSOCODEE = EntegreFDLL.Class.Entegref.GetLogins.userID
                                                });
                                            }
                                            var PRICELIST = convertler.ToDataTable(pRICELISTs);
                                            BulkInsertReturn = db.BulkInsertRetorn(PRICELIST, "PRICELIST");
                                            if (BulkInsertReturn == "Aktarım Tamamlandı")
                                            {
                                                if (musteriResmiPce.Image != null)
                                                {
                                                    List<PRODUCTPICTURE> pRODUCTPICTUREs = new List<PRODUCTPICTURE>();
                                                    foreach (PICTURE item in sss)
                                                    {
                                                        pRODUCTPICTUREs.Add(new PRODUCTPICTURE
                                                        {
                                                            PRPID = await REGISTER("260", sqlConnection.ConnectionString),
                                                            PRPPROID = pRODUCTs[0].PROID,
                                                            PRPIMAGE = EntegreFDLL.Class.VolantConvert.ToBase64String(item.GetImage.imageToByteArray()),
                                                            PRPISDEFAULT = true
                                                        });
                                                    }
                                                    var PRODUCTPICTURE = convertler.ToDataTable(pRODUCTPICTUREs);
                                                    BulkInsertReturn = db.BulkInsertRetorn(PRODUCTPICTURE, "PRODUCTPICTURE");

                                                }
                                                if (BulkInsertReturn == "Aktarım Tamamlandı")
                                                {

                                                    tran.Commit(); // Başarılıysa commit
                                                }
                                                else
                                                {
                                                    tran.Rollback(); // Hata olursa rollback
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        tran.Rollback(); // Hata olursa rollback
                                    }
                                }
                                catch (Exception ex)
                                {
                                    tran.Rollback(); // Hata olursa rollback
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtPROVAL.Text))
                    {
                        throw new Exception("Stok Kodu Boş Olamaz");
                    }
                    else if (string.IsNullOrWhiteSpace(txtPRONAME.Text))
                    {
                        throw new Exception("Stok Adı Boş Olamaz");
                    }
                    else
                    {
                        using (var sqlConnection = new SqlConnection(sql1))
                        {
                            await sqlConnection.OpenAsync();
                            using (var tran = sqlConnection.BeginTransaction())
                            {
                                var db = new DbTrans(sqlConnection, tran);
                                using (SqlCommand cmd = new SqlCommand())
                                {
                                    try
                                    {
                                        cmd.Transaction = tran;
                                        cmd.Connection = sqlConnection;
                                        cmd.CommandType = CommandType.Text;
                                        cmd.CommandText = $@"update PRODUCTS set PRONAME = @PRONAME, PROSTS = @PROSTS where PROID = @ID";
                                        cmd.Parameters.AddWithValue("@PRONAME", txtPRONAME.Text);
                                        cmd.Parameters.AddWithValue("@PROSTS", cmbPROSTS.SelectedIndex);
                                        cmd.Parameters.AddWithValue("@ID", txtPROVAL.Tag);
                                        var sonuc = cmd.ExecuteNonQuery();
                                        if (sonuc != 0)
                                        {
                                            tran.Commit();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        tran.Rollback(); // Hata olursa rollback
                                        throw new Exception(ex.Message);
                                    }

                                }
                            }

                            if (sqlConnection.State == ConnectionState.Closed)
                            {
                                await sqlConnection.OpenAsync();
                            }
                            for (int i = 0; i < ViewKirilim.RowCount; i++)
                            {
                                var DWPROUNIQ = ViewKirilim.GetRowCellValue(i, "DWPROUNIQ").ToString();
                                var NEWVAL = ViewKirilim.GetRowCellValue(i, "WPTREVAL").ToString();
                                var LASTVAL = ViewKirilim.GetRowCellValue(i, "LASTPROVAL").ToString();
                                if (LASTVAL == "")
                                {
                                    List<WAVEPRODUCTS> wAVEPRODUCTs = new List<WAVEPRODUCTS>
                                    {
                                        new WAVEPRODUCTS
                                        {
                                            WPROID = long.Parse(txtPROVAL.Tag.ToString()),
                                            WPROUNIQ = short.Parse(DWPROUNIQ),
                                            WPROVAL = NEWVAL,
                                        }
                                    };
                                    using (var tran = sqlConnection.BeginTransaction())
                                    {
                                        var db = new DbTrans(sqlConnection, tran);
                                        try
                                        {
                                            var WAVEPRODUCTS = convertler.ToDataTable(wAVEPRODUCTs);
                                            var BulkInsertReturn = db.BulkInsertRetorn(WAVEPRODUCTS, "WAVEPRODUCTS");
                                            if (BulkInsertReturn == "Aktarım Tamamlandı")
                                            {

                                                tran.Commit(); // Başarılıysa commit
                                            }
                                            else
                                            {
                                                tran.Rollback(); // Hata olursa rollback
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            tran.Rollback(); // Hata olursa rollback
                                            throw new Exception(ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    if (NEWVAL != LASTVAL)
                                    {
                                        using (var tran = sqlConnection.BeginTransaction())
                                        {
                                            using (SqlCommand cmd = new SqlCommand())
                                            {
                                                try
                                                {
                                                    cmd.Transaction = tran;
                                                    cmd.Connection = sqlConnection;
                                                    cmd.CommandType = CommandType.Text;
                                                    cmd.CommandText = $@"update WAVEPRODUCTS set WPROVAL = @WPROVAL where WPROUNIQ = @WPROUNIQ and WPROID = {txtPROVAL.Tag}";
                                                    cmd.Parameters.AddWithValue("@WPROUNIQ", DWPROUNIQ);
                                                    cmd.Parameters.AddWithValue("@WPROVAL", NEWVAL);
                                                    var sonuc = cmd.ExecuteNonQuery();
                                                    if (sonuc != 0)
                                                    {
                                                        tran.Commit();
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    tran.Rollback(); // Hata olursa rollback
                                                    throw new Exception(ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < ViewFiyat.RowCount; i++)
                            {
                                var DPRID = ViewFiyat.GetRowCellValue(i, "DPRID").ToString();
                                var Tutar = ViewFiyat.GetRowCellValue(i, "PRLPRICE").ToString();
                                
                                using (var tran = sqlConnection.BeginTransaction())
                                {
                                    var db = new DbTrans(sqlConnection, tran);
                                    var LIST = db.GetData($@"select * from PRICELIST where PRLPROID = {txtPROVAL.Tag} and PRLDPRID = {DPRID}");
                                    if (LIST != null)
                                    {
                                        List<PRICELIST> pRICELISTs = new List<PRICELIST>
                                        {
                                            new PRICELIST
                                            {
                                                PRLDPRID = long.Parse(DPRID),
                                                PRLPROID = long.Parse(txtPROVAL.Tag.ToString()),
                                                PRLSOCODEE = EntegreFDLL.Class.Entegref.GetLogins.userID,
                                                PRLPRICE = decimal.Parse(Tutar)
                                            }
                                        };
                                    }
                                    else
                                    {
                                        db.InsertValue($@"update PRICELIST set PRLPRICE = {Tutar} where PRLDPRID ={DPRID} and PRLPROID = {txtPROVAL.Tag.ToString()}");
                                        db.InsertValue($@"insert into HISTORYPRICELIST values({DPRID},{long.Parse(txtPROVAL.Tag.ToString())},{decimal.Parse(LIST.Rows[0]["PRLPRICE"].ToString())}, GETDATE(), {EntegreFDLL.Class.Entegref.GetLogins.userID},'U')");
                                    }
                                }                                    
                            }
                            if (musteriResmiPce.Image != null)
                            {
                                string BulkInsertReturn = "";
                                using (var tran = sqlConnection.BeginTransaction())
                                {
                                    var db = new DbTrans(sqlConnection, tran);
                                    List<PRODUCTPICTURE> pRODUCTPICTUREs = new List<PRODUCTPICTURE>();
                                    if (sss.Count > 0)
                                    {
                                        foreach (PICTURE item in sss)
                                        {
                                            if (item.PRPID == 0)
                                            {
                                                pRODUCTPICTUREs.Add(new PRODUCTPICTURE
                                                {
                                                    PRPID = await REGISTER("260", sql1),
                                                    PRPPROID = long.Parse(txtPROVAL.Tag.ToString()),
                                                    PRPIMAGE = EntegreFDLL.Class.VolantConvert.ToBase64String(item.GetImage.imageToByteArray()),
                                                    PRPISDEFAULT = true
                                                });
                                            }
                                        }
                                        var PRODUCTPICTURE = convertler.ToDataTable(pRODUCTPICTUREs);
                                        BulkInsertReturn = db.BulkInsertRetorn(PRODUCTPICTURE, "PRODUCTPICTURE");
                                        if (BulkInsertReturn == "Aktarım Tamamlandı")
                                        {

                                            tran.Commit(); // Başarılıysa commit
                                        }
                                        else
                                        {
                                            tran.Rollback(); // Hata olursa rollback
                                        }
                                    }
                                    else
                                    {
                                        BulkInsertReturn = db.InsertValue($"delete PRODUCTPICTURE where PRPPROID = {txtPROVAL.Tag}").ToString();
                                        if (BulkInsertReturn != "0")
                                        {

                                            tran.Commit(); // Başarılıysa commit
                                        }
                                        else
                                        {
                                            tran.Rollback(); // Hata olursa rollback
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                barButtonItem2_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowMessage(ex.Message, "", this, "Uyarı", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
            if (tamam)
            {
                txtPROVAL.Text = null;
                txtPRONAME.Text = null;
                txtPROVAL.Tag = null;
                gridKirilim.DataSource = null;
                gridFiyat.DataSource = null;
                gridTarihce.DataSource = null;
                navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
                layoutControl1.Enabled = false;
                layoutControl5.Enabled = false;
                NewUser = false;
                Stoklar();
                CustomMessageBox.ShowMessage("İşlem Tamam", "", this, "Bilgilendirme", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtPROVAL.Text = null;
            txtPRONAME.Text = null;
            txtPROVAL.Tag = null;
            gridKirilim.DataSource = null;
            gridFiyat.DataSource = null;
            gridTarihce.DataSource = null;
            navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            layoutControl1.Enabled = true;
            layoutControl5.Enabled = true;
            musteriResmiPce.Enabled = true;
            NewUser = true;
        }
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navBarKullanici.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            layoutControl1.Enabled = true;
            layoutControl5.Enabled = true;
            txtPROVAL.Enabled = false;
            musteriResmiPce.Enabled = true;
            NewUser = false;
        }
        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools", Properties.Settings.Default.Company, "Lütfen Bekleyin");
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
        private void barButtonItemStok_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage1;
        }
        private void barButtonItemExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage2;
            navigationFrame2.SelectedPage = navigationPage3;     
            gridVeriler.DataSource = null;
            navBarStok.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            tileBarItemKaydet.Enabled = false;
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            // Çalışma sayfasının içeriğini temizle
            worksheet.Clear(worksheet.GetDataRange());
            pRODUCTSUNITEDs = conn.GetData(@"
select PROUID,WPTREVAL as PROUVAL,PROUKIND,PROUNAME,PROUPROVALNICK,PROULEVEL,PROUMARKA from PRODUCTSUNITED
join WAVEPROTREE on WPTREUNIQ = 2 and WPTREVAL = '00' + PROUVAL", sql1).ToList<PRODUCTSUNITED>();
        }
        List<PRODUCTSUNITED> pRODUCTSUNITEDs = new List<PRODUCTSUNITED>();
        public static string WPTRENAME;
        public static string WPROVAL;
        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var DWPROUNIQ = ViewKirilim.GetRowCellValue(ViewKirilim.FocusedRowHandle, "DWPROUNIQ").ToString();
            frmKrilim krilim = new frmKrilim(DWPROUNIQ);
            krilim.ShowDialog();
            ViewKirilim.SetRowCellValue(ViewKirilim.FocusedRowHandle, "WPTREVAL", WPROVAL);
            ViewKirilim.SetRowCellValue(ViewKirilim.FocusedRowHandle, "WPTRENAME", WPTRENAME);
            ViewKirilim.RefreshRow(ViewKirilim.FocusedRowHandle);
        }
        // Excell İşlemleri
        private void btnDosyaSec_ItemClick(object sender, TileItemEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Dosyaları (*.xlsx;*.xls)|*.xlsx;*.xls|Tüm Dosyalar (*.*)|*.*";
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            // Çalışma sayfasının içeriğini temizle
            worksheet.Clear(worksheet.GetDataRange());
            spreadsheetControl1.Document.BeginUpdate();
            // Kullanıcıdan dosyayı seçmesini iste
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                spreadsheetControl1.Document.LoadDocument(openFileDialog.FileName, DocumentFormat.Xlsx);
            }
            spreadsheetControl1.Document.EndUpdate();

            Worksheet worksheet2 = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            CellRange usedRange = worksheet2.GetUsedRange();

            // Dolu sütunları dolaşarak sıralı harf isimlerini elde edin
            for (int columnIndex = usedRange.LeftColumnIndex; columnIndex <= usedRange.RightColumnIndex + 1; columnIndex++)
            {
                // Sütunun harf karşılığını hesaplayın
                string columnName = GetColumnName(columnIndex);
                // Sütun ismini listeye ekleyin
                cmbPROVAL.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbPRONAME.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbWPTREUNIQ1.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbWPTREUNIQ2.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbWPTREUNIQ3.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbWPTREUNIQ4.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbWPTREUNIQ5.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbWPTREUNIQ6.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbP.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbKK.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbTK5.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbTK9.Items.Add(new ComboBoxItem(columnIndex, columnName));
                cmbTK12.Items.Add(new ComboBoxItem(columnIndex, columnName));
            }
            var comboList = new List<System.Windows.Forms.ComboBox>
            {
                cmbPROVAL, cmbPRONAME,cmbWPTREUNIQ1,cmbWPTREUNIQ2, cmbWPTREUNIQ3, cmbWPTREUNIQ4, cmbWPTREUNIQ5, cmbWPTREUNIQ6,
                cmbP, cmbKK, cmbTK5, cmbTK9, cmbTK12
            };

            for (int col = usedRange.LeftColumnIndex; col <= usedRange.RightColumnIndex; col++)
            {
                string headerText = worksheet2.Cells[usedRange.TopRowIndex, col].DisplayText?.Trim();

                if (string.IsNullOrEmpty(headerText)) continue;

                foreach (var combo in comboList)
                {
                    if (combo.Tag != null &&
                        headerText.Equals(combo.Tag.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        // eşleşen combobox için doğru item’i seç
                        foreach (ComboBoxItem item in combo.Items)
                        {
                            if (item.Value == col + 1)
                            {
                                combo.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
            spreadsheetControl1.Document.Worksheets.ActiveWorksheet.Cells.AutoFitColumns();
            btnKontrol.Enabled = true;
        }
        private string GetColumnName(int index)
        {
            string columnName = "";

            while (index > 0)
            {
                int remainder = (index - 1) % 26;
                columnName = (char)('A' + remainder) + columnName;
                index = (index - 1) / 26;
            }
            return columnName;
        }
        public static int HarfinSirasi(string harf)
        {
            if (harf == "i")
            {
                harf = "I";
            }
            // Küçük harfleri kontrol etmek için harfin ASCII değerini 97'ye çıkarırız.
            // Büyük harfler için bu gerekli değildir.
            harf = harf.ToUpper();

            // Harfin alfabedeki sırasını bulmak için ASCII tablosundaki indeksi kullanırız.
            // Küçük harfler için indeksler 0'dan 25'e kadar, büyük harfler için 0'dan 25'e kadar.
            int sira = 0;
            if (harf == "")
            {
                sira = -1;
            }
            else
            {
                foreach (char c in harf)
                {
                    // 'A' = 1, 'B' = 2 ... 'Z' = 26
                    sira = sira * 26 + (c - 'A' + 1);
                }
            }

            return sira;
        }
        private void btnKontrol_ItemClick(object sender, TileItemEventArgs e)
        {
            var item = sender as TileItem; // veya TileBarItem

            List<WAVEPROTREE> wAVEPROTREEs = new List<WAVEPROTREE>();
            List<PROMAIN> pROMAINs = new List<PROMAIN>();
            int index = 1;
            long FIRSTID = 0;
            if (PRODUCTSLIST.Count != 0)
            {
                FIRSTID = PRODUCTSLIST.Max().PROID;
            }

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;

            HashSet<string> uniquePROVAL = new HashSet<string>();
            CellRange usedRange = worksheet.GetUsedRange();
            ProgressBarFrm progressForm = new ProgressBarFrm()
            {
                Start = 0,
                Finish = usedRange.RowCount,
                Position = 0,
                ToplamAdet = usedRange.RowCount.ToString(),
            };
            int SiraPROVAL = HarfinSirasi(cmbPROVAL.Text);
            int SiraPRONAME = HarfinSirasi(cmbPRONAME.Text);
            int SiraWPTREUNIQ1 = HarfinSirasi(cmbWPTREUNIQ1.Text);
            int SiraWPTREUNIQ2 = HarfinSirasi(cmbWPTREUNIQ2.Text);
            int SiraWPTREUNIQ3 = HarfinSirasi(cmbWPTREUNIQ3.Text);
            int SiraWPTREUNIQ4 = HarfinSirasi(cmbWPTREUNIQ4.Text);
            int SiraWPTREUNIQ5 = HarfinSirasi(cmbWPTREUNIQ5.Text);
            int SiraWPTREUNIQ6 = HarfinSirasi(cmbWPTREUNIQ6.Text);
            int SiraPESIN = HarfinSirasi(cmbP.Text);
            int SiraKK = HarfinSirasi(cmbKK.Text);
            int SiraTK5 = HarfinSirasi(cmbTK5.Text);
            int SiraTK9 = HarfinSirasi(cmbTK9.Text);
            int SiraTK12 = HarfinSirasi(cmbTK12.Text);
            executeBackground(
            () =>
            {
                progressForm.Show(this);
                //for (int rowIndex = index; rowIndex < usedRange.RowCount; rowIndex++)
                for (int rowIndex = index; rowIndex < 500; rowIndex++)
                {
                    try
                    {
                        var matched = PRODUCTSLIST.FirstOrDefault(z => z.PROVAL == usedRange[rowIndex, SiraPROVAL - 1].Value.ToString());
                        if (matched == null)
                        {
                            progressForm.UpdateDetails($"{usedRange[rowIndex, SiraPRONAME - 1].Value.ToString()} için işlem yapılıyor");
                            List<PRODUCTS> pRODUCTs = new List<PRODUCTS>();
                            List<PROUNIT> pROUNITs = new List<PROUNIT>();
                            List<WAVEPRODUCTS> wAVEPRODUCTs = new List<WAVEPRODUCTS>();
                            List<PRICELIST> pRICELISTs = new List<PRICELIST>();
                            var WAVEPROTREE = conn.GetData("select * from WAVEPROTREE", sql1).ToList<WAVEPROTREE>();
                            var DEFPRICE = conn.GetData("select * from DEFPRICE", sql1).ToList<DEFPRICE>();
                            PROMAIN pROMAIN = new PROMAIN();
                            if (uniquePROVAL.Add(usedRange[rowIndex, SiraPROVAL - 1].Value.ToString()))
                            {
                                var NEWPROID = FIRSTID + rowIndex;//await REGISTER("137", sql1);
                                    pRODUCTs.Add(new PRODUCTS
                                {
                                    PROID = NEWPROID,
                                    PROVAL = usedRange[rowIndex, SiraPROVAL - 1].Value.ToString(),
                                    PRONAME = usedRange[rowIndex, SiraPRONAME - 1].Value.ToString(),
                                    PROSOCODE = EntegreFDLL.Class.Entegref.GetLogins.userID
                                });
                                pROUNITs.Add(new PROUNIT
                                {
                                    PUNIPROID = NEWPROID,
                                    PUNISORT = 0,
                                    PUNIMULTIPLIER = 1
                                });
                                pROMAIN.PROID = NEWPROID;
                                var WPTREUNIQ1 = usedRange[rowIndex, SiraWPTREUNIQ1 - 1].Value.ToString();
                                var WPTREUNIQ2 = usedRange[rowIndex, SiraWPTREUNIQ2 - 1].Value.ToString();
                                var WPTREUNIQ3 = usedRange[rowIndex, SiraWPTREUNIQ3 - 1].Value.ToString();
                                var WPTREUNIQ4 = usedRange[rowIndex, SiraWPTREUNIQ4 - 1].Value.ToString();
                                var WPTREUNIQ5 = usedRange[rowIndex, SiraWPTREUNIQ5 - 1].Value.ToString();
                                var WPTREUNIQ6 = usedRange[rowIndex, SiraWPTREUNIQ6 - 1].Value.ToString();
                                if (WPTREUNIQ1 != "")
                                {
                                    var matched1 = WAVEPROTREE.FirstOrDefault(x => x.WPTREUNIQ == 1 && x.WPTRENAME == WPTREUNIQ1).WPTREVAL;
                                    var MaxWPTREVAL = WAVEPROTREE.Where(q => q.WPTREUNIQ == 1).Max(q => int.Parse(q.WPTREVAL));
                                    if (matched1 == null)
                                    {
                                        wAVEPROTREEs.Add(new WAVEPROTREE
                                        {
                                            WPTREUNIQ = 1,
                                            WPTRENAME = WPTREUNIQ1,
                                            WPTREVAL = (MaxWPTREVAL + 1).ToString("D5")
                                        });
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 1,
                                            WPROVAL = (MaxWPTREVAL + 1).ToString("D5")
                                        });
                                    }
                                    else
                                    {
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 1,
                                            WPROVAL = matched1
                                        });
                                    }
                                }
                                if (WPTREUNIQ2 != "")
                                {
                                    var matched2 = WAVEPROTREE.FirstOrDefault(x => x.WPTREUNIQ == 2 && x.WPTRENAME == WPTREUNIQ2).WPTREVAL;
                                    var MaxWPTREVAL2 = WAVEPROTREE.Where(q => q.WPTREUNIQ == 2).Max(q => int.Parse(q.WPTREVAL));
                                    if (matched2 == null)
                                    {
                                        wAVEPROTREEs.Add(new WAVEPROTREE
                                        {
                                            WPTREUNIQ = 2,
                                            WPTRENAME = WPTREUNIQ1,
                                            WPTREVAL = (MaxWPTREVAL2 + 1).ToString("D5")
                                        });
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 2,
                                            WPROVAL = (MaxWPTREVAL2 + 1).ToString("D5")
                                        });
                                    }
                                    else
                                    {
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 2,
                                            WPROVAL = matched2
                                        });
                                        pRODUCTs[0].PROPROUID = pRODUCTSUNITEDs.FirstOrDefault(z => z.PROUVAL == matched2).PROUID;
                                    }
                                }
                                if (WPTREUNIQ3 != "")
                                {
                                    var matched3 = WAVEPROTREE.FirstOrDefault(x => x.WPTREUNIQ == 3 && x.WPTRENAME == WPTREUNIQ3).WPTREVAL;
                                    var MaxWPTREVAL3 = WAVEPROTREE.Where(q => q.WPTREUNIQ == 3).Max(q => int.Parse(q.WPTREVAL));
                                    if (matched3 == null)
                                    {
                                        wAVEPROTREEs.Add(new WAVEPROTREE
                                        {
                                            WPTREUNIQ = 3,
                                            WPTRENAME = WPTREUNIQ1,
                                            WPTREVAL = (MaxWPTREVAL3 + 1).ToString("D5")
                                        });
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 3,
                                            WPROVAL = (MaxWPTREVAL3 + 1).ToString("D5")
                                        });
                                    }
                                    else
                                    {
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 3,
                                            WPROVAL = matched3
                                        });
                                    }
                                }
                                if (WPTREUNIQ4 != "")
                                {
                                    var matched4 = WAVEPROTREE.FirstOrDefault(x => x.WPTREUNIQ == 4 && x.WPTRENAME == WPTREUNIQ4).WPTREVAL;
                                    var MaxWPTREVAL4 = WAVEPROTREE.Where(q => q.WPTREUNIQ == 4).Max(q => int.Parse(q.WPTREVAL));
                                    if (matched4 == null)
                                    {
                                        wAVEPROTREEs.Add(new WAVEPROTREE
                                        {
                                            WPTREUNIQ = 4,
                                            WPTRENAME = WPTREUNIQ1,
                                            WPTREVAL = (MaxWPTREVAL4 + 1).ToString("D5")
                                        });
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 4,
                                            WPROVAL = (MaxWPTREVAL4 + 1).ToString("D5")
                                        });
                                    }
                                    else
                                    {
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 4,
                                            WPROVAL = matched4
                                        });
                                    }
                                }
                                if (WPTREUNIQ5 != "")
                                {
                                    var matched5 = WAVEPROTREE.FirstOrDefault(x => x.WPTREUNIQ == 5 && x.WPTRENAME == WPTREUNIQ5).WPTREVAL;
                                    var MaxWPTREVAL5 = WAVEPROTREE.Where(q => q.WPTREUNIQ == 5).Max(q => int.Parse(q.WPTREVAL));
                                    if (matched5 == null)
                                    {
                                        wAVEPROTREEs.Add(new WAVEPROTREE
                                        {
                                            WPTREUNIQ = 5,
                                            WPTRENAME = WPTREUNIQ1,
                                            WPTREVAL = (MaxWPTREVAL5 + 1).ToString("D5")
                                        });
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 5,
                                            WPROVAL = (MaxWPTREVAL5 + 1).ToString("D5")
                                        });
                                    }
                                    else
                                    {
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 5,
                                            WPROVAL = matched5
                                        });
                                    }
                                }
                                if (WPTREUNIQ6 != "")
                                {
                                    var matched6 = WAVEPROTREE.FirstOrDefault(x => x.WPTREUNIQ == 6 && x.WPTRENAME == WPTREUNIQ6).WPTREVAL;
                                    var MaxWPTREVAL6 = WAVEPROTREE.Where(q => q.WPTREUNIQ == 6).Max(q => int.Parse(q.WPTREVAL));
                                    if (matched6 == null)
                                    {
                                        wAVEPROTREEs.Add(new WAVEPROTREE
                                        {
                                            WPTREUNIQ = 6,
                                            WPTRENAME = WPTREUNIQ1,
                                            WPTREVAL = (MaxWPTREVAL6 + 1).ToString("D5")
                                        });
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 6,
                                            WPROVAL = (MaxWPTREVAL6 + 1).ToString("D5")
                                        });
                                    }
                                    else
                                    {
                                        wAVEPRODUCTs.Add(new WAVEPRODUCTS
                                        {
                                            WPROID = NEWPROID,
                                            WPROUNIQ = 6,
                                            WPROVAL = matched6
                                        });
                                    }
                                }
                                var PR1 = usedRange[rowIndex, SiraPESIN - 1].Value.ToString();
                                var PR2 = usedRange[rowIndex, SiraKK - 1].Value.ToString();
                                var PR3 = usedRange[rowIndex, SiraTK5 - 1].Value.ToString();
                                var PR4 = usedRange[rowIndex, SiraTK9 - 1].Value.ToString();
                                var PR5 = usedRange[rowIndex, SiraTK12 - 1].Value.ToString();
                                if (decimal.TryParse(PR1, out decimal number))
                                {
                                    var NEWDPRID = DEFPRICE.FirstOrDefault(x => x.DPRVAL == "P").DPRID;
                                    pRICELISTs.Add(new PRICELIST
                                    {
                                        PRLDPRID = NEWDPRID,
                                        PRLPROID = NEWPROID,
                                        PRLPRICE = number,
                                        PRLSOCODEE = EntegreFDLL.Class.Entegref.GetLogins.userID
                                    });
                                }
                                if (decimal.TryParse(PR2, out decimal number2))
                                {
                                    var NEWDPRID2 = DEFPRICE.FirstOrDefault(x => x.DPRVAL == "KK").DPRID;
                                    pRICELISTs.Add(new PRICELIST
                                    {
                                        PRLDPRID = NEWDPRID2,
                                        PRLPROID = NEWPROID,
                                        PRLPRICE = number2,
                                        PRLSOCODEE = EntegreFDLL.Class.Entegref.GetLogins.userID
                                    });
                                }
                                if (decimal.TryParse(PR3, out decimal number3))
                                {
                                    var NEWDPRID3 = DEFPRICE.FirstOrDefault(x => x.DPRVAL == "TK5").DPRID;
                                    pRICELISTs.Add(new PRICELIST
                                    {
                                        PRLDPRID = NEWDPRID3,
                                        PRLPROID = NEWPROID,
                                        PRLPRICE = number3,
                                        PRLSOCODEE = EntegreFDLL.Class.Entegref.GetLogins.userID
                                    });
                                }
                                if (decimal.TryParse(PR4, out decimal number4))
                                {
                                    var NEWDPRID4 = DEFPRICE.FirstOrDefault(x => x.DPRVAL == "TK9").DPRID;
                                    pRICELISTs.Add(new PRICELIST
                                    {
                                        PRLDPRID = NEWDPRID4,
                                        PRLPROID = NEWPROID,
                                        PRLPRICE = number4,
                                        PRLSOCODEE = EntegreFDLL.Class.Entegref.GetLogins.userID
                                    });
                                }
                                if (decimal.TryParse(PR5, out decimal number5))
                                {
                                    var NEWDPRID5 = DEFPRICE.FirstOrDefault(x => x.DPRVAL == "TK12").DPRID;
                                    pRICELISTs.Add(new PRICELIST
                                    {
                                        PRLDPRID = NEWDPRID5,
                                        PRLPROID = NEWPROID,
                                        PRLPRICE = number5,
                                        PRLSOCODEE = EntegreFDLL.Class.Entegref.GetLogins.userID
                                    });
                                }
                                pROMAIN.pRODUCTs = pRODUCTs;
                                pROMAIN.pRICELISTs = pRICELISTs;
                                pROMAIN.pROUNITs = pROUNITs;
                                pROMAIN.wAVEPRODUCTs = wAVEPRODUCTs;
                                pROMAINs.Add(pROMAIN);
                            }
                            else
                            {
                                memoEdit1.Text = memoEdit1.Text + $@"
{usedRange[rowIndex, SiraPROVAL - 1].Value.ToString()} Listede Tekrar Ediyor";
                            }
                        }
                        else
                        {
                            memoEdit1.Text = memoEdit1.Text + $@"
{usedRange[rowIndex, SiraPROVAL - 1].Value.ToString()} Stok Kodu Kayıtlı";
                        }
                        progressForm.PerformStep(this);

                    }
                    catch (Exception ex)
                    {
                        progressForm.PerformStep(this);
                        string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                        CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            },
                null,
                () =>
                {
                    completeProgress();
                    progressForm.Hide(this);
                    try
                    {
                        EntegreFDLL.Class.Entegref.SplashScreen(this, $"{item.Text} olarak işleniyor", Properties.Settings.Default.CompanyName, "Lütfen Bekleyin");
                        for (int rowIndex = index; rowIndex < 10; rowIndex++)

                        gridVeriler.DataSource = pROMAINs;
                        navigationFrame2.SelectedPage = navigationPage4;
                        navBarStok.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
                        tileBarItemKaydet.Enabled = true;

                        for (int i = 0; i < ViewVeriler.DataRowCount; i++)
                        {
                            ViewVeriler.ExpandMasterRow(i);
                        }
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
                });
        }
        private async void tileBarItemKaydet_ItemClick(object sender, TileItemEventArgs e)
        {
            var item = sender as TileItem; // veya TileBarItem
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, $"{item.Text} olarak işleniyor", Properties.Settings.Default.CompanyName, "Lütfen Bekleyin");
                List<PRODUCTS> products = new List<PRODUCTS>();
                List<WAVEPRODUCTS> waveProducts = new List<WAVEPRODUCTS>();
                List<PROUNIT> prounit = new List<PROUNIT>();
                List<PRICELIST> pricelist = new List<PRICELIST>();
                for (int i = 0; i < ViewVeriler.RowCount; i++)
                {
                    var rowObj = ViewVeriler.GetRow(i) as PROMAIN;
                    products.AddRange(rowObj.pRODUCTs);
                    waveProducts.AddRange(rowObj.wAVEPRODUCTs);
                    prounit.AddRange(rowObj.pROUNITs);
                    pricelist.AddRange(rowObj.pRICELISTs);
                }
                using (var sqlConnection = new SqlConnection(sql1))
                {
                    await sqlConnection.OpenAsync();
                    using (var tran = sqlConnection.BeginTransaction())
                    {
                        var db = new DbTrans(sqlConnection, tran);
                        try
                        {
                            var PRODUCTS = convertler.ToDataTable(products);
                            var BulkInsertReturn = db.BulkInsertRetorn(PRODUCTS, "PRODUCTS");
                            if (BulkInsertReturn == "Aktarım Tamamlandı")
                            {
                                var PROUNIT = convertler.ToDataTable(prounit);
                                BulkInsertReturn = db.BulkInsertRetorn(PROUNIT, "PROUNIT");


                                var PRICELIST = convertler.ToDataTable(pricelist);
                                BulkInsertReturn = db.BulkInsertRetorn(PRICELIST, "PRICELIST");


                                var WAVEPRODUCTS = convertler.ToDataTable(waveProducts);
                                BulkInsertReturn = db.BulkInsertRetorn(WAVEPRODUCTS, "WAVEPRODUCTS");
                                tran.Commit(); // Başarılıysa commit
                            }
                            else
                            {
                                tran.Rollback(); // Hata olursa rollback
                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback(); // Hata olursa rollback
                            throw new Exception(ex.Message);
                        }
                    }
                }
                navigationFrame1.SelectedPage = navigationPage1;
                navigationFrame2.SelectedPage = navigationPage3;
                gridVeriler.DataSource = null;
                navBarStok.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                tileBarItemKaydet.Enabled = false;
                Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
                // Çalışma sayfasının içeriğini temizle
                worksheet.Clear(worksheet.GetDataRange());
                Stoklar();
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
    }
}