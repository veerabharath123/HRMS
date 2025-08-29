using HRMS.WebApplication.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices(typeof(Program));

var app = builder.Build();
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
app.RegisterPipelineComponents(env,typeof(Program));

app.Run();
