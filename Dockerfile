FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /Bot.Runner

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish ./Bot.Runner/ -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /Bot
COPY --from=build-env /Bot.Runner/out .
ENTRYPOINT ["dotnet", "Bot.Runner.dll"]
