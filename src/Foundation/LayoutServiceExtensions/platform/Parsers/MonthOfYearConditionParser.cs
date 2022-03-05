using Newtonsoft.Json.Linq;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using Sitecore.Rules.Conditions.DateTimeConditions;

namespace Mvp.Foundation.LayoutServiceExtensions.Parsers
{
    public class MonthOfYearConditionParser : BaseParser
    {
        private const string _typeId = "MonthOfYear";

        public override object Parse(RuleCondition<ConditionalRenderingsRuleContext> condition)
        {
            var specificCondition = condition as CurrentMonthCondition<ConditionalRenderingsRuleContext>;
            return new JObject()
            {
                ["typeId"] = (JToken)_typeId,
                ["Month"] = (JToken)GetMonth(specificCondition.Month)
            };
        }

        private string GetMonth(string month)
        {
            var monthItem = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(month));
            return monthItem?.Name;
        }
    }
}