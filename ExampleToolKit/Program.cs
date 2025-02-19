using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<DbContext>(options =>
                    options.UseSqlServer(connectionString));


                var configiMongoDb = context.Configuration.GetSection("MongoDbSettings");
                if(configiMongoDb!=null){
                    // Lendo as configurações do MongoDB do appsettings.json
                    services.Configure<MongoDbSettings>(configiMongoDb);

                    // Registrando o contexto do MongoDB como Singleton
                    services.AddSingleton<MongoDbContext>(sp =>
                    {
                        var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                        return new MongoDbContext(settings.ConnectionString, settings.DatabaseName);
                    });
                }

                services.AddMediatR(typeof(Program));
                var kafkaSettings = context.Configuration.GetSection("KafkaSettings").Get<IntegrationEvent>();
                services.AddSingleton(kafkaSettings!);
                services.AddSingleton<MessageConsumer<string, string>>();
                services.AddSingleton<ResultConsumer>();
                services.AddSingleton(typeof(MessageProducer<>));
                services.AddSingleton(typeof(ResultProducer<>));
                services.AddTransient<AggregateRoot>();
                // Registrando Repositório e UnitOfWork
                services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
                services.AddScoped(typeof(IMongoDbRepository<>), typeof(MongoRepository<>));
                services.AddScoped(typeof(IUnitOfWork),typeof(UnitOfWork<>));
            });
}