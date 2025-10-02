# Entity Framework Core

## Introduction

Entity Framework Core (EF Core) is a modern object-database mapper for .NET. It supports LINQ queries, change tracking,
updates, and schema migrations.

## Database Providers

EF Core supports multiple database providers:

- SQL Server
- PostgreSQL
- MySQL
- SQLite
- Oracle
- In-Memory (for testing)
- Cosmos DB

## DbContext

The DbContext class is the primary way to interact with a database:

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("connection-string");
}
```

## Defining Entities

Entities represent tables in your database:

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List Orders { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
```

## Querying Data

EF Core uses LINQ for querying:

```csharp
// Find by ID
var user = await context.Users.FindAsync(1);

// Filter and project
var users = await context.Users
    .Where(u => u.Name.StartsWith("A"))
    .Select(u => new { u.Id, u.Name })
    .ToListAsync();

// Include related data
var usersWithOrders = await context.Users
    .Include(u => u.Orders)
    .ToListAsync();
```

## Adding and Modifying Data

```csharp
// Add
var user = new User { Name = "John", Email = "john@example.com" };
context.Users.Add(user);
await context.SaveChangesAsync();

// Update
user.Email = "newemail@example.com";
await context.SaveChangesAsync();

// Delete
context.Users.Remove(user);
await context.SaveChangesAsync();
```

## Migrations

Migrations allow you to evolve your database schema:

```bash
# Create a new migration
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## Configuration

You can configure entities using Fluent API:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.HasIndex(e => e.Email).IsUnique();
    });
}
```

## Performance Tips

- Use AsNoTracking() for read-only queries
- Avoid SELECT N+1 problems with Include()
- Use compiled queries for frequently executed queries
- Batch operations when possible
- Use appropriate indexes