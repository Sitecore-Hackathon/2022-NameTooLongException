using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Sitecore.LayoutService.Client.Response.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mvp.Foundation.LayoutServiceExtensions.Models
{
    /// <summary>
    /// Personalization Model
    /// </summary>
    public class PersonalizedComponent : Component
    {
        [DataMember(Name = "variants")]
        public List<Variant> Variants { get; set; }

    }

    
    /// <summary>
    /// Condition
    /// </summary>
    [JsonConverter(typeof(ConditionConverter))]
    public class Condition
    {
        public string typeId { get; set; }
        public string OriginalJson { get; set; }
    }

    public class Variant : FieldsReader, IPlaceholderFeature
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "variantId")]
        public string VariantId { get; set; }
        [DataMember(Name = "condition")]
        public Condition? Condition { get; set; }
    }

    public class ConditionConverter : JsonConverter<Condition>
    {
        public override bool CanWrite => false;

        public override Condition ReadJson(JsonReader reader, Type objectType, Condition existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Null)
                return null;

            JObject obj = JObject.Load(reader);
            var customObject = JsonConvert.DeserializeObject<Condition>(obj.ToString(), new JsonSerializerSettings
            {
                ContractResolver = new CustomContractResolver()
            });
            customObject.OriginalJson = obj.ToString();
            return customObject;
        }

        public override void WriteJson(JsonWriter writer, Condition value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            return null;
        }
    }
}
