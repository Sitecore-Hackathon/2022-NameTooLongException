# escape=`

ARG PARENT_IMAGE
ARG TOOLS_IMAGE
ARG SOLUTION_IMAGE

FROM ${TOOLS_IMAGE} as tools
FROM ${PARENT_IMAGE}
FROM ${SOLUTION_IMAGE} as solution

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

COPY --from=tools \tools\ \tools\

# Copy solution xconnect files
