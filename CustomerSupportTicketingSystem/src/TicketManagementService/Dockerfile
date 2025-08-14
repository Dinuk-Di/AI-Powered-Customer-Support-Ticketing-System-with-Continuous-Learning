# Use the official .NET 9.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files
COPY ["TicketManagementService.csproj", "./"]
COPY ["Models/", "./Models/"]
COPY ["Data/", "./Data/"]
COPY ["Services/", "./Services/"]
COPY ["Controllers/", "./Controllers/"]

# Restore dependencies
RUN dotnet restore "TicketManagementService.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "TicketManagementService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TicketManagementService.csproj" -c Release -o /app/publish

# Build the runtime image
FROM base AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "TicketManagementService.dll"]
