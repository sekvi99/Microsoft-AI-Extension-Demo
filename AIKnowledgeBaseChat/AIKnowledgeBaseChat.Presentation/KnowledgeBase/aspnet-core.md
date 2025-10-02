# ASP.NET Core

## Overview

ASP.NET Core is a cross-platform, high-performance framework for building modern, cloud-enabled, internet-connected applications.

## Key Features

### Unified Framework
ASP.NET Core provides a unified story for building web UI and web APIs. You can use the same patterns, primitives, and infrastructure for both.

### Dependency Injection
Built-in dependency injection promotes loose coupling and testability:

```csharp
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
}
```

### Middleware Pipeline
The middleware pipeline processes HTTP requests and responses:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Minimal APIs

ASP.NET Core supports minimal APIs for building lightweight HTTP APIs:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/users/{id}", (int id) => new { Id = id, Name = "User" });

app.Run();
```

## Razor Pages

Razor Pages is a page-based programming model that makes building web UI easier and more productive:

```csharp
public class IndexModel : PageModel
{
    [BindProperty]
    public string Name { get; set; }
    
    public void OnGet()
    {
        Name = "Welcome";
    }
    
    public IActionResult OnPost()
    {
        return RedirectToPage("Success");
    }
}
```

## Performance

ASP.NET Core is optimized for performance:
- Fast startup time
- Low memory footprint
- High request throughput
- Efficient resource utilization

## Hosting

ASP.NET Core applications can be hosted in various environments:
- Kestrel (cross-platform web server)
- IIS
- Nginx
- Apache
- Docker containers
- Cloud platforms (Azure, AWS, Google Cloud)