using Microsoft.AspNetCore.Builder;
using Sitecore.AspNet.RenderingEngine.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvp.Foundation.LayoutServiceExtensions.Middleware
{
    public class SetRenderingEnginePipeline : RenderingEnginePipeline
    {
        public override void Configure(IApplicationBuilder app)
        {
            //Register Custom Rendering Engine Middleware
            app.UseMiddleware<CustomRenderingEngineMiddleware>(Array.Empty<object>());
        }
    }
}
