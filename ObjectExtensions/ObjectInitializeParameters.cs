using System.Reflection;

namespace AltBuild.LinkedPath
{
    public class ObjectInitializeParameters
    {
        public object Object { get; set; }
        public object DefinitionObject { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public bool CreateEntities { get; set; }
        public InductiveRule Rule { get; init; }
        public InductiveRule GetMixRule(InductiveRule sourceRule) => GetMixRule(Rule, sourceRule);

        public static InductiveRule GetMixRule(InductiveRule paramRule, InductiveRule sourceRule)
        {
            if (paramRule.HasFlag(InductiveRule.Set))
            {
                if (sourceRule.HasFlag(InductiveRule.Datastore))
                    return paramRule & ~InductiveRule.Set;
                else
                    return paramRule & ~(InductiveRule.Set | InductiveRule.Datastore);
            }

            else if (paramRule.HasFlag(InductiveRule.Exclude))
                return sourceRule & ~(paramRule & ~InductiveRule.Exclude);

            else
                return paramRule | sourceRule;
        }
    }
}
