using System;
using System.Collections.Generic;
using System.Linq;

namespace AltBuild.LinkedPath
{
    [Flags]
    public enum RecordTargets
    {
        /// <summary>
        /// Not edit (Default)
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// 一次テーブルまで
        /// </summary>
        First = 0x0001,

        /// <summary>
        /// 二次テーブルまで
        /// </summary>
        Second = 0x0002,

        /// <summary>
        /// データテーブルが対象
        /// </summary>
        Data = 0x1000,

        /// <summary>
        /// マスターテーブルが対象
        /// </summary>
        Master = 0x2000,

        /// <summary>
        /// 一次データテーブルまで
        /// </summary>
        FirstData = First | Data,

        /// <summary>
        /// 一次マスターテーブルまで
        /// </summary>
        FirstMaster = First | Master,

        /// <summary>
        /// 二次データテーブルまで
        /// </summary>
        SecondData = Second | Data,

        /// <summary>
        /// 二次マスターテーブルまで
        /// </summary>
        SecondMaster = Second | Master,

        /// <summary>
        /// 全て対象
        /// </summary>
        All = Data | Master,

        /// <summary>
        /// Auto mode
        /// </summary>
        Auto = 0x800000,
    }
}
