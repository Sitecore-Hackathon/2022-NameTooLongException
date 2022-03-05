using Microsoft.Extensions.Configuration;
using Moosend.Api.Client.Common.Models;
using Mvp.Foundation.RulesEngine.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Foundation.RulesEngine.Rules
{
    /// <summary>
    /// Sitecore Send Email subscription check
    /// </summary>
    public class SendEmailSubscriptionRule : Rule
    {
        private static SendOptions _sendOptions { get; set; }
        private string SendEmailGroupId { get; set; }
        public string Value { get; set; }
        public override bool Execute()
        {
            _sendOptions = this._configuration.GetSection("SitecoreSend").Get<SendOptions>();
            //mailing list id from condition
            var mailingListId = new Guid(this.SendEmailGroupId);
            var apiKey = new Guid(_sendOptions.ApiKey);
            //Better user case is to get email id from OKta but for now , we are getting it from cookie
            this.Value = this._httpContext.Request.Cookies[$"mootrack_email_id"]; 


            //Sitecore Send client
            var apiClient = new Moosend.Api.Client.MoosendApiClient(apiKey);
            //Check if user is in Subscription list
            var response = apiClient.GetSubscriberByEmailAsync(mailingListId, Value);



            return !string.IsNullOrEmpty(response?.Result?.Email);
        }
    }
}
