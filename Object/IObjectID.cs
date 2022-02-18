using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// ID値（Guid, int, etc...）をセットするインターフェース
    /// </summary>
    public interface IObjectID
    {
        void SetID(object id);

        object GetID();
    }
}
