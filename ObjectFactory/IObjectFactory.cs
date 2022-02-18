using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object initialization interface
    /// </summary>
    public interface IObjectFactory
    {
        void InitializeObject(ObjectInitializeParameters parameters);
    }
}
