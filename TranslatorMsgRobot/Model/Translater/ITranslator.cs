namespace TranslatorMsgRobot.Model.Translater
{
    public interface ITranslator
    {

        public string Name{ get;}
        /// <summary>
        /// Traduction d'un texte
        /// </summary>
        /// <param name="textToTranslate">Text à traduire</param>
        /// <param name="sourceLangage">Langue source</param>
        /// <param name="targetLangage">Langue de sortie</param>
        /// <returns>Text traduit</returns>
        Task<string>  TranslateAsync(string textToTranslate, string sourceLangage, string targetLangage);

        Task<Message> TranslateMessageAsync(Message message, string targetLangage);
    }
}