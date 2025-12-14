# Integration Tests for NetClinic API

This directory contains comprehensive integration tests for the NetClinic API using Testcontainers to provide a realistic PostgreSQL database environment.

## Overview

These integration tests verify the complete functionality of the NetClinic API controllers by:

- **Using Testcontainers**: Each test class spins up a real PostgreSQL container
- **Testing Real Database Interactions**: Tests execute against actual database operations
- **Covering All Controllers**: Tests for OwnersController, PetsController, VetsController, and OopsController
- **Validating API Contracts**: Tests verify HTTP status codes, response formats, and business logic

## Test Structure

### BaseIntegrationTest
- Abstract base class that sets up the testing infrastructure
- Manages PostgreSQL Testcontainer lifecycle
- Configures WebApplicationFactory with test database
- Seeds test data (owners, pets, pet types, vets)
- Provides helper methods for database verification

### Controller Test Classes

#### OwnersControllerIntegrationTests
- **GET /owners** - List owners with pagination and filtering
- **GET /owners/{id}** - Get owner by ID
- **POST /owners** - Create new owner
- **PUT /owners/{id}** - Update existing owner
- Validates owner data creation, retrieval, and validation

#### PetsControllerIntegrationTests  
- **GET /owners/{ownerId}/pets** - Get pets by owner
- **GET /owners/{ownerId}/pets/{petId}** - Get specific pet
- **GET /owners/{ownerId}/pets/{petId}/visits** - Get pet visits
- **POST /owners/{ownerId}/pets/{petId}/visits** - Create visit
- **GET /pet/types** - Get all pet types
- Tests pet management and visit creation functionality

#### VetsControllerIntegrationTests
- **GET /vets** - List veterinarians with pagination
- **GET /vets/{id}** - Get veterinarian by ID
- Tests veterinarian data retrieval and pagination

#### OopsControllerIntegrationTests
- **GET /oops** - Tests exception handling behavior
- Validates that the controller throws expected exceptions

## Key Features

### Testcontainers Integration
```csharp
private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:15-alpine")
    .WithDatabase("netclinic_test")
    .WithUsername("test_user")
    .WithPassword("test_password")
    .WithCleanUp(true)
    .Build();
```

### Database Seeding
The base test class seeds realistic test data:
- 4 owners with complete contact information
- 5 pets with different types (dogs, cats) and birth dates
- 3 veterinarians
- 4 pet types (Dog, Cat, Bird, Hamster)

### Test Data Isolation
Each test method gets a fresh database instance, ensuring complete isolation between tests.

## Running the Tests

### Prerequisites
- Docker must be installed and running
- .NET 9.0 SDK

### Run All Integration Tests
```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

### Run Specific Controller Tests
```bash
dotnet test --filter "OwnersControllerIntegrationTests"
dotnet test --filter "PetsControllerIntegrationTests"  
dotnet test --filter "VetsControllerIntegrationTests"
```

## Test Coverage

### API Endpoints Tested
- ✅ GET /owners (with pagination and filtering)
- ✅ GET /owners/{id}
- ✅ POST /owners
- ✅ PUT /owners/{id}
- ✅ GET /owners/{ownerId}/pets
- ✅ GET /owners/{ownerId}/pets/{petId}
- ✅ GET /owners/{ownerId}/pets/{petId}/visits
- ✅ POST /owners/{ownerId}/pets/{petId}/visits
- ✅ GET /pet/types
- ✅ GET /vets
- ✅ GET /vets/{id}
- ✅ GET /oops (exception handling)

### Validation Scenarios
- ✅ Valid data creation and updates
- ✅ Invalid data validation (400 Bad Request)
- ✅ Resource not found scenarios (404 Not Found)
- ✅ Pagination behavior
- ✅ Data filtering and search
- ✅ Database constraint violations
- ✅ Exception handling

## Architecture Benefits

1. **Real Environment Testing**: Uses actual PostgreSQL instead of mocks
2. **Isolation**: Each test gets a clean database state
3. **Performance**: Testcontainers are efficiently managed and reused
4. **Reliability**: Tests catch database-related issues that unit tests miss
5. **Documentation**: Tests serve as living documentation of API behavior

## Known Issues & Considerations

1. **Test Speed**: Integration tests are slower than unit tests due to container startup
2. **Docker Dependency**: Requires Docker to be available on the test machine
3. **Resource Usage**: Each test spins up a PostgreSQL container

## Future Enhancements

- Add tests for error conditions and edge cases
- Include performance benchmarking
- Add tests for concurrent operations
- Extend to test authentication/authorization when implemented
- Add API versioning tests when versioning is implemented