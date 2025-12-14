# Unit Tests for NetClinic API

This directory contains comprehensive unit tests for the NetClinic API controllers and service classes using mock dependencies and in-memory databases.

## Overview

These unit tests verify individual component functionality by:

- **Using Mock Dependencies**: Controllers use mocked service dependencies via Moq
- **In-Memory Database Testing**: Services use Entity Framework in-memory databases
- **Isolated Component Testing**: Each class is tested in isolation from external dependencies
- **Fast Execution**: Tests run quickly without external infrastructure requirements
- **Comprehensive Coverage**: Tests cover controllers, services, and validation logic

## Test Structure

### Controllers Tests (`Unit/Controllers/`)
Unit tests for API controllers that mock service dependencies to test controller logic in isolation.

#### OwnersControllerTests
- **Setup**: Uses mocked `IOwnerService` and `ILogger<OwnersController>`
- **GET /owners** - Pagination, filtering, and response formatting
- **GET /owners/{id}** - Owner retrieval and not found scenarios  
- **POST /owners** - Owner creation and validation
- **PUT /owners/{id}** - Owner updates and error handling
- **Validation Logic** - Tests `ValidateOwnerDto` static method

**Key Test Categories:**
```csharp
// Happy path scenarios
[Fact] public async Task Get_ShouldReturnOwnerListDto_WhenOwnersExist()
[Fact] public async Task CreateOwner_ShouldReturnCreatedOwner_WhenValidOwnerProvided()

// Error scenarios  
[Fact] public async Task GetOwnerDetailsById_ShouldReturnNotFound_WhenOwnerDoesNotExist()
[Fact] public async Task CreateOwner_ShouldReturnBadRequest_WhenFirstNameIsEmpty()

// Validation testing
[Theory] public void ValidateOwnerDto_ShouldReturnError_WhenRequiredFieldIsEmpty()
```

#### PetsControllerTests  
- **Setup**: Uses mocked `IPetService` and `ILogger<PetsController>`
- **GET /owners/{ownerId}/pets** - Pet retrieval by owner
- **GET /owners/{ownerId}/pets/{petId}** - Individual pet retrieval
- **GET /owners/{ownerId}/pets/{petId}/visits** - Visit retrieval
- **POST /owners/{ownerId}/pets/{petId}/visits** - Visit creation
- **GET /pet/types** - Pet type listing
- **Validation Logic** - Tests `ValidatePetDto` and `ValidateVisitDto` methods

**Key Test Categories:**
```csharp
// Pet management
[Fact] public async Task GetPetsByOwnerId_ShouldReturnOkWithPets_WhenPetsExist()
[Fact] public async Task GetPetById_ShouldReturnNotFound_WhenPetDoesNotExist()

// Visit management  
[Fact] public async Task CreateVisit_ShouldReturnCreatedVisit_WhenValidVisitProvided()
[Fact] public async Task GetVisitsByPetId_ShouldReturnEmptyList_WhenPetHasNoVisits()

// Validation testing
[Fact] public void ValidateVisitDto_ShouldReturnNoErrors_WhenAllFieldsValid()
```

#### VetsControllerTests
- **Setup**: Uses mocked `IVetService` and `ILogger<VetsController>`  
- **GET /vets** - Veterinarian listing with pagination
- **GET /vets/{id}** - Individual veterinarian retrieval
- **Pagination Logic** - Tests page calculation and service interaction

**Key Test Categories:**
```csharp
// Vet retrieval
[Fact] public async Task Get_ShouldReturnVetListDto_WhenVetsExist()
[Fact] public async Task GetById_ShouldReturnNotFound_WhenVetDoesNotExist()

// Pagination
[Fact] public async Task Get_ShouldCalculateCorrectTotalPages()
[Theory] public async Task Get_ShouldCallServiceWithCorrectPageNumber(int page)
```

