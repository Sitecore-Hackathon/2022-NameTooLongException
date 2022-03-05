# Configure a new personalization rule/condition
When configuring a personalization condition to be compatible with this personalization setup, you need to make changes in both Sitecore and the Rendering Host application.

## Sitecore
First you will need to create a new 'Parser'. This parser needs to be based on _Mvp.Foundation.LayoutServiceExtensions.Parsers.BaseParser_.
The Parse method has a parameter of type _RuleCondition<ConditionalRenderingsRuleContext>_. When this parser is executed it will be populated with the explicit RuleCondition object of the condition you want to parse.
The parser needs to return a JObject, or at least an object which can be serialized by Newtonsoft.Json at a later step in the process.
Additionally you need to set the _typeId property in the Parser. This will be included in the output JSON and used to make this condition to an Rule in the Rendering Host.

Example:
```csharp
public class DayOfWeekConditionParser : BaseParser
{
    private const string _typeId = "DayOfWeek";

    public override object Parse(RuleCondition<ConditionalRenderingsRuleContext> condition)
    {
        var specificCondition = condition as DayOfWeekCondition<ConditionalRenderingsRuleContext>;
        return new JObject()
        {
            ["typeId"] = (JToken)_typeId,
            ["listOfDays"] = (JToken)GetListOfDays(specificCondition.DaysList)
        };
    }

    private JArray GetListOfDays(string daysList)
    {
        string[] strArray = daysList.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        JArray dayOfWeekList = new JArray();
        foreach (string key in strArray)
        {
            var dayOfWeekItem = Sitecore.Context.Database.GetItem(new Sitecore.Data.ID(key));
            dayOfWeekList.Add(dayOfWeekItem?.Name);
        }
        return dayOfWeekList;
    }
}
```

Secondly you need to add this new type to the configuration. You can add this to the _layoutService/personalizationConditionParsers/parser_ section of the Sitecore configuration. The `<parser>` element needs two attributes, 'name' and 'type'. 
For name you need to set the type name of the condition. For the `DayOfWeekCondition<ConditionalRenderingsRuleContext>` condition this would be 'DayOfWeekCondition\`1' (`<T>` is replaced with \`1).
The type attribute will be the full namespace to the parser you created.

Example:
```xml
<?xml version="1.0"?>
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
    <sitecore>
        <layoutService>
            <personalizationConditionParsers>
                <parser name="DayOfWeekCondition`1" type="Mvp.Foundation.LayoutServiceExtensions.Parsers.DayOfWeekConditionParser,Mvp.Foundation.LayoutServiceExtensions"/>
                <parser name="CurrentMonthCondition`1" type="Mvp.Foundation.LayoutServiceExtensions.Parsers.MonthOfYearConditionParser,Mvp.Foundation.LayoutServiceExtensions"/>
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
public class DayOfWeekRule : Rule
{
    public string[] ListOfDays { get; set; } = Array.Empty<string>();

    public override bool Execute()
    {
        var currentDay = DateTime.Now.DayOfWeek.ToString();
        return ListOfDays.Contains(currentDay);
    }
}
```

Secondly you need to add this new Rule to the configuration. The appsettings.json contains a "Rules" sections which is considered as a dictionary.
Add a property with the name you used in your _Parser.typeId_. As value use the full namespace to the Rule you created in the Rendering Host application.

Example:
```json
{
    "Rules": {
        "DayOfWeek": "Mvp.Foundation.RulesEngine.Rules.DayOfWeekRule, Mvp.Foundation.RulesEngine",
        "MonthOfYear": "Mvp.Foundation.RulesEngine.Rules.MonthOfYearRule, Mvp.Foundation.RulesEngine"
    }
}
```