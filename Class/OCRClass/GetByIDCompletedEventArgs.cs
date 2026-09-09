using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace EntegrefKrediOnay.Class.OCRClass
{
    [GeneratedCode("System.Web.Services", "4.8.9032.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    public class GetByIDCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public KTS Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (KTS)results[0];
            }
        }

        internal GetByIDCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
    [GeneratedCode("System.Web.Services", "4.8.9032.0")]
    public delegate void GetByIDCompletedEventHandler(object sender, GetByIDCompletedEventArgs e);
}
