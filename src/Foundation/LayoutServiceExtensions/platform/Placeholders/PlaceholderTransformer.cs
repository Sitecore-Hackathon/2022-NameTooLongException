using Sitecore.LayoutService.ItemRendering;
using System.Dynamic;

namespace Mvp.Foundation.LayoutServiceExtensions.Placeholders
{
    public class PlaceholderTransformer : Sitecore.JavaScriptServices.ViewEngine.LayoutService.Serialization.PlaceholderTransformer
    {
        /// <summary>
        /// If the rendering was processed by our custom functionality, the Contents will be an array of variants.
        /// If that is the case (RenderingParams contains "HasPersonalizationRules"), then add the Contents to a
        /// 'variants' property instead of the usual 'fields' property.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
		public override object TransformPlaceholderElement(RenderedPlaceholderElement element)
		{
            if (!(element is RenderedJsonRendering renderedJsonRendering))
            {
                return element;
            }
            dynamic val = new ExpandoObject();
			val.uid = renderedJsonRendering.Uid;
			val.componentName = renderedJsonRendering.ComponentName;
			val.dataSource = renderedJsonRendering.DataSource;
			val.@params = renderedJsonRendering.RenderingParams;
			// Change structure of result object if personalization has been applied
			if (renderedJsonRendering.Contents != null)
				if (renderedJsonRendering.RenderingParams.TryGetValue("HasPersonalizationRules", out var settingValue) 
                    && settingValue.Equals("1"))
					val.variants = renderedJsonRendering.Contents;
				else
					val.fields = renderedJsonRendering.Contents;
			if (renderedJsonRendering.Placeholders != null && renderedJsonRendering.Placeholders.Count > 0)
			{
				val.placeholders = TransformPlaceholders(renderedJsonRendering.Placeholders);
			}
			return val;
		}
	}
}