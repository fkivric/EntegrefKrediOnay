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

namespace EntegrefKrediOnay.Merkez
{
    public partial class frmMusteriKirilim : DevExpress.XtraEditors.XtraForm
    {
        string WPTREUNIQ;
        public frmMusteriKirilim(string _ID)
        {
            InitializeComponent();
            WPTREUNIQ = _ID;
        }
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;

        private void frmKrilim_Load(object sender, EventArgs e)
        {
            gridKirilmlar.DataSource = conn.GetData($@"select WCTREVAL,WCTRENAME from WAVECUSTREE where WCTREUNIQ = {WPTREUNIQ}", sql1);
        }

        private void ViewKirilmlar_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            frmMusteri.WCTREVAL = ViewKirilmlar.GetRowCellValue(e.RowHandle, "WCTREVAL").ToString();
            frmMusteri.WCTRENAME = ViewKirilmlar.GetRowCellValue(e.RowHandle, "WCTRENAME").ToString();
            this.Close();
            this.Dispose();
        }
    }
}