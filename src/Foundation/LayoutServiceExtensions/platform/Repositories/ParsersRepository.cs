using Mvp.Foundation.LayoutServiceExtensions.Parsers;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Xml;
using System.Xml;

namespace Mvp.Foundation.LayoutServiceExtensions.Repositories
{
    public class ParsersRepository
    {
        protected static SafeDictionary<string, BaseParser> Parsers { get; set; }

        static ParsersRepository()
        {
            Get();
        }

        public virtual BaseParser Get(string conditionName)
        {
            return Parsers[GetRequestName(conditionName)];
        }

        protected static string GetRequestName(string name)
        {
            Assert.ArgumentNotNullOrEmpty(name, "name");
            return name.ToUpperInvariant();
        }

        protected static void Get()
        {
            Parsers = new SafeDictionary<string, BaseParser>();
            foreach (XmlNode configNode in Factory.GetConfigNodes("layoutService/personalizationConditionParsers/parser"))
            {
                string name = XmlUtil.GetAttribute("name", configNode);
                string type = XmlUtil.GetAttribute("type", configNode);
                BaseParser request = Factory.CreateObject<BaseParser>(configNode);
                if (request == null || name == null)
                {
                    Log.Error("Could not instantiate personalization condition parser object, name:" + name + ", type:" + type, typeof(ParsersRepository));
                    continue;
                }
                lock (Parsers.SyncRoot)
                {
                    Parsers[GetRequestName(name)] = request;
                }
            }
        }
    }
}
