using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class DocumentCreationForm : DevExpress.XtraEditors.XtraForm
    {
        public DocumentCreationForm()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            SuspendLayout();
            InitializeComponent();
        }

        private void CreationForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Alt && e.KeyCode == Keys.H)
                {
                    Process.Start("Calc");
                }
            }
            catch
            {
            }
        }

        private void CreationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }

        private void CreationForm_Load(object sender, EventArgs e)
        {
            ResumeLayout();
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentCreationForm));
            base.SuspendLayout();
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            base.ClientSize = new System.Drawing.Size(284, 261);
            base.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            base.HelpButton = true;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.Name = "CreationForm";
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreationForm";
            base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(CreationForm_FormClosed);
            base.Load += new System.EventHandler(CreationForm_Load);
            base.KeyDown += new System.Windows.Forms.KeyEventHandler(CreationForm_KeyDown);
            base.ResumeLayout(false);
        }
    }
}