#### OopsControllerTests
- **Setup**: Simple controller with no dependencies
- **Exception Testing** - Verifies expected exception throwing behavior

```csharp
[Fact] public void Get_ShouldThrowException_WithExpectedMessage()
```

### Service Tests (`Unit/Services/`)
Unit tests for service classes that use Entity Framework in-memory databases to test business logic.

#### OwnerServiceTests
- **Setup**: Uses `InMemoryDatabase` with seeded test data
- **Database Operations**: Create, read, update operations on owners
- **Business Logic**: Filtering, pagination, and data validation
- **Entity Mapping**: Tests conversion between entities and DTOs

**Key Test Categories:**
```csharp
// Data retrieval
[Fact] public async Task GetOwnersByLastNameAsync_ShouldReturnAllOwners_WhenLastNameIsNull()
[Fact] public async Task GetOwnersByLastNameAsync_ShouldFilterByLastName_WhenProvided()

// CRUD operations
[Fact] public async Task CreateOwnerAsync_ShouldReturnOwnerWithId_WhenValidOwnerProvided()
[Fact] public async Task UpdateOwnerAsync_ShouldReturnUpdatedOwner_WhenValidDataProvided()

// Business logic
[Fact] public async Task GetOwnersByLastNameCountAsync_ShouldReturnCorrectCount()
```

#### PetServiceTests  
- **Setup**: Uses `InMemoryDatabase` with seeded pets, owners, and types
- **Pet Management**: Pet retrieval, visit management, type operations
- **Relationship Testing**: Tests entity relationships and foreign keys
- **Complex Queries**: Tests joins and filtering operations

**Key Test Categories:**
```csharp
// Pet operations
[Fact] public async Task GetPetsByOwnerIdAsync_ShouldReturnPets_WhenOwnerHasPets()
[Fact] public async Task GetPetByIdAsync_ShouldReturnPet_WhenPetExists()

// Visit management
[Fact] public async Task CreateVisitAsync_ShouldCreateAndReturnVisit_WhenValidDataProvided()
[Fact] public async Task GetVisitsByPetIdAsync_ShouldReturnVisits_WhenPetHasVisits()

// Type operations
[Fact] public async Task GetAllPetTypesAsync_ShouldReturnAllTypes()
```

#### VetServiceTests
- **Setup**: Uses `InMemoryDatabase` with seeded veterinarian data
- **Vet Operations**: Retrieval, pagination, and specialty management
- **Data Integrity**: Tests entity relationships and data consistency

**Key Test Categories:**
```csharp
// Vet retrieval
[Fact] public async Task GetAllVeterinariansAsync_ShouldReturnAllVets()
[Fact] public async Task GetVeterinarianByIdAsync_ShouldReturnVet_WhenVetExists()

// Pagination
[Fact] public async Task GetVeterinariansCountAsync_ShouldReturnCorrectCount()
```

## Test Infrastructure

### Mock Setup Pattern
Controllers use consistent mock setup patterns:

```csharp
public class ControllerTests
{
    private readonly Mock<ILogger<Controller>> _loggerMock;
    private readonly Mock<IService> _serviceMock;
    private readonly Controller _controller;

    public ControllerTests()
    {
        _loggerMock = new Mock<ILogger<Controller>>();
        _serviceMock = new Mock<IService>();
        _controller = new Controller(_loggerMock.Object, _serviceMock.Object);
    }
}
```

### In-Memory Database Pattern
Services use Entity Framework in-memory databases:

```csharp
public class ServiceTests : IDisposable
{
    private readonly NetClinicDbContext _context;
    private readonly Service _service;

    public ServiceTests()
    {
        var options = new DbContextOptionsBuilder<NetClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NetClinicDbContext(options);
        _service = new Service(_context, loggerMock.Object);
        SeedTestData();
    }
}
```

### Test Data Seeding
Each service test class seeds relevant test data:

