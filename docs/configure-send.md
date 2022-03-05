# Configure a Sitecore Send
Please refer to this step on creation of Sitecore Send form : https://help.moosend.com/hc/en-us/articles/4403356535954

Once the form is created, please create an item in Sitecore which can then be chosen as a datasource for SendForm Component.

![Sitecore Form Item](images/sitecore-form.png?raw=true "Sitecore Form Item")

![MVP Mentor Page](images/send-sitecore-form.png?raw=true "MVP Mentor Page")

You can check by submitting the form and if successful, email & motivation is added to email list in Sitecore Send Email list.

Once the form is created, please create and item in Sitecore which can then be chosen as a datasource for SendForm Component
![Email List](images/send-email-list.png?raw=true "Email List")

## Sitecore
First step is to configure a custom condition as it is not a OOTB Sitecore Condition. Execution can be left empty as we do that as a part of Rule in Rendering host Side.

Example:
```csharp
 public class SendEmailSubscriptionCondition<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string SendEmailGroupId { get; set; }
        public string Value { get; set; }

        // Dummy condition only for serialization purposes
        protected override bool Execute(T ruleContext)
        {
            return false;
        }
    }
```
Second step is to create a custom parser on Sitecore platform side using BaseParser.

Example:
```csharp
public class SendEmailSubscriptionConditionParser : BaseParser
{
    private const string _typeId = "SendRule";

    public override object Parse(RuleCondition<ConditionalRenderingsRuleContext> condition)
    {
        var specificCondition = condition as SendEmailSubscriptionCondition<ConditionalRenderingsRuleContext>;
        return new JObject()
        {
            ["typeId"] = (JToken)_typeId,
            ["sendEmailGroupId"] = (JToken)specificCondition.SendEmailGroupId,
            ["value"] = (JToken)specificCondition.Value
        };
    }
}
```

Once it is done, add below configuration to Parse the condition.

Example:
```xml
<?xml version="1.0"?>
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
    <sitecore>
        <layoutService>
            <personalizationConditionParsers>
               <parser name="SendEmailSubscriptionCondition`1" type="Mvp.Foundation.LayoutServiceExtensions.Parsers.SendEmailSubscriptionConditionParser,Mvp.Foundation.LayoutServiceExtensions"/>
            </personalizationConditionParsers>
        </layoutService>
    </sitecore>
</configuration>
```

## Rendering Host
On the Rendering Host side you need to create a Rule as counterpart for the condition in Sitecore. This Rule will be executed and returns either true or false to determine if the variant it is used in should be displayed on the page.

First you need to create a Rule based on type _Mvp.Foundation.RulesEngine.Rules.Rule_.
Add any properties to this Rule which have been added to the JSON object as returned by the earlier created Parser. These will be populated when the condition is deserialized as a Rule.
Then add your implementation for the _Execute()_ method, returning either true or false based on the outcome of your evaluation.

Example:
```csharp
public class SendEmailSubscriptionRule : Rule
{
    private static SendOptions _sendOptions { get; set; }
    private string SendEmailGroupId { get; set; }
    public string Value { get; set; }
    public override bool Execute()
    {
        try
        {
            _sendOptions = this._configuration.GetSection("SitecoreSend").Get<SendOptions>();
            //mailing list id from condition
            var mailingListId = new Guid(this.Value);
            var apiKey = new Guid(_sendOptions.ApiKey);
            //Better user case is to get email id from OKta but for now , we are getting it from cookie
            var emailId = this._httpContext.Request.Cookies[$"mootrack_email_id"];
            if (string.IsNullOrEmpty(emailId))
                return false;

            //Sitecore Send client
            var apiClient = new Moosend.Api.Client.MoosendApiClient(apiKey);
            //Check if user is in Subscription list
            var response = apiClient.GetSubscriberByEmailAsync(mailingListId, emailId);

            return !string.IsNullOrEmpty(response?.Result?.Email);
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
```

Secondly you need to add this new Rule to the configuration. The appsettings.json contains a "Rules" sections which is considered as a dictionary.
Add a property with the name you used in your _Parser.typeId_. As value use the full namespace to the Rule you created in the Rendering Host application.

Example:
```json
{
    "Rules": {
        "SendRule": "Mvp.Foundation.RulesEngine.Rules.SendEmailSubscriptionRule, Mvp.Foundation.RulesEngine"
    }
}
```