using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RecognizerVM
{
    public abstract class DataBaseView : IEnumerable, INotifyCollectionChanged
    {
        public MainViewModel VM { get; }
        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public DataBaseView(MainViewModel vm)
        {
            VM = vm;
        }


        public void RaiseCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public static BitmapImage GetBitmapImageFromByte(byte[] arr)
        {
            using (var ms = new System.IO.MemoryStream(arr))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }


        public abstract IEnumerator GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public class ClassListView : DataBaseView
    {
        List<string> classList = new();

        public ClassListView(MainViewModel vm) : base(vm) { }

        public async Task SetCollection()
        {
            classList = await VM.ClientSession.GetClassListAsync($"{MainViewModel.ServerUrl}api/content/class_list");
            RaiseCollectionChanged();
        }

        public override IEnumerator<string> GetEnumerator()
        {
            return classList.GetEnumerator();
        }
    }


    public class ImageListView : DataBaseView
    {
        List<BitmapImage> imageList = new();
        string selectedClass = "";

        public ImageListView(MainViewModel vm) : base(vm) { }


        public async Task SetCollection()
        {
            if (selectedClass == "") return;
            await SetCollection(selectedClass);
        }

        public async Task SetCollection(string _selectedClass)
        {
            selectedClass = _selectedClass;
            imageList = (await VM.ClientSession.GetImageByteListAsync($"{MainViewModel.ServerUrl}api/content/image_byte_list/{selectedClass}")).Select(x => GetBitmapImageFromByte(x)).ToList();
            RaiseCollectionChanged();
        }

        public override IEnumerator<BitmapImage> GetEnumerator()
        {
            return imageList.GetEnumerator();
        }
    }
}
