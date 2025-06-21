# # Base image to run the app
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081

# # Image to build the app
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["Web_Restaurant.csproj", "."]
# RUN dotnet restore "./Web_Restaurant.csproj"
# COPY . .
# RUN dotnet build "./Web_Restaurant.csproj" -c $BUILD_CONFIGURATION -o /app/build

# # Publish the app
# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./Web_Restaurant.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# # Final image
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Web_Restaurant.dll"]

# Base image for the build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
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