using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    public class ObjectInitializeParameters
    {
        public object Object { get; set; }
        public object DefinitionObject { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public bool CreateEntities { get; set; }
        public DObjectRule Rule { get; init; }
        public DObjectRule GetMixRule(DObjectRule sourceRule) => GetMixRule(Rule, sourceRule);

        public static DObjectRule GetMixRule(DObjectRule paramRule, DObjectRule sourceRule)
        {
            if (paramRule.HasFlag(DObjectRule.Set))
            {
                if (sourceRule.HasFlag(DObjectRule.Datastore))
                    return paramRule & ~DObjectRule.Set;
                else
                    return paramRule & ~(DObjectRule.Set | DObjectRule.Datastore);
            }

            else if (paramRule.HasFlag(DObjectRule.Exclude))
                return sourceRule & ~(paramRule & ~DObjectRule.Exclude);

            else
                return paramRule | sourceRule;
        }
    }
}
