namespace WS_Stbn; // Asegúrate de que coincida con tu namespace

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    // Usamos inyección de dependencias para obtener el logger configurado con Serilog
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("El Worker se ha iniciado en: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            // Aquí va la lógica principal de tu servicio
            _logger.LogInformation("El Worker está ejecutando una tarea en: {time}", DateTimeOffset.Now);
            _logger.LogWarning("Este es un mensaje de advertencia de ejemplo.");

            // Espera 10 segundos antes de la siguiente ejecución
            await Task.Delay(10000, stoppingToken);
        }

        _logger.LogInformation("El Worker se está deteniendo.");
    }
}