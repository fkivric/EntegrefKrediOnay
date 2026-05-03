using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntegrefKrediOnay.Class
{
    public static class CustomMessageBox
    {
        public static string ConvertToRtf(string plainText)
        {
            return $@"{{\rtf1\ansi\deff0{{\fonttbl{{\f0 Arial;}}}}\fs20 {plainText}}}";
        }
        static bool IsRtf(string text)
        {
            return !string.IsNullOrEmpty(text) && text.TrimStart().StartsWith(@"{\rtf");
        }


        public static System.Windows.Forms.DialogResult ShowMessage(string message,string detay , Form owner, string caption,bool Eror, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon ıcon)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;
            switch (buttons)
            {
                case System.Windows.Forms.MessageBoxButtons.OK:
                    using (EntegrefKrediOnay.frmNewMessageBox msgOK = new EntegrefKrediOnay.frmNewMessageBox())
                    {
                        msgOK.Text = caption;
                        msgOK.Message = message;
                        msgOK.detay = detay;// IsRtf(detay) ? detay : ConvertToRtf(detay);
                        msgOK.eror = Eror;
                        msgOK.TopMost = true;
                        if (owner != null)
                        {
                            msgOK.StartPosition = FormStartPosition.Manual;
                            msgOK.Location = new Point(
                                owner.Location.X + (owner.Width - msgOK.Width) / 2,
                                owner.Location.Y + (owner.Height - msgOK.Height) / 2);
                        }
                        else
                        {
                            msgOK.StartPosition = FormStartPosition.CenterScreen;
                        }                  
                        switch (ıcon)
                        {
                            case System.Windows.Forms.MessageBoxIcon.Information:
                                msgOK.MessageIcon = Properties.Resources.Entegref__1_;
                                break;
                            case System.Windows.Forms.MessageBoxIcon.Question:
                                msgOK.MessageIcon = Properties.Resources.question_32x32;
                                break;
                            case System.Windows.Forms.MessageBoxIcon.Warning:
                                msgOK.MessageIcon = Properties.Resources.bodetails_32x32;
                                break;
                            case System.Windows.Forms.MessageBoxIcon.Error:
                                msgOK.MessageIcon = Properties.Resources.bodetails_32x32;
                                break;
                        }
                        if (owner != null)
                        {
                            //msgOK.TopMost = true;
                            //msgOK.Activate();
                            result = msgOK.ShowDialog(owner);
                        }
                        else
                        {
                            result = msgOK.ShowDialog();
                        }
                    }
                    break;
                case System.Windows.Forms.MessageBoxButtons.OKCancel:
                    break;
                case System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore:
                    break;
                case System.Windows.Forms.MessageBoxButtons.YesNoCancel:
                    break;
                case System.Windows.Forms.MessageBoxButtons.YesNo:
                    break;
                case System.Windows.Forms.MessageBoxButtons.RetryCancel:
                    break;
                default:
                    break;
            }
            return result;
        }

    }
}
