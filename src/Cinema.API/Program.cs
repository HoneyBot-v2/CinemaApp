using Cinema.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SQLitePCL;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// initalize SQlite native provider
Batteries.Init();

// Controllers + JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// OpenAPI document (serves /openapi/v1.json)
builder.Services.AddOpenApi();

// EF Core
builder.Services.AddDbContext<ApiDbContext>(option =>
{
    option.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

// JWT auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string? jwtKey = builder.Configuration["JWT:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT:Key configuration value is missing or empty.");
        }

        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApiDbContext>();

WebApplication app = builder.Build();

// Serve OpenAPI JSON and Scalar UI
app.MapOpenApi(); // GET /openapi/v1.json

app.MapScalarApiReference(options =>
{
    options.Title = "Cinema API";
    // options.Theme = ScalarTheme.Default; // Optional: Light, Dark, Solarized
});

// Health check endpoint
app.MapHealthChecks("/healthz")
    .WithName("HealthCheck")
    .WithTags("Health")
    .WithOpenApi();

#if DEBUG
// Debug only authentication bypass
app.Use(async (context, next) =>
{
    // Inject a synthetic authenticated user so [Authorize] passes
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "DebugUser"),
        new Claim(ClaimTypes.NameIdentifier, "debug-user")
    };

    var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
    context.User = new ClaimsPrincipal(identity);
    await next();
});
#endif

// Auto-create / migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    db.Database.EnsureCreated();
    // if you use migrations
    //db.Database.Migrate();
}

// Pipeline
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
