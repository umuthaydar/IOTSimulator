using Microsoft.EntityFrameworkCore;
using IoTSimulator.Subscriber.Infrastructure.Data;
using IoTSimulator.Subscriber.Infrastructure.Repositories;
using IoTSimulator.Subscriber.Application.Interfaces;
using IoTSimulator.Subscriber.Application.Services;
using IoTSimulator.Subscriber.Infrastructure.Services;
using IoTSimulator.Subscriber.Hubs;
using IoTSimulator.Subscriber.Application.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(ApplicationMappingProfile));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=iot_subscriber;Username=postgres;Password=postgres";
    
builder.Services.AddDbContext<IoTDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<HouseRepository>();
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<DeviceRepository>();
builder.Services.AddScoped<SensorDataRepository>();

builder.Services.AddScoped<IHouseService, HouseService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddHostedService<MqttSubscriberService>();

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "IoT Simulator Subscriber API",
        Version = "v1",
        Description = "REST API for managing IoT devices, houses, rooms, and sensor data following Onion Architecture principles."
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Required for SignalR
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Simulator Subscriber API v1");
        c.RoutePrefix = string.Empty;
    });
}

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IoTDbContext>();
    await context.Database.EnsureCreatedAsync();
    await context.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHub<SensorDataHub>("/sensorDataHub");

app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.MapGet("/", () => new
{
    message = "IoT Simulator Subscriber API",
    version = "1.0.0",
    documentation = "/swagger",
    endpoints = new
    {
        houses = "/api/houses",
        rooms = "/api/rooms",
        devices = "/api/devices",
        sensorData = "/api/sensordata",
        health = "/health"
    }
});

app.Run();
