# Configure a rendering to be personalizable
By default, if no content resolver has been set on your JSON Rendering, it will use the renderingContentResolver as configured in the Layout Service configuration.
This one we replaced with our custom `PersonalizedRenderingContentsResolver`:
```xml
<?xml version="1.0"?>
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <layoutService>
      <configurations>
        <config name="jss">
          <rendering type="Sitecore.LayoutService.Configuration.DefaultRenderingConfiguration, Sitecore.LayoutService">
            <renderingContentsResolver type="Sitecore.LayoutService.ItemRendering.ContentsResolvers.RenderingContentsResolver, Sitecore.LayoutService">
			  <patch:attribute name="type" value="Mvp.Foundation.LayoutServiceExtensions.ContentsResolvers.PersonalizedRenderingContentsResolver, Mvp.Foundation.LayoutServiceExtensions" />
            </renderingContentsResolver>
          </rendering>
        </config>
      </configurations>
    </layoutService>
  </sitecore>
</configuration>
```

This will however not be used if you have set a specific Content Resolver on your Rendering.
If you still want to make use of the `PersonalizedRenderingContentsResolver` you can select it in the 'Rendering Contents Resolver' field on your Rendering item:



![Rendering Configuration](images/rendering-configuration.png?raw=true "Rendering Configuration")



> __Note__  
> 
> This Content Resolver only works as a Datasource Content Resolver, it does not work with Context Item or Child Item resolving.