using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TranslatorMsgRobot.Model.Translater;

namespace TranslatorMsgRobot.Model
{
    public class Message : INotifyPropertyChanged
    {
        #region Private properties
        private string name = "";
        private string text = "";
        private string langage = "";
        private string translateSource = "";
        #endregion

        #region Public properties
        public string Name
        {
            get
            {
                return name;
            }
            set
            {

                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {

                if (text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }
        public string Langage
        {
            get
            {
                return langage;
            }
            set
            {

                if (langage != value)
                {
                    langage = value;
                    OnPropertyChanged(nameof(Langage));
                }
            }
        }
        public string TranslateSource
        {
            get
            {
                return translateSource;
            }
            set
            {

                if (translateSource != value)
                {
                    translateSource = value;
                    OnPropertyChanged(nameof(TranslateSource));
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Constructor
        public Message(string name, string langage, string text, string translateSource)
        {
            Name = name;
            Text = text;
            Langage = langage;
            TranslateSource = translateSource;
        }
        #endregion

        #region methods

        public async void TranslateTo(ITranslator translator, string targetLangage)
        {
            
            var r = await translator.TranslateAsync(Text, Langage, targetLangage);

            this.Text = r;
            this.Langage = targetLangage;
            this.TranslateSource = translator.Name;
        }


        public virtual Message Copy()
        {
            return new Message(Name, Langage, Text, TranslateSource);
        }

        public override string ToString()
        {
            return $"{Name},{Langage},{Text},{TranslateSource} ";
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
