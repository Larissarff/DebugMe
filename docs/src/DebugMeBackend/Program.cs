using DebugMeBackend.Services;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();


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


builder.Services.AddSingleton<UserService>();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();