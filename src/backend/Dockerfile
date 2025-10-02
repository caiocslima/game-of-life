# Official .NET SDK image which contains all the tools to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY ["GameOfLife.csproj", "./"]
RUN dotnet restore "./GameOfLife.csproj"

# Copy the rest of the source code and build the application
COPY . .
RUN dotnet build "GameOfLife.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "GameOfLife.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Expose port 80 for the web application
EXPOSE 80

# Define the entry point for the container
ENTRYPOINT ["dotnet", "GameOfLife.dll"]
