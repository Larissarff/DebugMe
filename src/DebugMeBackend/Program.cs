using DebugMeBackend.Data;
using DebugMeBackend.HealthChecks;
using DebugMeBackend.Repositories;
using DebugMeBackend.Repositories.Interfaces;
using DebugMeBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    OpenApiInfo info = new OpenApiInfo
    {
        Title = "DebugMe API",
        Version = "v1",
        Description = "API backend do projeto DebugMe"
    };

    options.SwaggerDoc("v1", info);
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=debugme.db"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmotionRepository, EmotionRepository>();
builder.Services.AddScoped<IEventLogRepository, EventLogRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmotionService>();
builder.Services.AddScoped<EventLogService>();

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DebugMe API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration.ToString(),
            timestamp = DateTime.UtcNow,
            entries = report.Entries.ToDictionary(
                e => e.Key,
                e => new
                {
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.ToString()
                })
        };

        await context.Response.WriteAsJsonAsync(response);
    }
});

app.Run();
