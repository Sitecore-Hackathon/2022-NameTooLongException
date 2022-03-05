using Newtonsoft.Json.Linq;
using Mvp.Foundation.LayoutServiceExtensions.Conditions;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;

namespace Mvp.Foundation.LayoutServiceExtensions.Parsers
{
    public class BoxeverConditionParser : BaseParser
    {
        private const string _typeId = "BoxeverRule";

        public override object Parse(RuleCondition<ConditionalRenderingsRuleContext> condition)
        {
            var specificCondition = condition as BoxeverCondition<ConditionalRenderingsRuleContext>;
            return new JObject()
            {
                ["typeId"] = (JToken)_typeId,
                ["boxeverTestId"] = (JToken)specificCondition.BoxeverTestID,
                ["value"] = (JToken)specificCondition.Value
            };
        }
    }
}