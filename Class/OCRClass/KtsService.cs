using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace EntegrefKrediOnay.Class.OCRClass
{

    [GeneratedCode("System.Web.Services", "4.8.9032.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [WebServiceBinding(Name = "BasicHttpBinding_IKtsService", Namespace = "http://tempuri.org/")]
    public class KtsService : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetByIDOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if (IsLocalFileSystemWebService(base.Url) && !useDefaultCredentialsSetExplicitly && !IsLocalFileSystemWebService(value))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                useDefaultCredentialsSetExplicitly = true;
            }
        }

        public event GetByIDCompletedEventHandler GetByIDCompleted;

        public KtsService()
        {
            Url = "http://185.22.186.122:1989/KtsService.svc";
            if (IsLocalFileSystemWebService(Url))
            {
                UseDefaultCredentials = true;
                useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                useDefaultCredentialsSetExplicitly = true;
            }
        }

        [SoapDocumentMethod("http://tempuri.org/IKtsService/GetByID", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [return: XmlElement(IsNullable = true)]
        public KTS GetByID([XmlElement(IsNullable = true)] string Id)
        {
            object[] results = Invoke("GetByID", new object[1] { Id });
            return (KTS)results[0];
        }

        public void GetByIDAsync(string Id)
        {
            GetByIDAsync(Id, null);
        }

        public void GetByIDAsync(string Id, object userState)
        {
            if (GetByIDOperationCompleted == null)
            {
                GetByIDOperationCompleted = OnGetByIDOperationCompleted;
            }
            InvokeAsync("GetByID", new object[1] { Id }, GetByIDOperationCompleted, userState);
        }

        private void OnGetByIDOperationCompleted(object arg)
        {
            if (this.GetByIDCompleted != null)
            {
                InvokeCompletedEventArgs invokeArgs = (InvokeCompletedEventArgs)arg;
                this.GetByIDCompleted(this, new GetByIDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (url == null || url == string.Empty)
            {
                return false;
            }
            Uri wsUri = new Uri(url);
            if (wsUri.Port >= 1024 && string.Compare(wsUri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            return false;
        }
    }
}
