using kol1.Controllers;
using kol1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IVisitService, VisitService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();