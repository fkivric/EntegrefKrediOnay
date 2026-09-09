namespace EntegrefKrediOnay
{
    partial class FrmKimlikTarama
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
            this.comboBoxBelge = new System.Windows.Forms.ComboBox();
            this.pictureKimlik = new DevExpress.XtraEditors.PictureEdit();
            this.pictureFoto = new DevExpress.XtraEditors.PictureEdit();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureKimlik.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureFoto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxBelge
            // 
            this.comboBoxBelge.FormattingEnabled = true;
            this.comboBoxBelge.Location = new System.Drawing.Point(23, 13);
            this.comboBoxBelge.Name = "comboBoxBelge";
            this.comboBoxBelge.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBelge.TabIndex = 0;
            // 
            // pictureKimlik
            // 
            this.pictureKimlik.Location = new System.Drawing.Point(23, 41);
            this.pictureKimlik.Name = "pictureKimlik";
            this.pictureKimlik.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureKimlik.Size = new System.Drawing.Size(100, 96);
            this.pictureKimlik.TabIndex = 1;
            // 
            // pictureFoto
            // 
            this.pictureFoto.Location = new System.Drawing.Point(129, 41);
            this.pictureFoto.Name = "pictureFoto";
            this.pictureFoto.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureFoto.Size = new System.Drawing.Size(100, 96);
            this.pictureFoto.TabIndex = 2;
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(23, 144);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(100, 20);
            this.textEdit1.TabIndex = 3;
            // 
            // FrmKimlikTarama
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 268);
            this.Controls.Add(this.textEdit1);
            this.Controls.Add(this.pictureFoto);
            this.Controls.Add(this.pictureKimlik);
            this.Controls.Add(this.comboBoxBelge);
            this.Name = "FrmKimlikTarama";
            this.Text = "XtraForm1";
            this.Load += new System.EventHandler(this.FrmKimlikTarama_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureKimlik.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureFoto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxBelge;
        private DevExpress.XtraEditors.PictureEdit pictureKimlik;
        private DevExpress.XtraEditors.PictureEdit pictureFoto;
        private DevExpress.XtraEditors.TextEdit textEdit1;
    }
}