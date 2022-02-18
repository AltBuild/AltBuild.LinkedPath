using AltBuild.BaseExtensions;
using AltBuild.LinkedPath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
{
    public class DObjectEnumerable<T> : IEnumerable<T>
    {
        public IObjectBase Object { get; init; }

        public bool CheckOnly { get; set; }

        public IEnumerator GetEnumerator()
        {
            return null;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return null;
        }
    }
}
