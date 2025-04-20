# Base Image (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 6002

# Build Image (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching optimization
COPY *.sln ./
COPY Order.Api/*.csproj Order.Api/
COPY Order.Application/*.csproj Order.Application/
COPY Order.Domain/*.csproj Order.Domain/
COPY Order.Infrastructure/*.csproj Order.Infrastructure/

COPY BuildingBlocks/ApiClients/*.csproj BuildingBlocks/ApiClients/
COPY BuildingBlocks/Authentication/*.csproj BuildingBlocks/Authentication/
COPY BuildingBlocks/Authentication.Abstractions/*.csproj BuildingBlocks/Authentication.Abstractions/
COPY BuildingBlocks/Common/*.csproj BuildingBlocks/Common/
COPY BuildingBlocks/Core/*.csproj BuildingBlocks/Core/
COPY BuildingBlocks/EventBus/*.csproj BuildingBlocks/EventBus/
COPY BuildingBlocks/Persistence/*.csproj BuildingBlocks/Persistence/
COPY BuildingBlocks/Persistence.Abstractions/*.csproj BuildingBlocks/Persistence.Abstractions/
COPY BuildingBlocks/CommonAuthorization/*.csproj BuildingBlocks/CommonAuthorization/
# Restore Dependencies
#RUN dotnet restore --force --no-cache
RUN dotnet restore

# Kiểm tra xem project.assets.json có tồn tại không
RUN ls -l /src/Order.Api/obj/ || echo "project.assets.json NOT FOUND"

# Copy toàn bộ source code
COPY . .

# Xóa thư mục bin & obj để tránh lỗi build cache cũ
RUN find . -type d -name "bin" -exec rm -rf {} + && find . -type d -name "obj" -exec rm -rf {} +

RUN apt update && apt install -y curl

# Chạy lại restore để đảm bảo dependencies tồn tại
RUN dotnet restore

# Build Application
WORKDIR /src/Order.Api
RUN dotnet build -c Release -o /app/build --no-restore

# Publish Application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Final Stage (Runtime)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_HTTP_PORTS=6002
ENV ASPNETCORE_URLS=http://+:6002
ENTRYPOINT ["dotnet", "Order.Api.dll"]
