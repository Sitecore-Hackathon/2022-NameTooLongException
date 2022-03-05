using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Foundation.LayoutServiceExtensions.Internal;
using Mvp.Foundation.LayoutServiceExtensions.Serializer;
using Sitecore.Internal;
using Sitecore.LayoutService.Client;
using Sitecore.LayoutService.Client.Newtonsoft;

namespace Mvp.Foundation.LayoutServiceExtensions.Extentions
{
    public static class ServiceCollectionExtensions
    {
		public static ISitecoreLayoutClientBuilder AddSitecoreLayoutServiceWithPersonalizedComponent(this IServiceCollection services, Action<SitecoreLayoutClientOptions>? options = null)
		{
			Assert.ArgumentNotNull(services, "services");
			if (!services.Any((ServiceDescriptor s) => s.ServiceType == typeof(SitecoreLayoutServiceMarkerService)))
			{
				services.AddTransient((Func<IServiceProvider, ISitecoreLayoutClient>)delegate (IServiceProvider sp)
				{
					using IServiceScope serviceScope = sp.CreateScope();
					return ActivatorUtilities.CreateInstance<DefaultLayoutClient>(serviceScope.ServiceProvider, new object[1] { sp });
				});

				//Redistering CustomLayoutSerializer to serialize PERSONALIZED COMPONENT
				services.AddSingleton<ISitecoreLayoutSerializer, CustomLayoutServiceSerializer>();
			}
			if (options != null)
			{
				services.Configure(options);
			}
			return new SitecoreLayoutClientBuilder(services);
		}
	}
}
