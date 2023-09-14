#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Interview.UI/Interview.UI.csproj", "Interview.UI/"]
COPY ["Interview.Entities/Interview.Entities.csproj", "Interview.Entities/"]
RUN dotnet restore "Interview.UI/Interview.UI.csproj"
COPY . .
WORKDIR "/src/Interview.UI"
RUN dotnet build "Interview.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Interview.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Interview.UI.dll"]