![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

# Submission Boilerplate

Welcome to Sitecore Hackathon 2021.

The Hackathon site can be found at http://www.sitecorehackathon.org/sitecore-hackathon-2021/

The purpose of this repository is to hand-in your Hackathon submissions.

The documentation for this years Hackathon must be provided as a readme in Markdown format as part of your submission.

Read and follow the instructions found in this file ⟹ [FILL_THIS_IN.md](FILL_THIS_IN.md) ⟸

## Docker Hackathon "Starter-kit"

This year we have decided to include a starter-kit with a basic solution setup including preconfigured docker-compose environments that follow the recommended approaches from Sitecore.

> It is not required to use the starter-kit or Docker as long as all entry submission requirements are met.  
> _Do not spend time on the starter-kit if you do not have any previous experience with running Sitecore on Docker._  
> Prerequisites for using the setup is the same as fpr [the Sitecore Getting Started template](https://doc.sitecore.com/developers/100/developer-tools/en/walkthrough--using-the-getting-started-template.html). It is recommended to try this template before using the Hackathon "Starter-kit".

__Initial setup:__
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

_A small deviation from Sitecore Docker examples; docker-compose files has been moved the `.\docker` folder to keep the solution root clean._

> If you do not want to use the boilerplate Docker setup, feel free to delete the entire `.\_StarterKit` folder from your repository. You can also make any adjustments you'd like to the setup that would be needed for your submission.

## Entry Submission Requirements 

All teams are required to submit the following as part of their entry submission on or before the end of the Hackathon on **Saturday March 6th 2021 at 8PM EST**. The modules should be based on [Sitecore Experience Platform 10.0 Update-1](https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/100/Sitecore_Experience_Platform_100_Update1.aspx).

_⟹ [FILL_THIS_IN.md](FILL_THIS_IN.md). ⟸_

**Failure to meet any of the requirements will result in automatic disqualification.** 

Please reach out to any of the organisers or judges if you require any clarification.

- Sitecore Experience Platform 10.0 Update-1 Module (Sitecore package, Dockerfile or other - see _[FILL_THIS_IN.md](FILL_THIS_IN.md)_)

- Module code in a public Git source repository. We will be judging (amongst other things):
  - Cleanliness of code
  - Commenting where necessary (and only where necessary)
  - Code Structure
  - Coding standards & naming conventions

- Precise and Clear Installation Instructions document (1 – 2 pages)
- Usage documentation 
  - Module Purpose
  - Module Sitecore Hackathon Category
  - How does the end user use the Module?
  - Screenshots, etc.

- Create a 2 – 10 minutes video explaining the submission functionality and provide a link to the video (f. ex. on youtube)

  - What problem was solved
  - How did you solve it
  - What is the end result

_⟹ [FILL_THIS_IN.md](FILL_THIS_IN.md). ⟸_