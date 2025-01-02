using Microsoft.EntityFrameworkCore;
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

//Archivos config appsettings
builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


//Configuraciones para HTTP, MVC, Log para mensajes emergentes
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//Validacion de la conexion de base de datos
var connectionString = builder.Configuration.GetConnectionString("cadenaSQL")
    ?? throw new InvalidOperationException("La cadena de conexión 'cadenaSQL' no está configurada.");

builder.Services.AddDbContext<BaseAdmContext>(options =>
{
    options.UseMySQL(connectionString);
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
builder.Services.AddScoped<IReporteService, ReporteService>();

//configura la autenticaci�n en la aplicaci�n web utilizando el esquema de autenticaci�n de cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.Name = "CookieMakrotecno";
        option.LoginPath = "/Home/Login";
        option.LogoutPath = "/Home/Logout";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        option.SlidingExpiration = true;
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
