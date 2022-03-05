using Microsoft.Extensions.DependencyInjection;
using Mvp.Foundation.RulesEngine.Factories;

namespace Mvp.Foundation.RulesEngine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFoundationRulesEngine(this IServiceCollection services)
        {
            services.AddSingleton<IDefaultRuleFactory, DefaultRuleFactory>();

            return services;
        }
    }
}
