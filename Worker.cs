using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Serilog;
using System.Collections;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Net.Mail;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
using WS_CERT.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace WS_CERT; // Aseg�rate de que coincida con tu namespace

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    // Usamos inyecci�n de dependencias para obtener el logger configurado con Serilog
    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("El Worker se ha iniciado en: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var servidoresDev = await dbContext.Servidores.FromSqlRaw("EXEC [dbo].[usp_ObtenerServidoresDev]")
                                                           .ToListAsync(stoppingToken);

                int port = 443;
                ArrayList certificados = new ArrayList();
                foreach (var servidor in servidoresDev)
                {
                    try
                    {
                        _logger.LogInformation("Ingresando a server: {nombre}", servidor.Nombre);
                        DateTime _fechaActual = DateTime.Now;
                        var certificados2 = ObtenerDatosCertificados(servidor.Id, servidor.Nombre, _fechaActual);


 
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al obtener el certificado del servidor {host}.", servidor.Nombre);
                    }
                }
            }

            

                // Aqu� va la l�gica principal de tu servicio
                _logger.LogInformation("El Worker est� ejecutando una tarea en: {time}", DateTimeOffset.Now);
                _logger.LogWarning("Este es un mensaje de advertencia de ejemplo.");


            


            _logger.LogInformation("Generando reporte.");

            // Espera 1 dia antes de la siguiente ejecuci�n
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }

        _logger.LogInformation("El Worker se est� deteniendo.");
    }

    private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // Aquí puedes personalizar la validación si lo deseas
        return true; // Aceptar todos los certificados (solo para pruebas)
    }

    public static bool MailSendClient(string to, Attachment a)
    {
        string urlImagen = "";
        var urlBase64 = Convert.FromBase64String(urlImagen);
        var u = Encoding.UTF8.GetString(urlBase64);
        string contenido = AppDomain.CurrentDomain.BaseDirectory + @"Models\Resources\index.html";
        string paginaWeb = contenido.Replace("{img001}", u);
        string remitent = "echoquev@bcp.com.bo";
        string subject = "echoquev@bcp.com.bo";
        try
        {
            MailMessage message = new MailMessage(remitent, to)
            {
                Subject = subject,
                IsBodyHtml = true,
            };
            message.Attachments.Add(a);
            message.Body = paginaWeb;
            SmtpClient smtp = new SmtpClient("BTBEXC00")//NOSONAR
            {
                EnableSsl = Convert.ToBoolean("false"),
                UseDefaultCredentials = true,
                Port = 25
            };//NOSONAR
            smtp.Send(message);
            //_logger.LogInformation("SendMail.MailSendClient Adrres: {0} | Mail successfully sended.", to);
            message.Dispose();
            return true;
        }
        catch (Exception ex)
        {
            //_logger.LogInformation("SendMail.MailSendClient Adrres: {0} | Error sending mail: {1}", to, ex.Message);
            return false;
        }
    }

    private List<Certificado> ObtenerDatosCertificados(int idServidor ,string servidor, DateTime _fechaActual)
    {
        try
        {
            List<Certificado> listCert = new List<Certificado>();

            // Se arma el script de PowerShell con el filtrado del CN en IssuedTo e IssuedBy.
            string psScript =
                "Get-ChildItem -Path Cert:\\LocalMachine\\My | " +
                "Select-Object " +
                "Thumbprint, " +
                "@{Name='IssuedTo'; Expression={ " +
                "    $subject = $_.Subject; " +
                "    $match = [regex]::Match($subject, 'CN=([^,]+)'); " +
                "    if ($match.Success) { $match.Groups[1].Value } else { $subject } " +
                "}}, " +
                "@{Name='Subject'; Expression={ $_.Subject }}, " +
                "@{Name='Issuer'; Expression={ " +
                "    $issuer = $_.Issuer; " +
                "    $match = [regex]::Match($issuer, 'CN=([^,]+)'); " +
                "    if ($match.Success) { $match.Groups[1].Value } else { $issuer } " +
                "}}, " +
                "@{Name='IssuedBy'; Expression={ $_.Issuer }}, " +
                "@{Name='ValidFrom'; Expression={$_.NotBefore}}, " +
                "@{Name='ValidTo'; Expression={$_.NotAfter}}, " +
                "@{Name='IntendedPurposes'; Expression={ ($_.EnhancedKeyUsageList | ForEach-Object { $_.FriendlyName }) -join '; ' }}, " +
                "FriendlyName, " +
                "@{Name='CertificateTemplate'; Expression={ " +
                "    $template = $_.Extensions | Where-Object { $_.Oid.FriendlyName -eq 'Certificate Template Information' }; " +
                "    if ($template -ne $null) { $template.Format($true) } else { '' } " +
                "}} | ConvertTo-Csv -NoTypeInformation -Delimiter '|'";

            using Process proceso = new Process();
            proceso.StartInfo.FileName = "powershell.exe";
            // Se utiliza Invoke-Command para conectarse remotamente; el script se inyecta en el parámetro -ScriptBlock.
            proceso.StartInfo.Arguments = $"-Command \"Invoke-Command -ComputerName {servidor} -ScriptBlock {{{psScript}}}\"";
            proceso.StartInfo.RedirectStandardOutput = true;
            proceso.StartInfo.UseShellExecute = false;
            proceso.StartInfo.CreateNoWindow = true;
            proceso.Start();

            bool isHeader = true;
            while (!proceso.StandardOutput.EndOfStream)
            {
                string linea = proceso.StandardOutput.ReadLine();

                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(linea))
                {
                    var valores = linea.Split('|');
                    // Se omite la cabecera de CSV
                    
                    if (valores.Length == 10)
                    {
                        Certificado certificado = new Certificado
                        {
                            Id = idServidor,
                            Thumbprint = valores[0].Trim('"').Trim(),
                            IssuedTo = valores[1].Trim('"').Trim(),
                            Subject = valores[2].Trim('"').Trim(),
                            IssuedBy = valores[3].Trim('"').Trim(),
                            Issuer = valores[4].Trim('"').Trim(),
                            ValidFrom = DateTime.TryParse(valores[5].Trim('"').Trim(), out DateTime validoDesde) ? validoDesde : null,
                            ValidTo = DateTime.TryParse(valores[6].Trim('"').Trim(), out DateTime validoHasta) ? validoHasta : null,
                            IntendedPurposes = valores[7].Trim('"').Trim(),
                            FriendlyName = valores[8].Trim('"').Trim(),
                            CertificateTemplate = valores[9].Trim('"').Trim(),
                            FechaRecuperado = _fechaActual
                        };
                        listCert.Add(certificado);

                        _logger.LogInformation("Certificado: {nombre}", certificado.IssuedTo);
                    }
                }
            }

            proceso.WaitForExit();

            return listCert;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generando reporte certificados: {ex.Message}");
            throw new Exception("Error generando reporte certificados");
        }
    }
}