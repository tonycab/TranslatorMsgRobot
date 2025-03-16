using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TranslatorMsgRobot.Vue
{
    /// <summary>
    /// Logique d'interaction pour WindowEditTranslator.xaml
    /// </summary>
    public partial class WindowEditTranslator : Window
    {
        public WindowEditTranslator(object datacontext)
        {
            InitializeComponent();
            DataContext = datacontext;

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour le bouton OK
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour le bouton Annuler
            this.DialogResult = false;
            this.Close();
        }
    }
}
