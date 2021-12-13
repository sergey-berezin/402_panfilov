using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public async void OnClickExecute(object sender, RoutedEventArgs e)
        {
            await viewModel.ExectueHandler();
        }

        public async void OnClickStop(object sender, RoutedEventArgs e)
        {
            await viewModel.StopHandler();
        }

        public async void OnClickClear(object sender, RoutedEventArgs e)
        {
            await viewModel.ClearHandler();
        }

        public async void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            await viewModel.SelectionChangedHandler(((ListBox)sender).SelectedItem as string);
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
