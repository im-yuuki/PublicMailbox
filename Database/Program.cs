using PublicMailbox.Database.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddLogging();

var app = builder.Build();
app.MapGrpcService<ApiService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
app.Run();