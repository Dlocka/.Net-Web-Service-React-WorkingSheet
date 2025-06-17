using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

//inject
builder.Services.AddScoped<IStaffReadRepository, StaffReadRepository>();
builder.Services.AddScoped<IStaffWriteRepository, StaffWriteRepository>();
builder.Services.AddScoped<IStaffWriter, ManagerService>();
builder.Services.AddScoped<IStaffReader, ManagerService>();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddScoped<IWorkHoursReadRepository, WorkHoursReadRepository>();
builder.Services.AddScoped<IWorkHoursWriteRepository, WorkHoursWriteRepository>();
builder.Services.AddScoped<IWorkHoursService, WorkHoursService>();


builder.Services.AddScoped<IJobWriteRepository, JobWriteRepository>();
builder.Services.AddScoped<IJobReadRepository, JobReadRepository>();
builder.Services.AddScoped<IJobService,JobService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "TimeSheets API", Description = "Test for backend by Swagger from .Net", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("WorkingSheets") ?? "Data Source=WorkingSheets.db";
//To Do: Change the connection String into Configuration


builder.Services.AddSqlite<AppDbContext>(connectionString);
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

//Development env CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        // In dev environment, allow all origins
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    //Production env CORS
    options.AddPolicy("ProductionCorsPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors(app.Environment.IsDevelopment() ? "DevCorsPolicy" : "ProductionCorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCorsPolicy");

    app.MapOpenApi();
    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProductionCorsPolicy"); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
//Swagger UI test

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
