# C# Language Features

## Modern C# Features

C# continues to evolve with each version, introducing features that improve developer productivity and code quality.

## Pattern Matching

Pattern matching allows you to test if a value has a certain "shape" and extract information from it.

### Type Patterns
```csharp
if (obj is string s)
{
    Console.WriteLine($"String length: {s.Length}");
}
```

### Property Patterns
```csharp
if (person is { Age: > 18, Name: var name })
{
    Console.WriteLine($"{name} is an adult");
}
```

## Records

Records provide a concise syntax for creating reference types with value-based equality.

```csharp
public record Person(string FirstName, string LastName);

var person1 = new Person("John", "Doe");
var person2 = new Person("John", "Doe");
Console.WriteLine(person1 == person2); // True
```

## Nullable Reference Types

Nullable reference types help eliminate null reference exceptions by making the type system aware of nullability.

```csharp
string? nullableString = null; // Explicitly nullable
string nonNullableString = "Hello"; // Not nullable
```

## Async/Await

The async/await pattern simplifies asynchronous programming:

```csharp
public async Task FetchDataAsync()
{
    using var client = new HttpClient();
    return await client.GetStringAsync("https://api.example.com/data");
}
```

## LINQ (Language Integrated Query)

LINQ provides a unified syntax for querying collections:

```csharp
var adults = people
    .Where(p => p.Age >= 18)
    .OrderBy(p => p.Name)
    .Select(p => p.Name);
```

## Init-Only Properties

Init-only properties allow property values to be set during object initialization but become immutable afterwards:

```csharp
public class Person
{
    public string Name { get; init; }
    public int Age { get; init; }
}

var person = new Person { Name = "Alice", Age = 30 };
// person.Name = "Bob"; // Compilation error
```