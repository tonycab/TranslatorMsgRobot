using System.Collections.ObjectModel;

namespace TranslatorMsgRobot.Model
{
    internal interface IFichierMessages
    {
        string LangageFile { get; set; }
        ObservableCollection<Message> Messages { get; set; }
        string NameFile { get; set; }
        string PathFile { get; set; }

        void ReadFile(string pathFile);
        void Write(string pathFile);
    }
}