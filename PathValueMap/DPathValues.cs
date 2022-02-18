using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public class DPathValues : IEnumerable<DPathValue>
    {
        public List<DPathValue> Items { get; } = new List<DPathValue>();

        /// <summary>
        /// Get PathValue of Items. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DPathValue this[int index] => Items[index];

        /// <summary>
        /// Count of Items.
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// �w��̃A�C�e����Items�̉��Ԗڂ�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(DPathValue value) => Items.IndexOf(value);

        /// <summary>
        /// �w��ʒu�ɃA�C�e����}������
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, DPathValue value) => Items.Insert(index, value);

        /// <summary>
        /// �z����擾
        /// </summary>
        /// <returns></returns>
        public DPathValue[] ToArray() => Items.ToArray();

        public bool TryGet(int index, out DPathValue dPathValue)
        {
            if (Items.Count > index)
            {
                dPathValue = Items[index];
                return true;
            }

            dPathValue = null;
            return false;
        }

        /// <summary>
        /// �Ō���ɒǉ����A�e�ƑO��ɃA�^�b�`����B
        /// </summary>
        /// <param name="value"></param>
        public void Attach(DPathValue value)
        {
            // Get the last item.
            var lastItem = Items.Count > 0 ? Items[^1] : null;

            // Attach the Before and Next.
            if (lastItem != null)
            {
                lastItem.Next = value;
                value.Prev = lastItem;
            }

            // Attach the Parent.
            Items.Add(value);
        }

        /// <summary>
        /// �Ō���ɒǉ��i�A�^�b�`�͂��Ȃ��j
        /// </summary>
        /// <param name="value"></param>
        public void Add(DPathValue value)
        {
            Items.Add(value);
        }

        public IEnumerator<DPathValue> GetEnumerator()
        {
            return ((IEnumerable<DPathValue>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
}
