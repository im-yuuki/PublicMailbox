namespace PublicMailbox.Node.Data;

public record MailRecord(
    UInt64 Id,
    string Subject,
    string Sender,
    string RecipientName,
    string RecipientDomain,
    string Body,
    DateTime Received,
    string ContentType
)
{
    public static MailRecord CreateRecord()
    {
        return null;
    }
    
    public string Recipient => $"{RecipientName}@{RecipientDomain}";
    public string Time => Received.ToString("ddd, dd MMM yyyy HH:mm:ss zzz");
}