using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class Parsedresult
    {
        public object FileParseExitCode { get; set; }

        public string ParsedText { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorDetails { get; set; }
    }
}
