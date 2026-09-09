using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class YandexTranslate
    {
        [DataContract]
        internal class TranslateData
        {
            [DataMember(Name = "code")]
            internal int Code { get; set; }

            [DataMember(Name = "lang")]
            internal string Lang { get; set; }

            [DataMember(Name = "text")]
            internal List<string> Text { get; set; }
        }

        [DataContract]
        public class GetLangsData
        {
            [DataMember(Name = "dirs")]
            internal List<string> Dirs { get; set; }
        }

        [DataContract]
        internal class DetectData
        {
            [DataMember(Name = "code")]
            internal int Code { get; set; }

            [DataMember(Name = "lang")]
            internal string Lang { get; set; }
        }

        private string _apiKey;

        public void YandexTranslateGetApi()
        {
            _apiKey = "trnsl.1.1.20170508T133808Z.29502f417e7f6539.5a724d1f8e833930bd57c20dbdf7a2b6f8a51fcb";
        }

        public string GetTranslate(string text)
        {
            YandexTranslateGetApi();
            List<string> lstString = Translate("tr", text);
            return lstString[0];
        }

        public List<string> Translate(string lang, string text)
        {
            string requestString = string.Format("https://translate.yandex.net/api/v1.5/tr.json/translate?key={0}&text={1}&lang={2}&format={3}", _apiKey, text, lang, "plain");
            WebRequest request = WebRequest.Create(requestString);
            if (requestString.Length > 10240 && request.Method.StartsWith("GET"))
            {
                throw new ArgumentException("Text is too long (>10Kb)");
            }
            WebResponse response = request.GetResponse();
            DataContractJsonSerializer yandexDataContractSerializer = new DataContractJsonSerializer(typeof(TranslateData));
            TranslateData translateData;
            try
            {
                translateData = (TranslateData)yandexDataContractSerializer.ReadObject(response.GetResponseStream());
            }
            catch
            {
                translateData = new TranslateData();
                translateData.Text = new List<string>();
            }
            return translateData.Text;
        }
    }
}
