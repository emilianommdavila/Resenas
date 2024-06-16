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
using Resenas.Security.Tokens;

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
        ConfigureSwagger(services);

        // Configuración de MongoDB
        ConfigureMongoDB(services);

        // Configuración de RabbitMQ
        ConfigureRabbitMQ(services);

        // Configuración de Redis
        ConfigureRedis(services);

        // Registrar otros servicios
        RegisterOtherServices(services);

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
        var redisService = app.ApplicationServices.GetService<IRedisService>();
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
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
    }

    private void ConfigureMongoDB(IServiceCollection services)
    {
        services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDb"));
        services.Configure<VerificarToken>(Configuration.GetSection("ServicioAuth"));

        services.AddSingleton<IMongoClient, MongoClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton<IResenaRepository>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new ResenaRepository(settings);
        });
    }

    private void ConfigureRabbitMQ(IServiceCollection services)
    {
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

        services.AddSingleton<IRabbitMQService>(provider =>
        {
            var connection = provider.GetRequiredService<IConnection>();
            var redisService = provider.GetRequiredService<IRedisService>();
            return new Rabbit(connection, redisService);
        });
    }

    private void ConfigureRedis(IServiceCollection services)
    {
        services.AddSingleton<IRedisService>(provider =>
        {
            var redisConnectionString = Configuration.GetValue<string>("Redis:ConnectionString");
            return new RedisService(redisConnectionString);
        });
    }

    private void RegisterOtherServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();

        services.AddSingleton<VerificarToken>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var httpClient = provider.GetRequiredService<HttpClient>();
            var redisService = provider.GetRequiredService<IRedisService>();
            return new VerificarToken(httpClient, configuration, redisService);
        });
    }
}
