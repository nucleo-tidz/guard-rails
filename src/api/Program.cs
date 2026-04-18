using application;

using infrastructure;

using model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISharedContext, SharedContext>();
builder.Services.AddInfrastructure(builder.Configuration)
    .AddAI(builder.Configuration).AddGuardRail(builder.Configuration);
builder.Services.AddApplication();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
