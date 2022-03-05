using Newtonsoft.Json;

namespace Mvp.Foundation.RulesEngine.Models.Dtos.Response
{
    public class GetMultiVariantTestResponse
    {
        [JsonProperty("componentVersion")]
        public string ComponentVersion { get; set; }
    }
}
