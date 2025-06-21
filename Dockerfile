FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["Web_Restaurant.csproj", "."]
RUN dotnet restore "./Web_Restaurant.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
RUN dotnet build "./Web_Restaurant.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "./Web_Restaurant.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port that the app runs on
EXPOSE 8080

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Web_Restaurant.dll"]