using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductManagement.API.Middleware;
using ProductManagement.Application;
using ProductManagement.Infrastructure;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);




foreach (var source in builder.Configuration.Sources)
{
    Console.WriteLine($"Config source: {source}");
}

// Manuel configuration read test
try
{
    var config = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var connStr = config.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Manual read connection: {connStr}");
}
catch (Exception ex)
{
    Console.WriteLine($"Manual config read error: {ex.Message}");
}

Console.WriteLine("==============================");



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1",
        Description = "A comprehensive API for managing products with authentication"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Custom JWT Middleware
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Database Migration (Development only)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ProductManagement.Infrastructure.Data.ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
}

app.Run();
