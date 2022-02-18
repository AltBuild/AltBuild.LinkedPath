using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public static class InterfaceExtensions
    {
        /// <summary>
        /// キャッシュを保持
        /// </summary>
        static Dictionary<Type, object> KeyValuePairs = new();

        /// <summary>
        /// 対象ドメインの全タイプから指定のインターフェースを継承している全クラスを取得する
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static T[] GetInterfaceInstance<T>()
        {
            // 該当メソッド
            T[] results;

            // タイプを取得
            Type typeClass = typeof(T);

            // キャッシュを取得
            if (KeyValuePairs.TryGetValue(typeClass, out object classes) == false)
            {
                // リストを保持
                var list = new List<T>();

                // アセンブリ内で指定のインターフェースを継承する全クラスを取得
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    foreach (var type in asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeClass)))
                        list.Add((T)Activator.CreateInstance(type));

                // インターフェース一覧を保持
                KeyValuePairs[typeClass] = results = list.ToArray();

                // インターフェース一覧を返す
                return results;
            }
            
            else
            {
                return (T[])classes;
            }
        }
    }
}
