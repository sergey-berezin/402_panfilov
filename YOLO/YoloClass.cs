using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace YOLO
{
    public class YoloClass : IEnumerable<YoloResult>, INotifyCollectionChanged
    {
        public YoloClass(string _className) 
        {
            className = _className;
        }


        readonly string className;
        List<YoloResult> data = new();

        public List<YoloResult> DataList => data;

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public void Add(YoloResult item) 
        {
            data.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        public IEnumerator<YoloResult> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public override string ToString()
        {
            return $"[{data.Count}] {className}";
        }
    }
}
