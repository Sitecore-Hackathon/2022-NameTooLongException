using Mvp.Foundation.LayoutServiceExtensions.Conditions;
using Newtonsoft.Json.Linq;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvp.Foundation.LayoutServiceExtensions.Parsers
{
    public class SendEmailSubscriptionConditionParser : BaseParser
    {
        private const string _typeId = "SendTest";

        public override object Parse(RuleCondition<ConditionalRenderingsRuleContext> condition)
        {
            var specificCondition = condition as SendEmailSubscriptionCondition<ConditionalRenderingsRuleContext>;
            return new JObject()
            {
                ["typeId"] = (JToken)_typeId,
                ["sendEmailGroupId"] = (JToken)specificCondition.SendEmailGroupId,
                ["value"] = (JToken)specificCondition.Value
            };
        }
    }
}
