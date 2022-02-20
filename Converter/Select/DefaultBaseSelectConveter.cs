using AltBuild.LinkedPath;
using AltBuild.LinkedPath.Converters;
using AltBuild.LinkedPath.Parser;
using System;
using System.Collections.Generic;

namespace AltBuild.LinkedModel.SelectItems
{
    /// <summary>
    /// Default Select converter class.
    /// </summary>
    public class DefaultBaseSelectConverter : SelectConverterBase
    {
        /// <summary>
        /// object to SelectItems
        /// </summary>
        /// <param name="requirements">Select requirements</param>
        /// <param name="items">Datastore value</param>
        /// <returns>True: Successful, False: Failed</returns>
        public override bool TryGetSelectItems(ISelectRequirementsBase requirements, out ISelectItemsBase items)
        {
            items = null;
            return false;
        }

        public override bool TryInclude(ISelectRequirementsBase requirements, object value)
        {
            if (value != null)
            {
                // Get value type.
                var valueType = value.GetType();

                // Get member type.
                var memberType = requirements.InductInfo.ReturnType.GetPathType();

                if (memberType.UnderlyingType.Equals(valueType))
                {
                    if (requirements.InductInfo.TrySetValue(value))
                    {
                        // Success.
                        return true;
                    }
                }
            }

            // Failed.
            return false;
        }

        public override bool TryExclude(ISelectRequirementsBase requirements, object value)
        {
            if (value != null)
            {
                // Get value type.
                var valueType = value.GetType();

                // Get member type.
                var memberType = requirements.InductInfo.ReturnType.GetPathType();

                if (memberType.UnderlyingType.Equals(valueType))
                {
                    if (requirements.InductInfo.TrySetValue(value))
                    {
                        // Success.
                        return true;
                    }
                }
            }

            // Failed.
            return false;
        }
    }
}
