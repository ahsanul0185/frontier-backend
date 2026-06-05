using FrontierWeb.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

if (builder.Environment.IsProduction() && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATABASE_URL")))
{
    throw new InvalidOperationException("Missing DATABASE_URL. Set the Neon Postgres connection string in Railway environment variables.");
}

// Auth
var cfg = builder.Configuration;
var jwtKey = cfg["Auth:Key"] ?? "";
var jwtIssuer = cfg["Auth:Issuer"] ?? "BlogApi";
var jwtAudience = cfg["Auth:Audience"] ?? "BlogApiClients";

if (string.IsNullOrWhiteSpace(jwtKey) && !builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException("Missing Auth:Key. Set the JWT secret as the environment variable Auth__Key (or Auth:Key) in Railway.");
}

if (string.IsNullOrWhiteSpace(jwtKey))
{
    jwtKey = "DEV_ONLY_CHANGE_ME";
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            var allowedOrigins = new List<string>
            {
                "http://localhost:5173"
            };

            var frontendUrl = cfg["FrontendUrl"] ?? Environment.GetEnvironmentVariable("FRONTEND_URL");
            if (!string.IsNullOrWhiteSpace(frontendUrl))
            {
                allowedOrigins.Add(frontendUrl);
            }

            policy.WithOrigins(allowedOrigins.ToArray())
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT here. Swagger will send it as: Authorization: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme, Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddBlogInfrastructure(builder.Configuration);

var app = builder.Build();
await DataSeeder.SeedAsync(app.Services);

app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
