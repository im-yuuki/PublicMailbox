using System.Security.Authentication;
using PublicMailbox.Node.Data;
using SmtpServer;
using SmtpServer.Storage;

namespace PublicMailbox.Node.Services;

public class MailReceiverService(ILogger<MailReceiverService> logger, Storage storage) : IHostedService
{
    private readonly SmtpServer.SmtpServer _smtpServer = new (
        new SmtpServerOptionsBuilder()
            .ServerName("SMTP Server")
            .Endpoint(builder => {
                builder.Port(25, false);
                builder.AllowUnsecureAuthentication();
                builder.SupportedSslProtocols(SslProtocols.Tls12);
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
            })
            .Endpoint(builder => {
                builder.Port(587, true);
                builder.AllowUnsecureAuthentication();
                builder.SupportedSslProtocols(SslProtocols.Tls12);
                builder.ReadTimeout(TimeSpan.FromSeconds(10));
                // builder.Certificate(null);
            })
            .Build(),
        new ServiceCollection().AddSingleton<IMessageStore>(storage).BuildServiceProvider());

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting SMTP server");
        _smtpServer.StartAsync(cancellationToken).ConfigureAwait(false);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping SMTP server");
        _smtpServer.Shutdown();
        return Task.CompletedTask;
    }
}