namespace EntegrefKrediOnay.Merkez
{
    partial class frmBGOnayRaporu
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
            DevExpress.XtraEditors.TileItemElement tileItemElement1 = new DevExpress.XtraEditors.TileItemElement();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.tileBar1 = new DevExpress.XtraBars.Navigation.TileBar();
            this.tileBarGroup2 = new DevExpress.XtraBars.Navigation.TileBarGroup();
            this.tileBarItem1 = new DevExpress.XtraBars.Navigation.TileBarItem();
            this.chklistInvestigation = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.gridOnayRapor = new DevExpress.XtraGrid.GridControl();
            this.ViewOnayRapor = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cntİslemler = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.müşteriİşlemDetayıGörToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chklistInvestigation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOnayRapor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewOnayRapor)).BeginInit();
            this.cntİslemler.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.tileBar1);
            this.groupControl1.Controls.Add(this.chklistInvestigation);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.GroupStyle = DevExpress.Utils.GroupStyle.Light;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(1266, 144);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Rapor Filitresi";
            // 
            // tileBar1
            // 
            this.tileBar1.AppearanceItem.Normal.BackColor = System.Drawing.Color.Black;
            this.tileBar1.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.MidnightBlue;
            this.tileBar1.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileBar1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            this.tileBar1.Groups.Add(this.tileBarGroup2);
            this.tileBar1.Location = new System.Drawing.Point(896, 23);
            this.tileBar1.MaxId = 1;
            this.tileBar1.Name = "tileBar1";
            this.tileBar1.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
            this.tileBar1.Size = new System.Drawing.Size(368, 119);
            this.tileBar1.TabIndex = 2;
            this.tileBar1.Text = "tileBar1";
            // 
            // tileBarGroup2
            // 
            this.tileBarGroup2.Items.Add(this.tileBarItem1);
            this.tileBarGroup2.Name = "tileBarGroup2";
            // 
            // tileBarItem1
            // 
            this.tileBarItem1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            tileItemElement1.ImageOptions.Image = global::EntegrefKrediOnay.Properties.Resources._3dcylinder_32x32;
            tileItemElement1.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            tileItemElement1.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Top;
            tileItemElement1.Text = "Listele";
            this.tileBarItem1.Elements.Add(tileItemElement1);
            this.tileBarItem1.Id = 0;
            this.tileBarItem1.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
            this.tileBarItem1.Name = "tileBarItem1";
            this.tileBarItem1.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBarItem1_ItemClick);
            // 
            // chklistInvestigation
            // 
            this.chklistInvestigation.Dock = System.Windows.Forms.DockStyle.Left;
            this.chklistInvestigation.HorizontalScrollbar = true;
            this.chklistInvestigation.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
            this.chklistInvestigation.Location = new System.Drawing.Point(2, 23);
            this.chklistInvestigation.Name = "chklistInvestigation";
            this.chklistInvestigation.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.chklistInvestigation.Size = new System.Drawing.Size(894, 119);
            this.chklistInvestigation.TabIndex = 1;
            // 
            // gridOnayRapor
            // 
            this.gridOnayRapor.ContextMenuStrip = this.cntİslemler;
            this.gridOnayRapor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOnayRapor.Location = new System.Drawing.Point(0, 144);
            this.gridOnayRapor.MainView = this.ViewOnayRapor;
            this.gridOnayRapor.Name = "gridOnayRapor";
            this.gridOnayRapor.Size = new System.Drawing.Size(1266, 484);
            this.gridOnayRapor.TabIndex = 1;
            this.gridOnayRapor.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ViewOnayRapor});
            // 
            // ViewOnayRapor
            // 
            this.ViewOnayRapor.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn11,
            this.gridColumn10,
            this.gridColumn9,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn12,
            this.gridColumn13});
            this.ViewOnayRapor.GridControl = this.gridOnayRapor;
            this.ViewOnayRapor.GroupCount = 2;
            this.ViewOnayRapor.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SALAMOUNT", this.gridColumn2, "{0:0.##}")});
            this.ViewOnayRapor.Name = "ViewOnayRapor";
            this.ViewOnayRapor.OptionsCustomization.AllowFilter = false;
            this.ViewOnayRapor.OptionsView.ColumnAutoWidth = false;
            this.ViewOnayRapor.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.ViewOnayRapor.OptionsView.ShowFooter = true;
            this.ViewOnayRapor.OptionsView.ShowGroupPanel = false;
            this.ViewOnayRapor.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn10, DevExpress.Data.ColumnSortOrder.Ascending),
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn4, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.ViewOnayRapor.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.ViewOnayRapor_RowCellClick);
            this.ViewOnayRapor.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.ViewOnayRapor_RowStyle);
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Mağaza Kodu";
            this.gridColumn11.FieldName = "DIVVAL";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsFilter.AllowFilter = false;
            this.gridColumn11.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Mağaza Adı";
            this.gridColumn10.FieldName = "DIVNAME";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.OptionsColumn.ReadOnly = true;
            this.gridColumn10.OptionsFilter.AllowFilter = false;
            this.gridColumn10.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 0;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Müşteri";
            this.gridColumn9.FieldName = "CURVAL";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.ReadOnly = true;
            this.gridColumn9.OptionsFilter.AllowFilter = false;
            this.gridColumn9.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn9.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "CURVAL", "{0}")});
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 0;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Müşteri Adı";
            this.gridColumn1.FieldName = "CURNAME";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            this.gridColumn1.OptionsFilter.AllowFilter = false;
            this.gridColumn1.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Satış Tutarı";
            this.gridColumn2.FieldName = "SALAMOUNT";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.OptionsFilter.AllowFilter = false;
            this.gridColumn2.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn2.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SALAMOUNT", "{0:0.##}")});
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Onay Tarihi";
            this.gridColumn3.FieldName = "INVESDATE";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.ReadOnly = true;
            this.gridColumn3.OptionsFilter.AllowFilter = false;
            this.gridColumn3.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "İşlem Durumu";
            this.gridColumn4.FieldName = "DINSNAME";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.ReadOnly = true;
            this.gridColumn4.OptionsFilter.AllowFilter = false;
            this.gridColumn4.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Onay Notu";
            this.gridColumn5.FieldName = "INVESSALNOTES";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.ReadOnly = true;
            this.gridColumn5.OptionsFilter.AllowFilter = false;
            this.gridColumn5.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Müşteri Risk Puanı";
            this.gridColumn6.FieldName = "INVESRISK";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.ReadOnly = true;
            this.gridColumn6.OptionsFilter.AllowFilter = false;
            this.gridColumn6.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Müşteri Onaydaki Gecikmesi";
            this.gridColumn7.FieldName = "INVESCURLATE";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.ReadOnly = true;
            this.gridColumn7.OptionsFilter.AllowFilter = false;
            this.gridColumn7.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn7.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "INVESCURLATE", "{0:0.##}")});
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Kefil Adı";
            this.gridColumn8.FieldName = "INVESWARANTER";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.ReadOnly = true;
            this.gridColumn8.OptionsFilter.AllowFilter = false;
            this.gridColumn8.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "gridColumn12";
            this.gridColumn12.FieldName = "INVESCURID";
            this.gridColumn12.Name = "gridColumn12";
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "gridColumn13";
            this.gridColumn13.FieldName = "INVESSALID";
            this.gridColumn13.Name = "gridColumn13";
            // 
            // cntİslemler
            // 
            this.cntİslemler.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.müşteriİşlemDetayıGörToolStripMenuItem});
            this.cntİslemler.Name = "cntİslemler";
            this.cntİslemler.Size = new System.Drawing.Size(204, 26);
            // 
            // müşteriİşlemDetayıGörToolStripMenuItem
            // 
            this.müşteriİşlemDetayıGörToolStripMenuItem.Name = "müşteriİşlemDetayıGörToolStripMenuItem";
            this.müşteriİşlemDetayıGörToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.müşteriİşlemDetayıGörToolStripMenuItem.Text = "Müşteri İşlem Detayı Gör";
            this.müşteriİşlemDetayıGörToolStripMenuItem.Click += new System.EventHandler(this.müşteriİşlemDetayıGörToolStripMenuItem_Click);
            // 
            // frmBGOnayRaporu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1266, 628);
            this.ControlBox = false;
            this.Controls.Add(this.gridOnayRapor);
            this.Controls.Add(this.groupControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBGOnayRaporu";
            this.Text = "Onay Durum Raporu";
            this.Load += new System.EventHandler(this.frmBGOnayRaporu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chklistInvestigation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridOnayRapor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewOnayRapor)).EndInit();
            this.cntİslemler.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.CheckedListBoxControl chklistInvestigation;
        private DevExpress.XtraGrid.GridControl gridOnayRapor;
        private DevExpress.XtraGrid.Views.Grid.GridView ViewOnayRapor;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraBars.Navigation.TileBar tileBar1;
        private DevExpress.XtraBars.Navigation.TileBarGroup tileBarGroup2;
        private DevExpress.XtraBars.Navigation.TileBarItem tileBarItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private System.Windows.Forms.ContextMenuStrip cntİslemler;
        private System.Windows.Forms.ToolStripMenuItem müşteriİşlemDetayıGörToolStripMenuItem;
    }
}