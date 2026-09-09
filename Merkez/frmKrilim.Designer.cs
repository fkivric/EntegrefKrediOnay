namespace EntegrefKrediOnay.Merkez
{
    partial class frmKrilim
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
            this.gridKirilmlar = new DevExpress.XtraGrid.GridControl();
            this.ViewKirilmlar = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridKirilmlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewKirilmlar)).BeginInit();
            this.SuspendLayout();
            // 
            // gridKirilmlar
            // 
            this.gridKirilmlar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridKirilmlar.Location = new System.Drawing.Point(0, 0);
            this.gridKirilmlar.MainView = this.ViewKirilmlar;
            this.gridKirilmlar.Name = "gridKirilmlar";
            this.gridKirilmlar.Size = new System.Drawing.Size(898, 465);
            this.gridKirilmlar.TabIndex = 0;
            this.gridKirilmlar.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ViewKirilmlar});
            // 
            // ViewKirilmlar
            // 
            this.ViewKirilmlar.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.ViewKirilmlar.GridControl = this.gridKirilmlar;
            this.ViewKirilmlar.Name = "ViewKirilmlar";
            this.ViewKirilmlar.OptionsView.ShowAutoFilterRow = true;
            this.ViewKirilmlar.OptionsView.ShowGroupPanel = false;
            this.ViewKirilmlar.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.ViewKirilmlar_RowCellClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Kırılım Kodu";
            this.gridColumn1.FieldName = "WPTREVAL";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Kırılım Adı";
            this.gridColumn2.FieldName = "WPTRENAME";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // frmKrilim
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 465);
            this.Controls.Add(this.gridKirilmlar);
            this.Name = "frmKrilim";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stok Krilimları";
            this.Load += new System.EventHandler(this.frmKrilim_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridKirilmlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewKirilmlar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridKirilmlar;
        private DevExpress.XtraGrid.Views.Grid.GridView ViewKirilmlar;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
    }
}