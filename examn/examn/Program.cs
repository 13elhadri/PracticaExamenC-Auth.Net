using System.Text;
using examn.Database;
using examn.Funkos.Services;
using examn.Funkos.Storage.Config;
using examn.User.Services;
using examn.Utils.Auth.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using Serilog.Core;

var environment = InitLocalEnvironment();

string InitLocalEnvironment()
{
    Console.OutputEncoding = Encoding.UTF8; // Necesario para mostrar emojis
    Console.WriteLine("🤖 Proyecto Vives-Bank en .NET 🤖\n");
    var myEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
    return myEnvironment;
}

// Init App Configuration
var configuration = InitConfiguration();
IConfiguration InitConfiguration()
{
    var myConfiguration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{environment}.json", true)
        .Build();
    return myConfiguration;
}

var logger = InitLogConfig();

Logger InitLogConfig()
{
    return new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}


var builder = InitServices();

WebApplicationBuilder InitServices()
{
    var myBuilder = WebApplication.CreateBuilder(args);

    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    myBuilder.Services.AddLogging(logging =>
    {
        logging.ClearProviders(); 
        logging.AddSerilog(logger, true); 
    });
    logger.Debug("Logger por defecto: Serilog");
    
    
    // Services
    myBuilder.Services.AddScoped<IUserService, UserService>();
    myBuilder.Services.AddScoped<IFunkoService, FunkoService>();

    myBuilder.Services.AddScoped<FileStorageConfig>();
   
    myBuilder.Services.AddHttpContextAccessor();
    
    // Caché en memoria
    myBuilder.Services.AddMemoryCache();

    // Base de datos en PostgreSQL
   myBuilder.Services.AddDbContext<GeneralDbContext>(options =>
        options.UseNpgsql(myBuilder.Configuration.GetConnectionString("DefaultConnection")));
        

    // Añadimos los controladores
    myBuilder.Services.AddControllers();
    
    
    //Auth
    myBuilder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = myBuilder.Configuration["Jwt:Issuer"],
            ValidAudience = myBuilder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myBuilder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    message = "Acceso no autorizado. Debe iniciar sesión para acceder al recurso solicitado.",
                    path = context.Request.Path
                }));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    message = "Acceso denegado. No tiene los permisos requeridos para acceder al recurso solicitado.",
                    path = context.Request.Path
                }));
            }
        };
    });
    
    //Importante aquí definimos los roles de los usuarios permitidos
    myBuilder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    });
    
    myBuilder.Services.AddScoped<IJwtService, JwtService>();

    return myBuilder;
}

var app = builder.Build();

// Habilita redirección HTTPS si está habilitado
app.UseHttpsRedirection();

// Añadir esto si utilizas MVC para definir rutas, decimos que activamos el uso de rutas
app.UseRouting();

// Añadir para la autorización
app.UseAuthentication();
app.UseAuthorization();

// Añade los controladores a la ruta predeterminada
app.MapControllers();


Console.WriteLine($"🕹️ Running service in url: {builder.Configuration["urls"] ?? "not configured"} in mode {environment} 🟢");

logger.Information($"🕹️ Running service in url: {builder.Configuration["urls"] ?? "not configured"} in mode {environment} 🟢");

// Inicia la aplicación web
app.Run();