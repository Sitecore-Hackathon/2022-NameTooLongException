using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Mvp.Foundation.LayoutServiceExtensions.Conditions
{
    /// <summary>
    /// This Condition only exists so we can select it in the Rules Engine in Sitecore
    /// You will of course want to give it an implementation if you would want to 
    /// run this rule in Sitecore side as well in different scenarios.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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