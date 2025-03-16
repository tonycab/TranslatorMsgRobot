using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TranslatorMsgRobot.Model.Translater;

using TranslatorMsgRobot.ViewModel;
using TranslatorMsgRobot.Model;

namespace TranslatorMsgRobot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new Translator();
        }

        #region OpenFiledialog
        //OpenFileDialog pour le fichier source
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".sys";
            dlg.Filter = "sys (*.sys)|*.sys";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                ((Translator)DataContext).PathFileSourceMessages = filename;
            }
        }

        //OpenFileDialog pour le fichier cible
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".sys";
            dlg.Filter = "sys (*.sys)|*.sys";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                ((Translator)DataContext).PathFileTargetMessages = filename;
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension
            dlg.FileName = "STD_MSG_"+ ((Translator)DataContext).SelectedLangage;
            dlg.DefaultExt = ".sys";
            dlg.Filter = "sys (*.sys)|*.sys";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                ((Translator)DataContext).ExportTargetMessagesCommand.Execute(filename);
            }
        }

        #endregion

        #region Scroll Synchronisation
        private ScrollViewer sv1;
        private ScrollViewer sv2;
        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender == dataGridSource)
            {
                sv1 = GetScrollbar(dataGridSource, Orientation.Vertical);
            }
            else if (sender == dataGridTarget)
            {
                sv2 = GetScrollbar(dataGridTarget, Orientation.Vertical);
            }

            if (sv1 != null && sv2 != null)
            {
                sv1.ScrollChanged += (s, ev) => SyncScroll(sv1, sv2, ev);
                sv2.ScrollChanged += (s, ev) => SyncScroll(sv2, sv1, ev);
            }
        }

        private void DataGrid_LoadingRow(object sender, EventArgs e)
        {
            // Synchronise les hauteurs des lignes
            var grid = sender as DataGrid;
            grid.RowHeight = double.NaN; // Ajuste automatiquement les hauteurs des lignes
        }

        private ScrollViewer GetScrollbar(DependencyObject dep, Orientation orientation)
        {
            if (dep is ScrollViewer) return dep as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                var result = GetScrollbar(child, orientation);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void SyncScroll(ScrollViewer src, ScrollViewer dest, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                dest.ScrollToVerticalOffset(src.VerticalOffset);
            }
            if (e.HorizontalChange != 0)
            {
                dest.ScrollToHorizontalOffset(src.HorizontalOffset);
            }
        }
        #endregion

        private void dataGridTarget_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Text")
            {
                var editedItem = e.Row.Item as Message;
                if (editedItem != null)
                {
                    editedItem.TranslateSource = "Custom";
                }
            }
        }
    }
}
