using PublicMailbox.SmtpReceiver.Services;

var builder = new HostApplicationBuilder(args);
builder.Services.AddLogging();
builder.Services.AddSingleton<StorageService>();
builder.Services.AddHostedService<StorageService>();
builder.Services.AddHostedService<ReceiverService>();

var app = builder.Build();
await app.RunAsync();