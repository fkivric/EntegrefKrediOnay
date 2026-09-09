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
    public partial class frmKrilim : DevExpress.XtraEditors.XtraForm
    {
        string WPTREUNIQ;
        public frmKrilim(string _ID)
        {
            InitializeComponent();
            WPTREUNIQ = _ID;
        }
        SqlConnectionObject conn = new SqlConnectionObject();
        string sql1 = Properties.Settings.Default.connectionstring;

        private void frmKrilim_Load(object sender, EventArgs e)
        {
            gridKirilmlar.DataSource = conn.GetData($@"select WPTREVAL,WPTRENAME from WAVEPROTREE where WPTREUNIQ = {WPTREUNIQ}", sql1);
        }

        private void ViewKirilmlar_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            frmStok.WPROVAL = ViewKirilmlar.GetRowCellValue(e.RowHandle, "WPTREVAL").ToString();
            frmStok.WPTRENAME = ViewKirilmlar.GetRowCellValue(e.RowHandle, "WPTRENAME").ToString();
            this.Close();
            this.Dispose();
        }
    }
}