# escape=`

ARG PARENT_IMAGE
FROM ${PARENT_IMAGE}

WORKDIR /entry

COPY ./Start.bat ./

EXPOSE 3000

ENTRYPOINT ["Start.bat"]