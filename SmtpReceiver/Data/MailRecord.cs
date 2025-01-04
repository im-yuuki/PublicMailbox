using MimeKit;

namespace PublicMailbox.SmtpReceiver.Data;

public record MailRecord(
    Int64 Id,
    string Subject,
    string Sender,
    string RecipientName,
    string RecipientDomain,
    string Body,
    DateTime Received
)
{
    public static MailRecord CreateRecord(IMimeMessage message)
    {
        return new MailRecord(
            Id: 0,
            Subject: message.Subject,
            Sender: message.From.ToString(),
            RecipientName: message.To.Mailboxes.FirstOrDefault()?.Address ?? "",
            RecipientDomain: message.To.Mailboxes.FirstOrDefault()?.Domain ?? "",
            Body: message.TextBody,
            Received: message.Date.DateTime
        );
    }
    
    public string Recipient => $"{RecipientName}@{RecipientDomain}";
    public string Time => Received.ToString("ddd, dd MMM yyyy HH:mm:ss zzz");
}