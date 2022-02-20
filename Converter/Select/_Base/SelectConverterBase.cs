using AltBuild.LinkedPath.Parser;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AltBuild.LinkedPath.Converters
{
    /// <summary>
    /// SelectConverter base class.
    /// </summary>
    public abstract class SelectConverterBase
    {
        /// <summary>
        /// object to SelectItems
        /// </summary>
        /// <param name="requirements">Select requirements</param>
        /// <param name="items">Select items</param>
        /// <returns>True: Successful, False: Failed</returns>
        public virtual bool TryGetSelectItems(ISelectRequirementsBase requirements, [MaybeNullWhen(false)] out ISelectItemsBase items)
        {
            items = null;
            return false;
        }

        /// <summary>
        /// Select value to object
        /// </summary>
        /// <param name="requirements">Select requirements</param>
        /// <param name="value">Include value</param>
        /// <returns>True: Successful, False: Failed</returns>
        public virtual bool TrySelect(ISelectRequirementsBase requirements, object value) => false;

        /// <summary>
        /// Include value to object
        /// </summary>
        /// <param name="requirements">Select requirements</param>
        /// <param name="value">Include value</param>
        /// <returns>True: Successful, False: Failed</returns>
        public virtual bool TryInclude(ISelectRequirementsBase requirements, object value) => false;

        /// <summary>
        /// Exclude value to object
        /// </summary>
        /// <param name="requirements">Select requirements</param>
        /// <param name="value">Exclude value</param>
        /// <returns>True: Successful, False: Failed</returns>
        public virtual bool TryExclude(ISelectRequirementsBase requirements, object value) => false;

        /// <summary>
        /// Enable/Disable selected items.
        /// </summary>
        /// <param name="requirements">Select requirements</param>
        /// <param name="value">Exclude value</param>
        /// <returns>True: Successful, False: Failed</returns>
        public virtual bool TryEnable(ISelectRequirementsBase requirements, object value = null) => false;

        /// <summary>
        /// selected items remove.
        /// </summary>
        /// <param name="requirements">Select requirements</param>
        /// <param name="value">Exclude value</param>
        /// <returns>True: Successful, False: Failed</returns>
        public virtual bool TryRemove(ISelectRequirementsBase requirements, object value = null) => false;

        /// <summary>
        /// Throw convert exception on Export Method.
        /// </summary>
        /// <param name="columnInfo">Target column infomation</param>
        public virtual Exception ThrowConvertExceptionOnExport(MemberInfo memberInfo) =>
            new InvalidConvertException($"Model:{memberInfo} to Store:{memberInfo.Name}[{memberInfo.GetPathType()}] converting error. on {GetType()}");
    }
}
