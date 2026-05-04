namespace EntegrefKrediOnay
{
    partial class frmVKN
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVKN));
            this.btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.txtFirma = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVKN = new DevExpress.XtraEditors.TextEdit();
            this.btnLogo = new DevExpress.XtraEditors.SimpleButton();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFirma.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVKN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnKaydet
            // 
            this.btnKaydet.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnKaydet.Location = new System.Drawing.Point(20, 221);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(376, 25);
            this.btnKaydet.TabIndex = 5;
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_ClickAsync);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(372, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Firma VKN";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.txtFirma);
            this.panelControl1.Controls.Add(this.label2);
            this.panelControl1.Controls.Add(this.txtVKN);
            this.panelControl1.Controls.Add(this.label1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(20, 137);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(376, 84);
            this.panelControl1.TabIndex = 6;
            // 
            // txtFirma
            // 
            this.txtFirma.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtFirma.Location = new System.Drawing.Point(2, 58);
            this.txtFirma.Name = "txtFirma";
            this.txtFirma.Size = new System.Drawing.Size(372, 20);
            this.txtFirma.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(2, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(372, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "Firma Adı";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtVKN
            // 
            this.txtVKN.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtVKN.Location = new System.Drawing.Point(2, 20);
            this.txtVKN.Name = "txtVKN";
            this.txtVKN.Size = new System.Drawing.Size(372, 20);
            this.txtVKN.TabIndex = 3;
            // 
            // btnLogo
            // 
            this.btnLogo.ImageOptions.Image = global::EntegrefKrediOnay.Properties.Resources.importimage_32x32;
            this.btnLogo.Location = new System.Drawing.Point(20, 34);
            this.btnLogo.Name = "btnLogo";
            this.btnLogo.Size = new System.Drawing.Size(149, 23);
            this.btnLogo.TabIndex = 7;
            this.btnLogo.Text = "Firma LOGO Yükle";
            this.btnLogo.Click += new System.EventHandler(this.btnLogo_Click);
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureEdit1.Location = new System.Drawing.Point(20, 63);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            this.pictureEdit1.Size = new System.Drawing.Size(376, 74);
            this.pictureEdit1.TabIndex = 1;
            this.pictureEdit1.Visible = false;
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(175, 36);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(221, 20);
            this.textEdit1.TabIndex = 8;
            // 
            // frmVKN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 266);
            this.Controls.Add(this.pictureEdit1);
            this.Controls.Add(this.textEdit1);
            this.Controls.Add(this.btnLogo);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.btnKaydet);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmVKN";
            this.Load += new System.EventHandler(this.frmVKN_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtFirma.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVKN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txtVKN;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnLogo;
        private DevExpress.XtraEditors.TextEdit txtFirma;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TextEdit textEdit1;
    }
}