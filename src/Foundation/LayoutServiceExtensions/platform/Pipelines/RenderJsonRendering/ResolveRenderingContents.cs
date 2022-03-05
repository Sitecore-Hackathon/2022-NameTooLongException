using Sitecore.Diagnostics;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using Sitecore.LayoutService.Configuration;
using Mvp.Foundation.LayoutServiceExtensions.ContentsResolvers;

namespace Mvp.Foundation.LayoutServiceExtensions.Pipelines.RenderJsonRendering
{
    public class ResolveRenderingContents : BaseRenderJsonRendering
    {
        public ResolveRenderingContents(IConfiguration configuration)
          : base(configuration)
        {
        }

        /// <summary>
        /// If the rendering has personalization rules and its content was resolved 
        /// by the PersonalizedRenderingContentsResolver then add an additional RenderingParam.
        /// This is the only way to let the PlaceholderTransformer know to treat this rendering differently.
        /// </summary>
        /// <param name="args"></param>
        protected override void SetResult(RenderJsonRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.IsNotNull(args.Result, "args.Result cannot be null");
            args.Result.Contents = this.GetResolvedContents(args);
            if (args.Result.Contents != null 
                && args.Rendering.Properties.Contains("PersonlizationRules") 
                && Sitecore.Context.PageMode.IsNormal 
                && args.RenderingContentsResolver is PersonalizedRenderingContentsResolver)
                args.Result.RenderingParams.Add("HasPersonalizationRules", "1");
        }

        protected virtual object GetResolvedContents(RenderJsonRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.IsNotNull(args.RenderingContentsResolver, "args.RenderingContentsResolver cannot be null");
            return args.RenderingContentsResolver.ResolveContents(args.Rendering, args.RenderingConfiguration);
        }
    }
}