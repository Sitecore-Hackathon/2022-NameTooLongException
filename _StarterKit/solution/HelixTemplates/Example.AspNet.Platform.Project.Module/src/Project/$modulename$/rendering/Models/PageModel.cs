using System;
using Sitecore.AspNet.RenderingEngine.Binding.Attributes;
using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace $moduleNameSpace$.Rendering.Models
{
    /// <summary>
    /// An example of binding to fields on the requested server route (a.k.a the "Context Item").
    /// </summary>
    public class PageModel
    {
        [SitecoreRouteField]
        public TextField Title { get; set; }
    }
}
