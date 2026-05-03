using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntegrefKrediOnay.Class
{
    public class SplashScrenn
    {
        public static async Task RunWithSplashAsync(Form owner, bool Manage, int Maxvalue, string uygulama, Func<IProgress<(int percent, string message)>, CancellationToken, Task> action)
        {
            if (Manage)
            {
                using (var cts = new CancellationTokenSource())
                {
                    var splash = new frmCustomSplash(Manage);
                    splash._cts = cts;

                    var progress = new Progress<(int, string)>(p =>
                    {
                        splash.SetProgress(p.Item1, p.Item2);
                    });
                    splash.SetMaxValue(Maxvalue);
                    splash.SetProgresName(uygulama);
                    splash.Show(owner); // ❗ ShowDialog DEĞİL

                    try
                    {
                        await action(progress, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // iptal edildi, sessizce çık
                    }
                    finally
                    {
                        splash.CloseWithFade();
                    }
                }
            }
            else
            {
                using (var cts = new CancellationTokenSource())
                {
                    var splash = new frmCustomSplash(Manage);
                    splash._cts = cts;

                    var progress = new Progress<(int, string)>(p =>
                    {
                        splash.SetProgress2(p.Item2);
                    });
                    splash.SetProgresName(uygulama);
                    splash.Show(owner); // ❗ ShowDialog DEĞİL

                    try
                    {
                        await action(progress, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // iptal edildi, sessizce çık
                    }
                    finally
                    {
                        splash.CloseWithFade();
                    }
                }
            }
        }
    }
}
