using Newtonsoft.Json.Linq;
using Sitecore.Personalization.Rules.Conditions;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using System;

namespace Mvp.Foundation.LayoutServiceExtensions.Parsers
{
    public class DayOfWeekConditionParser : BaseParser
    {
        private const string _typeId = "DayOfWeek";

        public override object Parse(RuleCondition<ConditionalRenderingsRuleContext> condition)
        {
            var specificCondition = condition as DayOfWeekCondition<ConditionalRenderingsRuleContext>;
            return new JObject()
            {
                ["typeId"] = (JToken)_typeId,
                ["listOfDays"] = (JToken)GetListOfDays(specificCondition.DaysList)
            };
        }

        private JArray GetListOfDays(string daysList)
        {
            string[] strArray = daysList.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            JArray dayOfWeekList = new JArray();
            foreach (string key in strArray)
            {
                var dayOfWeekItem = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(key));
                dayOfWeekList.Add(dayOfWeekItem?.Name);
            }
            return dayOfWeekList;
        }
    }
}