FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Esta/Esta.csproj", "Esta/"]
RUN dotnet restore "Esta/Esta.csproj"
COPY . .
WORKDIR "/src/Esta"
RUN dotnet build "Esta.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Esta.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Esta.dll"]
