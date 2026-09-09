namespace EntegrefKrediOnay.Merkez
{
    partial class frmBGAIParametre
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
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            this.tileBar1 = new DevExpress.XtraBars.Navigation.TileBar();
            this.tileBarGroup2 = new DevExpress.XtraBars.Navigation.TileBarGroup();
            this.tileBarItem1 = new DevExpress.XtraBars.Navigation.TileBarItem();
            this.tileBarItem2 = new DevExpress.XtraBars.Navigation.TileBarItem();
            this.gridAIParametre = new DevExpress.XtraGrid.GridControl();
            this.ViewAIParametre = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.toggleSwitchAktif = new DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridAIParametre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewAIParametre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitchAktif)).BeginInit();
            this.SuspendLayout();
            // 
            // tileBar1
            // 
            this.tileBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tileBar1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            this.tileBar1.Groups.Add(this.tileBarGroup2);
            this.tileBar1.ItemSize = 40;
            this.tileBar1.Location = new System.Drawing.Point(0, 606);
            this.tileBar1.MaxId = 2;
            this.tileBar1.Name = "tileBar1";
            this.tileBar1.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
            this.tileBar1.Size = new System.Drawing.Size(1136, 79);
            this.tileBar1.TabIndex = 0;
            this.tileBar1.Text = "tileBar1";
            // 
            // tileBarGroup2
            // 
            this.tileBarGroup2.Items.Add(this.tileBarItem1);
            this.tileBarGroup2.Items.Add(this.tileBarItem2);
            this.tileBarGroup2.Name = "tileBarGroup2";
            // 
            // tileBarItem1
            // 
            this.tileBarItem1.AppearanceItem.Normal.BackColor = System.Drawing.Color.DarkBlue;
            this.tileBarItem1.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.Black;
            this.tileBarItem1.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileBarItem1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            tileItemElement1.Text = "Kaydet";
            this.tileBarItem1.Elements.Add(tileItemElement1);
            this.tileBarItem1.Id = 0;
            this.tileBarItem1.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
            this.tileBarItem1.Name = "tileBarItem1";
            // 
            // tileBarItem2
            // 
            this.tileBarItem2.AppearanceItem.Normal.BackColor = System.Drawing.Color.DarkBlue;
            this.tileBarItem2.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.Black;
            this.tileBarItem2.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileBarItem2.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            tileItemElement2.Text = "Sıfırla";
            this.tileBarItem2.Elements.Add(tileItemElement2);
            this.tileBarItem2.Id = 1;
            this.tileBarItem2.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
            this.tileBarItem2.Name = "tileBarItem2";
            // 
            // gridAIParametre
            // 
            this.gridAIParametre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAIParametre.Location = new System.Drawing.Point(0, 0);
            this.gridAIParametre.MainView = this.ViewAIParametre;
            this.gridAIParametre.Name = "gridAIParametre";
            this.gridAIParametre.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.toggleSwitchAktif});
            this.gridAIParametre.Size = new System.Drawing.Size(1136, 606);
            this.gridAIParametre.TabIndex = 1;
            this.gridAIParametre.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ViewAIParametre});
            // 
            // ViewAIParametre
            // 
            this.ViewAIParametre.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn7,
            this.gridColumn5,
            this.gridColumn6});
            this.ViewAIParametre.GridControl = this.gridAIParametre;
            this.ViewAIParametre.GroupCount = 1;
            this.ViewAIParametre.Name = "ViewAIParametre";
            this.ViewAIParametre.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full;
            this.ViewAIParametre.OptionsView.ColumnAutoWidth = false;
            this.ViewAIParametre.OptionsView.ShowGroupPanel = false;
            this.ViewAIParametre.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn6, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.FieldName = "AIROW_ID";
            this.gridColumn1.Name = "gridColumn1";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Parametre";
            this.gridColumn2.FieldName = "AIDEF_VAL";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Açıklama";
            this.gridColumn3.FieldName = "AIDEF_NAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsColumn.ReadOnly = true;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Parametre Değeri";
            this.gridColumn4.FieldName = "AIDEF_VALUE";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Puan";
            this.gridColumn7.FieldName = "AIDEF_RESULT";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.OptionsColumn.ReadOnly = true;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 4;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Aktif";
            this.gridColumn5.ColumnEdit = this.toggleSwitchAktif;
            this.gridColumn5.FieldName = "AIDEF_BOOL";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // toggleSwitchAktif
            // 
            this.toggleSwitchAktif.AutoHeight = false;
            this.toggleSwitchAktif.Name = "toggleSwitchAktif";
            this.toggleSwitchAktif.OffText = "Kapalı";
            this.toggleSwitchAktif.OnText = "Açık";
            this.toggleSwitchAktif.EditValueChanged += new System.EventHandler(this.toggleSwitchAktif_EditValueChanged);
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Kategori Adı";
            this.gridColumn6.FieldName = "AIDEF_GROUP";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.OptionsColumn.ReadOnly = true;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            // 
            // frmBGAIParametre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 685);
            this.Controls.Add(this.gridAIParametre);
            this.Controls.Add(this.tileBar1);
            this.Name = "frmBGAIParametre";
            this.Text = "AI Parametre Belirleme";
            this.Load += new System.EventHandler(this.frmAIParametre_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridAIParametre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ViewAIParametre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSwitchAktif)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Navigation.TileBar tileBar1;
        private DevExpress.XtraBars.Navigation.TileBarGroup tileBarGroup2;
        private DevExpress.XtraBars.Navigation.TileBarItem tileBarItem1;
        private DevExpress.XtraBars.Navigation.TileBarItem tileBarItem2;
        private DevExpress.XtraGrid.GridControl gridAIParametre;
        private DevExpress.XtraGrid.Views.Grid.GridView ViewAIParametre;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch toggleSwitchAktif;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
    }
}