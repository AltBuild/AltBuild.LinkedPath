using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// バリューマップフィルターリスト
    /// </summary>
    public class ValueMapFilterCollection : IEnumerable<IValueMapFilter>
    {
        public IReadOnlyCollection<IValueMapFilter> Items { get; }
        List<IValueMapFilter> _items = new List<IValueMapFilter>();

        public ValueMapFilterCollection()
        {
            Items = _items;
        }

        /// <summary>
        /// フィルターを追加する
        /// </summary>
        /// <param name="filter"></param>
        public void Add(IValueMapFilter filter) => _items.Add(filter);

        /// <summary>
        /// フィルターをコピーする
        /// </summary>
        /// <param name="filters"></param>
        public void Add(ValueMapFilterCollection filters) =>
            filters._items.ForEach(o => _items.Add(o));

        public IEnumerator<IValueMapFilter> GetEnumerator()
        {
            return ((IEnumerable<IValueMapFilter>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
