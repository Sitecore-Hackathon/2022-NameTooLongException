using System;
using Newtonsoft.Json;
using Mvp.Foundation.LayoutServiceExtensions.Internal;
using Mvp.Foundation.LayoutServiceExtensions.Serializer.Settings;
using Sitecore.Internal;
using Sitecore.LayoutService.Client;
using Sitecore.LayoutService.Client.Response;

namespace Mvp.Foundation.LayoutServiceExtensions.Serializer
{
	public class CustomLayoutServiceSerializer : ISitecoreLayoutSerializer
	{
		private static readonly Lazy<JsonSerializerSettings> _settings = new Lazy<JsonSerializerSettings>(new Func<JsonSerializerSettings>(CreateSerializerSettings));

		public SitecoreLayoutResponseContent Deserialize(string data)
		{
			Assert.ArgumentNotNull(data, "data");
			return JsonConvert.DeserializeObject<SitecoreLayoutResponseContent>(data, _settings.Value);
		}

		public static JsonSerializerSettings CreateSerializerSettings()
		{
			return new JsonSerializerSettings().SetDefaults();
		}
	}
}
