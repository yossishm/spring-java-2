# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
