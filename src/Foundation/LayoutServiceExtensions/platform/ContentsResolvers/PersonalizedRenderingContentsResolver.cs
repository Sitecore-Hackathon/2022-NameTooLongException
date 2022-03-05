using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Common;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.Helpers;
using Sitecore.LayoutService.ItemRendering.ContentsResolvers;
using Sitecore.Links;
using Sitecore.Layouts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sitecore.Mvc.Extensions;
using Mvp.Foundation.LayoutServiceExtensions.Models;
using Sitecore.Rules.ConditionalRenderings;
using Mvp.Foundation.LayoutServiceExtensions.Parsers;
using Newtonsoft.Json;
using Sitecore.Reflection;
using System;

namespace Mvp.Foundation.LayoutServiceExtensions.ContentsResolvers
{
    public class PersonalizedRenderingContentsResolver : IRenderingContentsResolver
    {
        private readonly IDictionary<string, string> parsers = new Dictionary<string, string>()
        {
            { "DayOfWeekCondition`1", "Mvp.Foundation.LayoutServiceExtensions.Parsers.DayOfWeekConditionParser,Mvp.Foundation.LayoutServiceExtensions" },
            { "CurrentMonthCondition`1", "Mvp.Foundation.LayoutServiceExtensions.Parsers.MonthOfYearConditionParser,Mvp.Foundation.LayoutServiceExtensions" },
            { "BoxeverCondition`1", "Mvp.Foundation.LayoutServiceExtensions.Parsers.BoxeverConditionParser,Mvp.Foundation.LayoutServiceExtensions" }
        };
        public bool IncludeServerUrlInMediaUrls { get; set; } = true;

        public bool UseContextItem { get; set; }

        public string ItemSelectorQuery { get; set; }

        public NameValueCollection Parameters { get; set; } = new NameValueCollection(0);

        public virtual object ResolveContents(
          Sitecore.Mvc.Presentation.Rendering rendering,
          IRenderingConfiguration renderingConfig)
        {
            Assert.ArgumentNotNull((object)rendering, nameof(rendering));
            Assert.ArgumentNotNull((object)renderingConfig, nameof(renderingConfig));

            // If rendering has personalization rules we want to process it differently, otherwise use default OOTB code
            if(rendering.Properties.Contains("PersonlizationRules") && Sitecore.Context.PageMode.IsNormal)
                return ResolvePersonalizedContents(rendering, renderingConfig);
            return ResolveSimpleContents(rendering, renderingConfig);
        }

        private object ResolveSimpleContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            Item contextItem = this.GetContextItem(rendering, renderingConfig);
            if (contextItem == null)
                return (object)null;
            if (string.IsNullOrWhiteSpace(this.ItemSelectorQuery))
                return (object)this.ProcessItem(contextItem, rendering, renderingConfig);
            JObject jobject = new JObject()
            {
                ["items"] = (JToken)new JArray()
            };
            IEnumerable<Item> items = this.GetItems(contextItem);
            List<Item> objList = items != null ? items.ToList<Item>() : (List<Item>)null;
            if (objList == null || objList.Count == 0)
                return (object)jobject;
            jobject["items"] = (JToken)this.ProcessItems((IEnumerable<Item>)objList, rendering, renderingConfig);
            return (object)jobject;
        }

        private object ResolvePersonalizedContents(Sitecore.Mvc.Presentation.Rendering rendering, IRenderingConfiguration renderingConfig)
        {
            string property = rendering.Properties["RenderingXml"];
            //RenderingRefernce contains all settings about a personalization configuration
            var reference = string.IsNullOrEmpty(property) ? null : new RenderingReference(XElement.Parse(property).ToXmlNode(), rendering.Item.Language, rendering.Item.Database);

            if (reference == null)
                return ResolveSimpleContents(rendering, renderingConfig);

            if (reference.Settings.Rules.Count <= 1)
                return ResolveSimpleContents(rendering, renderingConfig);

