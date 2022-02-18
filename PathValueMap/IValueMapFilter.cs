using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// バリューマップにフィルターを掛ける
    /// </summary>
    public interface IValueMapFilter
    {
        /// <summary>
        /// バリューマップの生成直後にフィルターを掛ける
        /// </summary>
        /// <param name="pathFrame"></param>
        void ValueMapFilter(DPathValueMap pathFrame);
    }
}
