using System.Net;
using System.Security.Authentication;
using SmtpServer;
using SmtpServer.Storage;

namespace PublicMailbox.SmtpReceiver.Services;

public class MailReceiverService : IHostedService
{
    private readonly ILogger<MailReceiverService> _logger;
    private readonly SmtpServer.SmtpServer _smtpServer;

    public MailReceiverService(ILogger<MailReceiverService> logger, IConfiguration configuration, RemoteStorageService storageService)
    {
        _logger = logger;
        var builder = new SmtpServerOptionsBuilder()
            .ServerName("SMTP Server")
            .Endpoint(builder => {
                // Default SMTP port
                builder.Endpoint(new IPEndPoint(IPAddress.Any, 25));
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
            })
            .Endpoint(builder => {
                // Default SMTPS port
                builder.Port(465, true);
                builder.SupportedSslProtocols(SslProtocols.Tls12);
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
            })
            .Endpoint(builder => {
                // Default SMTP with STARTTLS port
                builder.Port(587, true);
                builder.AllowUnsecureAuthentication();
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
            }).Endpoint(builder => {
                // Default SMTP IPv6 port
                builder.Endpoint(new IPEndPoint(IPAddress.IPv6Any, 25));
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
            })
            .Endpoint(builder => {
                // Default SMTPS IPv6 port
                builder.Endpoint(new IPEndPoint(IPAddress.IPv6Any, 465));
                builder.IsSecure(true);
                builder.SupportedSslProtocols(SslProtocols.Tls12);
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
            })
            .Endpoint(builder => {
                // Default SMTP with STARTTLS IPv6 port
                builder.Endpoint(new IPEndPoint(IPAddress.IPv6Any, 587));
                builder.AllowUnsecureAuthentication();
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
            })
            .Build();
        var serviceProvider = new ServiceCollection().AddSingleton<IMessageStore>(storageService).BuildServiceProvider();
        _smtpServer = new SmtpServer.SmtpServer(builder, serviceProvider);
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting SMTP server");
        _smtpServer.StartAsync(cancellationToken).ConfigureAwait(false);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping SMTP server");
        _smtpServer.Shutdown();
        return Task.CompletedTask;
    }
}