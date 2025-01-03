using PublicMailbox.Node.Data;
using PublicMailbox.Node.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddLogging();
builder.Services.AddSingleton<Storage>();
builder.Services.AddHostedService<MailReceiverService>();

var app = builder.Build();

app.MapGet("/", () => "Communication with this service must be made through a gRPC client");
app.MapGrpcService<GrpcHandlerService>();

await app.RunAsync();