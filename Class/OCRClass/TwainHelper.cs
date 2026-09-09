using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class TwainHelper
    {
        private static TwainSession _twain;
        public static List<string> GetScannerList()
        {
            List<string> scanners = new List<string>();

            // TWAIN session başlat
            var appId = TWIdentity.CreateFromAssembly(DataGroups.Image, typeof(TwainHelper).Assembly);
            _twain = new TwainSession(appId);
            _twain.Open(); // TWAIN Manager ile bağlantı açılır
            try
            {
                // Tüm kaynakları (scanner) al
                foreach (var src in _twain.GetSources())
                {
                    scanners.Add(src.Name);
                }

                _twain.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return scanners;
        }
    }
}
