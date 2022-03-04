# Docker Hackathon "Starter-kit"
This year we have again included a simple starter-kit that generates a preconfigured docker-compose environment that you can use for the Hackathon.

> It is not required to use the starter-kit or Docker as long as all entry submission requirements are met.  
> _Do not spend time on the starter-kit if you do not have any previous experience with running Sitecore on Docker._  

If you do not want to use the starter-kit, you can delete the entire `.\_StarterKit` folder from your repository along with the .ps1 files in the solution root.  

You are also completely free to make any adjustments that is needed for your submission to the generated setup . **Just keep it simple**, please.

## Prerequisites 
Refer to the prerequisites from the [Sitecore Getting Started template walk-through](https://doc.sitecore.com/developers/100/developer-tools/en/walkthrough--using-the-getting-started-template.html). 

It is recommended to have followed this walk-through succesfully on each machine that will make use of this Starter-kit to ensure all prerequisites are in place.

## Initial setup
> The script will stop IIS if it is running on the local machine.

1. Open a powershell terminal as Administrator
2. Navigate to this folder and run `.\Start-Hackathon.ps1`
    > For idea #3 run `.\Start-Hackathon.ps1 -IdeaThree` 
3. Follow on-screen instructions
4. Test, git commit and push so other team members can pull the generated setup
5. Other team members should then git pull and run `.\Start-Hackathon.ps1` to generate locally trusted certificates and add hosts entries

If setup fails or you need to re-run the setup wizard for other reasons then run `.\Remove-Starterkit.ps1` to reset and run `.\Start-Hackathon.ps1` again.

After the initial setup `.\Start-Hackathon.ps1` can be used to bring up the containers - similar to manually running `.\docker-compose up -d` from the `.\docker` folder.

To take the containers down again you can use `.\Stop-Hackathon.ps1` - this calls `docker-compose down` followed by `docker system prune`. This can sometime help if you experience issues with the Sitecore containers not starting up or do not respond after computer has been turned off or put in Sleep-mode.

If the certificates are invalid then make sure that the mkcert self-signed root certificate is in `Local Machine > Trusted root certificates.`  
### Solution setup in details
> **Optional to use**  
> _Do not waste valuable Hackathon time on this._

A simple solution build setup is included together with the Docker preset. 

The build setup caters specifically to use dotnet SDK style for _all_ projects (including net48) but it does not require it.  

You can use the included templates for [Sitecore Helix Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=AndersLaublaubplusco.SitecoreHelixVisualStudioTemplates) to get started quickly.  

The msbuild property `SitecoreRoleType` automatically load common msbuild properties for the specified roletype into a project. 
This include properties such as `publishUrl` which typically would have resided in a .pubxml file back in the old days.  

_example usage in csproj file:_  
```xml
    <PropertyGroup>
        <TargetFramework>net48</TargetFramework>
        <RootNamespace>Feature.FancyStuff.Platform</RootNamespace>
        <AssemblyName>Feature.FancyStuff.Platform</AssemblyName>
        <SitecoreRoleType>platform</SitecoreRoleType>
    </PropertyGroup>
```

This will load the properties from `.\_Build\props\Platform.Properties.props` into the project on build time. 

To see how projects is intended to be added in this setup, please use the included templates for the Sitecore Helix Visual Studio Extension.

`SitecoreRoleType` also ensure that Sitecore assemblies are not published to platform by automatically adding a package reference to `Sitecore.Assemblies.Platform`.    

The setup includes 2 role types, `platform` & `rendering`. You can easily add new roletypes for other publish targets if needed. This is done by adding a file `.\_Build\props\{role-type}.Properties.props`

Then to use this in a .csproj file set:  

```xml
    <SitecoreRoleType>{role-type}</SitecoreRoleType>
```

This will load the msbuild properties from the `{role-type}.Properties.props` file on build-time before any targets are called.

> if you don't want to use the solution setup or if you encounter issues but still would like to use the Docker preset then you should just delete / update the following:
> * Directory.build.props
> * Directory.build.targets
> * Packages.props
> * .\\_Build (entire folder)  
> 
> then you can use whatever setup that you are familiar with.  
> _Do not waste valuable Hackathon time._

## Deviation from the Sitecore Docker examples

_docker-compose files has been moved the `.\docker` folder to keep the solution root clean._

_all images has Sitecore Powershell Extensions pre-installed_  

# yask for Sitecore
This "starter-kit" _almost_ evolved into _yet another Starter-kit_ for Sitecore last year. If you have any input, suggestions or other and would like to contribute to it's further development please check out [this repository](https://github.com/LaubPlusCo/sitecore-docker-starterkit)