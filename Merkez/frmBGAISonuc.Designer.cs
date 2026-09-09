namespace EntegrefKrediOnay.Merkez
{
    partial class frmBGAISonuc
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
            DevExpress.XtraEditors.TileItemElement tileItemElement1 = new DevExpress.XtraEditors.TileItemElement();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBGAISonuc));
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.richEditControl1 = new DevExpress.XtraRichEdit.RichEditControl();
            this.groupControl9 = new DevExpress.XtraEditors.GroupControl();
            this.tileBar1 = new DevExpress.XtraBars.Navigation.TileBar();
            this.tileBarGroup4 = new DevExpress.XtraBars.Navigation.TileBarGroup();
            this.tileBarItem8 = new DevExpress.XtraBars.Navigation.TileBarItem();
            this.tileBarItem1 = new DevExpress.XtraBars.Navigation.TileBarItem();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl9)).BeginInit();
            this.groupControl9.SuspendLayout();
            this.SuspendLayout();
            // 
            // richEditControl1
            // 
            this.richEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richEditControl1.Location = new System.Drawing.Point(0, 0);
            this.richEditControl1.Name = "richEditControl1";
            this.richEditControl1.Size = new System.Drawing.Size(592, 403);
            this.richEditControl1.TabIndex = 0;
            this.richEditControl1.Text = "richEditControl1";
            // 
            // groupControl9
            // 
            this.groupControl9.AppearanceCaption.Options.UseTextOptions = true;
            this.groupControl9.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.groupControl9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupControl9.Controls.Add(this.tileBar1);
            this.groupControl9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupControl9.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl9.Location = new System.Drawing.Point(0, 403);
            this.groupControl9.Name = "groupControl9";
            this.groupControl9.Size = new System.Drawing.Size(592, 100);
            this.groupControl9.TabIndex = 3;
            this.groupControl9.Text = "Yapay Zeka Sonucu";
            // 
            // tileBar1
            // 
            this.tileBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileBar1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            this.tileBar1.Groups.Add(this.tileBarGroup4);
            this.tileBar1.ItemPadding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.tileBar1.ItemSize = 50;
            this.tileBar1.Location = new System.Drawing.Point(2, 23);
            this.tileBar1.MaxId = 5;
            this.tileBar1.Name = "tileBar1";
            this.tileBar1.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.tileBar1.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
            this.tileBar1.Size = new System.Drawing.Size(588, 75);
            this.tileBar1.TabIndex = 0;
            this.tileBar1.Text = "tileBar1";
            // 
            // tileBarGroup4
            // 
            this.tileBarGroup4.Items.Add(this.tileBarItem8);
            this.tileBarGroup4.Items.Add(this.tileBarItem1);
            this.tileBarGroup4.Name = "tileBarGroup4";
            // 
            // tileBarItem8
            // 
            this.tileBarItem8.AppearanceItem.Normal.BackColor = System.Drawing.Color.Black;
            this.tileBarItem8.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.DarkBlue;
            this.tileBarItem8.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileBarItem8.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            tileItemElement1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            tileItemElement1.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            tileItemElement1.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement1.Text = "Pdf Kaydet";
            this.tileBarItem8.Elements.Add(tileItemElement1);
            this.tileBarItem8.Id = 0;
            this.tileBarItem8.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
            this.tileBarItem8.Name = "tileBarItem8";
            this.tileBarItem8.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBarItem8_ItemClick);
            // 
            // tileBarItem1
            // 
            this.tileBarItem1.AppearanceItem.Normal.BackColor = System.Drawing.Color.DarkBlue;
            this.tileBarItem1.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.SteelBlue;
            this.tileBarItem1.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.tileBarItem1.AppearanceItem.Normal.ForeColor = System.Drawing.Color.Red;
            this.tileBarItem1.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileBarItem1.AppearanceItem.Normal.Options.UseFont = true;
            this.tileBarItem1.AppearanceItem.Normal.Options.UseForeColor = true;
            this.tileBarItem1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            tileItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            tileItemElement2.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
            tileItemElement2.Text = "Müşteri Notuna Kaydet";
            this.tileBarItem1.Elements.Add(tileItemElement2);
            this.tileBarItem1.Id = 3;
            this.tileBarItem1.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
            this.tileBarItem1.Name = "tileBarItem1";
            // 
            // frmBGAISonuc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 503);
            this.Controls.Add(this.richEditControl1);
            this.Controls.Add(this.groupControl9);
            this.IconOptions.Image = global::EntegrefKrediOnay.Properties.Resources.Entegref__1_;
            this.Name = "frmBGAISonuc";
            this.Text = "Entegref AI Sonuc";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmBGAISonuc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl9)).EndInit();
            this.groupControl9.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraRichEdit.RichEditControl richEditControl1;
        private DevExpress.XtraEditors.GroupControl groupControl9;
        private DevExpress.XtraBars.Navigation.TileBar tileBar1;
        private DevExpress.XtraBars.Navigation.TileBarGroup tileBarGroup4;
        private DevExpress.XtraBars.Navigation.TileBarItem tileBarItem8;
        private DevExpress.XtraBars.Navigation.TileBarItem tileBarItem1;
    }
}