# escape=`

# This Dockerfile will build the Sitecore solution and save the build artifacts
# for use in other images, such as 'cm' and 'rendering'. It does not produce a
# runnable image itself.

ARG BASE_IMAGE
ARG BUILD_IMAGE

FROM ${BUILD_IMAGE} AS nuget-prep
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Gather only artifacts necessary for NuGet restore, retaining directory structure
COPY _Build/ /nuget/_Build/
COPY *.sln nuget.config Directory.Build.targets *.props /nuget/
COPY src/ /temp/
RUN Invoke-Expression 'robocopy C:/temp C:/nuget/src /s /ndl /njh /njs *.csproj Packages.props'

FROM ${BUILD_IMAGE} AS builder
ARG BUILD_CONFIGURATION
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Create an empty working directory
WORKDIR /build

# Copy prepped NuGet artifacts, and restore as distinct layer to take better advantage of caching
COPY --from=nuget-prep ./nuget ./

# Restore NuGet packages
RUN nuget restore -Verbosity quiet

# Copy remaining source code
COPY src/ ./src/

# Ensure deploy folders exist
RUN mkdir ./docker/deploy/platform; mkdir ./docker/deploy/rendering; mkdir ./docker/deploy/identity

# Build the solution to generate build artifacts
## assumes that the msbuild property <SitecoreRoleType>platform|rendering</SitecoreRoleType> is used to target different deploy folders 
RUN Get-ChildItem *.sln | %{  msbuild $_.FullName /p:Configuration=$env:BUILD_CONFIGURATION /m /p:DeployOnBuild=true /p:IsLocalDockerDeploy=true }

# Final solution assets image
FROM ${BASE_IMAGE}
WORKDIR /artifacts

# Copy in all artifacts from build
COPY --from=builder /build/docker/deploy/ ./