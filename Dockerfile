FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY FinexaApi.sln ./
COPY FinexaApi.csproj ./
RUN dotnet restore

COPY . ./
WORKDIR /app/FinexaApi
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV DOTNET_ENVIRONMENT=Production

COPY --from=build /out ./

EXPOSE 80
ENTRYPOINT ["dotnet", "FinexaApi.dll"]