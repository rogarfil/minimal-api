using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using rogarfil_api.Data;
using rogarfil_api.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT key is not configured");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Monitoring");

// Login endpoint
app.MapPost("/api/login", async (LoginRequest login, AppDbContext context) =>
{
    var user = await context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

    if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
        return Results.Unauthorized();

    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Expires = DateTime.UtcNow.AddHours(2),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { Token = tokenString, User = new { user.Id, user.Username, user.Email } });
})
.WithName("Login")
.WithTags("Authentication");

// Get all users (protected endpoint)
app.MapGet("/api/users", async (AppDbContext context) =>
{
    var users = await context.Users
        .Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt })
        .ToListAsync();

    return Results.Ok(users);
})
.RequireAuthorization()
.WithName("GetUsers")
.WithTags("Users");

// Get user by id (protected endpoint)
app.MapGet("/api/users/{id}", async (int id, AppDbContext context) =>
{
    var user = await context.Users
        .Where(u => u.Id == id)
        .Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt })
        .FirstOrDefaultAsync();

    return user != null ? Results.Ok(user) : Results.NotFound();
})
.RequireAuthorization()
.WithName("GetUserById")
.WithTags("Users");

// Create new user
app.MapPost("/api/users", async (User newUser, AppDbContext context) =>
{
    newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
    context.Users.Add(newUser);
    await context.SaveChangesAsync();

    return Results.Created($"/api/users/{newUser.Id}", 
        new { newUser.Id, newUser.Username, newUser.Email, newUser.CreatedAt });
})
.WithName("CreateUser")
.WithTags("Users");

// Database migration endpoint
app.MapPost("/api/migrate", async (AppDbContext context) =>
{
    await context.Database.MigrateAsync();
    return Results.Ok(new { message = "Database migrated successfully" });
})
.WithName("MigrateDatabase")
.WithTags("Database");

app.Run();