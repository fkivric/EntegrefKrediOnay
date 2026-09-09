using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class.OCRClass
{
    [Serializable]
    public class eJarvis : ICloneable
    {
        public long? JarRef { get; set; }

        public string JarKod { get; set; }

        public string JarAdi { get; set; }

        public string JarClassName { get; set; }

        public string JarSendKey { get; set; }

        public string JarTip { get; set; }

        public string JarTipAdi { get; set; }

        public string SERI { get; set; }

        public string NO { get; set; }

        public string TCKIMLIKNO { get; set; }

        public string SOYADI { get; set; }

        public string ADI { get; set; }

        public string BABAADI { get; set; }

        public string ANAADI { get; set; }

        public string DOGUMYERI { get; set; }

        public string DOGUMTARIHI { get; set; }

        public string MEDENIHAL { get; set; }

        public string DIN { get; set; }

        public string KANGRUBU { get; set; }

        public string IL { get; set; }

        public string ILCE { get; set; }

        public string MAHALLEKOY { get; set; }

        public string CILTNO { get; set; }

        public string AILESIRANO { get; set; }

        public string SIRANO { get; set; }

        public string VERILDIGIYER { get; set; }

        public string VERILISNEDENI { get; set; }

        public string KAYITNO { get; set; }

        public string VERILISTARIHI { get; set; }

        public eJarvis orjRecord { get; set; }

        public string JarConfigSettingPrint { get; set; }

        public string JarConfigSettingPrintDefault { get; set; }

        public string JarConfigSettingFileSource { get; set; }

        public string JarConfigSettingLanguage { get; set; }

        public string JarConfigSettingCamera { get; set; }

        public string JarConfigEvrakSettingMailAdress { get; set; }

        public string JarConfigEvrakSettingSubeAdi { get; set; }

        public string JarConfigEvrakSettingKonu { get; set; }

        public string JarConfigEvrakSettingBody { get; set; }

        public string JarConfigEvrakSettingCCmail { get; set; }

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0L;
                return formatter.Deserialize(stream);
            }
        }
    }
}