- **Owners**: 3+ owners with different last names and contact information
- **Pets**: Multiple pets with different types, owners, and birth dates  
- **Pet Types**: Standard types (Dog, Cat, Bird, Hamster)
- **Vets**: 3+ veterinarians with different specialties
- **Visits**: Historical visit data linked to pets

## Running the Tests

### Prerequisites
- .NET 9.0 SDK
- No external dependencies (uses in-memory database)

### Run All Unit Tests
```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

### Run Specific Test Categories
```bash
# Controller tests only
dotnet test --filter "FullyQualifiedName~Controllers"

# Service tests only  
dotnet test --filter "FullyQualifiedName~Services"

# Specific test class
dotnet test --filter "OwnersControllerTests"
```

### Run with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage" --filter "FullyQualifiedName~Unit"
```

## Test Coverage

### Controller Logic Tested
- ✅ HTTP status code responses (200, 201, 400, 404)
- ✅ Request/response data transformation
- ✅ Pagination parameter handling
- ✅ Input validation and error responses
- ✅ Service method invocation verification
- ✅ Exception handling scenarios
- ✅ Action result types and values

### Service Logic Tested  
- ✅ CRUD operations (Create, Read, Update)
- ✅ Entity Framework query operations
- ✅ Business rule validation
- ✅ Data filtering and searching
- ✅ Pagination implementation
- ✅ Entity-to-DTO mapping
- ✅ Database relationship handling
- ✅ Error condition handling

### Validation Testing
- ✅ Required field validation
- ✅ Data format validation (telephone, dates)
- ✅ Business rule validation
- ✅ Input sanitization
- ✅ Boundary condition testing

## Architecture Benefits

1. **Fast Execution**: Tests run in milliseconds without external dependencies
2. **Reliable**: No flaky tests due to external services or timing issues
3. **Isolated**: Each test is completely independent
4. **Comprehensive**: Tests both controller logic and service business logic
5. **Maintainable**: Clear separation between controller and service concerns
6. **Debugging Friendly**: Easy to identify exact failure points

## Testing Patterns Used

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern for clarity:

```csharp
[Fact]
public async Task Method_Should_When()
{
    // Arrange
    var input = new InputData();
    _serviceMock.Setup(x => x.Method(input)).ReturnsAsync(expectedResult);
    
    // Act  
    var result = await _controller.Method(input);
    
    // Assert
    result.Should().BeOfType<ExpectedType>();
    _serviceMock.Verify(x => x.Method(input), Times.Once);
}
```

### Theory-Based Testing
Uses `[Theory]` with `[InlineData]` for testing multiple scenarios:

```csharp
[Theory]
[InlineData("", "lastName", "address", "city", "telephone", "firstName")]
[InlineData("firstName", "", "address", "city", "telephone", "lastName")]
public void ValidateDto_ShouldReturnError_WhenRequiredFieldIsEmpty(
    string firstName, string lastName, string address, string city, string telephone, string expectedErrorKey)
```

### Mock Verification
Verifies service interactions to ensure controller logic is correct:

```csharp
_serviceMock.Verify(x => x.GetOwnersByLastNameAsync(lastName, page, pageSize), Times.Once);
```

## Best Practices Demonstrated

1. **Single Responsibility**: Each test verifies one specific behavior
2. **Clear Naming**: Test names describe the scenario and expected outcome  
3. **Mock Isolation**: Controllers don't depend on real service implementations
4. **Data Independence**: Each service test uses a unique in-memory database
5. **Comprehensive Coverage**: Tests cover both success and error scenarios
6. **Realistic Data**: Test data represents real-world scenarios
7. **Performance Conscious**: Tests execute quickly for fast feedback cycles

## Future Enhancements

- Add performance benchmarking tests
- Include property-based testing for edge cases
- Add mutation testing to verify test quality
- Implement integration with code coverage reporting tools
- Add tests for concurrent operations when applicable
- Extend validation testing for complex business rules