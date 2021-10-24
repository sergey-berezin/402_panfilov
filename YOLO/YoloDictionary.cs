using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOLO
{
    public class YoloDictionary : IEnumerable<YoloClass>, INotifyCollectionChanged
    {
        Dictionary<string, YoloClass> dict = new();

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public void Add(YoloResult item)
        {
            if (!dict.ContainsKey(item.Label))
                dict.Add(item.Label, new YoloClass(item.Label));
            dict[item.Label].Add(item);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Clear()
        {
            dict.Clear();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        public IEnumerator<YoloClass> GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }
    }
}
