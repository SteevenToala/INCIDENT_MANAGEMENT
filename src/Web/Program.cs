using IncidentManagement.Application.Interfaces;
using IncidentManagement.Application.Services;
using IncidentManagement.Infrastructure.Data;
using IncidentManagement.Infrastructure.Repositories;
using IncidentManagement.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Database Context with optimizations
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(false);
    options.EnableDetailedErrors(false);
});

// Add Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IIncidenteRepository, IncidenteRepository>();
builder.Services.AddScoped<IBaseConocimientoRepository, BaseConocimientoRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<ILaboratorioRepository, LaboratorioRepository>();
builder.Services.AddScoped<IComputadoraRepository, ComputadoraRepository>();

// Add Application Services
builder.Services.AddScoped<IIncidenteService, IncidenteService>();
builder.Services.AddScoped<IBaseConocimientoService, BaseConocimientoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Authentication
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Comentado para evitar HTTP 400 en desarrollo
// app.UseHttpsRedirection();

// Enable serving static files from wwwroot
app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();

app.MapRazorComponents<IncidentManagement.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MudBlazor.MudAlert).Assembly);

app.Run();
