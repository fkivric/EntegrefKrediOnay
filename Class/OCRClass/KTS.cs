using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EntegrefKrediOnay.Class.OCRClass
{

    [Serializable]
    [GeneratedCode("System.Xml", "4.8.9032.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://schemas.datacontract.org/2004/07/Volant.KTSVCF")]
    public class KTS
    {
        private string adField;

        private string aNNEADField;

        private string bABAADField;

        private string cINSField;

        private string dOGUMTAHField;

        private string dOGUMYERField;

        private string idField;

        private string sOYADField;

        [XmlElement(IsNullable = true)]
        public string AD
        {
            get
            {
                return adField;
            }
            set
            {
                adField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string ANNEAD
        {
            get
            {
                return aNNEADField;
            }
            set
            {
                aNNEADField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string BABAAD
        {
            get
            {
                return bABAADField;
            }
            set
            {
                bABAADField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string CINS
        {
            get
            {
                return cINSField;
            }
            set
            {
                cINSField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string DOGUMTAH
        {
            get
            {
                return dOGUMTAHField;
            }
            set
            {
                dOGUMTAHField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string DOGUMYER
        {
            get
            {
                return dOGUMYERField;
            }
            set
            {
                dOGUMYERField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string ID
        {
            get
            {
                return idField;
            }
            set
            {
                idField = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string SOYAD
        {
            get
            {
                return sOYADField;
            }
            set
            {
                sOYADField = value;
            }
        }
    }

}
