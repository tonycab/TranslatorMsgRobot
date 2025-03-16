using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslatorMsgRobot.Model.Translater;

namespace TranslatorMsgRobot.Model
{
    public class FichierMessagesRobot : FichierMessages,IFichierMessages,INotifyPropertyChanged
    {

        public FichierMessagesRobot()
        {
        }

        public FichierMessagesRobot(string pathFile)
        {
            ReadFile(pathFile);
        }



        public override void ReadFile(string pathFile)
        {
            //Lecture du fichier  
            string input = File.ReadAllText(pathFile, Encoding.GetEncoding("ISO-8859-1"));

            //Chemin du fichier
            PathFile = pathFile;

            //Nom du fichier sans extension
            NameFile = Path.GetFileNameWithoutExtension(pathFile);

            //Extraction de la langue du fichier à parti du nom du fichier
            string[] countryCodes = { "EN", "FR", "DE", "IT", "PL", "ES", "RU", "PT" };
            string countryPattern = string.Join("|", countryCodes);
            string patternLangage = $@"STD_MSG(?:_({countryPattern}))?";
            Regex regexLangage = new Regex(patternLangage);
            LangageFile = regexLangage.Match(NameFile).Groups[1].Value != null ? regexLangage.Match(NameFile).Groups[1].Value : "FR";


            //Extraction des messages  
            string patternMessage = $@"CONST\s+(\w+)\s+(\w+?)(?:_({countryPattern}))?\s{{0,}}:=\s{{0,}}""([^""]+)"";(?:[ ]{{0,}}!(\w+))?\r\n";
            Regex regexMessage = new Regex(patternMessage);
            MatchCollection matchesMessage = regexMessage.Matches(input);

            //Ajout des messages dans la liste
            List<Message> messages = new List<Message>();
            foreach (Match match in matchesMessage)
            {
                var rapidType = match.Groups[1].Value;
                var name = match.Groups[2].Value;
                var countryCode = string.IsNullOrEmpty(match.Groups[3].Value) ? LangageFile : match.Groups[3].Value;
                var text = match.Groups[4].Value;
                var translateSource = match.Groups[5].Value;

                MessageRobot m = new MessageRobot(rapidType, name, countryCode, text, translateSource);

                Messages.Add(m);
            }
        }

        public override void Write(string pathFile)
        {
            //Ecriture du fichier
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathFile))
            {
                file.WriteLine($"MODULE STD_MSG_{LangageFile}(SYSMODULE)");

                file.WriteLine($"!File created : {DateTime.Now.ToString()}");
                foreach (var message in this.Messages)
                {
                    file.WriteLine(((MessageRobot)message).RapidFormat);
                }
                file.WriteLine($"ENDMODULE");
            }
        }
    }
}