            var variants = new JArray();
            foreach(var rule in reference.Settings.Rules.Rules)
            {
                // This is always the default rule
                if (rule.UniqueId.IsNull)
                {
                    var dataSourceItem = rendering.Item.Database.GetItem(new ID(reference.Settings.DataSource));
                    var variant = new Variant
                    {
                        Name = rule.Name,
                        VariantId = rule.UniqueId.ToGuid().ToString("B"),
                        Fields = ProcessItem(dataSourceItem, rendering, renderingConfig)
                    };

                    variants.Add(JObject.Parse(JsonConvert.SerializeObject(variant)));
                }
                else if (rule.Actions.Any() && rule.Condition != null)
                {
                    // Check if the action is of type SetDataSourceAction (the only one we support right now)
                    var action = rule.Actions.FirstOrDefault(a => a is SetDataSourceAction<ConditionalRenderingsRuleContext>) as SetDataSourceAction<ConditionalRenderingsRuleContext>;
                    if (action != null)
                    {
                        var conditionType = rule.Condition.GetType();
                        object conditionResult;
                        if (parsers.TryGetValue(conditionType.Name, out var parserType))
                        {
                            var parser = ReflectionUtil.CreateObject(Type.GetType(parserType)) as BaseParser;
                            conditionResult = parser.Parse(rule.Condition);
                            var dataSourceItem = rendering.Item.Database.GetItem(new ID(action.DataSource));
                            var variant = new Variant
                            {
                                Name = rule.Name,
                                VariantId = rule.UniqueId.ToGuid().ToString("B"),
                                Fields = ProcessItem(dataSourceItem, rendering, renderingConfig),
                                Condition = conditionResult
                            };
                            variants.Add(JObject.Parse(JsonConvert.SerializeObject(variant)));
                        }
                    }
                }
            }

            // If only 1 rule was compatible, then we just want to revert to default behavior
            if (variants.Count <= 1)
                return ResolveSimpleContents(rendering, renderingConfig);
            return variants;
        }

        protected virtual IEnumerable<Item> GetItems(Item contextItem)
        {
            Assert.ArgumentNotNull((object)contextItem, nameof(contextItem));
            return string.IsNullOrWhiteSpace(this.ItemSelectorQuery) ? Enumerable.Empty<Item>() : (IEnumerable<Item>)contextItem.Axes.SelectItems(this.ItemSelectorQuery);
        }

        protected virtual Item GetContextItem(
          Sitecore.Mvc.Presentation.Rendering rendering,
          IRenderingConfiguration renderingConfig)
        {
            if (this.UseContextItem)
                return Context.Item;
            if (string.IsNullOrWhiteSpace(rendering.DataSource))
                return (Item)null;
            Item obj = rendering.RenderingItem?.Database.GetItem(rendering.DataSource);
            if (obj != null)
                return obj;
            DataUri uri = DataUri.Parse(rendering.DataSource);
            if (!(uri != (DataUri)null))
                return (Item)null;
            return rendering.RenderingItem?.Database.GetItem(uri);
        }

        protected virtual JArray ProcessItems(
          IEnumerable<Item> items,
          Sitecore.Mvc.Presentation.Rendering rendering,
          IRenderingConfiguration renderingConfig)
        {
            JArray jarray = new JArray();
            foreach (Item obj in items)
            {
                JObject jobject1 = this.ProcessItem(obj, rendering, renderingConfig);
                JObject jobject2 = new JObject()
                {
                    ["id"] = (JToken)obj.ID.Guid.ToString(),
                    ["url"] = (JToken)LinkManager.GetItemUrl(obj, ItemUrlHelper.GetLayoutServiceUrlOptions()),
                    ["name"] = (JToken)obj.Name,
                    ["displayName"] = (JToken)obj.DisplayName,
                    ["fields"] = (JToken)jobject1
                };
                jarray.Add((JToken)jobject2);
            }
            return jarray;
        }

        protected virtual JObject ProcessItem(
          Item item,
          Sitecore.Mvc.Presentation.Rendering rendering,
          IRenderingConfiguration renderingConfig)
        {
            Assert.ArgumentNotNull((object)item, nameof(item));
            using (new SettingsSwitcher("Media.AlwaysIncludeServerUrl", (Switcher<bool, IncludeServerInMediaUrlSwitcher>.CurrentValue || this.IncludeServerUrlInMediaUrls).ToString()))
                return JObject.Parse(renderingConfig.ItemSerializer.Serialize(item));
        }
    }
}