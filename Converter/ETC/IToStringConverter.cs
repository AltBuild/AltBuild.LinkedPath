using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// プロパティ情報
    /// （都度生成して処理する）
    /// </summary>
    public interface IToStringConverter
    {
        /// <summary>
        /// 値をStringに変換する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string ToString(DPathValue pathValue, object value);
    }
}
