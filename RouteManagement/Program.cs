
using Neo4jClient;
using RouteManagement.Services;

namespace RouteManagement;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddSingleton<Neo4jService>();

        // Load configuration
        var configuration = builder.Configuration;

        var client = new BoltGraphClient(
            new Uri(configuration["Neo4j:Uri"]),
            configuration["Neo4j:User"],
            configuration["Neo4j:Password"]
        );
        client.ConnectAsync().Wait();

        builder.Services.AddSingleton<IGraphClient>(client);

        // Register your services
        builder.Services.AddScoped<CityService>();
        builder.Services.AddScoped<RouteService>();
        builder.Services.AddScoped<RelationshipService>();
        builder.Services.AddScoped<StopService>();
        builder.Services.AddScoped<ScheduleService>();
        builder.Services.AddScoped<BusService>();


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
