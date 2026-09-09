namespace EntegrefKrediOnay.Magaza
{
    partial class frmKasiyerMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKasiyerMain));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem6 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem7 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem8 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.ımageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ımageCollection1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ribbon.SearchEditItem,
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3,
            this.barButtonItem4,
            this.barButtonItem5,
            this.barButtonItem6,
            this.barButtonItem7,
            this.barButtonItem8});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 9;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1268, 158);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.AllowHtmlText = DevExpress.Utils.DefaultBoolean.False;
            this.barButtonItem1.Caption = "Müşteri Hesap İşlem Merkezi Aç";
            this.barButtonItem1.Id = 1;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.barButtonItem2.Caption = "Yeni Müşteri Kaydet";
            this.barButtonItem2.Id = 2;
            this.barButtonItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.Image")));
            this.barButtonItem2.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.LargeImage")));
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.barButtonItem3.Caption = "Kefil Kartı Kaydet";
            this.barButtonItem3.Id = 3;
            this.barButtonItem3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem3.ImageOptions.Image")));
            this.barButtonItem3.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem3.ImageOptions.LargeImage")));
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // barButtonItem4
            // 
            this.barButtonItem4.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.barButtonItem4.Caption = "Yeni Satış Kaydet";
            this.barButtonItem4.Id = 4;
            this.barButtonItem4.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem4.ImageOptions.Image")));
            this.barButtonItem4.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem4.ImageOptions.LargeImage")));
            this.barButtonItem4.Name = "barButtonItem4";
            this.barButtonItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem4_ItemClick);
            // 
            // barButtonItem5
            // 
            this.barButtonItem5.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.barButtonItem5.Caption = "Evrak Tarat";
            this.barButtonItem5.Id = 5;
            this.barButtonItem5.ImageOptions.Image = global::EntegrefKrediOnay.Properties.Resources.printer_32x32;
            this.barButtonItem5.ImageOptions.LargeImage = global::EntegrefKrediOnay.Properties.Resources.printer_32x32;
            this.barButtonItem5.Name = "barButtonItem5";
            // 
            // barButtonItem6
            // 
            this.barButtonItem6.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.barButtonItem6.Caption = "İstihbarat İşlemleri";
            this.barButtonItem6.Id = 6;
            this.barButtonItem6.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem6.ImageOptions.Image")));
            this.barButtonItem6.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem6.ImageOptions.LargeImage")));
            this.barButtonItem6.Name = "barButtonItem6";
            // 
            // barButtonItem7
            // 
            this.barButtonItem7.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            this.barButtonItem7.Caption = "Sevkiyat Sonuçlandırma";
            this.barButtonItem7.Id = 7;
            this.barButtonItem7.ImageOptions.Image = global::EntegrefKrediOnay.Properties.Resources.transit_32x32;
            this.barButtonItem7.ImageOptions.LargeImage = global::EntegrefKrediOnay.Properties.Resources.transit_32x32;
            this.barButtonItem7.Name = "barButtonItem7";
            // 
            // barButtonItem8
            // 
            this.barButtonItem8.Caption = "Dijital Arşiv Kontrol";
            this.barButtonItem8.Id = 8;
            this.barButtonItem8.ImageOptions.LargeImage = global::EntegrefKrediOnay.Properties.Resources.pdf;
            this.barButtonItem8.Name = "barButtonItem8";
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2,
            this.ribbonPageGroup3});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "Müşteri & Satış İşlemleri";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem2);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem3);
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem1);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Müşteri Kartı";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem7);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "Satış İşlemleri";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.AllowTextClipping = false;
            this.ribbonPageGroup3.ItemLinks.Add(this.barButtonItem5);
            this.ribbonPageGroup3.ItemLinks.Add(this.barButtonItem8);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "Dijital Arşiv İşlemleri";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 636);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1268, 24);
            // 
            // xtraTabControl
            // 
            this.xtraTabControl.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPagesAndTabControlHeader;
            this.xtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl.Location = new System.Drawing.Point(0, 158);
            this.xtraTabControl.MultiLine = DevExpress.Utils.DefaultBoolean.True;
            this.xtraTabControl.Name = "xtraTabControl";
            this.xtraTabControl.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
            this.xtraTabControl.Size = new System.Drawing.Size(1268, 478);
            this.xtraTabControl.TabIndex = 22;
            this.xtraTabControl.CloseButtonClick += new System.EventHandler(this.xtraTabControl_CloseButtonClick);
            this.xtraTabControl.SizeChanged += new System.EventHandler(this.xtraTabControl_SizeChanged);
            // 
            // ımageCollection1
            // 
            this.ımageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("ımageCollection1.ImageStream")));
            this.ımageCollection1.InsertImage(global::EntegrefKrediOnay.Properties.Resources.Entegref__1_, "Entegref__1_", typeof(global::EntegrefKrediOnay.Properties.Resources), 0);
            this.ımageCollection1.Images.SetKeyName(0, "Entegref__1_");
            this.ımageCollection1.Images.SetKeyName(1, "editdatasource_32x32.png");
            this.ımageCollection1.Images.SetKeyName(2, "summary_32x32.png");
            this.ımageCollection1.Images.SetKeyName(3, "showtestreport_32x32.png");
            this.ımageCollection1.Images.SetKeyName(4, "hyperlink_32x32.png");
            this.ımageCollection1.Images.SetKeyName(5, "editname_32x32.png");
            this.ımageCollection1.Images.SetKeyName(6, "convert_32x32.png");
            this.ımageCollection1.Images.SetKeyName(7, "bosale_32x32.png");
            this.ımageCollection1.Images.SetKeyName(8, "financial_32x32.png");
            this.ımageCollection1.Images.SetKeyName(9, "switchtimescalesto_32x32.png");
            this.ımageCollection1.Images.SetKeyName(10, "pivot_32x32.png");
            this.ımageCollection1.Images.SetKeyName(11, "deletedatasource_32x32.png");
            this.ımageCollection1.Images.SetKeyName(12, "movepivottable_32x32.png");
            this.ımageCollection1.Images.SetKeyName(13, "currency_32x32.png");
            this.ımageCollection1.Images.SetKeyName(14, "copymodeldifferences_32x32.png");
            this.ımageCollection1.Images.SetKeyName(15, "borole_32x32.png");
            this.ımageCollection1.Images.SetKeyName(16, "IYS.png");
            this.ımageCollection1.Images.SetKeyName(17, "egaranti.png");
            this.ımageCollection1.InsertImage(global::EntegrefKrediOnay.Properties.Resources.Yon_Logo_16x16, "Yon_Logo_16x16", typeof(global::EntegrefKrediOnay.Properties.Resources), 18);
            this.ımageCollection1.Images.SetKeyName(18, "Yon_Logo_16x16");
            // 
            // frmKasiyerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1268, 660);
            this.Controls.Add(this.xtraTabControl);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.IconOptions.Image = global::EntegrefKrediOnay.Properties.Resources.Entegref__1_;
            this.IsMdiContainer = true;
            this.Name = "frmKasiyerMain";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Mağaza Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmKasiyerMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ımageCollection1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl;
        private DevExpress.Utils.ImageCollection ımageCollection1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.BarButtonItem barButtonItem4;
        private DevExpress.XtraBars.BarButtonItem barButtonItem5;
        private DevExpress.XtraBars.BarButtonItem barButtonItem6;
        private DevExpress.XtraBars.BarButtonItem barButtonItem7;
        private DevExpress.XtraBars.BarButtonItem barButtonItem8;
    }
}