# Sử dụng .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj và restore dependencies
COPY ["Web_Restaurant.csproj", "."]
RUN dotnet restore "Web_Restaurant.csproj"

# Copy toàn bộ source code và build
COPY . .
RUN dotnet build "Web_Restaurant.csproj" -c Release -o /app/build

# Publish ứng dụng
RUN dotnet publish "Web_Restaurant.csproj" -c Release -o /app/publish

# Sử dụng runtime image để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy kết quả publish từ build stage
COPY --from=build /app/publish .

# Expose port 80 (hoặc port mà ứng dụng chạy)
EXPOSE 80

# Chạy ứng dụng khi container khởi động
ENTRYPOINT ["dotnet", "Web_Restaurant.dll"]