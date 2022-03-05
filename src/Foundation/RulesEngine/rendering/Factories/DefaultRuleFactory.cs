using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Mvp.Foundation.RulesEngine.Rules;
using Mvp.Foundation.RulesEngine.Utils;
using System;
using System.Collections.Generic;

namespace Mvp.Foundation.RulesEngine.Factories
{
	
    /// 1. Get Rule Object based on ID (from a dictionary or something) -> https://stackoverflow.com/questions/52407867/how-to-initialize-a-dictionary-type-during-dependency-injection
	/// 2. Rule Type should be based on an abstract with an execute function
	/// 3. Execute the Rule, which should return true or false
	public class DefaultRuleFactory : IDefaultRuleFactory
	{
		private readonly Dictionary<string, string> types = new Dictionary<string, string>()
		{
			{ "DayOfWeek", " Mvp.Foundation.RulesEngine.Rules.DayOfWeekRule, Mvp.Foundation.RulesEngine" },
			{ "MonthOfYear", " Mvp.Foundation.RulesEngine.Rules.MonthOfYearRule, Mvp.Foundation.RulesEngine" },
			{ "BoxeverTest", " Mvp.Foundation.RulesEngine.Rules.BoxeverRule, Mvp.Foundation.RulesEngine" }
		};

        private Type GetGenericType<T>(string typeName)
        {
            return ReflectionUtil.GetGenericType<T>(typeName);
        }

		public Rule GetRule(string id, string json, HttpContext httpContext, IConfiguration configration)
		{
			if (types.TryGetValue(id, out var ruleType))
            {
				var type = GetGenericType<Rule>(ruleType);
				if (type == null)
					return null;
				var rule = JsonConvert.DeserializeObject(json, type) as Rule;
				rule._httpContext = httpContext;
				rule._configuration = configration;
				return rule;

				//var result = ReflectionUtil.CreateObject(type) as Rule;
				//return result;
			}
			return null;
        }
    }
}
