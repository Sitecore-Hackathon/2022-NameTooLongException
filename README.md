![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")
# Sitecore Hackathon 2022

- MUST READ: **[Submission requirements](SUBMISSION_REQUIREMENTS.md)**
- [Entry form template](ENTRYFORM.md)
- [Starter kit instructions](STARTERKIT_INSTRUCTIONS.md)

## Team name
NameTooLongException

## Category
Best addition to the Sitecore MVP site

## Description
Our addition to the MVP site is client-side personalization based on configuration contributed in Sitecore.
We all know the Sitecore personalization engine might be a bit slow from time to time. If your frontend is relying on a direct connection to the Layout Service, you will still notice this performance impact when personalization is used heavily in the site. Sitecore performs best when everything can be cached, which simply can't be done when a component/page is personalized.


<img src="docs/images/personalization-normal.png?raw=true" width="250px" style="background-color: white" />



To mitigate this, we moved the personalization engine to the 'client-side'. Instead of Sitecore evaluating all the conditions and returning the correct variant of a rendering, we want to return all variants as part of the JSON response from the Layout Service. This way we can make Sitecore always cache the JSON response, even when personalization has been applied.



<img src="docs/images/personalization-clientside.png?raw=true" width="250px" style="background-color: white" />



Within the Rendering Host we then evaluate all the variants (if applicable) and only return one of the variants to the parts of the Rendering Engine which actually render the components. The components themselves don't know anything about what happened, they just receive a single object with content to display.

Next to the fact that this brings performance improvements to the Sitecore platform, this setup will also make personalization possible when using Experience Edge. Normally, when using Experience Edge, Sitecore is only able to push items to Experience Edge which are then stored and served when requested. When the frontend application requests a page from Experience Edge, there is no callback happening to Sitecore to evaluate any kind of personalization rules.
By adding all variants to the item pushed to Experience Edge, we can again trigger personalization rules to be evaluated in the frontend application instead. This way it is possible to use personalization when using Experience Edge!



<img src="docs/images/personalization-edge.png?raw=true" width="250px" style="background-color: white" />



To demonstrate that this works, we implemented 5 different rules:
1. **Day of Week;** an OOTB Sitecore rule which checks the current day of the week against a provided list
2. **Month of Year;** an OOTB Sitecore rule which checks the current month against a provided list
3. **Page visited;** a rule integrated with **Sitecore CDP/Personalize** which triggers an Decision Model in Personalize to see if the current visitor has visited a specific page
4. **Form submitted;** a rule integrated with **Sitecore Send** which checks if the visitor has submitted a form (inserted from Sitecore Send) on the site
5. **Personalize Multivariant Experiment;** a rule which triggers an Experiment in **Sitecore Personalize** whether to show the variant or not

Please read the *Usage Instructions* section below to find out how to add additional rules, personalize a rendering and what the configuration in Sitecore Personalize and Sitecore Send looks like.

## Video link(s)

⟹ [Short Video](https://youtu.be/MsX_R2JB8L4)

⟹ [Detailed Video](https://youtu.be/2DVdjIluBnI)



## Pre-requisites and Dependencies
As this is an addition to the MVP site, the changes have been added to the MVP solution. This means the pre-requisites are exactly the same as used in the MVP site today.

- [.NET Core (>= v 3.1) and .NET Framework 4.8](https://dotnet.microsoft.com/download)
- Approx 40gb HD space
- [Okta Developer Account](https://developer.okta.com/signup/)
- Valid Sitecore license

## Installation instructions
As this is an addition to the MVP site, the changes have been added to the MVP solution. This means the installation process is exactly the same as used in the MVP site today.

1. 🏃‍♂️ Run the Start-Environment script from an _elevated_ PowerShell terminal 

    ```ps1
    .\Start-Environment -LicensePath "C:\path\to\license.xml"
    ```
   _Note:_  The LicensePath argument only has to be used on the initial run of the script. The license file must be named `license.xml`, the script copies it to the folder `.\docker\license` where it also can be placed or updated manually.  

   > You **must** use an elevated/Administrator PowerShell terminal  
   > [Windows Terminal](https://github.com/microsoft/terminal/releases) looks best but the built-in Windows Powershell 5.1 terminal works too.

2. ☕ Follow the on screen instructions.  

   _Note:_ that you will be asked to fill in the following values with your Okta developer account details:
      - OKTA_DOMAIN (*must* include protocol, e.g. `OKTA_DOMAIN=https://dev-your-id.okta.com`)
      - OKTA_CLIENT_ID
      - OKTA_CLIENT_SECRET  
   [Sign up for an Okta Developer Account](https://developer.okta.com/signup/)

   _If the wizard is aborted prematurely or if the container build fails then use the `-InitializeEnvFile` switch to re-run the full wizard._

    ```ps1
    .\Start-Environment.ps1 -InitializeEnvFile
    ```  

3. 🔑 When prompted, log into Sitecore via your browser, and accept the device authorization.  

4. 🚀 Wait for the startup script to open a browser tab with the Sitecore Launchpad.  

5. 🛑 To Stop the environment again  
   
   ```ps1
   .\Stop-Environment.ps1
   ```  

## Usage instructions
Adding personalization on a page is exactly the same as what we are used to in Sitecore, no instructions needed.

Please read the following instructions if you want to:
- [Configure a rendering to be personalizable](docs/configure-rendering.md)
- [Configure a new personalization rule/condition](docs/configure-rule.md)
- [Configure the demo rule in Sitecore Personalize](docs/configure-personalize.md)
- [Configure the demo rule in Sitecore Send](docs/configure-send.md)

## Comments

### How does it work?
Below are some notes describing how this solution works. There are two parts to it, first changes on the Sitecore/Layout Service side and secondly the changes to the Rendering Engine.

#### Sitecore changes
1. Override the renderingContentsResolver
    1. If the rendering has personalization rules (and not running in EE), then trigger custom code instead of original.
    2. Get the raw RenderingXml from the rendering, this contains all the options set on the rendering
    3. Convert it in a RenderingReference, a typed version of the XML
    4. Loop through all the Rules (Variants) and create JObjects from it including the contents of the selected datasource for this variant
    5. Return a JArray with all the variants found
2. Override the ResolveRenderingContents processor in the renderJsonRendering pipeline (this pipeline is triggered for each rendering on a page)
    1. If the rendering passing through this pipeline contains personalization rules, add a "HasPersonalizationRules" setting to the RenderingParams for later usage
3. Override the IPlaceholderTransformer; this is where the final structure of result JSON is created for a rendering
    1. If the RenderingParams contains the "HasPersonalizationRules" option then set the contents as a variants property instead of fields

#### Rendering Engine changes
1. Sitecore please fix your code to be extendable..
2. Skipping to the part which really matters..
3. Create a custom PlaceHolderJsonConverter
    1. When the JItem contains variants, then deserialize to a custom object 'PersonalizedComponent' instead of the OOTB Component model
    2. This PersonalizedComponent object contains all conditions as typed objects while using the Component model as base for the regular content
    3. In a custom LayoutServiceSerializer start using a custom JsonSerializerSettingsExtensions to include this new CustomPlaceHolderJsonConverter to the serialize configuration instead of the regular one
4. Create a custom version of the RenderingEngineMiddleware
    1. After getting the deserialized Layout Service response, loop through all the found placeholders and renderings.
    2. If the rendering is of type PersonalizedComponent, then execute custom logic to parse it
        1. Transform the PersonalizedComponent into a executable Rule, using a RuleFactory
        2. Execute the Rule, if it returns true then that variant should be used
        3. Get the fields from the variant and set it on the regular Component fields property
From there it is business as usual. Components don't know what happened, for those parts of the code it is just processing regular Placeholders and Components and don't know anything about Conditions, Rules, Variants or PersonalizedComponents.
