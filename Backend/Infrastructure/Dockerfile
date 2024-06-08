# Use the .NET SDK image for building and running migrations
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR ../src/

# Copy the solution file and restore dependencies
COPY Backend.sln .
COPY Domain/Domain.csproj ./Domain/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/
COPY Api/Api.csproj ./Api/
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Install the dotnet-ef tool
RUN dotnet tool install --global dotnet-ef

# Ensure the dotnet tools path is added to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"