using Newtonsoft.Json;

namespace Mvp.Foundation.LayoutServiceExtensions.Models
{
    public class Variant
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("variantId")]
        public string VariantId { get; set; }
        [JsonProperty("condition")]
        public object Condition { get; set; }
        [JsonProperty("fields")]
        public object Fields { get; set; }
    }
}