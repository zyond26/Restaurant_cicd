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

# Create directory for DataProtection keys and set permissions
RUN mkdir -p /app/keys && chmod 777 /app/keys
VOLUME /app/keys

COPY --from=publish /app/publish .

# Expose ports (adjust to match your app configuration)
EXPOSE 5000 

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Web_Restaurant.dll"]

