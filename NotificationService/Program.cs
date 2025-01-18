using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environment == "Docker")
{
    builder.Configuration.AddJsonFile(
        "appsettings.Docker.json",
        optional: true,
        reloadOnChange: true
    );
}
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
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
app.ApplyPendingMigrations();
app.Run();
