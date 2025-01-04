using System.Buffers;
using ConsistentHashing;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace PublicMailbox.SmtpReceiver.Services;

public class RemoteStorageService(ILogger<RemoteStorageService> logger, IConfiguration configuration) : IMessageStore, IHostedService
{
    private readonly ConsistentHash<string> _hasher = ConsistentHash.Empty(Comparer<string>.Default);
    
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
        logger.LogInformation("Message received: {Subject} [{Sender} -> {Recipient}]", message.Subject, message.From, message.To);

        return SmtpResponse.Ok;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}