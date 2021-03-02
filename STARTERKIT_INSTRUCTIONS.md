# Docker Hackathon "Starter-kit"
This year we have included a simple starter-kit that generates a preconfigured docker-compose environment that you can use for the Hackathon.

> It is not required to use the starter-kit or Docker as long as all entry submission requirements are met.  
> _Do not spend time on the starter-kit if you do not have any previous experience with running Sitecore on Docker._  

If you do not want to use the starter-kit, you can delete the entire `.\_StarterKit` folder from your repository along with the .ps1 files in the solution root.  

You are also completely free to make any adjustments that is needed for your submission to the generated setup . Just keep it simple, please.

## Prerequisites 
Refer to the prerequisites from the [Sitecore Getting Started template walk-through](https://doc.sitecore.com/developers/100/developer-tools/en/walkthrough--using-the-getting-started-template.html). 

It is recommended to have followed this walk-through succesfully on each machine that will make use of this Starter-kit to ensure all prerequisites are in place.



## Initial setup
> If you have IIS running then stop it by running `iisreset /stop` or open 'IIS manager' > _right click computername_ > 'Stop'

1. Open a powershell terminal as Administrator
2. Navigate to this folder and run `.\Start-Hackathon.ps1`
3. Follow on-screen instructions
4. Test, git commit and push so other team members can pull the generated setup
5. Other team members should then git pull and run `.\Start-Hackathon.ps1` to generate locally trusted certificates and add hosts entries

If setup fails or you need to re-run the setup wizard for other reasons then run `.\Remove-Starterkit.ps1` to reset and run `.\Start-Hackathon.ps1` again.

After the initial setup `.\Start-Hackathon.ps1` can be used to bring up the containers - similar to manually running `.\docker-compose up -d` from the `.\docker` folder.

To take the containers down again you can use `.\Stop-Hackathon.ps1` - this calls `docker-compose down` followed by `docker system prune`. This can sometime help if you experience issues with the Sitecore containers not starting up or do not respond after computer has been turned off or put in Sleep-mode.

If the certificates are invalid then make sure that the mkcert self-signed root certificate is in `Local Machine > Trusted root certificates.`

## Deviations from Sitecore Docker examples

_docker-compose files has been moved the `.\docker` folder to keep the solution root clean._

_all images has Sitecore Powershell Extensions pre-installed_  

# yask for Sitecore
This "starter-kit" has evolved into _yet another Starter-kit_ for Sitecore. If you have any input, suggestions or other and would like to contribute to it's further development please check out [this repository](https://github.com/LaubPlusCo/sitecore-docker-starterkit)