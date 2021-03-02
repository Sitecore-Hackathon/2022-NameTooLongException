# escape=`

# This is an example Dockerfile for an ASP.NET Core Rendering Host.
# We use build stages to enable 'dotnet watch' during development, so
# that changes to your rendering code can be quickly tested, including in
# the Experience Editor. Be sure to watch the container logs in case of build errors.

ARG DEBUG_PARENT_IMAGE
ARG SOLUTION_IMAGE
ARG ENTRYPOINT_PROJECT

FROM ${DEBUG_PARENT_IMAGE} as debug
WORKDIR /solution/src/project

ARG ENTRYPOINT_PROJECT

ENV ENTRYPOINT_PROJECT_FOLDER="${ENTRYPOINT_PROJECT}/rendering"
ENV DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER=true

EXPOSE 80
ENTRYPOINT ["dotnet", "watch", "-v", "--project", ".\\$ENV:ENTRYPOINT_PROJECT_FOLDER", "run", "--no-launch-profile"]

FROM ${SOLUTION_IMAGE} as solution
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as release

ARG ENTRYPOINT_PROJECT

ENV ENTRYPOINT_ASSEMBLY="${ENTRYPOINT_PROJECT}.dll"

WORKDIR /app
COPY --from=solution /artifacts/rendering/ ./
EXPOSE 80
ENTRYPOINT ["dotnet", "dotnet $ENV:ENTRYPOINT_ASSEMBLY"]