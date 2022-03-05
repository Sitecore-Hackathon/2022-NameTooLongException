using Microsoft.AspNetCore.Mvc;
using Mvp.Foundation.LayoutServiceExtensions.Middleware;
using System;

namespace Mvp.Foundation.LayoutServiceExtensions.Filters
{
    public class UsePersonalizedSitecoreRenderingAttribute : MiddlewareFilterAttribute
    {
        public UsePersonalizedSitecoreRenderingAttribute()
          : this(typeof(SetRenderingEnginePipeline))
        {
        }

        public UsePersonalizedSitecoreRenderingAttribute(Type configurationType)
          : base(configurationType)
        {
        }
    }
}
