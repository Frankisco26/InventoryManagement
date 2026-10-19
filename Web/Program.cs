using InventoryManagement.Data.Base;
using InventoryManagement.Data.Services;
using InventoryManagement.Models.Identidad;
using InventoryManagement.Models.Utilitarios;
using InventoryManagement.Web.Authorizations;
using InventoryManagement.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// Cultura (separador decimal "." para precios)
// =====================================

var cultura = new CultureInfo("es-EC")
{
    NumberFormat =
    {
        CurrencyDecimalSeparator = ".",
        NumberDecimalSeparator = ".",
        PercentDecimalSeparator = ".",
        CurrencyGroupSeparator = ",",
        NumberGroupSeparator = ",",
        PercentGroupSeparator = ","
    }
};
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

// =====================================
// Base de datos (ContextoIdentidad y ContextoPrincipal)
// =====================================

void ConfigurarSqlServer(DbContextOptionsBuilder options, string nombreConexion)
{
    var connectionString = builder.Configuration.GetConnectionString(nombreConexion)
        ?? throw new InvalidOperationException($"Connection string '{nombreConexion}' not found.");

    options.UseSqlServer(connectionString,
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", Entornos.Migracion));
}

builder.Services.AddDbContext<ContextoIdentidad>(m => ConfigurarSqlServer(m, nameof(ContextoIdentidad)));
builder.Services.AddDbContext<ContextoPrincipal>(m => ConfigurarSqlServer(m, nameof(ContextoPrincipal)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// =====================================
// Identity
// =====================================

builder.Services
    .AddIdentity<Usuario, Rol>(options =>
    {
        // Password settings.
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 3;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings.
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(6);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = false;

        // User settings.
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = false;

        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ContextoIdentidad>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(6);
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.IsEssential = true;
});

// =====================================
// JWT Bearer para la API REST
// =====================================

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// =====================================
// Autorización por módulo (pantallas asignadas al rol en la empresa de la sesión)
// =====================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(nameof(Areas.Configuracion), policy => policy.AddRequirements(new ModuloAuthorizationRequirement(Areas.Configuracion)));
    options.AddPolicy(nameof(Areas.Stock), policy => policy.AddRequirements(new ModuloAuthorizationRequirement(Areas.Stock)));
});
builder.Services.AddScoped<IAuthorizationHandler, ModuloAuthorizationHandler>();
builder.Services.AddHttpContextAccessor();

// =====================================
// MVC, Razor Pages y sesión
// =====================================

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.IOTimeout = TimeSpan.FromHours(8);
    options.Cookie.IsEssential = true;
});

// =====================================
// Servicios
// =====================================

builder.Services.AddScoped<EmpresasUsuario>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();

var app = builder.Build();

Microsoft.AspNetCore.Identity.Extensions.SetHttpContextAccessor(app.Services.GetRequiredService<IHttpContextAccessor>());

// =====================================
// Pipeline
// =====================================

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Principal/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// =====================================
// Endpoints
// =====================================

app.MapControllerRoute("areas", "{area:exists}/{controller=Principal}/{action=Inicio}/{id?}");
app.MapControllerRoute("default", "{controller=Principal}/{action=Inicio}/{id?}");
app.MapRazorPages();

app.Run();
