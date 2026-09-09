using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace EntegrefKrediOnay.Class.Tema
{
    public class GridLayoutManager
    {
        #region Fields

        private GridControl _grid;
        private GridView _view;

        private string _userId;
        private string _FormName;
        private string _connectionString;

        private ContextMenuStrip _cms;

        private ToolStripMenuItem _menuThemes;
        private ToolStripMenuItem _menuSave;
        private ToolStripMenuItem _menuSaveAs;
        private ToolStripMenuItem _menuDelete;
        private ToolStripMenuItem _menuDefault;
        private ToolStripMenuItem _menuReset;
        private ToolStripMenuItem _menuColumn;
        private ToolStripMenuItem _menuBestFit;
        private ToolStripMenuItem _menuClearFilter;
        private ToolStripMenuItem _menuClearGroup;
        private ToolStripMenuItem _menuClearSort;
        

        private string _defaultLayoutXml = "";

        #endregion

        #region Constructor

        public GridLayoutManager(
            GridControl grid,
            GridView view,
            string userId,
            string FormName,
            string connectionString)
        {
            _grid = grid;
            _view = view;
            _userId = userId;
            _FormName = FormName;
            _connectionString = connectionString;

            SaveDefaultLayout();

            CreateContextMenu();

            LoadThemes();
        }

        #endregion

        #region Create Menu

        private void CreateContextMenu()
        {
            _cms = new ContextMenuStrip();

            _menuThemes = new ToolStripMenuItem("Temalar");

            _menuSave = new ToolStripMenuItem("Kaydet");
            _menuSaveAs = new ToolStripMenuItem("Farklı Kaydet");
            _menuDelete = new ToolStripMenuItem("Temayı Sil");
            _menuDefault = new ToolStripMenuItem("Varsayılan Olarak Ayarla");

            _menuReset = new ToolStripMenuItem("Varsayılana Dön");

            _menuColumn = new ToolStripMenuItem("Kolon Seçici");

            _menuBestFit = new ToolStripMenuItem("Kolonları Otomatik Boyutlandır");

            _menuClearFilter = new ToolStripMenuItem("Filtreleri Temizle");

            _menuClearGroup = new ToolStripMenuItem("Gruplamayı Temizle");

            _menuClearSort = new ToolStripMenuItem("Sıralamayı Temizle");

            _cms.Items.Add(_menuThemes);

            _cms.Items.Add(new ToolStripSeparator());

            _cms.Items.Add(_menuSave);
            _cms.Items.Add(_menuSaveAs);
            _cms.Items.Add(_menuDelete);
            _cms.Items.Add(_menuDefault);

            _cms.Items.Add(new ToolStripSeparator());

            _cms.Items.Add(_menuReset);

            _cms.Items.Add(new ToolStripSeparator());

            _cms.Items.Add(_menuColumn);
            _cms.Items.Add(_menuBestFit);
            _cms.Items.Add(_menuClearFilter);
            _cms.Items.Add(_menuClearGroup);
            _cms.Items.Add(_menuClearSort);

            _grid.ContextMenuStrip = _cms;

            #region Events

            _menuSave.Click += MenuSave_Click;
            _menuSaveAs.Click += MenuSaveAs_Click;
            _menuDelete.Click += MenuDelete_Click;
            _menuReset.Click += MenuReset_Click;
            _menuDefault.Click += MenuDefault_Click;

            _menuColumn.Click += MenuColumn_Click;
            _menuBestFit.Click += MenuBestFit_Click;
            _menuClearFilter.Click += MenuClearFilter_Click;
            _menuClearGroup.Click += MenuClearGroup_Click;
            _menuClearSort.Click += MenuClearSort_Click;

            #endregion
        }

        #endregion

        #region Theme Load

        private void LoadThemes()
        {
            _menuThemes.DropDownItems.Clear();

            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT
ID,
LAYOUTNAME
FROM EntegreF..USER_GRID_LAYOUT
WHERE USERID=@USERID
AND GRIDNAME=@GRIDNAME
AND FORMNAME = @FORMNAME
ORDER BY LAYOUTNAME", con);

                cmd.Parameters.AddWithValue("@USERID", _userId);
                cmd.Parameters.AddWithValue("@GRIDNAME", _grid.Name);
                cmd.Parameters.AddWithValue("@FORMNAME", _FormName);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ToolStripMenuItem item =
                        new ToolStripMenuItem();

                    item.Text = dr["LAYOUTNAME"].ToString();

                    item.Tag = dr["ID"];

                    item.Click += Theme_Click;

                    _menuThemes.DropDownItems.Add(item);
                }
            }
        }
        public void LoadDefault()
        {
            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlDataAdapter da =
                    new SqlDataAdapter(@"
select D.* from EntegreF..USER_GRID_LAYOUT L
inner join EntegreF..USER_GRID_LAYOUT_DETAIL D on LAYOUTID = L.ID
where ISDEFAULT = 1 
AND FORMNAME = @FORMNAME 
AND GRIDNAME = @GRIDNAME
AND USERID = @USERID", con);

                da.SelectCommand.Parameters.AddWithValue("@FORMNAME", _FormName);
                da.SelectCommand.Parameters.AddWithValue("@GRIDNAME", _grid.Name);
                da.SelectCommand.Parameters.AddWithValue("@USERID", _userId);

                DataTable dt = new DataTable();

                da.Fill(dt);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {

                        _view.BeginUpdate();

                        try
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                string fieldName =
                                    dr["COLUMNFIELDNAME"].ToString();

                                GridColumn col =
                                    _view.Columns.ColumnByFieldName(fieldName);

                                if (col == null)
                                    continue;

                                col.Visible =
                                    Convert.ToBoolean(dr["VISIBLE"]);

                                col.VisibleIndex =
                                    Convert.ToInt32(dr["VISIBLEINDEX"]);

                                col.Width =
                                    Convert.ToInt32(dr["WIDTH"]);
                            }
                        }
                        finally
                        {
                            _view.EndUpdate();
                        }
                    }
                }
            }
        }
        #endregion

        #region Theme Click

        private void Theme_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item =
                sender as ToolStripMenuItem;

            if (item == null)
                return;

            int layoutId =
                Convert.ToInt32(item.Tag);

            LoadLayout(layoutId);
        }

        #endregion

        #region Save Layout

        private void SaveLayout(string layoutName)
        {
            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
INSERT INTO EntegreF..USER_GRID_LAYOUT
(
USERID,
LAYOUTNAME,
FORMNAME,
GRIDNAME
)
OUTPUT INSERTED.ID
VALUES
(
@USERID,
@LAYOUTNAME,
@FORMNAME,
@GRIDNAME
)", con);

                cmd.Parameters.AddWithValue("@USERID", _userId);
                cmd.Parameters.AddWithValue("@LAYOUTNAME", layoutName);
                cmd.Parameters.AddWithValue("@GRIDNAME", _grid.Name);
                cmd.Parameters.AddWithValue("@FORMNAME", _FormName);

                int layoutId =
                    Convert.ToInt32(cmd.ExecuteScalar());

                foreach (GridColumn col in _view.Columns)
                {
                    SqlCommand cmdDet =
                        new SqlCommand(@"
INSERT INTO EntegreF..USER_GRID_LAYOUT_DETAIL
(
LAYOUTID,
COLUMNFIELDNAME,
VISIBLE,
VISIBLEINDEX,
WIDTH
)
VALUES
(
@LAYOUTID,
@COLUMNFIELDNAME,
@VISIBLE,
@VISIBLEINDEX,
@WIDTH
)", con);

                    cmdDet.Parameters.AddWithValue("@LAYOUTID", layoutId);

                    cmdDet.Parameters.AddWithValue(
                        "@COLUMNFIELDNAME",
                        col.FieldName);

                    cmdDet.Parameters.AddWithValue(
                        "@VISIBLE",
                        col.Visible);

                    cmdDet.Parameters.AddWithValue(
                        "@VISIBLEINDEX",
                        col.VisibleIndex);

                    cmdDet.Parameters.AddWithValue(
                        "@WIDTH",
                        col.Width);

                    cmdDet.ExecuteNonQuery();
                }
            }

            LoadThemes();

            XtraMessageBox.Show(
                "Tema kaydedildi.");
        }
        private void SaveLayoutDefault()
        {
            if (_menuThemes.DropDownItems.Count == 0)
            {
                XtraMessageBox.Show("Kayıtlı tema yok.");
                return;
            }

            string temaAdi = SelectTheme();

            if (string.IsNullOrWhiteSpace(temaAdi))
                return;

            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmdReset =
                    new SqlCommand(@"
UPDATE EntegreF..USER_GRID_LAYOUT
SET ISDEFAULT=0
WHERE USERID=@USERID
AND FORMNAME = @FORMNAME
AND GRIDNAME=@GRIDNAME", con);

                cmdReset.Parameters.AddWithValue(
                    "@USERID",
                    _userId);

                cmdReset.Parameters.AddWithValue(
                    "FORMNAME",
                    _FormName
                    );

                cmdReset.Parameters.AddWithValue(
                    "@GRIDNAME",
                    _grid.Name);

                cmdReset.ExecuteNonQuery();


                SqlCommand cmdFind =
                    new SqlCommand(@"
SELECT ID
FROM EntegreF..USER_GRID_LAYOUT
WHERE USERID=@USERID
AND GRIDNAME=@GRIDNAME
AND FORMNAME = @FORMNAME
AND LAYOUTNAME=@LAYOUTNAME", con);

                cmdFind.Parameters.AddWithValue("@USERID", _userId);
                cmdFind.Parameters.AddWithValue("@GRIDNAME", _grid.Name);
                cmdFind.Parameters.AddWithValue("@LAYOUTNAME", temaAdi);
                cmdFind.Parameters.AddWithValue("@FORMNAME", _FormName);
                object obj = cmdFind.ExecuteScalar();

                if (obj == null)
                {
                    XtraMessageBox.Show("Tema bulunamadı.");
                    return;
                }

                int layoutId = Convert.ToInt32(obj);                

                SqlCommand cmdMain =
                    new SqlCommand(@"UPDATE EntegreF..USER_GRID_LAYOUT set ISDEFAULT = 1 WHERE ID = @ID", con);

                cmdMain.Parameters.AddWithValue("@ID", layoutId);

                cmdMain.ExecuteNonQuery();
            }

            LoadThemes();

            XtraMessageBox.Show("Varsayılan Tema Olarak Ayarlandı.");
        }
        private string SelectTheme()
        {
            ComboBoxEdit combo = new ComboBoxEdit();

            foreach (ToolStripItem item in _menuThemes.DropDownItems)
            {
                ToolStripMenuItem menuItem =
                    item as ToolStripMenuItem;

                if (menuItem != null)
                {
                    combo.Properties.Items.Add(menuItem.Text);
                }
            }

            combo.SelectedIndex = 0;

            XtraInputBoxArgs args =
                new XtraInputBoxArgs();

            args.Caption = "Tema Seç";
            args.Prompt = "Varsayılan tema:";
            args.DefaultButtonIndex = 0;

            args.Editor = combo;

            object result =
                XtraInputBox.Show(args);

            if (result == null)
                return "";

            return result.ToString();
        }
        #endregion

        #region Load Layout

        private void LoadLayout(int layoutId)
        {
            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlDataAdapter da =
                    new SqlDataAdapter(@"
SELECT *
FROM EntegreF..USER_GRID_LAYOUT_DETAIL
WHERE LAYOUTID=@LAYOUTID", con);

                da.SelectCommand.Parameters.AddWithValue(
                    "@LAYOUTID",
                    layoutId);

                DataTable dt = new DataTable();

                da.Fill(dt);

                _view.BeginUpdate();

                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string fieldName =
                            dr["COLUMNFIELDNAME"].ToString();

                        GridColumn col =
                            _view.Columns.ColumnByFieldName(fieldName);

                        if (col == null)
                            continue;

                        col.Visible =
                            Convert.ToBoolean(dr["VISIBLE"]);

                        col.VisibleIndex =
                            Convert.ToInt32(dr["VISIBLEINDEX"]);

                        col.Width =
                            Convert.ToInt32(dr["WIDTH"]);
                    }
                }
                finally
                {
                    _view.EndUpdate();
                }
            }
        }

        #endregion

        #region Delete Theme

        private void DeleteTheme()
        {
            if (_menuThemes.DropDownItems.Count == 0)
            {
                XtraMessageBox.Show("Kayıtlı tema yok.");
                return;
            }

            string temaAdi = SelectTheme();

            if (string.IsNullOrWhiteSpace(temaAdi))
                return;

            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmdFind =
                    new SqlCommand(@"
SELECT ID
FROM EntegreF..USER_GRID_LAYOUT
WHERE USERID=@USERID
AND GRIDNAME=@GRIDNAME
AND FORMNAME = @FORMNAME
AND LAYOUTNAME=@LAYOUTNAME", con);

                cmdFind.Parameters.AddWithValue("@USERID", _userId);
                cmdFind.Parameters.AddWithValue("@GRIDNAME", _grid.Name);
                cmdFind.Parameters.AddWithValue("@LAYOUTNAME", temaAdi);
                cmdFind.Parameters.AddWithValue("@FORMNAME", _FormName);
                object obj = cmdFind.ExecuteScalar();

                if (obj == null)
                {
                    XtraMessageBox.Show("Tema bulunamadı.");
                    return;
                }

                int layoutId = Convert.ToInt32(obj);

                SqlCommand cmdDet =
                    new SqlCommand(@"
DELETE FROM EntegreF..USER_GRID_LAYOUT_DETAIL
WHERE LAYOUTID=@LAYOUTID", con);

                cmdDet.Parameters.AddWithValue(
                    "@LAYOUTID",
                    layoutId);

                cmdDet.ExecuteNonQuery();

                SqlCommand cmdMain =
                    new SqlCommand(@"
DELETE FROM EntegreF..USER_GRID_LAYOUT
WHERE ID=@ID", con);

                cmdMain.Parameters.AddWithValue("@ID", layoutId);

                cmdMain.ExecuteNonQuery();
            }

            LoadThemes();

            XtraMessageBox.Show("Tema silindi.");
        }

        #endregion

        #region Default Layout

        private void SaveDefaultLayout()
        {
            using (MemoryStream ms =
                new MemoryStream())
            {
                _view.SaveLayoutToStream(ms);

                _defaultLayoutXml =
                    Convert.ToBase64String(ms.ToArray());
            }
        }

        private void ResetLayout()
        {
            byte[] data =
                Convert.FromBase64String(_defaultLayoutXml);

            using (MemoryStream ms =
                new MemoryStream(data))
            {
                _view.RestoreLayoutFromStream(ms);
            }
        }

        #endregion

        #region Menu Events

        private void MenuSave_Click(object sender, EventArgs e)
        {
            string temaAdi =
                XtraInputBox.Show(
                    "Tema adı:",
                    "Tema Kaydet",
                    "");

            if (string.IsNullOrWhiteSpace(temaAdi))
                return;

            SaveLayout(temaAdi);
        }

        private void MenuSaveAs_Click(object sender, EventArgs e)
        {
            string temaAdi =
                XtraInputBox.Show(
                    "Yeni tema adı:",
                    "Farklı Kaydet",
                    "");

            if (string.IsNullOrWhiteSpace(temaAdi))
                return;

            SaveLayout(temaAdi);
        }

        private void MenuDelete_Click(object sender, EventArgs e)
        {
            DeleteTheme();
        }

        private void MenuDefault_Click(object sender, EventArgs e)
        {
            SaveLayoutDefault();
        }

        private void MenuReset_Click(object sender, EventArgs e)
        {
            ResetLayout();
        }

        private void MenuColumn_Click(object sender, EventArgs e)
        {
            _view.ColumnsCustomization();
        }

        private void MenuBestFit_Click(object sender, EventArgs e)
        {
            _view.BestFitColumns();
        }

        private void MenuClearFilter_Click(object sender, EventArgs e)
        {
            _view.ActiveFilter.Clear();
        }

        private void MenuClearGroup_Click(object sender, EventArgs e)
        {
            _view.ClearGrouping();
        }

        private void MenuClearSort_Click(object sender, EventArgs e)
        {
            _view.ClearSorting();
        }

        #endregion
    }
}