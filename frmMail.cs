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
using DevExpress.XtraGrid.Columns;
using Vol.Erp.LIB.Entities.UiUsings;
using System.IO;
using EntegrefKrediOnay.Properties;
using EntegreFDLL.Class;
using static EntegreFDLL.Class.DataTableClass;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.WinExplorer;

namespace EntegrefKrediOnay
{
    public partial class frmMail : DevExpress.XtraEditors.XtraForm
    {

        public frmMail()
        {
            InitializeComponent();
        }
        private List<eFtpFile> lFtpFile = null;
        private void frmMail_Load(object sender, EventArgs e)
        {
            txtFrom.Text = Entegref.GetLogins.MailAdress;
            lFtpFile = new List<eFtpFile>();
            winExplorerView1.Columns.Clear();
            winExplorerView1.Columns.Add(new GridColumn
            {
                FieldName = "Name"
            });
            winExplorerView1.Columns.Add(new GridColumn
            {
                FieldName = "FileType"
            });
            winExplorerView1.Columns.Add(new GridColumn
            {
                FieldName = "Picture"
            });
            winExplorerView1.ColumnSet.TextColumn = winExplorerView1.Columns["Name"];
            winExplorerView1.ColumnSet.DescriptionColumn = winExplorerView1.Columns["FileType"];
            winExplorerView1.ColumnSet.MediumImageColumn = winExplorerView1.Columns["Picture"];
        }
        private void btnAddAttachment_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in openFileDialog.FileNames)
                    {
                        string fileName = Path.GetFileName(file);
                        Image img = null;
                        try
                        {
                            Icon icon = Icon.ExtractAssociatedIcon(file);
                            if (icon != null)
                                img = icon.ToBitmap();
                        }
                        catch { /* simge alınamazsa yok say */ }

                        lFtpFile.Add(new eFtpFile
                        {
                            FileType = FtpType.Pdf, // Uzantıya göre ayarlayabilirsiniz
                            Name = fileName,
                            Path = file,
                            Picture = img ?? Resources.pdf // Varsayılan
                        });
                    }
                }
            }
            layoutControlItem8.Size = new Size(1088, 50);
            gridControl1.DataSource = null;
            gridControl1.DataSource = lFtpFile;
        }
        private void SetExplorerView()
        {
            WinExplorerView view = gridControl1.MainView as WinExplorerView;
            if (view != null)
            {
                // Varsayılan sütunları temizle
                view.Columns.Clear();

                // Picture sütunu (simge)
                DevExpress.XtraGrid.Columns.GridColumn colPicture = view.Columns.Add();
                colPicture.FieldName = "Picture";
                colPicture.Caption = "";
                colPicture.Visible = true;
                colPicture.Width = 60;
                colPicture.OptionsColumn.AllowEdit = false;

                // RepositoryItemPictureEdit
                RepositoryItemPictureEdit riPicture = gridControl1.RepositoryItems.Add("PictureEdit") as RepositoryItemPictureEdit;
                riPicture.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                colPicture.ColumnEdit = riPicture;

                // Name sütunu (dosya adı)
                DevExpress.XtraGrid.Columns.GridColumn colName = view.Columns.Add();
                colName.FieldName = "Name";
                colName.Caption = "Dosya Adı";
                colName.Visible = true;
                colName.Width = 200;
                colName.OptionsColumn.AllowEdit = false;

                // Path sütunu (isteğe bağlı, gizli olabilir)
                DevExpress.XtraGrid.Columns.GridColumn colPath = view.Columns.Add();
                colPath.FieldName = "Path";
                colPath.Visible = false;
            }
        }
        Entegref GetEntegref = new Entegref();
        private async void btnSend_Click(object sender, EventArgs e)
        {
            // Burada mail gönderme işlemi yapılacak
            string from = txtFrom.Text;
            string to = txtTo.Text;
            string cc = txtCC.Text;
            string bcc = txtBcc.Text;
            string subject = txtSubject.Text;
            string body = richEditControl.HtmlText; // veya Document.GetHtmlText()

            string[] alicilar = {txtTo.Text};
            string konu = $"Müşteri Satış Onayı Sonuçlandırıldı";
            string htmlBody = richEditControl.HtmlText;

            var ekler = new List<string>();
            var Eklistesi = winExplorerView1.DataSource as DataTable;
            for (int i = 0; i < Eklistesi.Rows.Count; i++)
            {
                ekler.Add(Eklistesi.Rows[i][""].ToString());
            }
            List<string> Gonderenler = new List<string>();
            Gonderenler.Add(Entegref.GetLogins.MailAdress);
            Gonderenler.Add(Entegref.GetLogins.MailPassword);
            Gonderenler.Add("Akçaylar Ticaret A.Ş");
            string[] gonderen = Gonderenler.ToArray();
            MailRequest request = new MailRequest();
            request.To = alicilar;  //new string[] { "ismailkelem@yonavm.com.tr", "fatihkivric@yonavm.com.tr"};//alicilar;
            request.Sender = gonderen;
            request.Body = htmlBody;
            request.Subject = konu;
            request.MailHost = Entegref.GetLogins.MailHost;
            request.MailPort = Entegref.GetLogins.MailPort;
            request.AttachmentPaths = ekler;

            var result = await GetEntegref.SendMail(request);
        }
    }
}