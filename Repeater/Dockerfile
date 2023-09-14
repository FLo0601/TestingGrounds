FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /Repeater

# Copy everything
COPY *.csproj ./
# Restore as distinct layers
RUN dotnet restore

COPY . ./
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /Repeater
COPY --from=build-env /Repeater/out ./
ENTRYPOINT ["./Repeater"]
