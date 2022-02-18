using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// 値リストを保持するインターフェース２
    /// </summary>
    public interface IObjectIndexer
    {
        object this[int index] { get; set; }

        int Count { get; }

        bool CanAdd { get; }

        object AddNewRecord(int? indexOfInsert = null);
    }
}
