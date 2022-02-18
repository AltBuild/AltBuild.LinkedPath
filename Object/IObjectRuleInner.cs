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
    ///  オブジェクトのRuleを直接取得／設定する
    /// </summary>
    public interface IObjectRuleInner
    {
        DObjectRule Rule { get; set; }
    }
}
