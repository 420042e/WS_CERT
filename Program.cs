using Serilog;
using WS_CERT; // El namespace de tu proyecto
using Microsoft.EntityFrameworkCore;
using WS_CERT.Data; // <-- AÑADIR ESTO

// Tu configuración de Serilog (esto está perfecto, no se toca)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "D:\\BCB_LOGS\\logs\\mi-servicio-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Iniciando el servicio...");

    IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService(options =>
        {
            options.ServiceName = "Mi Servicio con Serilog y BD";
        })
        .UseSerilog()
        .ConfigureServices((hostContext, services) => // <-- Usamos el hostContext para acceder a la configuración
        {
            // === INICIO: CÓDIGO AÑADIDO PARA LA BASE DE DATOS ===

            // 1. Obtiene la configuración desde el hostContext (que lee appsettings.json)
            IConfiguration configuration = hostContext.Configuration;

            // 2. Lee tu cadena de conexión
            /*var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3. Registra tu DbContext para que pueda ser inyectado
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));*/

            // === FIN: CÓDIGO AÑADIDO ===

            // Registra tu worker (esto ya lo tenías)
            services.AddHostedService<Worker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "El servicio no pudo iniciar.");
}
finally
{
    Log.CloseAndFlush();
}