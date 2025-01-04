using PublicMailbox.SmtpReceiver.Data;
using PublicMailbox.SmtpReceiver.Services;

var builder = new HostApplicationBuilder(args);
builder.Services.AddLogging();
builder.Services.AddSingleton<RemoteStorageService>();
builder.Services.AddSingleton<MailReceiverService>();
builder.Services.AddHostedService<RemoteStorageService>();
builder.Services.AddHostedService<MailReceiverService>();

var app = builder.Build();
await app.RunAsync();