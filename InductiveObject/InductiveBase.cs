using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Basic classes of the Inductive object
    /// </summary>
    public abstract partial class InductiveBase : IInductiveBase
    {
        /// <summary>
        /// 定義元インスタンス
        /// </summary>
        [JsonIgnore]
        [InductiveIgnore]
        [InductiveControl]
        public object DefinitionObject
        {
            get => _definitionObject;
            set
            {
                if (this == value)
                    throw new InvalidProgramException($"Set yourself error. Member is {nameof(DefinitionObject)}.");

                _definitionObject = value;
            }
        }
        object _definitionObject;

        /// <summary>
        /// Get member value.
        /// </summary>
        /// <param name="pathMember">Path member</param>
        /// <returns>MemberValue object</returns>
        public virtual InductInfo Induct(string path, InductMethods methods = InductMethods.Normal) => Induct(PathFactory.Parse(path), methods);

        /// <summary>
        /// Get member value.
        /// </summary>
        /// <param name="pathMember">Path member</param>
        /// <returns>MemberValue object</returns>
        public virtual InductInfo Induct(PathMember path, InductMethods methods = InductMethods.Normal) => InductInfo.Create(path, this, null, methods);

        /// <summary>
        /// To string with PathElements string
        /// </summary>
        /// <param name="pathElements">String of path element</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual string ToString(string pathElements) => ToString(PathElements.Parse(pathElements));

        /// <summary>
        /// To string with PathElements.
        /// </summary>
        /// <param name="pathElements"></param>
        /// <returns></returns>
        public virtual string ToString(PathElements pathElements)
        {
            if (pathElements == null)
                return ToString();

            if (pathElements.TryGetFormatOnlyElement(out string formatString))
            {
                if (FormatConverterFactory.TryGetConverterWithDestineType(typeof(string), formatString, this, out object destine))
                    return (string)destine;

                else
                    return ToString();
            }

            // Init results
            var results = new StringBuilder();

            // Element loops
            foreach (var element in pathElements)
            {
                if (element.Type == PathElementType.String)
                {
                    results.Append(element.Source);
                }

                else if (element.Type == PathElementType.Value)
                {
                    string value = null;

                    // this target.
                    if (string.IsNullOrWhiteSpace(element.PathMember?.Name))
                    {
                        value = ToString();
                    }

                    // Target member
                    else
                    {
                        var inductInfo = this.Induct(element.PathMember);
                        if (inductInfo?.ReturnValue != null)
                        {
                            var pathFrame = element.PathMember?.Frame;
                            if (pathFrame != null && pathFrame.Attributes.TryGetFormat(out string format))
                                value = inductInfo.ReturnValue.ToString(PathElements.Parse(format));

                            else if (inductInfo.ReturnValue is string atValue)
                                value = atValue;

                            else if (inductInfo.ReturnValue is IInductiveBase iObjectBase)
                                value = iObjectBase.ToString();

                            else
                                value = inductInfo.ReturnValue.ToString(PathElements.Default);
                        }
                    }

                    // append
                    if (value != null)
                        results.Append(value);
                }
            }
            return results.ToString();
        }

        public override bool Equals(object obj) => base.GetHashCode() == obj.GetHashCode();

        public override int GetHashCode() => base.GetHashCode();
    }
}
