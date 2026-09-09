using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class Enums
    {
        public enum IdCard
        {
            front,
            back
        }

        public enum ScanObject
        {
            TcKimlik,
            YeniTcKimlik,
            Ehliyet,
            YeniEhliyet,
            Pasaport
        }
    }
}
