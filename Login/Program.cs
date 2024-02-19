using Microsoft.EntityFrameworkCore;
//Esto es para acceder a la carpeta models
using Plataforma.Models;
using Plataforma.Servicios.Contrato;
using Plataforma.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// configura los servicios de MVC (Model-View-Controller) en la aplicaci�n web
builder.Services.AddControllersWithViews();

//Configura el contexto de la base de datos en la aplicacion, osea la variable cadenaSQL que se asigna en appsettings.json
builder.Services.AddDbContext<BaseAdmContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("cadenaSQL"));
});
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
//configura la autenticaci�n en la aplicaci�n web utilizando el esquema de autenticaci�n de cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.Name = "CookieMakrotecno";
        option.LoginPath = "/Home/Login";
        option.LogoutPath = "/Home/Logout";
        option.ExpireTimeSpan = TimeSpan.Zero;
        //Si estamos viendo algo el tiempo de expiracion de mi cookie se aumenta 20 minutos mas.
        option.SlidingExpiration = false;
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
    //configuran middleware para manejar excepciones y servir archivos est�ticos, respectivamente.
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
//establecen el middleware para enrutamiento, autenticaci�n y autorizaci�n, respectivamente
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//establece una ruta predeterminada para la aplicaci�n web
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
