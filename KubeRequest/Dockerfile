FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /KubeRequest

# Copy everything
COPY *.csproj ./
# Restore as distinct layers
RUN dotnet restore

COPY . ./
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /KubeRequest
COPY --from=build-env /KubeRequest/out ./
ENTRYPOINT ["./KubeRequest"]
