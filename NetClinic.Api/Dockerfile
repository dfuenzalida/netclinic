
# Stage 1: restore and build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["NetClinic.Api.csproj", "./"]
RUN dotnet restore "NetClinic.Api.csproj"

# copy everything else and build
COPY . ./
RUN dotnet build "NetClinic.Api.csproj" -c Release -o /app/build /p:UseAppHost=false

# Stage 2: publish
FROM build AS publish
RUN dotnet publish "NetClinic.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Stage 3: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

ARG DEBIAN_FRONTEND=noninteractive

RUN apt update && \
	apt install -y wget curl && \
	apt clean && \
	rm -rf /var/lib/apt/lists/*

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5299
EXPOSE 5299

COPY --from=publish /app/publish ./

HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
	CMD curl -f http://localhost/healthz || exit 1

ENTRYPOINT ["dotnet", "NetClinic.Api.dll"]

