using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mvp.Foundation.LayoutServiceExtensions.Models;
using Mvp.Foundation.RulesEngine.Factories;
using Sitecore.AspNet.RenderingEngine;
using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.LayoutService.Client;
using Sitecore.LayoutService.Client.Request;
using Sitecore.LayoutService.Client.Response;
using Sitecore.LayoutService.Client.Response.Model;

namespace Mvp.Foundation.LayoutServiceExtensions.Middleware
{
    public class CustomRenderingEngineMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ISitecoreLayoutRequestMapper _requestMapper;

        private readonly ISitecoreLayoutClient _layoutService;
        private readonly IDefaultRuleFactory _ruleFactory;
        private readonly RenderingEngineOptions _options;
        private readonly IConfiguration _configuration;

        private HttpContext _httpContext;


        public CustomRenderingEngineMiddleware(RequestDelegate next, ISitecoreLayoutRequestMapper requestMapper, ISitecoreLayoutClient layoutService, IOptions<RenderingEngineOptions> options, IDefaultRuleFactory ruleFactory, IServiceProvider serviceprovider)
        {
            _next = next;
            _requestMapper = requestMapper;
            _layoutService = layoutService;
            _ruleFactory = ruleFactory;
            _options = options.Value;
            _configuration = (IConfiguration)serviceprovider?.GetService(typeof(IConfiguration));
        }

        public async Task Invoke(HttpContext httpContext, IViewComponentHelper viewComponentHelper, IHtmlHelper htmlHelper)
        {
            if (httpContext.Items.ContainsKey("CustomRenderingEngineMiddelware"))
            {
                throw new ApplicationException("CustomRenderingEngineMiddelware::InvalidRenderingEngineConfiguration");
            }
            if (httpContext.GetSitecoreRenderingContext() == null)
            {
                // Set Context
                _httpContext = httpContext;
                //STEP 1: GET LAYOUT RESPONSE WITH PERSONALIZED COMPONENT
                SitecoreLayoutResponse response = await GetSitecoreLayoutResponse(httpContext).ConfigureAwait(continueOnCapturedContext: false);
                //STEP 2: PROCESS RULES & CONVERT PERSONALIZEDCOMPONENT BACK TO COMPONENT by executing rules
                ConvertPlaceholders(response.Content.Sitecore.Route.Placeholders);
                //FINALLY SET RESPONSE in renderingContext (continue as it would normally)
                SitecoreRenderingContext renderingContext = new SitecoreRenderingContext
                {
                    Response = response,
                    RenderingHelpers = new RenderingHelpers(viewComponentHelper, htmlHelper)
                };
                httpContext.SetSitecoreRenderingContext(renderingContext);
            }
            else
            {
                httpContext.GetSitecoreRenderingContext().RenderingHelpers = new RenderingHelpers(viewComponentHelper, htmlHelper);
            }
            foreach (Action<HttpContext> postRenderingAction in _options.PostRenderingActions)
            {
                postRenderingAction(httpContext);
            }
            httpContext.Items.Add("CustomRenderingEngineMiddelware", null);
            await _next(httpContext).ConfigureAwait(continueOnCapturedContext: false);
        }

        private void ConvertPlaceholders(Dictionary<string, Placeholder> placeholders)
        {
            foreach (var ph in placeholders)
                ConvertPlaceholder(ph.Value);
        }

        private void ConvertPlaceholder(Placeholder placeholder)
        {
            foreach (var feature in placeholder)
            {
                if (feature is PersonalizedComponent personalizedComponent)
                {
                    var result = ProcessPersonalizedComponent(personalizedComponent);
                    personalizedComponent.Fields = result.Fields;

                    if (personalizedComponent?.Placeholders?.Any() ?? false)
                        ConvertPlaceholders(personalizedComponent.Placeholders);
                }
                else if (feature is Component component && (component?.Placeholders?.Any() ?? false))
                    ConvertPlaceholders(component.Placeholders);
            }
        }

        private Component ProcessPersonalizedComponent(PersonalizedComponent component)
        {
            if (component.Variants == null)
                return component;
            foreach (var variant in component.Variants)
            {
                if (variant.VariantId == Guid.Empty.ToString("B"))
                {
                    component.Fields = variant.Fields;
                    return component as Component;
                }
                //Get rule using type id, json and additionally httpcontext & configuration
                var rule = _ruleFactory.GetRule(variant.Condition.typeId, variant.Condition.OriginalJson, _httpContext, _configuration);
                if (rule?.Execute() ?? false)
                {
                    //If rule returned true on execution, then we set this variant's Fields to the Component.Fields property
                    component.Fields = variant.Fields;
                    //Then return the Component to stop the loop
                    return component as Component;
                }
            }
            return component;
        }

        private async Task<SitecoreLayoutResponse> GetSitecoreLayoutResponse(HttpContext httpContext)
        {
            SitecoreLayoutRequest sitecoreLayoutRequest = _requestMapper.Map(httpContext.Request);
            return await _layoutService.Request(sitecoreLayoutRequest).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
