using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorMsgRobot.Model
{
    public class MessageRobot : Message, INotifyPropertyChanged
    {
        private string rapidType = "";
        public string RapidType
        {
            get
            {
                return rapidType;
            }
            set
            {
                if (rapidType != value)
                {
                    rapidType = value;
                    OnPropertyChanged(nameof(RapidType));
                }
            }
        }

        public MessageRobot(string rapidType, string name,  string langage, string text, string translateSource) : base( name,langage, text, translateSource)
        {
            this.rapidType = rapidType;
        }

        public string RapidFormat
        {
            get
            {
                var typerapid = Langage == "FR" ? "USER_MSG" : "USER_MSG_TRANSLATE";
                var namerapid = Langage == "FR" ? Name : $"{Name}_{Langage}";

                return $"CONST {typerapid} {namerapid} := \"{Text}\"; !{TranslateSource}";
            }

        }

        public override Message Copy()
        {
            return new MessageRobot(RapidType,Name, Langage, Text, TranslateSource);
        }

    }
}
