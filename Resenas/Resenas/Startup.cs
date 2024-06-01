using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Resenas.Model;
using Resenas.Model.Repositories;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Resenas.Model.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Resenas.Middleware.Auth;
using Resenas.Middleware.Rabbit;
using RabbitMQ.Client;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
       // Configuración de Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API de Resenas",
                Description = "API para gestionar reseñas de productos",
                Contact = new OpenApiContact
                {
                    Name = "Tu nombre",
                    Email = "tu@email.com",
                    Url = new Uri("https://example.com"),
                },
            });
        });

        // Configuración de MongoDB
        services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));
        services.Configure<VerificarToken>(Configuration.GetSection("ServicioAuth"));

        // Configuración de RabbitMQ
        services.Configure<RabbitMQSettings>(Configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IConnection>(provider =>
        {
            var rabbitMQSettings = provider.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings.HostName,
                UserName = rabbitMQSettings.UserName,
                Password = rabbitMQSettings.Password
            };
          
            return factory.CreateConnection();
        });

        // Registrar servicios de MongoDB
        services.AddSingleton<IMongoClient, MongoClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        // Registrar Repositories
        services.AddSingleton<IResenaRepository>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new ResenaRepository(settings);
        });

        // Registrar otros servicios
        services.AddSingleton<HttpClient>();
        services.AddSingleton<VerificarToken>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var httpClient = provider.GetRequiredService<HttpClient>();
            return new VerificarToken(httpClient, configuration);
        });

        // Registrar RabbitMQService
        Console.WriteLine("Rabbit");
        services.AddSingleton<IRabbitMQService, Rabbit>();
        Console.WriteLine("Termino Rabbit");
        services.AddAuthorization();
        services.AddControllers();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Resenas V1");
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        // Inicializar el servicio RabbitMQ
        var rabbitService = app.ApplicationServices.GetService<IRabbitMQService>();

    }
}
