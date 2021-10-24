using System.Windows;
using RecognizerVM;

namespace RecognizerUI
{
    public partial class MainWindow : Window
    {
        MainViewModel viewModel = new MainViewModel(new WPFUIServices());
     
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public void OnClickChooseDirectory(object sender, RoutedEventArgs e)
        {
            viewModel.ChooseDirectoryHandler();
        }

        public void OnClickExecute(object sender, RoutedEventArgs e)
        {
            viewModel.ExectueHandler();
        }

        public void OnClickStop(object sender, RoutedEventArgs e)
        {
            viewModel.StopHandler();
        }
    }


    public class WPFUIServices : IUIServices
    {
        public bool ChooseDirectory(ref string filename, string dirPath)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = System.IO.Path.GetFullPath(dirPath);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.SelectedPath;
                return true;
            }

            return false;
        }

        public void ConfirmError(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool? ConfirmWarning(string text, string title)
        {
            switch (MessageBox.Show(text, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }
    }
}
