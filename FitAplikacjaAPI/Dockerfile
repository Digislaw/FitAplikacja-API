FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["FitAplikacjaAPI/FitAplikacjaAPI.csproj", "FitAplikacjaAPI/"]
COPY ["FitAplikacja.Infrastructure/FitAplikacja.Infrastructure.csproj", "FitAplikacja.Infrastructure/"]
COPY ["FitAplikacja.Core/FitAplikacja.Core.csproj", "FitAplikacja.Core/"]
RUN dotnet restore "FitAplikacjaAPI/FitAplikacjaAPI.csproj"
COPY . .
WORKDIR "/src/FitAplikacjaAPI"
RUN dotnet build "FitAplikacjaAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FitAplikacjaAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitAplikacjaAPI.dll"]