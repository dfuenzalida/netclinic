# Netclinic

A reimplementation of the [Spring Petclinic](https://github.com/spring-projects/spring-petclinic) using a React front-end and a .NET Core backend.

![NetClinic screenshot](https://raw.githubusercontent.com/dfuenzalida/netclinic/refs/heads/main/netclinic-screenshot.png)

## Requisites

* A recent LTS version of [NodeJS](https://nodejs.org/) to build the front-end
* [Microsoft .NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet) or later
* [Docker](docker.com) or [Podman](https://podman-desktop.io/) with [Podman Compose](https://podman-desktop.io/docs/compose/setting-up-compose)

## Development

For convenience, a Docker Compose file is provided for development. Run `docker-compose -f compose-dev.yaml up` to start:
* A PostgreSQL server, configured to create the database schema and load data on startup
* An NGINX Reverse Proxy configured to have the front-end and backend accessible from the same port (preventing CORS issues)
* The [adminer database manager](https://www.adminer.org/en/) on port 8088 to inspect the database

Start the backend API with: `dotnet run --environment Development` from the `NetClinic.Api` directory. You can validate that the API is working by opening `http://localhost:5299/api/vets` and the veterinarians data as JSON. 

To run the front-end, go to the `frontend/react-frontend` folder and install the dependencies with `npm install`, then start the front-end server with `npm run dev`. The Next.js server will run in port 8000, but in order for the API and front-end to be accessible from the same port, open the web app on `http://localhost:3000/`.

## Testing

To test locally, run the multi-container version with: `docker-compose up`.

## Deployment

To create a release of the front-end, run `npm run export` from the `frontend/react-frontend` folder, which will create the static content in the `out` folder.

To create a release of the backend, go to the `NetClinic.Api` directory and run `dotnet publish -c Release -o publish`.

## Cloud deployment

A convenient way to deploy this app is to use:
* [Azure Database for PostgreSQL flexible server](https://azure.microsoft.com/products/postgresql/) for your database,
* [Azure App Service](https://azure.microsoft.com/products/app-service/) to run the backend API,
* [Azure Static Web Apps](https://azure.microsoft.com/products/app-service/static) to run the front-end. The front-end can be [linked to the API](https://learn.microsoft.com/en-us/azure/static-web-apps/apis-app-service) in a few simple steps. 
  * To deploy the frontend to Azure Static Web Apps, run `npx swa deploy --env production --app-location out --resource-group $resourceGroup --app-name $appName`
