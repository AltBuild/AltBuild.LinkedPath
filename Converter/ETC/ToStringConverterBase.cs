using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// ToStringコンバーターの基本クラス
    /// </summary>
    public class ToStringConverterBase : IToStringConverter
    {
        /// <summary>
        /// コンバーターインスタンスを取得する
        /// </summary>
        public static IToStringConverter Converter
        {
            get => _converter ??= new ToStringConverterBase();
            set => _converter = value;
        }
        static IToStringConverter _converter;

        /// <summary>
        /// 値をStringに変換する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToString(IObjectBase target, object value)
        {
            return value != null ? value.ToString() : null;
        }

        /// <summary>
        /// 値をStringに変換する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToString(DPathValue pathValue, object value)
        {
            return value != null ? value.ToString() : null;
        }
    }
}
