FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/FitnessTrainingApp/FitnessTrainingApp.csproj src/FitnessTrainingApp/
RUN dotnet restore src/FitnessTrainingApp/FitnessTrainingApp.csproj

COPY src/FitnessTrainingApp/ src/FitnessTrainingApp/
WORKDIR /src/src/FitnessTrainingApp
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FitnessTrainingApp.dll"]
