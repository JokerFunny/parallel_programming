FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /src
COPY . .
RUN dotnet restore Lab5/Lab5.csproj
RUN dotnet publish -c Release -o /src/result Lab5/Lab5.csproj

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /src/result /app
ENTRYPOINT [ "dotnet", "/app/Lab5.dll" ]