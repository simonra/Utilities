FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore /App/PseudonymizationApi/PseudonymizationApi.csproj
# Build and publish a release
# RUN dotnet publish -c Release -o out
RUN dotnet publish --no-restore --configuration Release --output out /App/PseudonymizationApi/PseudonymizationApi.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "PseudonymizationApi.dll"]
