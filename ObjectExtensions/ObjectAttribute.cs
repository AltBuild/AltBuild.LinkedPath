using System;

namespace AltBuild.LinkedPath
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public class ObjectAttribute : System.Attribute
    {
        /// <summary>
        /// Empty flag.
        /// </summary>
        public bool IsEmpty { get; init; }

        /// <summary>
        /// Empty object.
        /// </summary>
        public static readonly ObjectAttribute Empty = new ObjectAttribute { IsEmpty = true };

        /// <summary>
        /// Ignore.
        /// </summary>
        public static readonly ObjectAttribute Ignore = new ObjectAttribute { IsEmpty = true, NotCopy = true, NotCreate = true };

        /// <summary>
        /// 親オブジェクトの生成時（新規 or クローン）にインスタンス化をしない
        /// </summary>
        public bool NotCreate { get; set; } = false;

        /// <summary>
        /// オブジェクトのコピー時は対象外
        /// </summary>
        public bool NotCopy { get; set; } = false;

        /// <summary>
        /// Cache or Datastore read only. （実体を保持しない。コピーしない。このメソッドを経由して編集しない。）
        /// </summary>
        public bool ReferenceOnly { get; set; } = false;

        /// <summary>
        /// Ruleのサポート
        /// </summary>
        public bool SupportedRule => !(NotCopy || NotCreate);

        public override string ToString()
        {
            return $"{nameof(NotCreate)}={NotCreate}, {nameof(NotCopy)}={NotCopy}, {nameof(ReferenceOnly)}={ReferenceOnly}";
        }
    }
}
