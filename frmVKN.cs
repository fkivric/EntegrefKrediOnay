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
using System.Xml;

namespace EntegrefKrediOnay
{
    public partial class frmVKN : Form
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
        Entegref GetEntegref = new Entegref();
        private async void frmVKN_Load(object sender, EventArgs e)
        {
            var builder = new SqlConnectionStringBuilder(Program.sql2);            
            this.Tag = builder.InitialCatalog;
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
                        GetEntegref.Securety(Application.StartupPath);
                        string text = txtFirma.Text;

                        string fileNameWithoutExtension = this.Tag.ToString();
                        try
                        {
                            XmlTextWriter writer = new XmlTextWriter(Application.StartupPath + "\\EntegreFConnection.xml", Encoding.UTF8);
                            writer.WriteStartDocument();
                            writer.WriteStartElement("root");
                            writer.WriteStartElement("DB");
                            writer.WriteAttributeString("SERVERNAME", "62.244.219.23,1435".TextSifrele());
                            writer.WriteAttributeString("DATABASE", "VDB_JUNEMED01".TextSifrele());
                            writer.WriteAttributeString("LOGIN", "sa".TextSifrele());
                            writer.WriteAttributeString("PASSWORD", "MagicUser2026!".TextSifrele());
                            writer.WriteAttributeString("IntegratedSecurity", "false".TextSifrele());
                            writer.WriteAttributeString("COMPANY", "01".TextSifrele());
                            writer.WriteAttributeString("DIVISON", "00".TextSifrele());
                            writer.WriteAttributeString("YEAR", DateTime.Now.Year.ToString().TextSifrele());
                            writer.WriteAttributeString("pathOfPrints", (Application.StartupPath + "\\").TextSifrele());
                            writer.WriteAttributeString("pathOfArchive", (Application.StartupPath + "\\Arşiv\\").TextSifrele());
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndDocument();
                            writer.Close();
                        }
                        catch (Exception exp)
                        {
                            CustomMessageBox.ShowMessage(exp.Message, "", this, "", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

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
                        CustomMessageBox.ShowMessage("Lütfen 10 Haneli Lisans Vergi Kimkik nosu Giriniz.", "", this, "Uyarı", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    CustomMessageBox.ShowMessage("Lütfen Vergi Kimlik Nosu girerek devam edin", "", this, "Uyarı", false, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
    }
}