using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public interface IPathValueIndexes
    {
        int Count { get; }

        void Add(int index);
    }
}
