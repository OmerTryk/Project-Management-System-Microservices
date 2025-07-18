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
builder.Services.AddDistributedMemoryCache(); // Session verilerini bellek �zerinde saklayaca��z
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi (iste�e ba�l�)
    options.Cookie.IsEssential = true; // �erezlerin gerekli oldu�unu belirtir
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()  // T�m kaynaklardan gelen taleplere izin verir
               .AllowAnyMethod()  // T�m HTTP metodlar�na izin verir
               .AllowAnyHeader(); // T�m ba�l�klara izin verir
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
