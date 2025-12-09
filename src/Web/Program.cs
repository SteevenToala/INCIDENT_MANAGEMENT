using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using IncidentManagement.Application.Services;
using IncidentManagement.Application.Interfaces;
using IncidentManagement.Infrastructure.Data;
using IncidentManagement.Infrastructure.Repositories;
using Web.Components;
using Web.Configuration;
using Web.Services;
using DotNetEnv;

// Cargar variables de entorno desde .env
// Buscar el archivo .env en la raíz del repositorio
var currentDir = Directory.GetCurrentDirectory();
var envPath = Path.Combine(currentDir, "..", "..", ".env");

// Si no existe, intentar desde la raíz del proyecto
if (!File.Exists(envPath))
{
    envPath = Path.Combine(currentDir, ".env");
}

// Si aún no existe, buscar hacia arriba en el árbol de directorios
if (!File.Exists(envPath))
{
    var searchDir = new DirectoryInfo(currentDir);
    while (searchDir != null && !File.Exists(Path.Combine(searchDir.FullName, ".env")))
    {
        searchDir = searchDir.Parent;
    }
    
    if (searchDir != null)
    {
        envPath = Path.Combine(searchDir.FullName, ".env");
    }
}

if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"[Config] ✓ Archivo .env cargado desde: {envPath}");
}
else
{
    Console.WriteLine($"[Config] ⚠ No se encontró archivo .env. Usando valores de appsettings.json");
    Console.WriteLine($"[Config]   Directorio actual: {currentDir}");
}

var builder = WebApplication.CreateBuilder(args);

// Sobrescribir configuración con variables de entorno
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbTrustedConnection = Environment.GetEnvironmentVariable("DB_TRUSTED_CONNECTION");

if (!string.IsNullOrEmpty(dbServer) && !string.IsNullOrEmpty(dbName))
{
    var connectionString = $"Server={dbServer};Database={dbName};Trusted_Connection={dbTrustedConnection ?? "True"};TrustServerCertificate=True;MultipleActiveResultSets=true";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    Console.WriteLine($"[Config] Connection String configurado desde .env");
}

// Configurar JWT desde variables de entorno
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
var jwtExpiration = Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES");

if (!string.IsNullOrEmpty(jwtSecretKey))
{
    builder.Configuration["Jwt:SecretKey"] = jwtSecretKey;
    builder.Configuration["Jwt:Issuer"] = jwtIssuer ?? "IncidentManagement";
    builder.Configuration["Jwt:Audience"] = jwtAudience ?? "IncidentManagementUsers";
    builder.Configuration["Jwt:ExpirationMinutes"] = jwtExpiration ?? "10080";
    Console.WriteLine($"[Config] JWT configurado desde .env");
}

// Configurar Push Notifications desde variables de entorno
var vapidPublicKey = Environment.GetEnvironmentVariable("PUSH_VAPID_PUBLIC_KEY");
var vapidPrivateKey = Environment.GetEnvironmentVariable("PUSH_VAPID_PRIVATE_KEY");
var vapidSubject = Environment.GetEnvironmentVariable("PUSH_VAPID_SUBJECT");

if (!string.IsNullOrEmpty(vapidPublicKey))
{
    builder.Configuration["PushNotifications:VapidPublicKey"] = vapidPublicKey;
    builder.Configuration["PushNotifications:VapidPrivateKey"] = vapidPrivateKey ?? "";
    builder.Configuration["PushNotifications:VapidSubject"] = vapidSubject ?? "mailto:admin@incidentmanagement.com";
    Console.WriteLine($"[Config] Push Notifications configurado desde .env");
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Controllers para API
builder.Services.AddControllers();

// Add MudBlazor
builder.Services.AddMudServices();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IIncidenteRepository, IncidenteRepository>();
builder.Services.AddScoped<IBaseConocimientoRepository, BaseConocimientoRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<ISuscripcionPushRepository, SuscripcionPushRepository>();
builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<ILaboratorioRepository, LaboratorioRepository>();
builder.Services.AddScoped<IComputadoraRepository, ComputadoraRepository>();
builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();
builder.Services.AddScoped<ICatalogoServicioRepository, CatalogoServicioRepository>();

// Add Application Services
builder.Services.AddScoped<IIncidenteService, IncidenteService>();
builder.Services.AddScoped<IBaseConocimientoService, BaseConocimientoService>();
builder.Services.AddScoped<IEstudiantePortalService, EstudiantePortalService>();
builder.Services.AddScoped<IAsignacionService, AsignacionService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();

// Add Web Services
builder.Services.AddScoped<AuthService>();

// Add HttpContextAccessor para acceder a cookies
builder.Services.AddHttpContextAccessor();

// JWT settings & token service
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? throw new InvalidOperationException("Jwt settings are missing.");

builder.Services.AddSingleton<ITokenService, TokenService>();

// Add HttpClient para llamadas API (si es necesario para otras operaciones internas)
builder.Services.AddScoped<HttpClient>();

// Add Authentication con JWT validado desde cookie
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
