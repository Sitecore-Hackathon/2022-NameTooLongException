using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mvp.Foundation.LayoutServiceExtensions.Internal;
using Mvp.Foundation.LayoutServiceExtensions.Models;
using Sitecore.Internal;
using Sitecore.LayoutService.Client.Response.Model;

namespace Mvp.Foundation.LayoutServiceExtensions.Serializer.Converters
{
    public class CustomPlaceHolderJsonConverter : JsonConverter<Placeholder>
    {
        public override bool CanWrite => false;

        public override Placeholder ReadJson(JsonReader reader, Type objectType, Placeholder existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Assert.ArgumentNotNull(reader, "reader");
            Assert.ArgumentNotNull(objectType, "objectType");
            Assert.ArgumentNotNull(serializer, "serializer");
            Placeholder placeholder = existingValue ?? new Placeholder();
            foreach (JToken item in JArray.Load(reader))
            {
                try
                {
                    if (item["type"]?.ToString() == "text/sitecore")
                    {
                        placeholder.Add(serializer.Deserialize<EditableChrome>(item.CreateReader()));
                    }
                    else
                    {
                        //Serializing to NTLE's custom PersonalizedComponent
                        if (item["variants"] != null)
                            placeholder.Add(serializer.Deserialize<PersonalizedComponent>(item.CreateReader()));
                        else
                            placeholder.Add(serializer.Deserialize<Component>(item.CreateReader()));
                    }
                }
                catch(Exception ex)
                {
                    continue;
                }
            }
            return placeholder;
        }

        public override void WriteJson(JsonWriter writer, Placeholder value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
