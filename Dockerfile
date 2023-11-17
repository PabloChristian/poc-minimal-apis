FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ParkingLot/ParkingLot.csproj", "ParkingLot/"]
RUN dotnet restore "ParkingLot/ParkingLot.csproj"
COPY . .
WORKDIR "/src/ParkingLot"
RUN dotnet build "ParkingLot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParkingLot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParkingLot.dll"]
