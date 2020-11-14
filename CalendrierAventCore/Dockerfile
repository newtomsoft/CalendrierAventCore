#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["CalendrierAventCore/CalendrierAventCore.csproj", "CalendrierAventCore/"]
COPY ["Dal/Dal.csproj", "Dal/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Tools/Tools.csproj", "Tools/"]
RUN dotnet restore "CalendrierAventCore/CalendrierAventCore.csproj"
COPY . .
WORKDIR "/src/CalendrierAventCore"
RUN dotnet build "CalendrierAventCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CalendrierAventCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CalendrierAventCore.dll"]