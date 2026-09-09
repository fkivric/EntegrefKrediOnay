using EntegrefKrediOnay.Properties;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntegrefKrediOnay.Class.OCRClass
{
    public class GoogleAnnotate
    {
        private static byte[] jSonData = Resources.key;

        private GoogleCredential _credential;

        public string ApplicationName => "Ocr";

        public string JsonResult { get; set; }

        public string TextResult { get; set; }

        public string Error { get; set; }

        public eIdCard rIdCard { get; set; }

        private string JsonKeypath => Application.StartupPath + "\\key.json";

        private GoogleCredential CreateCredential()
        {
            if (_credential != null)
            {
                return _credential;
            }
            using (MemoryStream stream2 = new MemoryStream(jSonData))
            {
                string[] scopes = new string[1] { VisionService.Scope.CloudPlatform };
                GoogleCredential credential = GoogleCredential.FromStream(stream2);
                return _credential = credential.CreateScoped(scopes);
            }
        }

        private VisionService CreateService(GoogleCredential credential)
        {
            return new VisionService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                GZipEnabled = true
            });
        }

        public void GetText(string imgPath, string language)
        {
            string textResult = (JsonResult = "");
            TextResult = textResult;
            byte[] file = File.ReadAllBytes(imgPath);
            GoogleCredential credential = CreateCredential();
            VisionService service = CreateService(credential);
            service.HttpClient.Timeout = new TimeSpan(1, 1, 1);
            BatchAnnotateImagesRequest batchRequest = new BatchAnnotateImagesRequest();
            batchRequest.Requests = new List<AnnotateImageRequest>();
            batchRequest.Requests.Add(new AnnotateImageRequest
            {
                Features = new List<Feature>
            {
                new Feature
                {
                    Type = "TEXT_DETECTION",
                    MaxResults = 1
                }
            },
                ImageContext = new ImageContext
                {
                    LanguageHints = new List<string> { language }
                },
                Image = new Image
                {
                    Content = Convert.ToBase64String(file)
                }
            });
            ImagesResource.AnnotateRequest annotate = service.Images.Annotate(batchRequest);
            BatchAnnotateImagesResponse batchAnnotateImagesResponse = annotate.Execute();
            if (!batchAnnotateImagesResponse.Responses.Any())
            {
                return;
            }
            AnnotateImageResponse annotateImageResponse = batchAnnotateImagesResponse.Responses[0];
            if (annotateImageResponse.Error != null)
            {
                if (annotateImageResponse.Error.Message != null)
                {
                    Error = annotateImageResponse.Error.Message;
                }
                return;
            }
            if (rIdCard == null)
            {
                rIdCard = new eIdCard();
            }
            if (annotateImageResponse.TextAnnotations == null || !annotateImageResponse.TextAnnotations.Any())
            {
                return;
            }
            AnnotateImageResponse annotateImageResponseSOYADI = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseADI = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseBABAADI = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseANAADI = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseDOGUMYERI = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseMEDENIHALI = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseDIN = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseKANGRUBU = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseIL = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseILCE = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseMAHALLEKOY = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseVERILDIGIYER = new AnnotateImageResponse();
            AnnotateImageResponse annotateImageResponseVERILISNEDENI = new AnnotateImageResponse();
            annotateImageResponseSOYADI.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseADI.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseBABAADI.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseANAADI.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseDOGUMYERI.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseMEDENIHALI.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseDIN.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseKANGRUBU.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseIL.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseILCE.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseMAHALLEKOY.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseVERILDIGIYER.TextAnnotations = new List<EntityAnnotation>();
            annotateImageResponseVERILISNEDENI.TextAnnotations = new List<EntityAnnotation>();
            for (int i = 1; i < annotateImageResponse.TextAnnotations.Count; i++)
            {
                int y1 = 0;
                int y2 = 0;
                int height = Math.Abs(annotateImageResponse.TextAnnotations[i].BoundingPoly.Vertices[1].Y.Value - annotateImageResponse.TextAnnotations[i].BoundingPoly.Vertices[3].Y.Value);
                if (annotateImageResponse.TextAnnotations[i].BoundingPoly.Vertices[1].Y.HasValue)
                {
                    y1 = annotateImageResponse.TextAnnotations[i].BoundingPoly.Vertices[1].Y.Value;
                }
                if (annotateImageResponse.TextAnnotations[i].BoundingPoly.Vertices[3].Y.HasValue)
                {
                    y2 = annotateImageResponse.TextAnnotations[i].BoundingPoly.Vertices[3].Y.Value;
                }
                if (y1 >= 0 && y1 <= 111 && y2 >= 0 && y2 <= 111)
                {
                    rIdCard.SERI += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 111 && y1 <= 222 && y2 >= 111 && y2 <= 222)
                {
                    rIdCard.NO += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 190 && y1 <= 333 && y2 >= 222 && y2 <= 333)
                {
                    rIdCard.TCKIMLIKNO += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 333 && y1 <= 444 && y2 >= 333 && y2 <= 460)
                {
                    annotateImageResponseSOYADI.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 444 && y1 <= 555 && y2 >= 444 && y2 <= 555)
                {
                    annotateImageResponseADI.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 555 && y1 <= 666 && y2 >= 555 && y2 <= 666)
                {
                    annotateImageResponseBABAADI.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 666 && y1 <= 777 && y2 >= 666 && y2 <= 777)
                {
                    annotateImageResponseANAADI.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 777 && y1 <= 888 && y2 >= 777 && y2 <= 888)
                {
                    annotateImageResponseDOGUMYERI.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 888 && y1 <= 999 && y2 >= 888 && y2 <= 999)
                {
                    rIdCard.DOGUMTARIHI += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 999 && y1 <= 1111 && y2 >= 999 && y2 <= 1111)
                {
                    annotateImageResponseMEDENIHALI.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 1111 && y1 <= 1222 && y2 >= 1111 && y2 <= 1222)
                {
                    annotateImageResponseDIN.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 1222 && y1 <= 1333 && y2 >= 1222 && y2 <= 1333)
                {
                    annotateImageResponseKANGRUBU.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 1333 && y1 <= 1444 && y2 >= 1333 && y2 <= 1444)
                {
                    annotateImageResponseIL.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 1444 && y1 <= 1555 && y2 >= 1444 && y2 <= 1555)
                {
                    annotateImageResponseILCE.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 1555 && y1 <= 1666 && y2 >= 1555 && y2 <= 1666)
                {
                    annotateImageResponseMAHALLEKOY.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 1666 && y1 <= 1777 && y2 >= 1666 && y2 <= 1777)
                {
                    rIdCard.CILTNO += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 1777 && y1 <= 1888 && y2 >= 1777 && y2 <= 1888)
                {
                    rIdCard.AILESIRANO += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 1888 && y1 <= 1999 && y2 >= 1888 && y2 <= 1999)
                {
                    rIdCard.SIRANO += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 1999 && y1 <= 2111 && y2 >= 1999 && y2 <= 2111)
                {
                    annotateImageResponseVERILDIGIYER.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 2111 && y1 <= 2222 && y2 >= 2111 && y2 <= 2222)
                {
                    annotateImageResponseVERILISNEDENI.TextAnnotations.Add(annotateImageResponse.TextAnnotations[i]);
                }
                if (y1 >= 2222 && y1 <= 2333 && y2 >= 2222 && y2 <= 2333)
                {
                    rIdCard.KAYITNO += annotateImageResponse.TextAnnotations[i].Description;
                }
                if (y1 >= 2333 && y1 <= 2400 && y2 >= 2333 && y2 <= 2444)
                {
                    rIdCard.VERILISTARIHI += annotateImageResponse.TextAnnotations[i].Description;
                }
            }
            if (rIdCard.SERI != null)
            {
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("SERI", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("SERİ", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("SER", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("SE", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("ER", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("ERİ", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("ERI", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("RI", "");
                rIdCard.SERI = rIdCard.SERI.ToUpper().Replace("Rİ", "");
            }
            if (rIdCard.NO != null)
            {
                rIdCard.NO = ReplaceString(rIdCard.NO.ToUpper(), 1);
            }
            if (rIdCard.TCKIMLIKNO != null)
            {
                rIdCard.TCKIMLIKNO = ReplaceString(rIdCard.TCKIMLIKNO.ToUpper(), 1);
            }
            if (rIdCard.DOGUMTARIHI != null)
            {
                rIdCard.DOGUMTARIHI = ReplaceString(rIdCard.DOGUMTARIHI.ToUpper(), 2);
            }
            if (rIdCard.CILTNO != null)
            {
                rIdCard.CILTNO = ReplaceString(rIdCard.CILTNO.ToUpper(), 1);
            }
            if (rIdCard.AILESIRANO != null)
            {
                rIdCard.AILESIRANO = ReplaceString(rIdCard.AILESIRANO.ToUpper(), 1);
            }
            if (rIdCard.SIRANO != null)
            {
                rIdCard.SIRANO = ReplaceString(rIdCard.SIRANO.ToUpper(), 1);
            }
            if (rIdCard.KAYITNO != null)
            {
                rIdCard.KAYITNO = ReplaceString(rIdCard.KAYITNO.ToUpper(), 1);
            }
            if (rIdCard.VERILISTARIHI != null)
            {
                rIdCard.VERILISTARIHI = ReplaceString(rIdCard.VERILISTARIHI.ToUpper(), 2);
            }
            rIdCard.SOYADI = Find(annotateImageResponseSOYADI, 383);
            rIdCard.ADI = Find(annotateImageResponseADI, 494);
            rIdCard.ADI = rIdCard.ADI.Replace("ADI", string.Empty);
            rIdCard.BABAADI = Find(annotateImageResponseBABAADI, 605);
            rIdCard.BABAADI = rIdCard.BABAADI.Replace("ADI", string.Empty);
            rIdCard.ANAADI = Find(annotateImageResponseANAADI, 716);
            rIdCard.ANAADI = rIdCard.ANAADI.Replace("ADI", string.Empty);
            rIdCard.DOGUMYERI = Find(annotateImageResponseDOGUMYERI, 827);
            string MedeniHal = Find(annotateImageResponseMEDENIHALI, 1049);
            if (!string.IsNullOrEmpty(MedeniHal))
            {
                if (language == "tr")
                {
                    if (MedeniHal.Substring(0, 1) == "E")
                    {
                        rIdCard.MEDENIHAL = "EVLİ";
                    }
                    else if (MedeniHal.Substring(0, 1) == "B")
                    {
                        rIdCard.MEDENIHAL = "BEKAR";
                    }
                    else if (MedeniHal.Substring(0, 1) == "D")
                    {
                        rIdCard.MEDENIHAL = "DUL";
                    }
                    else
                    {
                        rIdCard.MEDENIHAL = MedeniHal;
                    }
                }
                else if (MedeniHal.Substring(0, 1) == "E")
                {
                    rIdCard.MEDENIHAL = "EVLI";
                }
                else if (MedeniHal.Substring(0, 1) == "B")
                {
                    rIdCard.MEDENIHAL = "BEKAR";
                }
                else if (MedeniHal.Substring(0, 1) == "D")
                {
                    rIdCard.MEDENIHAL = "DUL";
                }
                else
                {
                    rIdCard.MEDENIHAL = MedeniHal;
                }
            }
            else
            {
                rIdCard.MEDENIHAL = MedeniHal;
            }
            string DinTahmin = Find(annotateImageResponseDIN, 1161);
            if (!string.IsNullOrEmpty(DinTahmin))
            {
                if (language == "tr")
                {
                    if (DinTahmin.Substring(0, 2) == "İS")
                    {
                        rIdCard.DIN = "İSLAM";
                    }
                    else if (DinTahmin.Substring(0, 2) == "IS")
                    {
                        rIdCard.DIN = "İSLAM";
                    }
                    else if (DinTahmin.Substring(0, 2) == "iS")
                    {
                        rIdCard.DIN = "İSLAM";
                    }
                    else
                    {
                        rIdCard.DIN = DinTahmin;
                    }
                }
                else if (DinTahmin.Substring(0, 2) == "İS")
                {
                    rIdCard.DIN = "ISLAM";
                }
                else if (DinTahmin.Substring(0, 2) == "IS")
                {
                    rIdCard.DIN = "ISLAM";
                }
                else if (DinTahmin.Substring(0, 2) == "iS")
                {
                    rIdCard.DIN = "ISLAM";
                }
                else
                {
                    rIdCard.DIN = DinTahmin;
                }
            }
            else
            {
                rIdCard.DIN = DinTahmin;
            }
            rIdCard.KANGRUBU = Find(annotateImageResponseKANGRUBU, 1272);
            rIdCard.IL = Find(annotateImageResponseIL, 1383);
            rIdCard.ILCE = Find(annotateImageResponseILCE, 1494);
            rIdCard.MAHALLEKOY = Find(annotateImageResponseMAHALLEKOY, 1605);
            if (language == "tr")
            {
                rIdCard.MAHALLEKOY = rIdCard.MAHALLEKOY.Replace("MAHALLE", " MAHALLE");
                rIdCard.MAHALLEKOY = rIdCard.MAHALLEKOY.Replace("KOY", " KÖY");
                rIdCard.MAHALLEKOY = rIdCard.MAHALLEKOY.Replace("KÖY", " KÖY");
            }
            else
            {
                rIdCard.MAHALLEKOY = rIdCard.MAHALLEKOY.Replace("MAHALLE", " MAHALLE");
                rIdCard.MAHALLEKOY = rIdCard.MAHALLEKOY.Replace("KÖY", " KOY");
            }
            rIdCard.VERILDIGIYER = Find(annotateImageResponseVERILDIGIYER, 2058);
            string verilisNedeni = Find(annotateImageResponseVERILISNEDENI, 2161);
            if (!string.IsNullOrEmpty(verilisNedeni))
            {
                if (language == "tr")
                {
                    if (verilisNedeni.Substring(0, 1) == "K")
                    {
                        rIdCard.VERILISNEDENI = "KAYIP";
                    }
                    else if (verilisNedeni.Length > 1)
                    {
                        if (verilisNedeni.Substring(0, 2) == "DE")
                        {
                            rIdCard.VERILISNEDENI = "DEĞİŞTİRME";
                        }
                        else if (verilisNedeni.Substring(0, 2) == "DO")
                        {
                            rIdCard.VERILISNEDENI = "DOĞUM";
                        }
                        else
                        {
                            rIdCard.VERILISNEDENI = verilisNedeni;
                        }
                    }
                    else
                    {
                        rIdCard.VERILISNEDENI = verilisNedeni;
                    }
                }
                else if (verilisNedeni.Substring(0, 1) == "K")
                {
                    rIdCard.VERILISNEDENI = "KAYIP";
                }
                else if (verilisNedeni.Substring(0, 2) == "DE")
                {
                    rIdCard.VERILISNEDENI = "DEGISTIRME";
                }
                else if (verilisNedeni.Substring(0, 2) == "DO")
                {
                    rIdCard.VERILISNEDENI = "DOGUM";
                }
                else
                {
                    rIdCard.VERILISNEDENI = verilisNedeni;
                }
            }
            else
            {
                rIdCard.VERILISNEDENI = verilisNedeni;
            }
            rIdCard.DOGUMYERI = ReplaceCharacter(rIdCard.DOGUMYERI, language);
            rIdCard.SERI = ReplaceCharacter(rIdCard.SERI, language);
            rIdCard.ADI = ReplaceCharacter(rIdCard.ADI, language);
            rIdCard.AILESIRANO = ReplaceCharacter(rIdCard.AILESIRANO, language);
            rIdCard.ANAADI = ReplaceCharacter(rIdCard.ANAADI, language);
            rIdCard.BABAADI = ReplaceCharacter(rIdCard.BABAADI, language);
            rIdCard.CILTNO = ReplaceCharacter(rIdCard.CILTNO, language);
            rIdCard.DIN = ReplaceCharacter(rIdCard.DIN, language);
            rIdCard.IL = ReplaceCharacter(rIdCard.IL, language);
            rIdCard.ILCE = ReplaceCharacter(rIdCard.ILCE, language);
            rIdCard.KANGRUBU = ReplaceCharacter(rIdCard.KANGRUBU, language);
            rIdCard.KAYITNO = ReplaceCharacter(rIdCard.KAYITNO, language);
            rIdCard.SOYADI = ReplaceCharacter(rIdCard.SOYADI, language);
            rIdCard.TCKIMLIKNO = ReplaceCharacter(rIdCard.TCKIMLIKNO, language);
            rIdCard.MEDENIHAL = ReplaceCharacter(rIdCard.MEDENIHAL, language);
            rIdCard.NO = ReplaceCharacter(rIdCard.NO, language);
            rIdCard.MAHALLEKOY = ReplaceCharacter(rIdCard.MAHALLEKOY, language);
            rIdCard.VERILISNEDENI = ReplaceCharacter(rIdCard.VERILISNEDENI, language);
            rIdCard.SIRANO = ReplaceCharacter(rIdCard.SIRANO, language);
            TextResult = annotateImageResponse.TextAnnotations[0].Description.Replace("\n", "\r\n");
            JsonResult = JsonConvert.SerializeObject(annotateImageResponse.TextAnnotations[0]);
        }

        public string Find(AnnotateImageResponse annotateImageResponse1, int y)
        {
            string txt = "";
            annotateImageResponse1 = BlackHole(annotateImageResponse1);
            List<EntityAnnotation> aIR = (from x in annotateImageResponse1.TextAnnotations
                                          where x.Description != string.Empty
                                          orderby Math.Abs(x.BoundingPoly.Vertices[1].Y.Value - x.BoundingPoly.Vertices[3].Y.Value) descending
                                          select x).ToList();
            if (aIR.Count != 0)
            {
                decimal height = Math.Abs(aIR[0].BoundingPoly.Vertices[1].Y.Value - aIR[0].BoundingPoly.Vertices[3].Y.Value);
                decimal FindHeight = Math.Ceiling(Convert.ToDecimal(height / 10m * 9m));
                aIR.RemoveAll((EntityAnnotation x) => (decimal)Math.Abs(x.BoundingPoly.Vertices[1].Y.Value - x.BoundingPoly.Vertices[3].Y.Value) <= FindHeight);
                if (aIR.Count != 0)
                {
                    aIR = aIR.OrderBy((EntityAnnotation x) => x.BoundingPoly.Vertices[1].X.Value).ToList();
                    int sayi = 1;
                    for (int i = 0; i < aIR.Count; i++)
                    {
                        if (sayi == 2)
                        {
                            txt = txt + " " + aIR[i].Description;
                            continue;
                        }
                        sayi = 2;
                        txt += aIR[i].Description;
                    }
                }
            }
            return txt;
        }

        public AnnotateImageResponse BlackHole(AnnotateImageResponse annotateImageResponse1)
        {
            if (annotateImageResponse1.TextAnnotations.Count != 0)
            {
                for (int i = 0; i < annotateImageResponse1.TextAnnotations.Count; i++)
                {
                    switch (annotateImageResponse1.TextAnnotations[i].Description)
                    {
                        case "SERİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "SERI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "T.C.KİMLİK NO.":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "T.":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "C.":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "T.C.KİMLİK":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "T.C.":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "NO":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "SOYADI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "ADI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "AD":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "ADİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "BABA":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILDiGi":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "ANA ADI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "ANA":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOGUM YERI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOĞUM YERI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "YERI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "YERİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOĞUM":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOGUM":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOGUM TARIHI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOGUM TARİHİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOĞUM TARIHI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DOĞUM TARİHİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "TARIHI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "TARİHİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "MEDENI HALI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "MEDENİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "EDENI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "EDENİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "EDENi":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "MEDENI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "MEDEN":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "HALİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "HALI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "HAL":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DINI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DİNİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DiNi":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DINİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "DİNI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "KAN":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "GRUBU":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "KAN GRUBU":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "KANGRUBU":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "IL":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "İL":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "İLÇE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "ILCE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "İLCE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "LCE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "ILÇE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "MAHALLE - KÖY":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "MAHALLE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "KOY":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "KÖY":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILDIGI YER":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILİS":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILIS":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLDİĞI YER":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLDIGI YER":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILDİGI YER":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILDİGİ YER":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLDIĞI YER":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILDIGI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLDİĞI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLDIGI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILDİGI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILDİGİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLDIĞI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERiLis":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLİS":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "YER":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLİŞ NEDENİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILİŞ NEDENİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILİS NEDENİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERIL":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİL":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILIS NEDENI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILISNEDENI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILISNEDEN":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILISNEDE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILISNED":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILISNE":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILISN":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERİLİŞ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "NEDENI":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "MEDENi":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "HALi":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "NEDENİ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "VERILIŞ":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                        case "NEDEN":
                            annotateImageResponse1.TextAnnotations[i].Description = string.Empty;
                            break;
                    }
                }
            }
            return annotateImageResponse1;
        }

        public string ReplaceCharacter(string txt, string language)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                if (language != "tr")
                {
                    txt = txt.Replace("i", "I");
                }
                txt = txt.Replace(".", string.Empty);
                txt = txt.Replace(",", string.Empty);
                txt = txt.Replace(":", string.Empty);
                txt = txt.Replace(";", string.Empty);
                txt = txt.Replace("!", string.Empty);
                txt = txt.Replace("'", string.Empty);
                txt = txt.Replace("^", string.Empty);
                txt = txt.Replace("+", string.Empty);
                txt = txt.Replace("%", string.Empty);
                txt = txt.Replace("&", string.Empty);
                txt = txt.Replace("/", string.Empty);
                txt = txt.Replace("(", string.Empty);
                txt = txt.Replace(")", string.Empty);
                txt = txt.Replace("=", string.Empty);
                txt = txt.Replace("?", string.Empty);
                txt = txt.Replace("_", string.Empty);
                txt = txt.Replace("£", string.Empty);
                txt = txt.Replace("#", string.Empty);
                txt = txt.Replace("$", string.Empty);
                txt = txt.Replace("½", string.Empty);
                txt = txt.Replace("{", string.Empty);
                txt = txt.Replace("[", string.Empty);
                txt = txt.Replace("]", string.Empty);
                txt = txt.Replace("}", string.Empty);
                txt = txt.Replace("-", string.Empty);
                txt = txt.ToUpper();
                return txt;
            }
            return txt;
        }

        public string ReplaceString(string txt, int sayi)
        {
            switch (sayi)
            {
                case 1:
                    txt = txt.Replace("Q", string.Empty);
                    txt = txt.Replace("W", string.Empty);
                    txt = txt.Replace("E", string.Empty);
                    txt = txt.Replace("R", string.Empty);
                    txt = txt.Replace("T", string.Empty);
                    txt = txt.Replace("Y", string.Empty);
                    txt = txt.Replace("U", string.Empty);
                    txt = txt.Replace("I", string.Empty);
                    txt = txt.Replace("O", string.Empty);
                    txt = txt.Replace("P", string.Empty);
                    txt = txt.Replace("Ğ", string.Empty);
                    txt = txt.Replace("Ü", string.Empty);
                    txt = txt.Replace("A", string.Empty);
                    txt = txt.Replace("S", string.Empty);
                    txt = txt.Replace("D", string.Empty);
                    txt = txt.Replace("F", string.Empty);
                    txt = txt.Replace("G", string.Empty);
                    txt = txt.Replace("H", string.Empty);
                    txt = txt.Replace("J", string.Empty);
                    txt = txt.Replace("K", string.Empty);
                    txt = txt.Replace("L", string.Empty);
                    txt = txt.Replace("Ş", string.Empty);
                    txt = txt.Replace("İ", string.Empty);
                    txt = txt.Replace("Z", string.Empty);
                    txt = txt.Replace("X", string.Empty);
                    txt = txt.Replace("C", string.Empty);
                    txt = txt.Replace("V", string.Empty);
                    txt = txt.Replace("B", string.Empty);
                    txt = txt.Replace("N", string.Empty);
                    txt = txt.Replace("M", string.Empty);
                    txt = txt.Replace("Ö", string.Empty);
                    txt = txt.Replace("Ç", string.Empty);
                    txt = txt.Replace(" ", string.Empty);
                    txt = txt.Replace(".", string.Empty);
                    txt = txt.Replace(",", string.Empty);
                    break;
                case 0:
                    txt = txt.Replace(" ", string.Empty);
                    txt = txt.Replace("0", string.Empty);
                    txt = txt.Replace("1", string.Empty);
                    txt = txt.Replace("2", string.Empty);
                    txt = txt.Replace("3", string.Empty);
                    txt = txt.Replace("4", string.Empty);
                    txt = txt.Replace("5", string.Empty);
                    txt = txt.Replace("6", string.Empty);
                    txt = txt.Replace("7", string.Empty);
                    txt = txt.Replace("8", string.Empty);
                    txt = txt.Replace("9", string.Empty);
                    break;
                case 2:
                    txt = txt.Replace("Q", string.Empty);
                    txt = txt.Replace("W", string.Empty);
                    txt = txt.Replace("E", string.Empty);
                    txt = txt.Replace("R", string.Empty);
                    txt = txt.Replace("T", string.Empty);
                    txt = txt.Replace("Y", string.Empty);
                    txt = txt.Replace("U", string.Empty);
                    txt = txt.Replace("I", string.Empty);
                    txt = txt.Replace("O", string.Empty);
                    txt = txt.Replace("P", string.Empty);
                    txt = txt.Replace("Ğ", string.Empty);
                    txt = txt.Replace("Ü", string.Empty);
                    txt = txt.Replace("A", string.Empty);
                    txt = txt.Replace("S", string.Empty);
                    txt = txt.Replace("D", string.Empty);
                    txt = txt.Replace("F", string.Empty);
                    txt = txt.Replace("G", string.Empty);
                    txt = txt.Replace("H", string.Empty);
                    txt = txt.Replace("J", string.Empty);
                    txt = txt.Replace("K", string.Empty);
                    txt = txt.Replace("L", string.Empty);
                    txt = txt.Replace("Ş", string.Empty);
                    txt = txt.Replace("İ", string.Empty);
                    txt = txt.Replace("Z", string.Empty);
                    txt = txt.Replace("X", string.Empty);
                    txt = txt.Replace("C", string.Empty);
                    txt = txt.Replace("V", string.Empty);
                    txt = txt.Replace("B", string.Empty);
                    txt = txt.Replace("N", string.Empty);
                    txt = txt.Replace("M", string.Empty);
                    txt = txt.Replace("Ö", string.Empty);
                    txt = txt.Replace("Ç", string.Empty);
                    txt = txt.Replace(" ", string.Empty);
                    break;
            }
            return txt;
        }
    }
}
