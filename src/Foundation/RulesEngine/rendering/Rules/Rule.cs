using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Mvp.Foundation.RulesEngine.Rules
{
    public abstract class Rule
    {
        public string TypeId { get; set; }
        /// <summary>
        /// Context accessor
        /// </summary>
        public HttpContext _httpContext { get; set; }
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration _configuration { get; set; }


        public abstract bool Execute();
    }
}
