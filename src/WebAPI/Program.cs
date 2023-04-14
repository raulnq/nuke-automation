using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebAPI.Application;
using WebAPI.Infrastructure;
using Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

try
{
    Log.Information("Starting host");

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    builder.Services.AddApplication();

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHealthChecks();

    app.UseSwagger(builder.Configuration);

    app.UseLocalization();

    app.UseProblemDetails();

    app.UseDefaultFiles();

    app.UseStaticFiles();

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.UseRouting();

    app.UseCors(builder.Configuration);

    app.UseAuthentication();

    app.UseAuthorization();

    app.UseHttpLogging();

    app.MapControllers();

    app.Run();
}
finally
{
    Log.Information("Ending host");

    Log.CloseAndFlush();
}