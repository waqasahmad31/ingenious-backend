using Ingenious.Data;
using Ingenious.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy.WithOrigins(
            new string[] {
                "http://localhost:4200", "https://telemed.bingeable.tech"
            }) // Frontend origin
                        .AllowAnyHeader()
                        .AllowAnyMethod()); // Allow credentials for SignalR
});

// Add SignalR
builder.Services.AddSignalR();
// Add Authentication (if required for user-specific messages)
builder.Services.AddAuthentication();

// Add Scoped Services
builder.Services.ConfigureScopedServices();

// Configure Services
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureSwagger();

var app = builder.Build();

// Custom Middleware
app.UseCustomMiddleware();


// Run Application
app.Run();
