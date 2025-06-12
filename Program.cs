using Serilog;
using WS_CERT; // Aseg�rate de que coincida con el namespace de tu proyecto

// Configura Serilog. Escribir� en la consola y en un archivo rotativo.
// El archivo se llamar� "logs/mi-servicio-AAAAMMDD.log"
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Nivel m�nimo de log a capturar
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Ignora los logs informativos de Microsoft
    .Enrich.FromLogContext()
    .WriteTo.Console() // Escribe en la consola (�til para depuraci�n)
    .WriteTo.File(
        "D:\\BCB_LOGS\\logs\\mi-servicio-.log", // Ruta del archivo de log
        rollingInterval: RollingInterval.Day, // Crea un nuevo archivo cada d�a
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // Formato de salida
    .CreateLogger();

try
{
    // Informamos que el servicio est� iniciando
    Log.Information("Iniciando el servicio...");

    IHost host = Host.CreateDefaultBuilder(args)
        // Habilita la ejecuci�n como Servicio de Windows
        .UseWindowsService(options =>
        {
            options.ServiceName = "Mi Servicio B�sico con Serilog";
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
    // Aseg�rate de que todos los logs se escriban antes de cerrar
    Log.CloseAndFlush();
}