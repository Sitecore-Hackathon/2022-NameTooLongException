# escape=`

ARG PARENT_IMAGE
ARG SOLUTION_IMAGE
ARG HEADLESS_SERVICES_IMAGE
ARG SPE_IMAGE
ARG TOOLS_IMAGE

FROM ${SOLUTION_IMAGE} as solution
FROM ${TOOLS_IMAGE} as tools
FROM ${SPE_IMAGE} as spe
FROM ${HEADLESS_SERVICES_IMAGE} AS headless_services
FROM ${PARENT_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR C:\build
ARG NODEJS_VERSION
RUN Invoke-WebRequest -OutFile nodejs.zip -UseBasicParsing "https://nodejs.org/dist/v$env:NODEJS_VERSION/node-v$env:NODEJS_VERSION-win-x64.zip"; `
    Expand-Archive nodejs.zip -DestinationPath C:\; `
    Rename-Item "c:\\node-v$env:NODEJS_VERSION-win-x64" c:\nodejs; `
    setx /M PATH $($Env:PATH + ';C:\nodejs'); `
    Remove-Item -Path nodejs.zip -Force;

# Copy development tools and entrypoint
COPY --from=tools \tools\ \tools\

WORKDIR C:\inetpub\wwwroot

# Add SPE module
COPY --from=spe \module\cm\content .\
RUN Rename-Item -Path "c:\inetpub\wwwroot\App_Config\Include\Spe\Spe.IdentityServer.config.disabled" -NewName "Spe.IdentityServer.config"

# Add JSS module
COPY --from=headless_services \module\cm\content .\
COPY --from=headless_services \module\tools \module\tools
RUN C:\module\tools\Initialize-Content.ps1 -TargetPath .\; `
    Remove-Item -Path C:\module -Recurse -Force;

# Allow configurable JSS Import User
COPY .\config .\App_Config\Include\

# Copy solution website files
COPY --from=solution /artifacts/platform/ ./
