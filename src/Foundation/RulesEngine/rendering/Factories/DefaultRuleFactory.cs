using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Mvp.Foundation.RulesEngine.Rules;
using Mvp.Foundation.RulesEngine.Utils;
using System;
using System.Collections.Generic;

namespace Mvp.Foundation.RulesEngine.Factories
{
	public class DefaultRuleFactory : IDefaultRuleFactory
	{
		private readonly Dictionary<string, string> types = new Dictionary<string, string>()
		{
			{ "DayOfWeek", " Mvp.Foundation.RulesEngine.Rules.DayOfWeekRule, Mvp.Foundation.RulesEngine" },
			{ "MonthOfYear", " Mvp.Foundation.RulesEngine.Rules.MonthOfYearRule, Mvp.Foundation.RulesEngine" },
			{ "BoxeverTest", " Mvp.Foundation.RulesEngine.Rules.BoxeverRule, Mvp.Foundation.RulesEngine" }
		};

        private Type GetGenericType(string typeName)
        {
            return ReflectionUtil.GetGenericType(typeName);
        }

        // Based on the Rule ID, get the type from configuration and create an instance of it.
        // Added raw JSON as input for this method in order to get all the settings necessary for the Rule to execute.
		public Rule GetRule(string id, string json, HttpContext httpContext, IConfiguration configuration)
		{
			if (types.TryGetValue(id, out var ruleType))
            {
				var type = GetGenericType(ruleType);
				if (type == null)
					return null;
				var rule = JsonConvert.DeserializeObject(json, type) as Rule;
				rule._httpContext = httpContext;
				rule._configuration = configuration;
				return rule;
			}
			return null;
        }
    }
}
