using Newtonsoft.Json;

namespace Mvp.Foundation.RulesEngine.Models.Dtos.Response
{
    public class GetBoxeverBinaryDecisionResponse
    {
        [JsonProperty("binaryDecision")]
        public string BinaryDecision { get; set; }
    }
}
