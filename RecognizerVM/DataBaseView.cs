using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media.Imaging;

namespace RecognizerVM
{
    public abstract class DataBaseView : IEnumerable, INotifyCollectionChanged
    {
        public DataBaseView(MainViewModel vm)
        {
            VM = vm;
            VM.DBManager.DataChanged += RaiseCollectionChanged;
        }

        public MainViewModel VM { get; }
        public event NotifyCollectionChangedEventHandler CollectionChanged;


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
        public ClassListView(MainViewModel vm) : base(vm) { }

        public override IEnumerator<string> GetEnumerator()
        {
            return VM.DBManager.GetClassList().GetEnumerator();
        }
    }


    public class ImageListView : DataBaseView
    {
        public ImageListView(MainViewModel vm) : base(vm) { }

        private string selectedClass;

        public void SetSelectedClass(string c)
        {
            selectedClass = c;
        }

        public override IEnumerator<BitmapImage> GetEnumerator()
        {
            return VM.DBManager.GetImageList(selectedClass).Select(x => GetBitmapImageFromByte(x)).GetEnumerator();
        }
    }
}
