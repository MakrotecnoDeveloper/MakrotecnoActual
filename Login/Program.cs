using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "logs", "app-log-.txt");
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();


builder.Host.UseSerilog();

//Archivos config appsettings
/*builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);*/


//Configuraciones para HTTP, MVC, Log para mensajes emergentes
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFile("Logs/app-log-{Date}.txt");

//Validacion de la conexion de base de datos
var connectionString = builder.Configuration.GetConnectionString("cadenaSQL")
    ?? throw new InvalidOperationException("La cadena de conexión 'cadenaSQL' no está configurada.");

builder.Services.AddDbContext<BaseAdmContext>(options =>
{
    options.UseSqlServer(connectionString);
});

//Limite de envio de correos
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10 MB
});

//Servicios-Contrato/Implementacion
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
//builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IDispositivoService, DispositivoService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrdenServicioService, OrdenServicioService>();
builder.Services.AddScoped<IComprasService, ComprasService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IFlujoCajaService, FlujoCajaService>();
builder.Services.AddScoped<IGananciaService, GananciaService>();
builder.Services.AddScoped<IStreamingService, StreamingService>();
builder.Services.AddScoped<ITercerosService, TercerosService>();
builder.Services.AddScoped<IGastosService, GastosService>();
builder.Services.AddScoped<IInicioService, InicioService>();
builder.Services.AddScoped<INominaService, NominaService>();

//configura la autenticaci�n en la aplicaci�n web utilizando el esquema de autenticaci�n de cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CookieMakrotecno";
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = false;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // ✅ Evita agregar ?ReturnUrl
                if (context.Request.Path.StartsWithSegments("/Home/Login"))
                {
                    context.Response.Redirect("/Home/Login");
                }
                else
                {
                    context.Response.Redirect("/Home/Login"); // fuerza redirección limpia
                }
                return Task.CompletedTask;
            }
        };
    });



builder.Services.AddControllersWithViews(options =>
{
    //configura el comportamiento de cach� de las respuestas en las vistas.
    options.Filters.Add(
            new ResponseCacheAttribute
            {
                NoStore = true,
                Location = ResponseCacheLocation.None,
            }
        );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    Console.WriteLine("Esta en modo Produccion");
    //configuran middleware para manejar excepciones y servir archivos est�ticos, respectivamente.
    app.UseExceptionHandler("/Home/Error");
}else
{
    Console.WriteLine("Esta en modo Pruebas");
}
app.UseStaticFiles();
//establecen el middleware para enrutamiento, autenticaci�n y autorizaci�n, respectivamente
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
//establece una ruta predeterminada para la aplicaci�n web
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();
