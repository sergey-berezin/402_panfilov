using System;
using System.Threading.Tasks;

namespace RecognizerVM
{
    public interface IUIServices
    {
        bool ChooseDirectory(ref string filename, string dirPath);
        void ConfirmError(string errorText, string errorTitle);
        bool? ConfirmWarning(string text, string title);
    }

    public class MainViewModel : ViewModelBase
    {
        public const string ServerUrl = "http://localhost:5000/";

        ClientSession clientSession = new();
        string inputPath = "";
        string progress;
        IUIServices svc;
        ClassListView classListView;
        ImageListView imageListView;

        public ClassListView ClassListView => classListView;
        public ImageListView ImageListView => imageListView;
        public ClientSession ClientSession => clientSession;
        public string InputPath
        {
            get => inputPath;
            set
            {
                inputPath = value;
                RaisePropertyChanged();
            }
        }
        public string Progress
        {
            get => progress;
            set
            {
                progress = value;
                RaisePropertyChanged();
            }
        }


        public MainViewModel(IUIServices _svc)
        {
            svc = _svc;
            classListView = new(this);
            imageListView = new(this);

            _ = RefreshContentFromServerAsync();
        }


        public void ChooseDirectoryHandler()
        {
            string filename = string.Empty;
            string dirPath = System.IO.Path.Combine(@"..\..\..\..\Assets\");
            if (svc.ChooseDirectory(ref filename, dirPath))
            {
                InputPath = filename;
            }
        }

        public async Task StopHandler()
        {
            if (!await CheckServer()) return;

            await clientSession.PostStringAsync($"{ServerUrl}api/content/stop_recognition", "");
        }

        public async Task ClearHandler()
        {
            if (!await CheckServer()) return;

            await clientSession.DeleteAsync($"{ServerUrl}api/content/clear_database");

            await RefreshContentFromServerAsync();
        }

        public async Task SelectionChangedHandler(string arg)
        {
            if (!await CheckServer()) return;
            if (arg == null) return;

            await imageListView.SetCollection(arg.Substring(arg.IndexOf(' ') + 1));
        }

        public async Task ExectueHandler()
        {
            if (!await CheckServer()) return;

            if ((await clientSession.GetStringAsync($"{ServerUrl}api/content/processing")) == "true")
            {
                svc.ConfirmError("Предыдущая обработка еще не завершена", "Ошибка");
                return;
            }

            if (inputPath == "")
            {
                svc.ConfirmError("Выберите папку с изображениями для обработки", "Ошибка");
                return;
            }

            await clientSession.PostStringAsync($"{ServerUrl}api/content/start_detection", inputPath);

            while (true)
            {
                Progress = await clientSession.GetStringAsync($"{ServerUrl}api/content/progress");
                await Task.Delay(250);

                if ((await clientSession.GetStringAsync($"{ServerUrl}api/content/processing")) == "false")
                    break;
            }

            await RefreshContentFromServerAsync();
        }


        public async Task RefreshContentFromServerAsync()
        {
            if (!await CheckServer()) return;

            Progress = await clientSession.GetStringAsync($"{ServerUrl}api/content/progress");
            await classListView.SetCollection();
            await imageListView.SetCollection();
        }

        void CallServerErrorNotification(string errorText)
        {
            svc.ConfirmError(errorText, "Server error");
        }

        async Task<bool> CheckServer()
        {
            try
            {
                await clientSession.GetStringAsync($"{ServerUrl}api/content/processing");
                return true;
            }
            catch (Exception e)
            {
                CallServerErrorNotification(e.Message);
                return false;
            }
        }
    }
}
