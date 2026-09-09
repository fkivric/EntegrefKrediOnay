namespace EntegrefKrediOnay.Magaza
{
    partial class frmMusteriSec
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
            this.gridStoklar = new DevExpress.XtraGrid.GridControl();
            this.ViewStoklar = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.toggleSwitch1 = new DevExpress.XtraEditors.ToggleSwitch();
            ((System.ComponentModel.ISupportInitialize)(this.gridStoklar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewStoklar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitch1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gridStoklar
            // 
            this.gridStoklar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridStoklar.Location = new System.Drawing.Point(0, 52);
            this.gridStoklar.MainView = this.ViewStoklar;
            this.gridStoklar.Name = "gridStoklar";
            this.gridStoklar.Size = new System.Drawing.Size(920, 585);
            this.gridStoklar.TabIndex = 0;
            this.gridStoklar.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ViewStoklar});
            this.gridStoklar.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridStoklar_EditorKeyDown);
            // 
            // ViewStoklar
            // 
            this.ViewStoklar.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.ViewStoklar.GridControl = this.gridStoklar;
            this.ViewStoklar.Name = "ViewStoklar";
            this.ViewStoklar.OptionsView.ColumnAutoWidth = false;
            this.ViewStoklar.OptionsView.ShowAutoFilterRow = true;
            this.ViewStoklar.OptionsView.ShowGroupPanel = false;
            this.ViewStoklar.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.ViewStoklar_RowClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "CURID";
            this.gridColumn1.FieldName = "CURID";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Müşteri Kodu";
            this.gridColumn2.FieldName = "CURVAL";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Müşteri Adı";
            this.gridColumn3.FieldName = "CURNAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsColumn.ReadOnly = true;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Son Alışveriş Tutarı";
            this.gridColumn4.FieldName = "SALAMOUNT";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.OptionsColumn.ReadOnly = true;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Son Alışveriş Tarihi";
            this.gridColumn5.FieldName = "SALDATE";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.OptionsColumn.ReadOnly = true;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.toggleSwitch1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(920, 52);
            this.groupControl1.TabIndex = 1;
            this.groupControl1.Text = "Müşteri Tipi";
            // 
            // toggleSwitch1
            // 
            this.toggleSwitch1.Location = new System.Drawing.Point(12, 26);
            this.toggleSwitch1.Name = "toggleSwitch1";
            this.toggleSwitch1.Properties.OffText = "Kimlik Verisi Olmayan";
            this.toggleSwitch1.Properties.OnText = "Kimlik Taraması Olan";
            this.toggleSwitch1.Size = new System.Drawing.Size(160, 18);
            this.toggleSwitch1.TabIndex = 0;
            // 
            // frmMusteriSec
            // 
            this.Appearance.Options.UseTextOptions = true;
            this.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 637);
            this.Controls.Add(this.gridStoklar);
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Image = global::EntegrefKrediOnay.Properties.Resources.Entegref__1_;
            this.Name = "frmMusteriSec";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stok Bul";
            this.Load += new System.EventHandler(this.frmStokBul_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridStoklar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewStoklar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitch1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridStoklar;
        private DevExpress.XtraGrid.Views.Grid.GridView ViewStoklar;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.ToggleSwitch toggleSwitch1;
    }
}