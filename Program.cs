using Serilog;
using WS_Stbn; // Asegúrate de que coincida con el namespace de tu proyecto

// Configura Serilog. Escribirá en la consola y en un archivo rotativo.
// El archivo se llamará "logs/mi-servicio-AAAAMMDD.log"
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Nivel mínimo de log a capturar
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Ignora los logs informativos de Microsoft
    .Enrich.FromLogContext()
    .WriteTo.Console() // Escribe en la consola (útil para depuración)
    .WriteTo.File(
        "logs/mi-servicio-.log", // Ruta del archivo de log
        rollingInterval: RollingInterval.Day, // Crea un nuevo archivo cada día
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // Formato de salida
    .CreateLogger();

try
{
    // Informamos que el servicio está iniciando
    Log.Information("Iniciando el servicio...");

    IHost host = Host.CreateDefaultBuilder(args)
        // Habilita la ejecución como Servicio de Windows
        .UseWindowsService(options =>
        {
            options.ServiceName = "Mi Servicio Básico con Serilog";
        })
        // Integra Serilog con el sistema de logging de .NET
        .UseSerilog()
        .ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    // Captura y loguea cualquier error fatal durante el inicio
    Log.Fatal(ex, "El servicio no pudo iniciar.");
}
finally
{
    // Asegúrate de que todos los logs se escriban antes de cerrar
    Log.CloseAndFlush();
}