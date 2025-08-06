using Pg1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión PostgreSQL desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar Identity con roles y opciones de contraseña
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar sesión con memoria distribuida
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MiTienda.Session";
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurar cultura predeterminada (es-PE con símbolo de moneda S/)
var cultureInfo = new CultureInfo("es-PE")
{
    NumberFormat =
    {
        CurrencySymbol = "S/"
    }
};
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Agregar servicios MVC y API
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// Swagger - Documentación API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GGStore API", Version = "v1" });
});

// Configurar rutas y tiempos para la cookie de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Ingresar";
    options.AccessDeniedPath = "/Login/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

// Aplicar migraciones y datos iniciales
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones o seed: {ex.Message}");
        throw;
    }
}

// Middleware pipeline

app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GGStore API v1");
    c.RoutePrefix = "swagger"; // Para acceder en /swagger
});

// Importante: Mover UseExceptionHandler y UseHsts para entorno producción si quieres
// En desarrollo se puede usar UseDeveloperExceptionPage (como ya tienes arriba)
app.UseExceptionHandler("/Home/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Mapear rutas y controladores
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
