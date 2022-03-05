using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Mvp.Foundation.RulesEngine.Rules;

namespace Mvp.Foundation.RulesEngine.Factories
{
    public interface IDefaultRuleFactory
    {
        Rule GetRule(string id, string json, HttpContext httpContext, IConfiguration configuration);
    }
}
