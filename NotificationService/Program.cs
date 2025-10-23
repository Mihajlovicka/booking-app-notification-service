using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Extensions;
using Serilog;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


var serviceName = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "unknown-service";
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environment == "Docker")
{
    builder.Configuration.AddJsonFile(
        "appsettings.Docker.json",
        optional: true,
        reloadOnChange: true
    );
    builder.AddMonitoring();
    
}
builder.Configuration.AddEnvironmentVariables();

var mongoConnection = builder.Configuration.GetConnectionString("DefaultConnection"); 
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient(mongoConnection);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = new MongoUrl(mongoConnection).DatabaseName;
    var database = client.GetDatabase(databaseName);
    return new AppDbContext(database);
});

builder.Services.AddCustomCors();
builder.Services.AddKafkaServices(builder.Configuration);
builder.Services.AddCustomServices();
builder.AddAppAuthentication();

builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true
);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

var app = builder.Build();

var meter = new Meter("custom_metrics_"+serviceName, "1.0");
var responseSizeCounter = meter.CreateCounter<long>("http_response_size_bytes");
app.Use(async (context, next) =>
{
    var originalBodyStream = context.Response.Body;
    using var memoryStream = new MemoryStream();
    context.Response.Body = memoryStream;

    await next();

    var size = memoryStream.Length;
    responseSizeCounter.Add(size, new KeyValuePair<string, object?>("service_name", serviceName));

    memoryStream.Seek(0, SeekOrigin.Begin);
    await memoryStream.CopyToAsync(originalBodyStream);
    context.Response.Body = originalBodyStream;
});
var uniqueVisitorCounter = meter.CreateCounter<long>("unique_visitors");

app.Use(async (context, next) =>
{
    await next();

    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var browser = context.Request.Headers["User-Agent"].ToString() ?? "unknown";
    var timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm"); // group by minute

    // Use tags as labels
    uniqueVisitorCounter.Add(1, new KeyValuePair<string, object?>("ip", ip),
                               new KeyValuePair<string, object?>("browser", browser),
                               new KeyValuePair<string, object?>("timestamp", timestamp));
});

app.UseSerilogRequestLogging(options =>
{
    options.IncludeQueryInRequestPath = true;
});

app.UseRequestResponseLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        var exceptionHandler = context.RequestServices.GetRequiredService<IExceptionHandler>();
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        if (exception != null)
        {
            await exceptionHandler.TryHandleAsync(context, exception, context.RequestAborted);
        }
    });
});
app.UseAuthorization();

app.UseCors(CorsExtensions.GetCorsPolicyName());

app.MapControllers();
app.EnsureDatabaseSetup();
app.Run();
