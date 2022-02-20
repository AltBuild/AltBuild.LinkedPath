using System;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// （Nullable判定）
    /// </summary>
    public class PathType
    {
        public static PathType Empty { get; } = new PathType(null);

        /// <summary>
        /// 元の型
        /// </summary>
        public Type NativeType { get; }

        /// <summary>
        /// Nullable型の場合に Nullableを解いた型を返す
        /// ジェネリックの場合は 元の型を返す
        /// その他は そのまま返す。
        /// </summary>
        public Type UnderlyingType { get; }

        /// <summary>
        /// Nullable型の場合に Nullableを解いた型を返す
        /// Nullable以外の場合は、全て null を保持
        /// </summary>
        public Type NullableUnderlyingType { get; }

        /// <summary>
        /// モデル参照型を含むジェネリック型の場合にそれを定義している型
        /// </summary>
        public Type GenericTypeDefinition { get; }

        /// <summary>
        /// ジェネリック型の場合に 元となる型を返す（最初の型のみ）
        /// </summary>
        public Type GenericUnderlyingType { get; }

        /// <summary>
        /// ジェネリック型の場合に 元の型を保持
        /// </summary>
        public Type[] GenericArguments { get; }

        /// <summary>
        /// 元がNullable型であったか？
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// 元が ValueType → true
        /// Nullable を解いて ValueType → true
        /// その他 false
        /// </summary>
        public bool IsValueType => UnderlyingType.IsValueType;

        public bool TryGenericTypeDefinition(out Type type) =>
            (type = GenericTypeDefinition) != null;

        public PathType(Type nativeType)
        {
            if (nativeType != null)
            {
                // Store source type.
                NativeType = nativeType;

                // Case: generic type.
                if (nativeType.IsGenericType)
                {
                    // Get definition type.
                    GenericTypeDefinition = nativeType.GetGenericTypeDefinition();

                    // Get generic inner types.
                    GenericArguments = nativeType.GetGenericArguments();

                    // Get generic underlyingType
                    if (GenericArguments.Length > 0)
                        UnderlyingType = GenericUnderlyingType = GenericArguments[0];
                    else
                        UnderlyingType = GenericTypeDefinition;

                    // check nullable
                    if (GenericTypeDefinition.Equals(typeof(Nullable<>)))
                    {
                        // Nullable check.
                        NullableUnderlyingType = Nullable.GetUnderlyingType(nativeType);
                        IsNullable = true;
                    }
                }

                // Case: not generic type.
                else
                {
                    // Nullable check.
                    NullableUnderlyingType = Nullable.GetUnderlyingType(nativeType);

                    // Is nullable type.
                    if (NullableUnderlyingType != null)
                    {
                        UnderlyingType = NullableUnderlyingType;
                        IsNullable = true;
                    }

                    // Store source type.
                    else
                    {
                        UnderlyingType = nativeType;
                    }
                }
            }
        }
    }
}
