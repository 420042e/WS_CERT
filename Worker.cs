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
                        using (TcpClient client = new TcpClient(servidor.Nombre, port))
                        using (SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null))
                        {
                            sslStream.AuthenticateAsClient(servidor.Nombre);
                            X509Certificate cert = sslStream.RemoteCertificate;

                            if (cert != null)
                            {

                                var info = new CertificadoInfo
                                {
                                    Host = servidor.Nombre,
                                    Sujeto = cert.Subject,
                                    Emisor = cert.Issuer,
                                    ValidoDesde = cert.GetEffectiveDateString(),
                                    ValidoHasta = cert.GetExpirationDateString(),
                                    Observacion = ""
                                };
                                certificados.Add(info);


                                _logger.LogInformation("Certificado del servidor {host}:", servidor.Nombre);
                                _logger.LogInformation(" - Sujeto: {subject}", cert.Subject);
                                _logger.LogInformation(" - Emisor: {issuer}", cert.Issuer);
                                _logger.LogInformation(" - Válido desde: {start}", cert.GetEffectiveDateString());
                                _logger.LogInformation(" - Válido hasta: {end}", cert.GetExpirationDateString());
                            }
                            else
                            {
                                _logger.LogWarning("No se pudo obtener el certificado del servidor {host}.", servidor.Nombre);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //BorrarE
                        var info = new CertificadoInfo
                        {
                            Host = servidor.Nombre,
                            Sujeto = "null",
                            Emisor = "null",
                            ValidoDesde = "null",
                            ValidoHasta = "null",
                            Observacion = "Error al obtener el certificado del servidor " + servidor.Nombre
                        };
                        certificados.Add(info);

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
}