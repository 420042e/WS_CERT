using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Serilog;

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

            string archive = "DetalleMonitorYape-" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            using var wbook = new XLWorkbook();

            var ws1 = wbook.Worksheets.Add("Resumen");
            ws1.Cell(1, 1).Value = "RESUMEN DE LOS DATOS PROCESADOS";
            ws1.Range("A1:O1").Row(1).Merge();
            ws1.Cell(1, 2).Value = "FECHA DE PROCESAMIENTO: ";
            ws1.Range("A2:O2").Row(1).Merge();
            ws1.Cell(1, 3).Value = "REGISTROS OBSERVADOS POR:";
            ws1.Range("C4:I4").Row(1).Merge();
            wbook.SaveAs(archive);

            // Espera 10 segundos antes de la siguiente ejecución
            await Task.Delay(10000, stoppingToken);
        }

        _logger.LogInformation("El Worker se está deteniendo.");
    }
}