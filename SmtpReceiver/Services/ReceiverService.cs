using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using SmtpServer;

namespace PublicMailbox.SmtpReceiver.Services;

public class ReceiverService(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<ReceiverService> logger)
    : SmtpServer.SmtpServer(BuildOptions(configuration, logger), serviceProvider), IHostedService
{
    private static ISmtpServerOptions BuildOptions(IConfiguration configuration, ILogger logger)
    {
        var builder = new SmtpServerOptionsBuilder().ServerName("SMTP Server");
        // Base SMTP endpoint
        builder.Endpoint(def => {
            def.Port(25);
        });
        builder.Endpoint(def => {
            def.Endpoint(new IPEndPoint(IPAddress.IPv6Any, 25));
        });
        // Load a X.509 certificate from the configured path (supports PEM and DER encoded certificates)
        var path = configuration.GetSection("CertificatePath")?.Value;
        if (path is null) {
            logger.LogWarning("No certificate configured. Using unencrypted SMTP.");
            // Still listen on port 587 without STARTTLS support
            builder.Endpoint(def => {
                def.Port(587);
                def.AllowUnsecureAuthentication();
            });
            builder.Endpoint(def => {
                def.Endpoint(new IPEndPoint(IPAddress.IPv6Any, 587));
                def.AllowUnsecureAuthentication();
            });
        }
        else {
            var certificate = X509CertificateLoader.LoadCertificateFromFile(path);
            logger.LogInformation("Certificate serial number: {SerialNumber}", certificate.SerialNumber);
            // Enable STARTTLS
            builder.Endpoint(def => {
                def.Port(587);
                def.AllowUnsecureAuthentication();
                def.Certificate(certificate);
            });
            builder.Endpoint(def => {
                def.Endpoint(new IPEndPoint(IPAddress.IPv6Any, 587));
                def.AllowUnsecureAuthentication();
                def.Certificate(certificate);
            });
            // Enable SSL/TLS
            builder.Endpoint(def => {
                def.Port(465, true);
                def.SupportedSslProtocols(SslProtocols.Tls12);
                def.Certificate(certificate);
            });
            builder.Endpoint(def => {
                def.Endpoint(new IPEndPoint(IPAddress.IPv6Any, 465));
                def.IsSecure(true);
                def.SupportedSslProtocols(SslProtocols.Tls12);
                def.Certificate(certificate);
            });
        }
        return builder.Build();
    }

    public new Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting SMTP server");
        // The base StartAsync is likely RunAsync, so I need to wrap it up
        base.StartAsync(cancellationToken).ConfigureAwait(false);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping SMTP server");
        Shutdown();
        return Task.CompletedTask;
    }
}