using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace TranslatorMsgRobot.Model.Translater
{
    class TranslatorDeepL : ITranslator
    {

        private string apiKey = "07147095-b177-479a-85dd-28eca99b7740:fx";

        public string ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }

        public string Name => "DeepL";

        /// <summary>
        /// Traduction d'un texte
        /// </summary>
        /// <param name="textToTranslate">Text à traduire</param>
        /// <param name="sourceLangage">Langue source</param>
        /// <param name="targetLangage">Langue de sortie</param>
        /// <returns>Text traduit</returns>
        public async Task<string> TranslateAsync(string textToTranslate, string sourceLangage, string targetLangage)
        {
            try
            {
                if (textToTranslate == null || textToTranslate == "")
                {
                    return null;
                }

                DeepLTranslationResult translationResult;


                var jsonData = new JObject
                {
                    ["text"] = new JArray(textToTranslate),
                    ["target_lang"] = targetLangage,
                    ["source_lang"] = sourceLangage
                };

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"DeepL-Auth-Key {apiKey}");
                    var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://api-free.deepl.com/v2/translate", content);

                    var result = await response.Content.ReadAsStringAsync();

                    translationResult = JsonConvert.DeserializeObject<DeepLTranslationResult>(result);


                }

                return translationResult.Translations[0].Text;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception : {ex.Message}");
                return null;
            }
        }

        public async Task<Message> TranslateMessageAsync(Message message, string targetLangage)
        {
            try
            {
                int count = 0;
                string mes = null;

                do
                {
                    mes = await TranslateAsync(message.Text, message.Langage, targetLangage);
                } while ((mes == null || mes == "") && count < 3);

                var cp = message.Copy();
                cp.Langage = targetLangage;
                cp.Text = mes;
                cp.TranslateSource = Name;

                return cp;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception : {ex.Message}");
                return null;
            }
        }

        private class DeepLTranslationResult
        {
            [JsonProperty("translations")]
            public Translation[] Translations { get; set; }
        }

        private class Translation
        {
            [JsonProperty("detected_source_language")]
            public string DetectedSourceLanguage { get; set; }

            [JsonProperty("text")]
            public string Text
            {
                get; set;

            }
        }

        public XElement ToXml()
        {
            XElement xelement = new XElement("TranslatorDeepL");
            xelement.Add(new XElement("ApiKey", ApiKey));
            return xelement;
        }

        public void FromXml(XElement root)
        {
            ApiKey = root.Element("ApiKey").Value;
        }

        public static TranslatorDeepL LoadFromFile(string path)
        {
            XElement root = XElement.Load(path);
            TranslatorDeepL translator = new TranslatorDeepL();
            translator.FromXml(root);
            
            return translator;
        }

        public void SaveToFile(string path)
        {
            XElement xmlRoot = ToXml();
            XDocument xDocument = new XDocument(xmlRoot);

            xDocument.Save(path);
        }

    }
}
