using System.Buffers;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace PublicMailbox.Node.Data;

public class Storage(ILogger<Storage> logger) : IMessageStore
{
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
        logger.LogInformation($"Message received: {message.Subject} from {message.From} to {message.To}\n{message.TextBody}");

        return SmtpResponse.Ok;
    }
}