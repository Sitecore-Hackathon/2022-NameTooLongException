using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;

namespace Mvp.Foundation.LayoutServiceExtensions.Parsers
{
    public abstract class BaseParser
    {
        public string TypeId { get; set; }
        public abstract object Parse(RuleCondition<ConditionalRenderingsRuleContext> condition);
    }
}