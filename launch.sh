gulp clean min
dotnet restore
ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=https://*:8051 dotnet run
