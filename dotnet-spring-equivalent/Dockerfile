# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Port will be configured via environment variable
ARG PORT=5000
EXPOSE $PORT

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpringJavaEquivalent.csproj", "."]
RUN dotnet restore "SpringJavaEquivalent.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "SpringJavaEquivalent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpringJavaEquivalent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpringJavaEquivalent.dll"]
