using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class Rootobject
    {
        public Parsedresult[] ParsedResults { get; set; }

        public int OCRExitCode { get; set; }

        public bool IsErroredOnProcessing { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorDetails { get; set; }
    }
}
