using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public static class ObjectFactory
    {
        static ConcurrentDictionary<Type, IObjectFactory[]> factoryList = new ();

        /// <summary>
        /// Create the Object special member.
        /// </summary>
        /// <param name="obj">Member initialize target object.</param>
        /// <param name="rule">Rule</param>
        public static void Initialize(object obj, DObjectRule rule = DObjectRule.Writable)
        {
            // メンバーの初期化
            Initialize(new ObjectInitializeParameters { Object = obj, CreateEntities = true, Rule = rule });
        }

        /// <summary>
        /// オブジェクトのプロパティを初期化する
        /// </summary>
        /// <param name="obj">Member initialize target object.</param>
        /// <param name="sourceModel">Member initialize target object.</param>
        /// <param name="isCreateInstance">Instantiate a DObject base.</param>
        /// <param name="rule">DObjectRule flag.</param>
        public static void Initialize(ObjectInitializeParameters parameters)
        {
            // インターフェースインスタンスを取得（生成）
            IObjectFactory[] factories =
                factoryList.GetOrAdd(typeof(IObjectFactory), t => InterfaceExtensions.GetInterfaceInstance<IObjectFactory>());

            // All initialize
            foreach (var factory in factories)
                factory.InitializeObject(parameters);
        }
    }
}
