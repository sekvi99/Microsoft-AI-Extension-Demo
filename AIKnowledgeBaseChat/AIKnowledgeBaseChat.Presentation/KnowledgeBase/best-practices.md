# .NET Development Best Practices

## Code Organization

### SOLID Principles

Follow SOLID principles for maintainable code:

- **Single Responsibility**: A class should have one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes should be substitutable for base classes
- **Interface Segregation**: Many specific interfaces are better than one general interface
- **Dependency Inversion**: Depend on abstractions, not concretions

### Clean Architecture

Organize your application into layers:
1. **Domain Layer**: Core business logic and entities
2. **Application Layer**: Use cases and business rules
3. **Infrastructure Layer**: External concerns (database, file system, APIs)
4. **Presentation Layer**: UI or API endpoints

## Dependency Injection

Use constructor injection for dependencies:

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;
    
    public UserService(IUserRepository repository, ILogger<UserService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
}
```

## Async/Await Best Practices

- Always use async/await for I/O operations
- Don't block async code with `.Result` or `.Wait()`
- Use `ConfigureAwait(false)` in library code
- Avoid `async void` except for event handlers

```csharp
// Good
public async Task GetUserAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}

// Bad - blocking
public User GetUser(int id)
{
    return _repository.GetByIdAsync(id).Result; // Don't do this!
}
```

## Exception Handling

- Catch specific exceptions, not generic ones
- Don't swallow exceptions
- Use try-catch only when you can handle the exception
- Log exceptions with context

```csharp
try
{
    await ProcessDataAsync();
}
catch (InvalidOperationException ex)
{
    _logger.LogError(ex, "Failed to process data for user {UserId}", userId);
    throw;
}
```

## Testing

### Unit Testing
Test individual components in isolation:

```csharp
[Fact]
public async Task GetUserAsync_WithValidId_ReturnsUser()
{
    // Arrange
    var mockRepo = new Mock();
    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new User { Id = 1, Name = "Test" });
    var service = new UserService(mockRepo.Object);
    
    // Act
    var result = await service.GetUserAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

### Integration Testing
Test components working together with TestServer.

## Configuration

- Use appsettings.json for configuration
- Use User Secrets for local development
- Use environment variables or Azure Key Vault in production
- Use the Options pattern for strongly-typed configuration

```csharp
public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
}

// Registration
services.Configure(configuration.GetSection("Email"));

// Usage
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions settings)
    {
        _settings = settings.Value;
    }
}
```

## Logging

Use structured logging with meaningful context:

```csharp
_logger.LogInformation(
    "User {UserId} performed action {Action} at {Timestamp}",
    userId, action, DateTime.UtcNow);
```

## Security

- Never store secrets in code
- Use authentication and authorization
- Validate all input
- Use HTTPS
- Keep dependencies up to date
- Use parameterized queries to prevent SQL injection

## Performance

- Profile before optimizing
- Use caching wisely
- Minimize database round trips
- Use pagination for large datasets
- Consider memory allocation patterns