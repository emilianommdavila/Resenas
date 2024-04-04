using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Resenas.Model;
using Resenas.Model.Repositories;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Resenas.Model.Interfaces;
using Microsoft.OpenApi.Models;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwaggerGen();
        // Configuración de MongoDB
        services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));

        // Agregar el servicio de MongoDB
        services.AddSingleton<IMongoClient, MongoClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        // Registrar el servicio ResenaRepository con la dependencia de MongoDbSettings resuelta
        services.AddSingleton<IResenaRepository>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new ResenaRepository(settings);
        });
        services.AddSingleton<Resenas.Middleware.Rabbit.Rabbit>();
        services.AddAuthorization();
        services.AddControllers();

        // Otros servicios...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
