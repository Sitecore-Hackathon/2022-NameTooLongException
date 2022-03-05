using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.JavaScriptServices.ViewEngine.LayoutService.Serialization;
using System.Linq;

namespace Mvp.Foundation.LayoutServiceExtensions
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            var descriptorIPlaceholderTransformer = serviceCollection.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IPlaceholderTransformer));
            serviceCollection.Remove(descriptorIPlaceholderTransformer);
            serviceCollection.AddTransient<IPlaceholderTransformer, Mvp.Foundation.LayoutServiceExtensions.Placeholders.PlaceholderTransformer>();
        }
    }
}