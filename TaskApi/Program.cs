using MassTransit;
using Microsoft.EntityFrameworkCore;
using TaskApi.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);

    });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache(); // Session verilerini bellek üzerinde saklayacaðýz
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi (isteðe baðlý)
    options.Cookie.IsEssential = true; // Çerezlerin gerekli olduðunu belirtir
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()  // Tüm kaynaklardan gelen taleplere izin verir
               .AllowAnyMethod()  // Tüm HTTP metodlarýna izin verir
               .AllowAnyHeader(); // Tüm baþlýklara izin verir
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
