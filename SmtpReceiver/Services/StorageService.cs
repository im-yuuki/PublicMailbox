using System.Buffers;
using ConsistentHashing;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace PublicMailbox.SmtpReceiver.Services;

public class StorageService(ILogger<StorageService> logger, IConfiguration configuration) : IMessageStore, IHostedService
{
    private readonly ConsistentHash<string> _hasher = ConsistentHash.Empty(Comparer<string>.Default);
    private readonly HashSet<string> _allowedDomains = [];
    
    private bool CheckAllowedDomain(string domain)
    {
        return _allowedDomains.Count == 0 || _allowedDomains.Contains(domain);
    }
    
    public async Task<SmtpResponse> SaveAsync(
        ISessionContext context, 
        IMessageTransaction transaction, 
        ReadOnlySequence<byte> buffer,
        CancellationToken cancellationToken) 
    {
        await using var stream = new MemoryStream();

        var position = buffer.GetPosition(0);
        while (buffer.TryGet(ref position, out var memory)) await stream.WriteAsync(memory, cancellationToken);
        stream.Position = 0;

        var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);
        logger.LogInformation("Message received: {Subject} [{Sender} -> {Recipient}]", 
            message.Subject, 
            message.From.FirstOrDefault(), 
            message.To.FirstOrDefault()
            );

        return SmtpResponse.Ok;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _allowedDomains.Clear();
        foreach (var item in configuration.GetSection("AllowedDomains").GetChildren())
        {
            if (item.Value is null) continue;
            _allowedDomains.Add(item.Value);
        }
        if (_allowedDomains.Count == 0) logger.LogWarning("No allowed domains configured. All domains will be accepted.");
        else logger.LogInformation("Allowing {DomainsCount} domains", _allowedDomains.Count);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}