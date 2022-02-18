using AltBuild.BaseExtensions;

using System;

namespace AltBuild.LinkedPath
{
    public static class TypeConverterExtensions
    {
        public static bool TryGet<T>(object sourceValue, out T destineValue)
        {
            if (TryGet(typeof(T), sourceValue, out object objDestineValue))
            {
                destineValue = (T)objDestineValue;
                return true;
            }
            else
            {
                destineValue = default;
                return false;
            }
        }

        /// <summary>
        /// Try get convert value.
        /// </summary>
        /// <param name="destineType">destine type.</param>
        /// <param name="sourceValue">source value</param>
        /// <param name="destineValue">destine value</param>
        /// <returns>Succesful: true.  Failed: false.</returns>
        public static bool TryGet(Type destineType, object sourceValue, out object destineValue)
        {
            // 値が null の場合
            if (sourceValue == null)
            {
                if (destineType.Equals(typeof(string)))
                {
                    destineValue = default(string);
                    return true;
                }
                else if (destineType.Equals(typeof(Guid)))
                {
                    destineValue = default(Guid);
                    return true;
                }
                else if (destineType.Equals(typeof(int)))
                {
                    destineValue = default(int);
                    return true;
                }
                else if (destineType.Equals(typeof(bool)))
                {
                    destineValue = default(bool);
                    return true;
                }
                else if (destineType.Equals(typeof(DateTime)))
                {
                    destineValue = default(DateTime);
                    return true;
                }
                else if (destineType.Equals(typeof(long)))
                {
                    destineValue = default(long);
                    return true;
                }
                else if (destineType.Equals(typeof(decimal)))
                {
                    destineValue = default(decimal);
                    return true;
                }
                else if (destineType.Equals(typeof(double)))
                {
                    destineValue = default(double);
                    return true;
                }
                else if (destineType.Equals(typeof(short)))
                {
                    destineValue = default(short);
                    return true;
                }
                else if (destineType.Equals(typeof(float)))
                {
                    destineValue = default(float);
                    return true;
                }

                // 変換できず
                destineValue = null;
                return false;
            }

            var sourceType = sourceValue.GetType();

            // Case: Destine type is object.
            if (destineType.Equals(typeof(object)))
            {
                if (sourceType.Equals(typeof(string)))
                    destineValue = sourceValue.ToString().Trim(new char[] { '\"' });

                else
                    destineValue = sourceValue;

                return true;
            }

            // Case: Type matching.
            if (destineType.Equals(sourceType))
            {
                if (sourceType.Equals(typeof(string)))
                    destineValue = sourceValue.ToString().Trim(new char[] { '\"' });

                else
                    destineValue = sourceValue;

                return true;
            }

            // Nullable
            var destineVariableType = new PathType(destineType);
            if (destineVariableType.IsNullable)
            {
                if (destineVariableType.NullableUnderlyingType.Equals(sourceType))
                {
                    if (sourceType.Equals(typeof(string)))
                    {
                        destineValue = (string)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(Guid)))
                    {
                        destineValue = (Guid?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(int)))
                    {
                        destineValue = (int?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(bool)))
                    {
                        destineValue = (bool?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(DateTime)))
                    {
                        destineValue = (DateTime?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(long)))
                    {
                        destineValue = (long?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(decimal)))
                    {
                        destineValue = (decimal?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(double)))
                    {
                        destineValue = (double?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(short)))
                    {
                        destineValue = (short?)sourceValue;
                        return true;
                    }
                    else if (sourceType.Equals(typeof(float)))
                    {
                        destineValue = (float?)sourceValue;
                        return true;
                    }
                }
            }

            // 元の値が文字列の場合　string → ***
            if (sourceType.Equals(typeof(string)))
            {
                // 処理要求タイプ
                var requestType = destineType;

                // 先タイプが Nullable の時は解く
                var baseType = Nullable.GetUnderlyingType(destineType);
                bool isNullable = (baseType != null);
                
                // Nullable の時は 解いた型を要求タイプにする
                if (isNullable)
                    requestType = baseType;

                if (requestType.Equals(typeof(int)))
                {
                    if (int.TryParse(sourceValue.ToString(), out int value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(Guid)))
                {
                    if (Guid.TryParse(sourceValue.ToString(), out Guid value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(bool)))
                {
                    if (bool.TryParse(sourceValue.ToString().ToLower(), out bool value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(DateTime)))
                {
                    if (DateTime.TryParse(sourceValue.ToString(), out DateTime value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(long)))
                {
                    if (long.TryParse(sourceValue.ToString(), out long value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(decimal)))
                {
                    if (decimal.TryParse(sourceValue.ToString(), out decimal value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(double)))
                {
                    if (double.TryParse(sourceValue.ToString(), out double value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(short)))
                {
                    if (short.TryParse(sourceValue.ToString(), out short value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(float)))
                {
                    if (float.TryParse(sourceValue.ToString(), out float value))
                    {
                        destineValue = value;
                        return true;
                    }
                }
                else if (requestType.Equals(typeof(IFormatProvider)) && DateTimeExtensions.TryStringToFormatProvider(sourceValue.ToString().Trim(new[] { '\"' }), out IFormatProvider formatProvider))
                {
                    destineValue = formatProvider;
                    return true;
                }

                // 変換できなくても、Nullable の時のみ
                // null と true を返す
                if (isNullable)
                {
                    destineValue = null;
                    return true;
                }
            }

            // 変換できず
            destineValue = null;
            return false;
        }
    }
}
