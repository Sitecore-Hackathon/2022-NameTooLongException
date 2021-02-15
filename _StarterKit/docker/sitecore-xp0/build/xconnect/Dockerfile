# escape=`

ARG PARENT_IMAGE
ARG TOOLS_IMAGE
ARG SOLUTION_IMAGE

FROM ${TOOLS_IMAGE} as tools
FROM ${SOLUTION_IMAGE} as solution

FROM ${PARENT_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

COPY --from=tools \tools\ \tools\
