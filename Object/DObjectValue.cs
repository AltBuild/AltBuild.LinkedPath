using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public class DObjectValue
    {
        public static DObjectValue Unknown { get; } = new DObjectValue { Type = DValueType.Unknown, Value = null };
        public static DObjectValue False { get; } = new DObjectValue { Type = DValueType.Value, Value = false };
        public static DObjectValue True { get; } = new DObjectValue { Type = DValueType.Value, Value = true };

        public DValueType Type { get; init; }

        public object Value { get; init; }
    }
}
