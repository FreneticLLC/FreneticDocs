call gulp clean min
set ASPNETCORE_ENVIRONMENT=Development
SET ASPNETCORE_URLS=https://*:8051
dotnet restore
dotnet run
