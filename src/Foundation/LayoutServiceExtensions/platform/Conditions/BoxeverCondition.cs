using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Mvp.Foundation.LayoutServiceExtensions.Conditions
{
    public class BoxeverCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string BoxeverTestID { get; set; }
        public string Value { get; set; }

        // Dummy condition only for serialization purposes
        protected override bool Execute(T ruleContext)
        {
            return false;
        }
    }
}