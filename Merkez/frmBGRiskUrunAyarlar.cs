using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using EntegreFDLL;
using EntegrefKrediOnay.Class;

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmBGRiskUrunAyarlar : DevExpress.XtraEditors.XtraForm
    {
        public frmBGRiskUrunAyarlar()
        {
            InitializeComponent();
        }
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;
        string sql2 = Properties.Settings.Default.connectionstring2;
        public class KrediPuan_RiskUrunGurupPuan
        {
            public int PROUID { get; set; }
            public string PROUNAME { get; set; }
            public int Risk_id { get; set; }
            public string Risk_Adi { get; set; }
            public decimal Risk_TutarMin { get; set; }
            public decimal Risk_TutarMax { get; set; }
        }
        public static List<KrediPuan_RiskUrunGurupPuan> krediPuan_s = new List<KrediPuan_RiskUrunGurupPuan>();
        private void frmBGRiskUrunAyarlar_Load(object sender, EventArgs e)
        {
            var dt = conn.GetData("select id as Risk_id,Risk_Adi from EntegreF..KrediPuan_RiskUrunGurupPuan", sql1);
            repositoryItemSearchLookUpEdit1.DataSource = dt;
            repositoryItemSearchLookUpEdit1.ValueMember = "Risk_id";
            repositoryItemSearchLookUpEdit1.DisplayMember = "Risk_Adi";
        }

        private void tileBtnListele_ItemClick(object sender, TileItemEventArgs e)
        {
            Veri();
        }
        void Veri()
        {
            var dt = conn.GetData(@"select PROUID,PROUNAME,Risk_id,Risk_Adi,Risk_TutarMin, Risk_TutarMax from PRODUCTSUNITED
            left outer join EntegreF..KrediPuan_RiskUrunGurup on VOLUID = PROUID
            left outer join EntegreF..KrediPuan_RiskUrunGurupPuan on id = Risk_id
            order by PROUID", sql1);
            krediPuan_s = dt.ToList<KrediPuan_RiskUrunGurupPuan>();
            gridBGRiskUrunAyarlar.DataSource = krediPuan_s;
        }
        private void repositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor == null) return;

            var view = ViewBGRiskUrunAyarlar;
            int rowHandle = view.GetRowHandle(view.FocusedRowHandle);
            if (rowHandle < 0) return;

            var idObj = editor.EditValue;
            if (idObj == null) return;

            int ID;
            if (!int.TryParse(idObj.ToString(), out ID)) return;

            var dt = conn.GetData(
                $"select Risk_Adi, Risk_TutarMin, Risk_TutarMax " +
                $"from EntegreF..KrediPuan_RiskUrunGurupPuan where id = {ID}",
                sql1);

            if (dt.Rows.Count == 0) return;
            // Seçilen ID’yi mutlaka satıra yaz
            view.SetRowCellValue(rowHandle, "Risk_id", ID);
            view.SetRowCellValue(rowHandle, "Risk_Adi", dt.Rows[0]["Risk_Adi"]);
            view.SetRowCellValue(rowHandle, "Risk_TutarMin", dt.Rows[0]["Risk_TutarMin"]);
            view.SetRowCellValue(rowHandle, "Risk_TutarMax", dt.Rows[0]["Risk_TutarMax"]);
            view.RefreshRow(rowHandle);
        }

        private void tileBtnGuncelle_ItemClick(object sender, TileItemEventArgs e)
        {
            var item = sender as TileItem; // veya TileBarItem
            try
            {
                EntegreFDLL.Class.Entegref.SplashScreen(this, $"{item.Text} olarak işleniyor", Properties.Settings.Default.CompanyName, "Lütfen Bekleyin");
                for (int i = 0; i < ViewBGRiskUrunAyarlar.RowCount; i++)
                {
                    var PROUID = int.Parse(ViewBGRiskUrunAyarlar.GetRowCellValue(i, "PROUID").ToString());
                    var Riskid = int.Parse(ViewBGRiskUrunAyarlar.GetRowCellValue(i, "Risk_id").ToString());
                    var matched = krediPuan_s.FirstOrDefault(z => z.PROUID == PROUID && z.Risk_id == Riskid);
                    if (matched == null)
                    {
                        conn.InsertValue($@"insert into EntegreF..KrediPuan_RiskUrunGurup values ({Riskid},{PROUID})", sql1);
                    }
                    else
                    {
                        conn.InsertValue($@"update EntegreF..KrediPuan_RiskUrunGurup set Risk_id = {Riskid} where VOLUID = {PROUID}", sql1);
                    }
                }
                Veri();
            }
            catch (Exception ex)
            {
                string hataDetay = $"Hata Mesajı: {ex.Message}\n {Environment.NewLine} Program Adı: {ex.Source}\n {Environment.NewLine} İşlem: {ex.TargetSite}\n {Environment.NewLine} Hata Satırı:\n{ex.StackTrace}";
                CustomMessageBox.ShowMessage("İşlem Hatası Detaya Bekanız", hataDetay, this, "Uyarı", true, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 1000, this);
            }
        }

        private void tileBtnYeni_ItemClick(object sender, TileItemEventArgs e)
        {
            gridBGRiskUrunAyarlar.DataSource = null;
        }
    }
}