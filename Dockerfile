FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /TranslateBot

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore ./TranslateBot/
# Build and publish a release
RUN dotnet publish ./TranslateBot/ -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /TranslateBot
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "TranslateBot.dll"]
