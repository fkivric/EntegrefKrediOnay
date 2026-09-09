using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid;

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace EntegrefKrediOnay.Class.Tema
{
    public class PivotLayoutManager
    {
        #region Fields

        private PivotGridControl _pivot;

        private string _userId;

        private string _connectionString;

        private string _FormName;

        private ContextMenuStrip _cms;

        private ToolStripMenuItem _menuThemes;

        private ToolStripMenuItem _menuSave;

        private ToolStripMenuItem _menuSaveAs;

        private ToolStripMenuItem _menuDelete;

        private ToolStripMenuItem _menuDefault;

        private ToolStripMenuItem _menuReset;

        private ToolStripMenuItem _menuFieldList;

        private ToolStripMenuItem _menuBestFit;

        private ToolStripMenuItem _menuExportExcel;

        private byte[] _defaultLayout;

        #endregion

        #region Constructor

        public PivotLayoutManager(
            PivotGridControl pivot,
            string userId,
            string Formname,
            string connectionString)
        {
            _pivot = pivot;

            _userId = userId;

            _FormName = Formname;

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

            _menuThemes =
                new ToolStripMenuItem("Temalar");

            _menuSave =
                new ToolStripMenuItem("Kaydet");

            _menuSaveAs =
                new ToolStripMenuItem("Farklı Kaydet");

            _menuDelete =
                new ToolStripMenuItem("Tema Sil");

            _menuDefault =
                new ToolStripMenuItem("Varsayılan Yap");

            _menuReset =
                new ToolStripMenuItem("Varsayılana Dön");

            _menuFieldList =
                new ToolStripMenuItem("Alan Listesi");

            _menuBestFit =
                new ToolStripMenuItem("Kolonları Sığdır");

            _menuExportExcel =
                new ToolStripMenuItem("Excel'e Aktar");

            _cms.Items.Add(_menuThemes);

            _cms.Items.Add(new ToolStripSeparator());

            _cms.Items.Add(_menuSave);

            _cms.Items.Add(_menuSaveAs);

            _cms.Items.Add(_menuDelete);

            _cms.Items.Add(_menuDefault);

            _cms.Items.Add(_menuReset);

            _cms.Items.Add(new ToolStripSeparator());

            _cms.Items.Add(_menuFieldList);

            _cms.Items.Add(_menuBestFit);

            _cms.Items.Add(_menuExportExcel);

            _pivot.ContextMenuStrip = _cms;

            #region Events

            _menuSave.Click += MenuSave_Click;

            _menuSaveAs.Click += MenuSaveAs_Click;

            _menuDelete.Click += MenuDelete_Click;

            _menuDefault.Click += MenuDefault_Click;

            _menuReset.Click += MenuReset_Click;

            _menuFieldList.Click += MenuFieldList_Click;

            _menuBestFit.Click += MenuBestFit_Click;

            _menuExportExcel.Click += MenuExportExcel_Click;

            #endregion
        }

        #endregion

        #region Load Themes

        private void LoadThemes()
        {
            _menuThemes.DropDownItems.Clear();

            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(@"
SELECT
ID,
LAYOUTNAME
FROM EntegreF..USER_PIVOT_LAYOUT
WHERE USERID=@USERID
AND GRIDNAME=@GRIDNAME
AND FORMNAME = @FORMNAME
ORDER BY LAYOUTNAME", con);

                cmd.Parameters.AddWithValue(
                    "@USERID",
                    _userId);

                cmd.Parameters.AddWithValue(
                    "@GRIDNAME",
                    _pivot.Name);

                cmd.Parameters.AddWithValue(
                    "@FORMNAME",
                    _FormName);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    ToolStripMenuItem item =
                        new ToolStripMenuItem();

                    item.Text =
                        dr["LAYOUTNAME"].ToString();

                    item.Tag =
                        dr["ID"];

                    item.Click += Theme_Click;

                    _menuThemes.DropDownItems.Add(item);
                }
            }
        }

        #endregion

        #region Theme Click

        private void Theme_Click(
            object sender,
            EventArgs e)
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

        private void SaveLayout(
            string layoutName,
            bool isDefault = false)
        {
            using (MemoryStream ms =
                new MemoryStream())
            {
                _pivot.SaveLayoutToStream(ms);

                byte[] data =
                    ms.ToArray();

                using (SqlConnection con =
                    new SqlConnection(_connectionString))
                {
                    con.Open();

                    if (isDefault)
                    {
                        SqlCommand cmdReset =
                            new SqlCommand(@"
UPDATE EntegreF..USER_PIVOT_LAYOUT
SET ISDEFAULT=0
WHERE USERID=@USERID
AND FORMNAME = @FORMNAME
AND GRIDNAME=@GRIDNAME", con);

                        cmdReset.Parameters.AddWithValue(
                            "@USERID",
                            _userId);

                        cmdReset.Parameters.AddWithValue(
                            "@GRIDNAME",
                            _pivot.Name);

                        cmdReset.Parameters.AddWithValue(
                            "@FORMNAME",
                            _FormName
                            );

                        cmdReset.ExecuteNonQuery();
                    }

                    SqlCommand cmd =
                        new SqlCommand(@"
INSERT INTO EntegreF..USER_PIVOT_LAYOUT
(
USERID,
LAYOUTNAME,
FORMNAME,
GRIDNAME,
LAYOUTDATA,
ISDEFAULT
)
VALUES
(
@USERID,
@LAYOUTNAME,
@FORMNAME,
@GRIDNAME,
@LAYOUTDATA,
@ISDEFAULT
)", con);

                    cmd.Parameters.AddWithValue(
                        "@USERID",
                        _userId);

                    cmd.Parameters.AddWithValue(
                        "@LAYOUTNAME",
                        layoutName);

                    cmd.Parameters.AddWithValue(
                        "@FORMNAME",
                        _FormName);

                    cmd.Parameters.AddWithValue(
                        "@GRIDNAME",
                        _pivot.Name);

                    cmd.Parameters.AddWithValue(
                        "@LAYOUTDATA",
                        data);

                    cmd.Parameters.AddWithValue(
                        "@ISDEFAULT",
                        isDefault);

                    cmd.ExecuteNonQuery();
                }
            }

            LoadThemes();

            XtraMessageBox.Show(
                "Tema kaydedildi.");
        }

        #endregion

        #region Load Layout

        private void LoadLayout(int layoutId)
        {
            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(@"
SELECT LAYOUTDATA
FROM EntegreF..USER_PIVOT_LAYOUT
WHERE ID=@ID", con);

                cmd.Parameters.AddWithValue(
                    "@ID",
                    layoutId);

                object obj =
                    cmd.ExecuteScalar();

                if (obj == null)
                    return;

                byte[] data =
                    (byte[])obj;

                using (MemoryStream ms =
                    new MemoryStream(data))
                {
                    _pivot.RestoreLayoutFromStream(ms);
                }
            }
        }
        public void LoadDefaultLayout()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
SELECT ID
FROM EntegreF..USER_PIVOT_LAYOUT
WHERE ISDEFAULT=1
AND USERID=@USERID
AND FORMNAME = @FORMNAME
AND GRIDNAME=@GRIDNAME", con);

                cmd.Parameters.AddWithValue("@USERID", _userId);
                cmd.Parameters.AddWithValue("@FORMNAME", _FormName);
                cmd.Parameters.AddWithValue("@GRIDNAME", _pivot.Name);

                object obj =
                   cmd.ExecuteScalar();

                if (obj == null)
                    return;

                int layoutId =
                    Convert.ToInt32(obj);

                LoadLayout(layoutId);
                _pivot.RefreshData();
            }
        }

        #endregion

        #region Delete Theme

        private void DeleteTheme(string themeName)
        {
            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(@"
DELETE FROM EntegreF..USER_PIVOT_LAYOUT
WHERE USERID=@USERID
AND GRIDNAME=@GRIDNAME
AND FORMNAME = @FORMNAME
AND LAYOUTNAME=@LAYOUTNAME", con);

                cmd.Parameters.AddWithValue(
                    "@USERID",
                    _userId);

                cmd.Parameters.AddWithValue(
                    "@GRIDNAME",
                    _pivot.Name);

                cmd.Parameters.AddWithValue(
                    "@FORMNAME",
                    _FormName);

                cmd.Parameters.AddWithValue(
                    "@LAYOUTNAME",
                    themeName);

                cmd.ExecuteNonQuery();
            }

            LoadThemes();

            XtraMessageBox.Show(
                "Tema silindi.");
        }

        #endregion

        #region Default Layout

        private void SaveDefaultLayout()
        {
            using (MemoryStream ms =
                new MemoryStream())
            {
                _pivot.SaveLayoutToStream(ms);

                _defaultLayout =
                    ms.ToArray();
            }
        }

        private void ResetLayout()
        {
            using (MemoryStream ms =
                new MemoryStream(_defaultLayout))
            {
                _pivot.RestoreLayoutFromStream(ms);
            }
        }

        #endregion

        #region Theme Selector

        private int SelectThemeID()
        {
            ComboBoxEdit combo =
        new ComboBoxEdit();

            DataTable dt =
                new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("NAME");

            foreach (ToolStripItem item
                in _menuThemes.DropDownItems)
            {
                ToolStripMenuItem menuItem =
                    item as ToolStripMenuItem;

                if (menuItem != null)
                {
                    dt.Rows.Add(
                        Convert.ToInt32(menuItem.Tag),
                        menuItem.Text);
                }
            }

            if (dt.Rows.Count == 0)
            {
                XtraMessageBox.Show(
                    "Kayıtlı tema bulunamadı.");

                return 0;
            }

            combo.Properties.TextEditStyle =
                TextEditStyles.DisableTextEditor;

            foreach (DataRow dr in dt.Rows)
            {
                combo.Properties.Items.Add(
                    dr["NAME"].ToString());
            }

            combo.SelectedIndex = 0;

            XtraInputBoxArgs args =
                new XtraInputBoxArgs();

            args.Caption = "Tema Seç";

            args.Prompt = "Tema seçiniz:";

            args.Editor = combo;

            object result =
                XtraInputBox.Show(args);

            if (result == null)
                return 0;

            string secilenTema =
                result.ToString();

            DataRow[] rows =
                dt.Select(
                    "NAME='" +
                    secilenTema.Replace("'", "''") +
                    "'");

            if (rows.Length == 0)
                return 0;

            return Convert.ToInt32(
                rows[0]["ID"]);
        }

        #endregion

        #region Menu Events

        private void MenuSave_Click(
            object sender,
            EventArgs e)
        {
            string temaAdi =
                XtraInputBox.Show(
                    "Tema adı:",
                    "Tema Kaydet",
                    "");

            if (string.IsNullOrWhiteSpace(
                temaAdi))
                return;

            SaveLayout(temaAdi);
        }

        private void MenuSaveAs_Click(
            object sender,
            EventArgs e)
        {
            string temaAdi =
                XtraInputBox.Show(
                    "Yeni tema adı:",
                    "Farklı Kaydet",
                    "");

            if (string.IsNullOrWhiteSpace(
                temaAdi))
                return;

            SaveLayout(temaAdi);
        }

        private void MenuDelete_Click(
            object sender,
            EventArgs e)
        {
            int id = SelectThemeID();

            if (id == 0)
                return;

            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(@"
SELECT LAYOUTNAME
FROM EntegreF..USER_PIVOT_LAYOUT
WHERE ID=@ID", con);

                cmd.Parameters.AddWithValue(
                    "@ID",
                    id);

                object obj =
                    cmd.ExecuteScalar();

                if (obj == null)
                    return;

                DeleteTheme(obj.ToString());
                XtraMessageBox.Show(
                    "Seçilen Tema Silindi.");
            }
        }

        private void MenuDefault_Click(
            object sender,
            EventArgs e)
        {
            int id = SelectThemeID();

            if (id == 0)
                return;

            using (SqlConnection con =
                new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmdReset =
                    new SqlCommand(@"
UPDATE EntegreF..USER_PIVOT_LAYOUT
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
                    _pivot.Name);

                cmdReset.ExecuteNonQuery();

                SqlCommand cmd =
                    new SqlCommand(@"
UPDATE EntegreF..USER_PIVOT_LAYOUT
SET ISDEFAULT=1
WHERE ID=@ID", con);

                cmd.Parameters.AddWithValue(
                    "@ID",
                    id);

                cmd.ExecuteNonQuery();
            }

            XtraMessageBox.Show(
                "Varsayılan tema ayarlandı.");
        }

        private void MenuReset_Click(
            object sender,
            EventArgs e)
        {
            ResetLayout();
        }

        private void MenuFieldList_Click(
            object sender,
            EventArgs e)
        {
            _pivot.FieldsCustomization();
        }

        private void MenuBestFit_Click(
            object sender,
            EventArgs e)
        {
            _pivot.BestFit();
        }

        private void MenuExportExcel_Click(
            object sender,
            EventArgs e)
        {
            SaveFileDialog sd =
                new SaveFileDialog();

            sd.Filter =
                "Excel Dosyası|*.xlsx";

            if (sd.ShowDialog() !=
                DialogResult.OK)
                return;

            _pivot.ExportToXlsx(
                sd.FileName);

            XtraMessageBox.Show(
                "Excel aktarımı tamamlandı.");
        }

        #endregion
    }
}