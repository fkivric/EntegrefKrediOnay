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
using DevExpress.XtraSplashScreen;
using System.Threading;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Controls;
using Newtonsoft.Json;
using System.Net;
using EntegrefKrediOnay.Class;
using EntegreFDLL.Class;
using EntegreFDLL;
using static EntegreFDLL.Class.VolantApiClass;
using EntegreFDLL.Main;
using EntegrefKrediOnay.Merkez;

namespace EntegrefKrediOnay
{
    public partial class frmBGKefil : DevExpress.XtraEditors.XtraForm
    {
        public static EntegreFBGConfigProvider FBGConfigProvider = new EntegreFBGConfigProvider();
        string WRTRID;
        string SALID;
        string CURID;
        string WRTRCURID;
        public frmBGKefil(string _wrtrid, string _salid, string _curid)
        {
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, "Saha Yönetim Destek Tools",Properties.Settings.Default.CompanyName, "Kefil Bilgileri Açılıyor");
                CURID = _curid;
                SALID = _salid;
                WRTRID = _wrtrid;
                InitializeComponent();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
                DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
                defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
                DevExpress.Skins.SkinManager.EnableFormSkins();
                DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                Control.CheckForIllegalCrossThreadCalls = false;
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
        BGClass GetBGClass = new BGClass();
        SqlConnectionObject conn = new SqlConnectionObject();
        private Image kimlikresmi;
        private Image Portreresmi;
        private async void frmKefil_Load(object sender, EventArgs e)
        {
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            pictureEdit1.MouseEnter += PictureEdit1_MouseEnter;
            pictureEdit1.MouseMove += PictureEdit1_MouseMove;
            pictureEdit1.MouseLeave += PictureEdit1_MouseLeave;

            pictureEdit2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            var pdfsize = navigationFrame1.Size;
            flyoutPanel1.OwnerControl = this;
            flyoutPanel1.Size = pdfsize;// new Size(700, 700);
            var pdfloc = navigationFrame1.Location;
            flyoutPanel1.Options.Location = new Point(250, 250); //pdfloc; //
            flyoutPanel1.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
            flyoutPanel1.Options.AnimationType = DevExpress.Utils.Win.PopupToolWindowAnimation.Fade;
            await Bilgiler();
        }
        Warranter myDeserializedClass = new Warranter();
        private async Task Bilgiler()
        {
            DataTable urunler = new DataTable();
            DataTable ekstre = new DataTable();
            DataTable gecikmedetay = new DataTable();
            double Pesin = 0;
            double Taksitli = 0;
            double Odenen = 0;
            double Kalan = 0;
            double Geciken = 0;
            double DigerKefilTutar = 0;
            string RiskYuzdesi = "";
            string Ortalama = "";
            try
            {
                urunler = conn.GetData($@"select PROVAL as [Stok Kodu],PRONAME as [Stok Adı],ORDCHQUAN as [Satış Adeti],ORDCHBALANCE as [Satış Tutarı],
                    case when (select SALAMOUNT from SALES where SALID = ORDSALID) <= Risk_TutarMax then Risk_id else Risk_id+1 end as Risktutar,
                    case when ORDCHBALANCE >= Risk_TutarMax then Risk_id+1 else Risk_id end as Riskid ,
                    DIVNAME as [Satış Yapılan Mağaza]
			        from ORDERS 
                    left outer join DIVISON on DIVVAL = ORDDIVISON
                    left outer join ORDERSCHILD on ORDCHORDID = ORDID
                    left outer join PRODUCTS on PROID = ORDCHPROID
                    left outer join EntegreF.dbo.KrediPuan_RiskUrunGurup on VOLUID = PROPROUID
                    left outer join EntegreF.dbo.KrediPuan_RiskUrunGurupPuan on id = Risk_id
                    where ORDSALID in({SALID})
                    union 
                    select '' as [Stok Kodu],'' as [Stok Adı],sum(ORDCHQUAN) as [Satış Adeti],sum(ORDCHBALANCE) as [Satış Tutarı],
                                        p.id as Risktutar,
                                        p.id as Riskid ,
                                        DIVNAME as [Satış Yapılan Mağaza] from (
                                        select DIVNAME, sum(ORDCHQUAN) as ORDCHQUAN,sum(ORDCHBALANCE) as ORDCHBALANCE from ORDERS 
                                        left outer join DIVISON on DIVVAL = ORDDIVISON
                                        left outer join ORDERSCHILD on ORDCHORDID = ORDID
                                        left outer join PRODUCTS on PROID = ORDCHPROID
                                        where ORDSALID in({SALID})
                                        group by DIVNAME)toplamlar
                                        outer apply(select * from EntegreF.dbo.KrediPuan_RiskSatısGurupPuan p where ORDCHBALANCE between p.Risk_TutarMin and p.Risk_TutarMax) p
                                        group by p.id,DIVNAME
                    order by 1 desc", Properties.Settings.Default.connectionstring);
                gridUrunler.DataSource = urunler;
                ViewUrunler.Columns[4].Visible = false;
                ViewUrunler.Columns[5].Visible = false;
                ViewUrunler.FocusedRowHandle = GridControl.InvalidRowHandle;
                ViewUrunler.OptionsSelection.MultiSelect = false;
                ViewUrunler.OptionsBehavior.Editable = false;
                ViewUrunler.OptionsBehavior.ReadOnly = true;
                ViewUrunler.OptionsBehavior.ReadOnly = true;
                ViewUrunler.OptionsSelection.EnableAppearanceFocusedRow = false;
                ViewUrunler.Appearance.FocusedRow.Options.UseBackColor = false;
                ViewUrunler.Appearance.SelectedRow.Options.UseBackColor = false;
                ViewUrunler.OptionsView.ColumnAutoWidth = false;
                ViewUrunler.ExpandAllGroups();
                ViewUrunler.OptionsView.BestFitMaxRowCount = -1;
                ViewUrunler.BestFitColumns(true);

                var Kefildiger = conn.GetData($@"
                select 0 as sira, SALWSALID,CURVAL [Müşteri No],CURNAME [Müşteri Adı],'' as [Adet],SALAMOUNT as [Tutar],
                Cast(tolamtaksit as char(2)) +'/'+ cast(kalantaksit as char(2)) as [Taksit Durumu] from SALESWARRANTERS 
                left outer join SALES on SALID = SALWSALID
                left outer join CURRENTS on CURID = SALCURID
                outer apply(select count(*) tolamtaksit from INSTALMENT where INSSALID = SALWSALID) isnscount
                outer apply(select count(*) kalantaksit from INSTALMENT where INSSALID = SALWSALID and INSBALANCE > 0) isnskalan
                where SALWWRTRID = {WRTRID} --and SALWSALID not in ({SALID})
                union all
                select 1 as sira, SALWSALID,
                case when ORDCHILD.PROVAL is not null then ORDCHILD.PROVAL else INVCHILD.PROVAL end,
                case when ORDCHILD.PRONAME is not null then ORDCHILD.PRONAME else INVCHILD.PRONAME end,
                cast(case when ORDCHILD.ORDCHQUAN is not null then ORDCHILD.ORDCHQUAN else INVCHILD.PROBHQUAN end as varchar(100)),
                case when ORDCHILD.ORDCHBALANCE is not null then ORDCHILD.ORDCHBALANCE else INVCHILD.INVCHBALANCE end,
                ''
                from SALESWARRANTERS 
                outer apply (select PROVAL,PRONAME,ORDCHQUAN,ORDCHBALANCE from ORDERS
			                left outer join ORDERSCHILD on ORDCHORDID = ORDID
			                left outer join PRODUCTS on PROID = ORDCHPROID
			                where ORDSALID = SALWSALID) ORDCHILD
                outer apply (select PROVAL,PRONAME,PROBHQUAN,INVCHBALANCE from INVOICE
			                left outer join INVOICECHILD on INVCHINVID = INVID
			                left outer join INVOICECHILDPROBH on INVCHPBHID = INVCHID
			                left outer join PRODUCTSBEHAVE on PROBHID = INVCHPBHPROBHID
			                left outer join PRODUCTS on PROID = PROBHPROID
			                where INVSALID = SALWSALID) INVCHILD
                where SALWWRTRID = {WRTRID} --and SALWSALID not in ({SALID})
                order by 2,1", Properties.Settings.Default.connectionstring);
                if (Kefildiger != null )
                {
                    gridDigerKefilSatislar.DataSource = Kefildiger;
                    ViewDigerKefilSatislar.Columns[4].Visible = false;
                    ViewDigerKefilSatislar.Columns[5].Visible = false;
                    ViewDigerKefilSatislar.FocusedRowHandle = GridControl.InvalidRowHandle;
                    ViewDigerKefilSatislar.OptionsSelection.MultiSelect = false;
                    ViewDigerKefilSatislar.OptionsBehavior.Editable = false;
                    ViewDigerKefilSatislar.OptionsBehavior.ReadOnly = true;
                    ViewDigerKefilSatislar.OptionsBehavior.ReadOnly = true;
                    ViewDigerKefilSatislar.OptionsSelection.EnableAppearanceFocusedRow = false;
                    ViewDigerKefilSatislar.Appearance.FocusedRow.Options.UseBackColor = false;
                    ViewDigerKefilSatislar.Appearance.SelectedRow.Options.UseBackColor = false;
                    ViewDigerKefilSatislar.OptionsView.ColumnAutoWidth = false;
                    ViewDigerKefilSatislar.ExpandAllGroups();
                    ViewDigerKefilSatislar.OptionsView.BestFitMaxRowCount = -1;
                    ViewDigerKefilSatislar.BestFitColumns(true);
                    for (int i = 0; i < Kefildiger.Rows.Count; i++)
                    {
                        if (Kefildiger.Rows[i]["sira"].ToString() == "0")
                        {
                            DigerKefilTutar += double.Parse(Kefildiger.Rows[i]["Tutar"].ToString());
                        }
                    }
                }
                else
                {
                    DigerKefilTutar = 0;
                }
                FBGConfigProvider.filter.curId = long.Parse(CURID);
                FBGConfigProvider.filter.WRTRID = long.Parse(WRTRID);
                FBGConfigProvider.Servis = "GetWarranters";
                await Task.Run(async () =>
                {
                    var sonuc = await GetBGClass.VolantServisAsync(FBGConfigProvider);
                    myDeserializedClass = JsonConvert.DeserializeObject<Warranter>(sonuc);
                });
                if (myDeserializedClass.success)
                {

                    string CURVAL = myDeserializedClass.rWarranters.curValk__BackingField?.ToString() ?? null;
                    lblAciklama.Text = conn.GetValueConnection($"select SALWRELATEDEGREE from SALESWARRANTERS where SALWSALID in ({SALID})", Properties.Settings.Default.connectionstring);
                    if (CURVAL != null)
                    {
                        WRTRCURID = conn.GetValueConnection($"select CURID from CURRENTS where CURVAL = '{CURVAL}'", Properties.Settings.Default.connectionstring);

                        var qs = string.Format("select CURID,CURVAL,CURNAME,CUPIDENTITY,CUPPORTRAIT from CURRENTS " +
                        "left outer join CUSTOMERPICTURE on CUPCURID = CURID " +
                        "where CURID = '{0}' AND CUSTOMERPICTURE.CUPTYPE = 1", WRTRCURID);// sabit "1993863"
                        var kimlik = conn.GetData(qs, Properties.Settings.Default.connectionstring);
                        if (kimlik != null)
                        {
                            if (kimlik.Rows.Count > 0)
                            {
                                kimlikresmi = VolantConvert.FromBase64String(kimlik.Rows[0]["CUPIDENTITY"].ToString()).byteArrayToImage();
                                Portreresmi = VolantConvert.FromBase64String(kimlik.Rows[0]["CUPPORTRAIT"].ToString()).byteArrayToImage();
                            }
                        }
                        pictureEdit1.Image = Portreresmi;//Class.Convert.FromBase64String(kimlikresmi).byteArrayToImage();
                        //Image resim = Class.Convert.FromBase64String(kimlikresmi).byteArrayToImage();
                        //string kaydetmeYolu = "resmi.png";
                        //resim.Save(kaydetmeYolu);

                        ekstre = conn.GetData($@"select * from(
                    select 2 as sira,SALID as ID,SALDATE as tarih,DIVNAME as MagazaAdı,
                    case when SALID > 0 then 'Alışveriş Toplamı' else 'İade Toplamı' end as Tip,
                    case when SALID > 0 then 'Alışveriş Toplamı' else 'İade Toplamı' end as [Ürün Kodu],
                    case when SALID > 0 then 'Alışveriş Toplamı' else 'İade Toplamı' end as [Ürün Adı],
                    isnull(case when ORDCHQUAN != 0 then case when SALID < 0 then -1*ORDCHQUAN else ORDCHQUAN end else case when SALID < 0 then -1*INVCHQUAN else INVCHQUAN end end, 0) as [Satılan Adet], 
                    isnull(case when SALID < 0 then -1*ORDCHBALANCE else ORDCHBALANCE end, 0) as [Teslimat Bekleyen Adet],
                    case when SALID < 0 then -1*SALAMOUNT else SALAMOUNT end as [Alisveriş Tutar],
                    cast(TaksitKalan as Char(2)) as [Kalan Taksit Sayısı], '' as Satici
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
                    outer apply (select count(*) as TaksitKalan from INSTALMENT where INSSALID = SALID and INSBALANCE > 0) INSTALMENT
                    where SALCURID = {WRTRCURID}
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
                    where ORDCURID = {WRTRCURID}
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
                    where SALCURID = {WRTRCURID}
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
                    where SALCURID = {WRTRCURID}
                    AND SALSALEKIND = 'P'", Properties.Settings.Default.connectionstring));

                        Taksitli = double.Parse(conn.GetValueConnection($@"
                    select isnull(sum(SALAMOUNT),0)  from SALES s
                    where SALCURID = {WRTRCURID}
                    and SALID > 0
                    and not exists (select * from SALES i where i.SALCANSALID = s.SALID) 
                    AND SALSALEKIND = 'T'", Properties.Settings.Default.connectionstring));

                        Odenen = double.Parse(conn.GetValueConnection($@"
                    select 
                    isnull(sum(case when PCDSDC = 0 then PCDSAMOUNT else PCDSAMOUNT*-1 end
                    +PCDSEARLYPAYDISC+PCDSLATEINCOME),0)
                    from PROCEEDS 
                    where PCDSCURID = {WRTRCURID} ", Properties.Settings.Default.connectionstring));

                        Kalan = double.Parse(conn.GetValueConnection($@"
                    select isnull(sum(INSBALANCE),0)  from INSTALMENT
                    where INSCURID = {WRTRCURID}
                    AND INSBALANCE > 0", Properties.Settings.Default.connectionstring));

                        gecikmedetay = conn.GetData($@"select count(*) ,isnull(sum(PCDSLATEINCOME), 0) from PROCEEDS where PCDSCURID = {CURID} and isnull(PCDSLATEINCOME,0) != 0", Properties.Settings.Default.connectionstring);
                        Ortalama = conn.GetValueConnection($@"
                    select isnull(sum(DATEDIFF(Day,INSFIXDATE,PCDSDATE))/count(*),0) from INSTALMENT
                    outer apply(select PCDSDATE from INSTALMENTPROCEEDS 
			                    left outer join PROCEEDS on PCDSID = INSPCDPCDID
			                    where INSPCDINSID = INSID 
			                    and INSPCDLATEINCOME != 0) odeme
                     where INSCURID = {WRTRCURID}  and odeme.PCDSDATE is not NULL", Properties.Settings.Default.connectionstring);

                        var gc = conn.GetValueConnection($@"
                        select sum(isnull(INSBALANCE,0)) from INSTALMENT
                        where INSCURID = {WRTRCURID}
                        AND INSBALANCE > 0
                        AND INSFIXDATE  between DATEADD(day,-60, GETDATE()) and getdate()", Properties.Settings.Default.connectionstring);
                        if (gc != "")
                        {
                            Geciken = double.Parse(gc);
                        }
                        RiskYuzdesi = conn.GetValueConnection($@"
                        select ROUND(RiskYuzdesi,0) from (
                            select sum(SALAMOUNT)+{DigerKefilTutar} as SALAMOUNT from (
                            select isnull(sum(INSBALANCE),0) as SALAMOUNT from INSTALMENT
                            where INSCURID = {WRTRCURID} and INSBALANCE > 0
                            union
                            select sum(SALAMOUNT) as SALAMOUNT from SALES
                            where SALID in ({SALID})
                            )sonuc
                        ) SALES
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
                        UpdateRiskBar(int.Parse(RiskYuzdesi.Replace(".00", "")));

                        if (CheckFtpFolderExists(WRTRCURID + "/E-Devlet"))
                        {
                            chkEDevlet.Checked = true;
                            chkEDevlet.Text = "Kefil Belgeleri Yüklü";
                        }
                        txtPesin.EditValue = Pesin;
                        txtTaksitli.EditValue = Taksitli;
                        txtOdenen.EditValue = Odenen;
                        txtKalan.EditValue = Kalan;
                        txtVadeFarkiTtuari.EditValue = gecikmedetay.Rows[0][0].ToString();
                        txtVadeFarkiTtuari.EditValue = gecikmedetay.Rows[0][1].ToString();
                        txtVadeFarkiGun.EditValue = Ortalama;
                        if (Geciken > 10)
                        {
                            blinkTimer.Start();
                            txtGeciken.EditValue = Geciken;
                        }
                        else
                        {
                            blinkTimer.Stop();
                            txtGeciken.EditValue = 0;
                        }
                        
                        gridEkstre.DataSource = ekstre;
                        if (ekstre != null)
                        {
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
                        }
                        await Musteri();
                        await Notlar();
                    }
                    else
                    {
                        var qs = string.Format("SELECT CUSTOMERPICTURE.* FROM  CUSTOMERPICTURE WHERE CUSTOMERPICTURE.CUPCURID = '{0}' AND CUSTOMERPICTURE.CUPTYPE = 2", WRTRID);// sabit "1993863"
                        var kimlik = conn.GetData(qs, Properties.Settings.Default.connectionstring);
                        if (kimlik != null)
                        {
                            if (kimlik.Rows.Count > 0)
                            {
                                kimlikresmi = VolantConvert.FromBase64String(kimlik.Rows[0]["CUPIDENTITY"].ToString()).byteArrayToImage();
                                Portreresmi = VolantConvert.FromBase64String(kimlik.Rows[0]["CUPPORTRAIT"].ToString()).byteArrayToImage();
                            }
                        }
                        pictureEdit1.Image = Portreresmi;//Class.Convert.FromBase64String(kimlikresmi).byteArrayToImage();
                        //pictureEdit1.Refresh();
                        RiskYuzdesi = conn.GetValueConnection($@"
                        select ROUND(RiskYuzdesi,0) from (
                            select sum(SALAMOUNT)+{DigerKefilTutar} as SALAMOUNT from (
                            select sum(SALAMOUNT) as SALAMOUNT from SALES
                            where SALID in ({SALID})
                            )sonuc
                        ) SALES
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
                        UpdateRiskBar(int.Parse(RiskYuzdesi.Replace(".00", "")));
                        tileBarItem3.Enabled = false;
                        tileBarItem4.Enabled = false;
                        txtMagaza.EditValue = conn.GetValueConnection($"select SALWRELATEDEGREE from SALESWARRANTERS where SALWWRTRID = '{WRTRID}'", Properties.Settings.Default.connectionstring);
                        txtKodu.EditValue = "";
                        txtAdi.EditValue = myDeserializedClass.rWarranters.WRTRNAMEk__BackingField;
                        txtSoyadi.EditValue = myDeserializedClass.rWarranters.WRTRSURNAMEk__BackingField;
                        txtTC.EditValue = myDeserializedClass.rWarranters.WRTRIDNOk__BackingField;
                        txtVknName.EditValue = "";
                        txtVknNo.EditValue = "";
                        txtSgkNoı.EditValue = myDeserializedClass.rWarranters.WRTRSGKNOk__BackingField;
                        togAktif.IsOn = true;
                        dteDogumTarihi.EditValue = myDeserializedClass.rWarranters.WRTRBIRTHDATEk__BackingField;


                        //Kimlik Bilgileri
                        txtCuzdanNo.EditValue = myDeserializedClass.rWarranters.WRTRIDSERIALk__BackingField;
                        txtBaba.EditValue = myDeserializedClass.rWarranters.WRTRFATHERk__BackingField;
                        txtAnne.EditValue = myDeserializedClass.rWarranters.WRTRMOTHERk__BackingField;
                        txtDogumYeri.EditValue = myDeserializedClass.rWarranters.WRTRBIRTHPLACEk__BackingField;
                        txtDogumIl.EditValue = myDeserializedClass.rWarranters.WRTRIDCITYk__BackingField;
                        txtDogumIlce.EditValue = myDeserializedClass.rWarranters.WRTRIDCOUNTYk__BackingField;
                        txtMahlle.EditValue = myDeserializedClass.rWarranters.WRTRIDDISTRICTk__BackingField;
                        txtCiltNo.EditValue = myDeserializedClass.rWarranters.WRTRIDREGISTERNOk__BackingField;
                        txtAileSiraNo.EditValue = myDeserializedClass.rWarranters.WRTRIDFAMILYNOk__BackingField;
                        txtSiraNo.EditValue = myDeserializedClass.rWarranters.WRTRIDSORTNOk__BackingField;
                        txtKayitNo.EditValue = myDeserializedClass.rWarranters.WRTRIDREGISTERNOk__BackingField;
                        txtVerildigiYer1.EditValue = myDeserializedClass.rWarranters.WRTRIDGIVENPLACEk__BackingField;
                        dteVerildiTarih1.EditValue = myDeserializedClass.rWarranters.WRTRIDGIVENDATEk__BackingField;

                        int vierilisnedeni = int.Parse(myDeserializedClass.rWarranters.WRTRIDGIVENREASONk__BackingField);
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
                        txtEhilyetNo.EditValue = "";
                        txtVerildiYer2.EditValue = "";
                        dteVerildiTarih2.EditValue = "";

                        txtEvIl.EditValue = myDeserializedClass.rWarranters.WRTRCOUNTYk__BackingField;
                        txtEvIlce.EditValue = myDeserializedClass.rWarranters.WRTRCITYk__BackingField;
                        txtEvMahalle.EditValue = myDeserializedClass.rWarranters.WRTRADR1k__BackingField;
                        txtEvAdres.EditValue = myDeserializedClass.rWarranters.WRTRADR2k__BackingField;
                        txtPKod.EditValue = myDeserializedClass.rWarranters.WRTRPOSTALCODEk__BackingField;
                        txtMail.EditValue = myDeserializedClass.rWarranters.WRTREMAILk__BackingField;

                        txtIsIl.EditValue = myDeserializedClass.rWarranters.WRTRWORKCITYk__BackingField;
                        txtIsIlce.EditValue = myDeserializedClass.rWarranters.WRTRWORKCOUNTk__BackingField;
                        txtIsAdi.EditValue = myDeserializedClass.rWarranters.WRTRWORKk__BackingField;
                        txtIsTel.EditValue = myDeserializedClass.rWarranters.WRTRWORKPHONEk__BackingField;
                        txtIsadres1.EditValue = myDeserializedClass.rWarranters.WRTRWORKADR1k__BackingField;
                        txtIsAdres2.EditValue = myDeserializedClass.rWarranters.WRTRWORKADR2k__BackingField;

                        txtGsm1.EditValue = myDeserializedClass.rWarranters.WRTRGSMk__BackingField;
                        txtGsm2.EditValue = "";
                        txtGsm3.EditValue = "";

                        txtTel1.EditValue = myDeserializedClass.rWarranters.WRTRPHONEk__BackingField;
                        txtTel2.EditValue = "";
                        txtTel3.EditValue = "";
                    }
                }
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ViewUrunler_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var riskid = gv.GetRowCellValue(e.RowHandle, "Riskid")?.ToString();
            var risktutar = gv.GetRowCellValue(e.RowHandle, "Risktutar")?.ToString();

            if (riskid == null || riskid == "") return;

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
        private void UpdateRiskBar(int value)
        {
            int barHeight = panelBar.Height;
            int barWidth = panelBar.Width;

            Bitmap bmp = new Bitmap(barWidth, barHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Bölümlerin genişlikleri
                int greenWidth = (int)(barWidth * 0.24);
                int yellowWidth = (int)(barWidth * 0.25);
                int orangeWidth = (int)(barWidth * 0.25);
                int redWidth = barWidth - greenWidth - yellowWidth - orangeWidth;

                // Soldan sağa renkler
                int x = 0;
                g.FillRectangle(Brushes.Green, x, 0, greenWidth, barHeight); x += greenWidth;
                g.FillRectangle(Brushes.Yellow, x, 0, yellowWidth, barHeight); x += yellowWidth;
                g.FillRectangle(Brushes.Orange, x, 0, orangeWidth, barHeight); x += orangeWidth;
                g.FillRectangle(Brushes.Red, x, 0, redWidth, barHeight);

                // Yüzde + kategori yazısı
                string category = GetCategoryName(value);
                string text = $"{value}% - {category}";
                using (Font f = new Font("Segoe UI", 10, FontStyle.Bold))
                using (Brush b = new SolidBrush(Color.Black))
                {
                    var textSize = g.MeasureString(text, f);
                    g.DrawString(text, f, b, (barWidth - textSize.Width) / 2, (barHeight - textSize.Height) / 2);
                }
            }

            panelBar.BackgroundImage = bmp;
            panelBar.BackgroundImageLayout = ImageLayout.Stretch;

            // Ok pozisyonu (0 solda, 100 sağda)
            int positionX = (int)(value / 100.0 * barWidth) - lblArrow.Width / 2;
            lblArrow.Left = panelBar.Left + Math.Max(0, Math.Min(barWidth - lblArrow.Width, positionX));
            lblArrow.Top = panelBar.Bottom + 2; // Ok barın altında
            lblArrow.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblArrow.ForeColor = Color.Black;
            lblArrow.Text = "▲"; // Yatayda yukarı bakan ok
        }
        private string GetCategoryName(int value)
        {
            if (value <= 24) return "Güvenli";
            if (value <= 49) return "Orta";
            if (value <= 74) return "Riskli";
            return "Kritik";
        }
        private bool isRed = true;
        private void blinkTimer_Tick(object sender, EventArgs e)
        {
            if (isRed)
            {
                txtGeciken.BackColor = Color.White;
                txtGeciken.ForeColor = Color.Red;
                txtGeciken.Font = new Font(txtGeciken.Font.FontFamily, 12, FontStyle.Bold);

                isRed = false;
            }
            else
            {
                txtGeciken.BackColor = Color.White;
                txtGeciken.ForeColor = Color.Black;
                txtGeciken.Font = new Font(txtGeciken.Font.FontFamily, 8, FontStyle.Regular);
                isRed = true;
            }
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

        public async Task Musteri()
        {
            FBGConfigProvider.filter.curId = long.Parse(WRTRCURID);
            FBGConfigProvider.Servis = "GetCustomerForProfile";
            Currents myDeserializedClass = new Currents();
            await Task.Run(async () =>
            {
                var sonuc = await GetBGClass.VolantServisAsync(FBGConfigProvider);
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
        public async Task Notlar()
        {
            FBGConfigProvider.filter.curId = long.Parse(WRTRCURID);
            FBGConfigProvider.Servis = "GetAllCurNotes";
            CurNotes myDeserializedClass = new CurNotes();
            List<Notes> curNotes = new List<Notes>();
            await Task.Run(async () =>
            {
                var sonuc = await GetBGClass.VolantServisAsync(FBGConfigProvider);
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
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            var dt = converter.ToDataTable(curNotes);
            gridNotes.DataSource = dt;
        }
        private void tileBarItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage1;
        }

        private void tileBarItem2_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage2;
        }

        private void tileBarItem3_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage3;
        }

        private void tileBarItem4_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage4;
        }
        private void tileBarItem6_ItemClick(object sender, TileItemEventArgs e)
        {
            navigationFrame1.SelectedPage = navigationPage5;
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
        private void tileBarItem5_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(WRTRCURID))
            {
                if (CheckFtpFolderExists(WRTRCURID + "/E-Devlet") || CheckFtpFolderExists(WRTRCURID + "/Yüklenen Dosyalar"))
                {
                    CustomMessageBox.ShowMessage("Kefil Müşteri Hesabı Belgeleri Açıklacak", "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmBGFtp devletKrediPuan = new frmBGFtp(WRTRCURID);
                    devletKrediPuan.ShowDialog();
                }
            }
            else
            {
                if (CheckFtpFolderExists(CURID + "/E-Devlet") || CheckFtpFolderExists(CURID + "/Yüklenen Dosyalar"))
                {
                    CustomMessageBox.ShowMessage("Satıştaki Müşteri Hesabı Belgeleri Açıklacak", "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmBGFtp devletKrediPuan = new frmBGFtp(CURID);
                    devletKrediPuan.ShowDialog();
                }
            }
        }

        private void ViewDigerKefilSatislar_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var sira = gv.GetRowCellValue(e.RowHandle, "sira")?.ToString();
            var SALWSALID = gv.GetRowCellValue(e.RowHandle, "SALWSALID")?.ToString();

            if (sira == null) return;
            if (sira == "0")
            {
                e.Appearance.BackColor = Color.Red;
                e.Appearance.ForeColor = Color.White;
            }
            e.Appearance.Options.UseBackColor = true;
            e.Appearance.Options.UseForeColor = true;

            // ÇOK ÖNEMLİ: RowStyle'ın seçili/odak stilinden sonra uygulanmasını sağlayarak üzerine yazılmasını engelle
            e.HighPriority = true;
        }
    }
}