FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore ./RpaWorkerService.Worker/RpaWorkerService.Worker.csproj
RUN dotnet publish ./RpaWorkerService.Worker/RpaWorkerService.Worker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RpaWorkerService.Worker.dll"]