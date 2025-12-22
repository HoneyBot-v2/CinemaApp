# API Testing Setup Documentation

This document outlines the testing framework and package decisions for the Cinema API test suite.

## Date Created
January 2025

## Overview
We are creating comprehensive xUnit tests for all API controllers in the Cinema application. This document captures the key decisions and package selections made during the planning phase.

## Testing Framework Decisions

### xUnit Version
- **Version**: 3.2.0 (latest stable as of January 2025)
- **Rationale**: Latest stable version with improved dependency injection support for test class constructors
- **Key Features**:
  - Test class constructors can now use dependency injection
  - `IClassFixture<T>` and `ICollectionFixture<T>` still work as before
  - Assertion APIs remain largely the same

### Mocking Framework
- **Framework**: NSubstitute (instead of Moq)
- **Rationale**: More concise syntax and popular alternative to Moq
- **Usage Example**:
  ```csharp
  var config = Substitute.For<IConfiguration>();
  config["JWT:Key"].Returns("TestKeyForJWTTokenGeneration123456789");
  config["JWT:Issuer"].Returns("TestIssuer");
  config["JWT:Audience"].Returns("TestAudience");
  ```

### Assertion Library
- **Library**: FluentAssertions
- **Rationale**: Provides readable, expressive assertions especially useful for testing `ActionResult` types and HTTP responses
- **Key Benefits**:
  - Fluent syntax: `result.Should().BeOfType<OkObjectResult>()`
  - HTTP response testing: `response.Should().HaveStatusCode(HttpStatusCode.OK)`
  - Object comparison: `result.Value.Should().BeEquivalentTo(expectedData)`
  - Database assertions: `dbContext.Users.Should().Contain(u => u.Email == "test@email.com")`

### Test Data Generation
- **Library**: Bogus
- **Rationale**: Generates realistic test data automatically (users, emails, dates, etc.) reducing manual test data setup
- **Usage**: Use `Faker<T>` to generate test data programmatically in test fixtures

## NuGet Packages

### Required Packages
1. **xunit** (v3.2.0)
   - Core testing framework

2. **xunit.runner.visualstudio** (v3.2.0)
   - Visual Studio test runner integration

3. **Microsoft.AspNetCore.Mvc.Testing**
   - Integration testing support
   - Enables `WebApplicationFactory` for full request/response cycle testing

4. **Microsoft.EntityFrameworkCore.InMemory**
   - In-memory database for testing
   - Avoids external database dependencies

5. **NSubstitute**
   - Mocking framework for dependencies and IConfiguration

6. **FluentAssertions**
   - Readable assertion library

7. **Bogus**
   - Test data generation library

## Test Project Structure

### Project Location
- `src/Cinema.API.Tests/Cinema.API.Tests.csproj`

### Target Framework
- .NET 10.0 (matching the API project)

### Key Files
1. `TestBase.cs` - Base class with in-memory database setup and authentication helpers
2. `Fixtures/TestDataFixture.cs` - Reusable test data fixtures
3. `Controllers/UsersControllerTests.cs` - User authentication tests
4. `Controllers/MoviesControllerTests.cs` - Movie CRUD tests
5. `Controllers/ReservationsControllerTests.cs` - Reservation tests
6. `Controllers/ScreeningsControllerTests.cs` - Screening tests
7. `Controllers/SeatsControllerTests.cs` - Seat retrieval tests

## Testing Approach

### Database Testing
- Use `UseInMemoryDatabase` with unique database names per test to avoid conflicts
- Seed minimal required data for each test
- Use `EnsureCreated()` to create database schema

### Authentication Testing
- Create helper methods to generate valid JWT tokens for testing
- Substitute `IConfiguration` with test JWT settings using NSubstitute
- Set up `HttpContext` with authenticated user claims for protected endpoints

### Controller Testing
- Create controllers directly with substituted dependencies
- For authenticated endpoints, set up `ControllerContext` with authenticated user
- Use `ControllerContext` to set up request context for base URL generation

## Test Coverage Goals

### Controllers to Test
1. **UsersController**
   - Login (valid/invalid credentials)
   - Register (valid/duplicate email)

2. **MoviesController** (requires authentication)
   - GetAllMovies (with different movie types)
   - GetMovie (by ID)
   - PostMovie (create)
   - DeleteMovie (with cascading deletes)

3. **ReservationsController** (requires authentication)
   - GetAllReservations
   - GetReservationByUserId
   - ReserveSeats (with validation and conflict handling)

4. **ScreeningsController** (requires authentication)
   - GetScreeningByMovieId
   - PostScreening (with seat generation)
   - DeleteScreening

5. **SeatsController** (requires authentication)
   - GetAllSeats (by screening ID)

## Key Testing Patterns

### Arrange-Act-Assert Pattern
All tests follow the AAA pattern:
- **Arrange**: Set up test data and dependencies
- **Act**: Execute the method under test
- **Assert**: Verify the results using FluentAssertions

### Test Isolation
- Each test uses a unique in-memory database
- Tests are independent and can run in any order
- Proper cleanup/disposal of resources

### Authentication Testing
- Test both authenticated and unauthenticated scenarios
- Verify proper authorization checks
- Test JWT token generation and validation

## Future Considerations

### Potential Enhancements
- Integration tests using `WebApplicationFactory` for full HTTP pipeline testing
- Performance testing for critical endpoints
- Contract testing for API responses
- Test coverage reporting

## Notes

- All tests should be maintainable and readable
- Use descriptive test method names following the pattern: `MethodName_Scenario_ExpectedResult`
- Keep test data minimal and focused on what's being tested
- Use Bogus for generating realistic but varied test data

## References

- [xUnit.net Documentation](https://xunit.net/)
- [NSubstitute Documentation](https://nsubstitute.github.io/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Bogus Documentation](https://github.com/bchavez/Bogus)
- [ASP.NET Core Testing Documentation](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
