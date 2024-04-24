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

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddSwaggerGen();
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
        services.AddSingleton<HttpClient>();
        services.AddSingleton<VerificarToken>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var httpClient = provider.GetRequiredService<HttpClient>();
            return new VerificarToken(httpClient, configuration);
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
            //app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Resenas V1");
            });
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
