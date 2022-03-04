using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.AspNet.RenderingEngine.Extensions;

namespace $moduleNamespace$.Rendering.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static RenderingEngineOptions AddFeature$moduleName$(this RenderingEngineOptions options)
        {
            /*
            options.AddModelBoundView<MyModel>("MyView");
            */
            return options;
        }
    }
}