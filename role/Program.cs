using Microsoft.OpenApi.Models;
using role_api.Contexts;
using role_api.Data;
using role_api.Data.Interfaces;
using role_api.Services;
using role_api.Services.Interfaces;
using Serilog;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();
try
{
    Log.Information("Application Starting.");
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddSingleton<DbContext>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IRoleService, RoleService>();

// Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo
                { Title = "Test Project", Version = "v1", Description = "Role Service API" });
    });

    var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Role Service");
        c.DefaultModelsExpandDepth(-1);
    });
// }

    app.UseCors(x => x
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}