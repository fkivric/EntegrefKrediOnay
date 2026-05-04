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
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using DevExpress.LookAndFeel;
using System.Data.SqlClient;
using EntegreFDLL;
using EntegreFDLL.Class;
using EntegrefKrediOnay.Class;
using static EntegreFDLL.Class.DataTableClass;

namespace EntegrefKrediOnay
{
    public partial class frmVKN : MetroFramework.Forms.MetroForm
    {
        public frmVKN()
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "McSkin";
            DefaultLookAndFeel defaultLookAndFeel = new DefaultLookAndFeel();
            defaultLookAndFeel.LookAndFeel.SkinName = "McSkin";
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            InitializeComponent();
        }
        SqlConnectionObject conn = new SqlConnectionObject();
        private async void frmVKN_Load(object sender, EventArgs e)
        {
            frmEntegrefSettings entegrefSettings = new frmEntegrefSettings();
            entegrefSettings.VolXml();
            //frmLogin frmLogin = new frmLogin();
            //frmLogin.VolXml();
            var builder = new SqlConnectionStringBuilder(Program.sql2);
            //Program.sql1 = builder.ConnectionString;
            this.Tag = builder.InitialCatalog;
            ////Seçilen veritabanını ata
            //builder.InitialCatalog = "EntegreF";
            ////Güncellenmiş connection string
            //Properties.Settings.Default.connectionstring2 = builder.ConnectionString;
            //Program.sql2 = builder.ConnectionString;
            //Properties.Settings.Default.Save();
            using (SqlConnection sqlConnection = new SqlConnection(Program.sql2))
            {
                try
                {
                    sqlConnection.Open();
                    if (Program.configProvider.VKN != null)
                    {
                        Entegref Getentegref = new Entegref();
                        var company = await Getentegref.CompanyName(Program.configProvider);
                        using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 ResimVerisi FROM EntegreF..CompanyImages WHERE @Company LIKE '%' + ResimAdi + '%'", sqlConnection))
                        {
                            cmd.Parameters.AddWithValue("@Company", Program.configProvider.CompanyName);
                            object result = cmd.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                byte[] imageBytes = (byte[])result;
                                using (MemoryStream ms = new MemoryStream(imageBytes))
                                {
                                    pictureEdit1.Image = Image.FromStream(ms);
                                    pictureEdit1.Visible = true;
                                }
                            }
                            else
                            {
                                // Alternatif parçalı kontrol
                                string[] companyParts = company.Substring(0, company.Length - 2).Split('_');
                                foreach (string part in companyParts)
                                {
                                    using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 1 ResimVerisi FROM EntegreF..CompanyImages WHERE ResimAdi LIKE '%' + @Part + '%'", sqlConnection))
                                    {
                                        cmd2.Parameters.AddWithValue("@Part", part);
                                        object result2 = cmd2.ExecuteScalar();

                                        if (result2 != null && result2 != DBNull.Value)
                                        {
                                            byte[] imageBytes = (byte[])result2;
                                            using (MemoryStream ms = new MemoryStream(imageBytes))
                                            {
                                                pictureEdit1.Image = Image.FromStream(ms);
                                                pictureEdit1.Visible = true;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    entegrefSettings.ShowDialog();
                }
            }
        }
        private void btnLogo_Click(object sender, EventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "Resim Türü|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Bir Resim Seçin";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureEdit1.Visible = true;
                    pictureEdit1.Image = Image.FromFile(openFileDialog.FileName);
                    pictureEdit1.Tag = openFileDialog.FileName; // Dosya yolunu sakla                    
                }
            }
        }

        private async void btnKaydet_ClickAsync(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtVKN.Text))
            {
                MessageBox.Show("Programı Kulanmak ve Lisans Anahtarı oluşturmak için Zorunludur.");
            }
            else
            {
                if (txtVKN.Text != "")
                {
                    if (txtVKN.Text.Length == 10)
                    {
                        if (pictureEdit1.Tag != null)
                        {
                            string imagePath = pictureEdit1.Tag.ToString();
                            string text = txtFirma.Text;

                            string fileNameWithoutExtension = this.Tag.ToString();
                            byte[] imageBytes = File.ReadAllBytes(imagePath);
                            var control = conn.GetValueConnection($@"select * from EntegreF..CompanyImages where ResimAdi = '{fileNameWithoutExtension}'",Program.sql2);
                            if (control == "")
                            {
                                using (SqlConnection sqlConnection = new SqlConnection(Program.sql2))
                                {
                                    sqlConnection.Open();
                                    using (SqlCommand cmd = new SqlCommand("INSERT INTO EntegreF..CompanyImages (ResimAdi, ResimVerisi) VALUES (@ResimAdi, @ResimVerisi)", sqlConnection))
                                    {
                                        cmd.Parameters.AddWithValue("@ResimAdi", fileNameWithoutExtension);
                                        cmd.Parameters.Add("@ResimVerisi", SqlDbType.VarBinary).Value = imageBytes;
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            // Seçilen resmi hedef dizine kopyala ve metni kaydet
                            //string targetFolder = Path.Combine(Application.StartupPath, "Image");
                            //Entegref.CreateDirectoryIfNotExists(targetFolder);

                            //string targetImagePath = Path.Combine(targetFolder, Path.GetFileName(imagePath));
                            //File.Copy(imagePath, targetImagePath, true);

                            //string textFilePath = Path.Combine(targetFolder, "imageInfo.txt");
                            //File.WriteAllText(textFilePath, text);

                            string _s6 = System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); // proje adı

                            Program.configProvider.VKN = txtVKN.Text;
                            Entegref client = new Entegref();
                            string response = await client.CheckCompany(Program.configProvider);
                            List<Sonuc> myDeserializedClass = JsonConvert.DeserializeObject<List<Sonuc>>(response);
                            if (myDeserializedClass[0].status == false)
                            {
                                string response2 = await client.NewCompany(Program.configProvider);
                                List<Sonuc> myDeserializedClass2 = JsonConvert.DeserializeObject<List<Sonuc>>(response2);
                                if (myDeserializedClass2[0].status == true)
                                {
                                    Properties.Settings.Default.VKN = txtVKN.Text;
                                    Properties.Settings.Default.Save();
                                    RegistryKey key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\{Program.configProvider.ProductName}");
                                    key.SetValue("ApplicationVKN", txtVKN.Text);
                                    key.Close();
                                    this.Close();
                                    this.Dispose();
                                }
                            }
                            else
                            {
                                Program.configProvider.VKN = txtVKN.Text;
                                Properties.Settings.Default.VKN = txtVKN.Text;
                                Properties.Settings.Default.Save();
                                RegistryKey key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\{Program.configProvider.ProductName}");
                                key.SetValue("ApplicationVKN", txtVKN.Text);
                                key.Close();
                                this.Close();
                                this.Dispose();
                            }
                        }
                        else
                        {
                            CustomMessageBox.ShowMessage("Lütfen Firma Logosu Seçin", "", this, "Uyarı",false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        CustomMessageBox.ShowMessage("Lütfen 10 Haneli Lisans Vergi Kimkik nosu Giriniz.", "", this, "Uyarı",false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    CustomMessageBox.ShowMessage("Lütfen Vergi Kimlik Nosu girerek devam edin", "", this, "Uyarı",false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
    }
}