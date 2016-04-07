using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MixMusic.Common
{
    public static class Extensions
    {
        public static void AddRang<T>(this ObservableCollection<T> oc,IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            foreach(var item in collection)
            {
                oc.Add(item);
            }
        }
    }
}
