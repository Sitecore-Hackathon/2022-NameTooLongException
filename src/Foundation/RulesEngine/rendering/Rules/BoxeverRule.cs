using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Mvp.Foundation.RulesEngine.Models.Dtos.Request;
using Mvp.Foundation.RulesEngine.Models.Dtos.Response;
using System;
using System.Net.Http;
using System.Text;
using Mvp.Foundation.RulesEngine.Options;

namespace Mvp.Foundation.RulesEngine.Rules
{
    // This code is taken from the video, very crappy code though...
    public class BoxeverRule : Rule
    {
        private static BoxeverOptions _boxeverOptions { get; set; }
        private string BoxeverTestID { get; set; }
        private string BoxeverID { get; set; }
        public string Value { get; set; }

        public override bool Execute()
        {
            try
            {
                _boxeverOptions = this._configuration.GetSection("Boxever").Get<BoxeverOptions>();
                this.BoxeverID = this._httpContext.Request.Cookies[$"bid_{_boxeverOptions.ClientKey}"];
                //Boxever Experimentation Full Stack Test POST
                const string url = "https://api.boxever.com/v2/callFlows";

                // TODO: See comments
                var getMultiVariantTestRequest = new GetBoxeverBinaryDecisionRequest
                {
                    BrowserId = this.BoxeverID,
                    Channel = _boxeverOptions.Configuration.Channel,
                    ClientKey = _boxeverOptions.ClientKey,
                    CurrencyCode = _boxeverOptions.Configuration.CurrencyCode,
                    FriendlyId = this.BoxeverTestID,
                    Language = _boxeverOptions.Configuration.Language,// TODO: Update to use context language
                    PointOfSale = _boxeverOptions.Configuration.PointOfSale// TODO: Update to use settings item
                };

                var client = new HttpClient();
                client.BaseAddress = new Uri(url);
                //byte[] cred = UTF8Encoding.UTF8.GetBytes("ENTER THE HASHED CREDENTIALS HERE");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _boxeverOptions.ClientSecret);//System.Convert.ToBase64String(cred));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var request = JsonConvert.SerializeObject(getMultiVariantTestRequest);
                HttpContent content = new StringContent(request, UTF8Encoding.UTF8, "application/json");
                HttpResponseMessage message = client.PostAsync(url, content).Result;
                if (message.IsSuccessStatusCode)
                {
                    // Get JSON Result
                    string result = message.Content.ReadAsStringAsync().Result;
                    // Deserialize JSON .....
                    var getMultiVariantTestResponse = JsonConvert.DeserializeObject<GetBoxeverBinaryDecisionResponse>(result); // Should create BoxeverJSON object
                    var componentVersion = getMultiVariantTestResponse.BinaryDecision;
                    // Compare contentVersion to value and return boolean
                    return componentVersion == this.Value;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
    }
}
