using Newtonsoft.Json;

namespace Mvp.Foundation.RulesEngine.Models.Dtos.Request
{
    public class GetBoxeverBinaryDecisionRequest
    {
        [JsonProperty("browserId")]
        public string BrowserId { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonProperty("friendlyId")]
        public string FriendlyId { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
        
        [JsonProperty("pointOfSale")]
        public string PointOfSale { get; set; }
    }
}
