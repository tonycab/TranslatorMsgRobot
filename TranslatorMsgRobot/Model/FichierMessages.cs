using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorMsgRobot.Model
{
    public class FichierMessages : INotifyPropertyChanged
    {

        private ObservableCollection<Message> messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                if (messages != value)
                {
                    messages = value;
                    OnPropertyChanged();
                }
            }
        }

        private string langageFile = "";
        public string LangageFile
        {
            get
            {
                return langageFile;
            }
            set
            {
                if (langageFile != value)
                {
                    langageFile = value;
                    OnPropertyChanged();
                }
            }
        }

        private string nameFile = "";
        public string NameFile
        {
            get
            {
                return nameFile;
            }
            set
            {
                if (nameFile != value)
                {
                    nameFile = value;
                    OnPropertyChanged();
                }
            }
        }

        private string pathFile = "";
        public string PathFile
        {
            get
            {
                return pathFile;
            }
            set
            {
                if (pathFile != value)
                {
                    pathFile = value;
                    OnPropertyChanged();
                }
            }
        }

        public FichierMessages()
        {
        }

        public FichierMessages(string pathFile)
        {
            ReadFile(pathFile);
        }

        public virtual void ReadFile(string pathFile)
        {
            using (var lecteur = new StreamReader(pathFile))
            {
                while (!lecteur.EndOfStream)
                {
                    var ligne = lecteur.ReadLine();
                    var valeurs = ligne.Split(',');

                    if (valeurs.Length >= 4) 
                    {
                        var mes = new Message(valeurs[0], valeurs[1], valeurs[2], valeurs[3]);
                        Messages.Add(mes);
                    }
                }
            }
        }

        public virtual void Write(string pathFile)
        {
            //Ecriture du fichier
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathFile))
            {

                foreach (var message in this.Messages)
                {
                    file.WriteLine((message));
                }
               
            }



        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
