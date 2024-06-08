# Use the ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY Backend.sln .
COPY Domain/Domain.csproj ./Domain/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/
COPY Api/Api.csproj ./Api/
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . .

WORKDIR /src/Api
RUN dotnet build -c Release -o /app

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app

# Stage 3: Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Api.dll"]

