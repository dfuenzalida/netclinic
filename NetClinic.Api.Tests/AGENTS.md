# NetClinic.Api.Tests

This project is the unit/integration Tests companion project to the backend project at `../NetClinic.Api`.

### One-time Setup

```
dotnet tool install -g dotnet-reportgenerator-globaltool
export PATH="$PATH:$HOME/.dotnet/tools"
```

### Testing

Run the tests with coverage:

```
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"
```

Generate an HTML report

```
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./coveragereport" -reporttypes:Html
```
