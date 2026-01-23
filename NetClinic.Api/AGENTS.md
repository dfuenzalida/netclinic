# NetClinic.Api

This is a web API project written in C# with .NET 9.0 using the usual Controllers, Services and Model classes.

The project is the backend API that provides the data to be rendered in the front-end. Currently the only front-end implementation is in the folder `../frontend/react-frontend` which is written in TypeScript with React.

## Development

During development, the approach is to launch some utility containers with `docker-compose -f compose-dev.yaml up` from the parent folder. This starts a PostgreSQL container, and NGINX load balancer and the Adminer tool. NGINX is configured to proxy the front-end web server and this backend API, exposing them in port 3000.

## Testing

Unit and integration tests can be found in the folder `../NetClinic.Api.Tests`.

