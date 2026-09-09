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
using DevExpress.XtraTab;
using EntegrefKrediOnay.Class.BGClass;

namespace EntegrefKrediOnay
{
    public partial class frmKefilCoklu : DevExpress.XtraEditors.XtraForm
    {
        List<BGKefil.Kefil> GetKefils = new List<BGKefil.Kefil>();
        public frmKefilCoklu(List<BGKefil.Kefil> kefils)
        {
            InitializeComponent();
            GetKefils.AddRange(kefils);
        }
        private void frmKefilCoklu_Load(object sender, EventArgs e)
        {
            string Kefiller = "";
            for (int i = 0; i < GetKefils.Count; i++)
            {
                Kefiller += GetKefils[i].WRTRNAME;
                OpenTabForm(new frmBGKefil(GetKefils[i].WRTRID, GetKefils[i].SALID, GetKefils[i].CURID), GetKefils[i].WRTRNAME);
            }
            this.Text = $"Satışa Ait Kefil Bilgileri {Kefiller}";
        }
        public void OpenTabForm(Form form,string _adi)
        {
            bool isFormOpen = false;
            // İlgili formun açık olup olmadığını kontrol etmek için bir bayrak kullanıyoruz.

            for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
            {

                if (xtraTabControl.TabPages[i].Text == form.Text)
                {
                    isFormOpen = true;
                    // Aynı isme sahip bir formun açık olduğunu belirledik.
                    break;
                }
            }
            if (isFormOpen)
            {
                XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                args.AutoCloseOptions.Delay = 2000;
                args.Caption = "Uyarı";
                args.Text = form.Text + "Açık.";
                args.Buttons = new DialogResult[] { DialogResult.OK };
                args.AutoCloseOptions.ShowTimerOnDefaultButton = true;
                form.Focus();
                XtraMessageBox.Show(args).ToString();
                form.Close();
                form.Dispose();
                return;
                // Form zaten açıksa kullanıcıyı uyar ve işlemi sonlandır.
            }
            else
            {
                int imageIndex;
                if (!int.TryParse(form.Tag?.ToString(), out imageIndex))
                {
                    imageIndex = 0; // Eğer dönüşüm başarısız olursa imageIndex'i 0 olarak ayarlar
                }

                XtraTabPage newTabPage = new XtraTabPage();
                newTabPage.Text = form.Text + " " + _adi;
                newTabPage.ImageIndex = imageIndex;
                xtraTabControl.TabPages.Add(newTabPage);
                xtraTabControl.SelectedTabPageIndex = xtraTabControl.TabPages.Count - 1;
                form.MdiParent = this;
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.Parent = xtraTabControl.TabPages[xtraTabControl.TabPages.Count - 1];
                form.Show();
            }
        }
        //bool FormMode = false;
        private void xtraTabControl_CloseButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (xtraTabControl.SelectedTabPage.Name == "anaSayfaTab")
                {
                    return;
                }
                if (xtraTabControl.SelectedTabPage.Controls.Count > 0)
                {
                    foreach (object item in xtraTabControl.SelectedTabPage.Controls)
                    {
                        if (item is Form && item is Form frm)
                        {
                            frm.Close();
                            frm.Dispose();
                        }
                    }
                }
                xtraTabControl.SelectedTabPage.Controls[0].Dispose();
            }
            catch
            {
            }
            xtraTabControl.TabPages.Remove(xtraTabControl.SelectedTabPage);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }
        public void xtraTabControlSizeRule()
        {
            try
            {
                for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
                {
                    Application.DoEvents();
                    xtraTabControl.TabPages[i].Width = xtraTabControl.Width;
                    xtraTabControl.TabPages[i].Height = xtraTabControl.Height;
                    foreach (Control item in xtraTabControl.TabPages[i].Controls)
                    {
                        Application.DoEvents();
                        xtraTabControl.TabPages[i].Controls.Remove(item);
                        if (item is Form frm)
                        {
                            frm.Dock = DockStyle.Fill;
                            frm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                            frm.AutoSize = false;
                            frm.Size = new Size(xtraTabControl.TabPages[i].Width, xtraTabControl.TabPages[i].Height);
                        }
                        xtraTabControl.TabPages[i].Controls.Add(item);
                        item.Refresh();
                        Application.DoEvents();
                    }
                    Application.DoEvents();
                }
            }
            catch
            {
            }
        }
        private void xtraTabControl_SizeChanged(object sender, EventArgs e)
        {
            if (!base.DesignMode)
            {
                xtraTabControlSizeRule();
            }
        }

    }
}