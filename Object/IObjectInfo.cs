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
    ///  DObject情報にアクセスするインターフェース
    /// </summary>
    public interface IObjectInfo
    {
        public DObjectType Type => DObjectType.None;
    }
}
