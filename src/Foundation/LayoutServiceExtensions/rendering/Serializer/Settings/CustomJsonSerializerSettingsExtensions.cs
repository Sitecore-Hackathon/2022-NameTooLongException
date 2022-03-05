using Newtonsoft.Json;
using Mvp.Foundation.LayoutServiceExtensions.Serializer.Converters;
using Sitecore.LayoutService.Client.Newtonsoft;
using Sitecore.LayoutService.Client.Newtonsoft.Converters;

namespace Mvp.Foundation.LayoutServiceExtensions.Serializer.Settings
{
    public static class CustomJsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings SetDefaults(this JsonSerializerSettings settings)
        {
            AddConverters(settings);
            SetContractResolver(settings);
            settings.Formatting = Formatting.Indented;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            return settings;
        }

        internal static JsonSerializerSettings SetContractResolver(this JsonSerializerSettings settings)
        {
            settings.ContractResolver = CustomDataContractResolver.Instance;
            return settings;
        }

        internal static JsonSerializerSettings AddConverters(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new FieldReaderJsonConverter());
            //settings.Converters.Add(new PlaceholderJsonConverter());
            //Use our customer PlaceholderConverter - which converts Component to PersonalizedComponent
            settings.Converters.Add(new CustomPlaceHolderJsonConverter());
            settings.Converters.Add(new DeviceJsonConverter());
            return settings;
        }
    }
}
