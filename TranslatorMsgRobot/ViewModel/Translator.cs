using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TranslatorMsgRobot.Model;
using TranslatorMsgRobot.Model.Translater;
using TranslatorMsgRobot.Vue;
using static System.Net.Mime.MediaTypeNames;

namespace TranslatorMsgRobot.ViewModel
{

    class Translator : INotifyPropertyChanged
    {
        #region Properties
        #region Private properties
        private IFichierMessages sourceFileMessages;
        private IFichierMessages targetFileMessages;
        private Message selectedTargetMessages;
        private string pathFileSourceMessages;
        private string pathFileTargetMessages;
        private double translateProgress;
        private ITranslator ServiceTranslator;
        #endregion

        #region Public properties 
        public event PropertyChangedEventHandler PropertyChanged;

        public IFichierMessages SourceFileMessages
        {
            get
            {
                return sourceFileMessages;
            }
            set
            {
                if (sourceFileMessages != value)
                {
                    sourceFileMessages = value;
                    OnPropertyChanged();
                }
            }
        }
        public IFichierMessages TargetFileMessages
        {
            get
            {
                return targetFileMessages;
            }
            set
            {
                if (targetFileMessages != value)
                {
                    targetFileMessages = value;
                    OnPropertyChanged();
                }
            }
        }

        public Message SelectedTargetMessages
        {
            get
            {
                return selectedTargetMessages;
            }
            set
            {
                if (selectedTargetMessages != value)
                {
                    selectedTargetMessages = value;
                    OnPropertyChanged();
                }
            }
        }

        public string[] Langages { get; set; } = { "EN", "FR", "DE", "IT", "PL", "ES", "RU", "PT" };
        public string SelectedLangage { get; set; } = "EN";
        public string[] TranslatorService { get; set; } = { "DeepL" };
        public string SelectedTranslatorService { get; set; } = "DeepL";
        public string PathFileSourceMessages
        {
            get
            {
                return pathFileSourceMessages;
            }
            set
            {
                if (pathFileSourceMessages != value)
                {
                    pathFileSourceMessages = value;

                    OnPropertyChanged();

                }
            }
        }
        public string PathFileTargetMessages
        {
            get
            {
                return pathFileTargetMessages;
            }
            set
            {
                if (pathFileTargetMessages != value)
                {
                    pathFileTargetMessages = value;

                    OnPropertyChanged();

                }
            }
        }
        public double TranslateProgress
        {
            get
            {
                return translateProgress;
            }
            set
            {
                if (translateProgress != value)
                {
                    translateProgress = value;

                    OnPropertyChanged();

                }
            }
        }
        #endregion

        #region Commands
        public ICommand TranslateCommand { get; set; }
        public ICommand LoadSourceMessagesCommand { get; set; }
        public ICommand LoadTargetMessagesCommand { get; set; }
        public ICommand ExportTargetMessagesCommand { get; set; }
        public ICommand ClearTargetMessagesCommand { get; set; }
        public ICommand TranslateItemCommand { get; set; }

        public ICommand ConfigurationDeepLCommand { get; set; }

        #endregion
        #endregion

        #region Constructor
        public Translator()
        {

            //Création d'un fichier de configuration pour DeepL
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appPath = Path.Combine(appDataPath, "SIIF\\MessagesTranslator");
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
                TranslatorDeepL dplCreate = new TranslatorDeepL();
                dplCreate.SaveToFile(Path.Combine(appPath,"DeepL.xml"));
            }


            TranslatorDeepL dpl = TranslatorDeepL.LoadFromFile(Path.Combine(appPath, "DeepL.xml"));
            ServiceTranslator = dpl;

            TranslateCommand = new RelayCommand(new Action<object>((o) => TranslatFileMessages()), (o) => PathFileSourceMessages != null);
            LoadSourceMessagesCommand = new RelayCommand(new Action<object>((o) => LoadFileSourceMessages()));
            LoadTargetMessagesCommand = new RelayCommand(new Action<object>((o) => LoadFileTargetMessages()));
            ExportTargetMessagesCommand = new RelayCommand(new Action<object>((o) => ExportFileTargetMessages((string)o)));
            ClearTargetMessagesCommand = new RelayCommand(new Action<object>((o) => { TargetFileMessages?.Messages.Clear(); PathFileTargetMessages = ""; }));
            TranslateItemCommand = new RelayCommand(new Action<object>((o) =>{SelectedTargetMessages?.TranslateTo(ServiceTranslator,SelectedLangage);}));
            ConfigurationDeepLCommand = new RelayCommand(new Action<object>((o) => { 
         
                WindowEditTranslator windowEditTranslator = new WindowEditTranslator(dpl);
                bool? a = windowEditTranslator.ShowDialog();

                if (a == true)
                {
                    dpl.SaveToFile(Path.Combine(appPath, "DeepL.xml"));
                }
                else
                {
                    dpl= TranslatorDeepL.LoadFromFile(Path.Combine(appPath, "DeepL.xml"));
                    ServiceTranslator = dpl;
                }

            }));


        }
        #endregion

        #region Methods
        private void LoadFileSourceMessages()
        {
            if (PathFileSourceMessages != null && File.Exists(PathFileSourceMessages))
            {
                SourceFileMessages = new FichierMessagesRobot(PathFileSourceMessages);

                ((RelayCommand)TranslateCommand).RaiseCanExecuteChanged();
            }

        }
        private void LoadFileTargetMessages()
        {
            if (PathFileTargetMessages != null && File.Exists(PathFileTargetMessages))
            {
                TargetFileMessages = new FichierMessagesRobot(PathFileTargetMessages);
            }
        }
        private void ExportFileTargetMessages(string path)
        {
            TargetFileMessages.Write(path);
        }
        private async void TranslatFileMessages()
        {
            IEnumerable<Message> messages;
            

            //Si un fichier de traduction existe on ajoute les nouveaux messages
            if (TargetFileMessages != null)
            {
                messages = TargetFileMessages.Messages.Select(m => m).UnionBy(SourceFileMessages.Messages.Select(m => m), m => m.Name);
            }
            //Sinon on traduit tous les messages
            else
            {
                messages = SourceFileMessages.Messages;
            }

            //Initialisation de la barre de progression
            TranslateProgress = 0.0;
            double countInc = 100.0 / messages.Count();

            //Traduction des messages
            TargetFileMessages = new FichierMessagesRobot();
            TargetFileMessages.LangageFile = SelectedLangage;

            foreach (var message in messages)
            {
                //Traduction du message qui n'ont pas été personnalisé
                if (message.TranslateSource != "Custom")
                {
                    var m = await ServiceTranslator.TranslateMessageAsync(message, SelectedLangage);
                    
                    TargetFileMessages.Messages.Add(m);
                }
                //Sinon on ajoute le message tel quel
                else
                {
                    TargetFileMessages.Messages.Add(message);
                }

                //Mise à jour de la barre de progression
                TranslateProgress = TranslateProgress + countInc;

            }


        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }


}
