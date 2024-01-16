FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.19-amd64

COPY DiscordReaderMod/ /app/

RUN apk update
RUN apk upgrade
RUN apk add zip

# Move to app directory
WORKDIR /app

# Fetch dependencies if not present
RUN dotnet restore

# Publish project
RUN dotnet publish -c Release -o /app/dist/

# Zip project
WORKDIR /app/dist/
RUN zip /app/mod.zip *

ENTRYPOINT ["tail", "-f", "/dev/null"]
