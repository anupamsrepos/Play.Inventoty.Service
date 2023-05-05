using MassTransit;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo()
                .AddMongoRepository<InventoryItem>("InventoryItems") //THis to Store Inventory items in database
                .AddMongoRepository<CatalogItem>("CatalogItems")     //This is to store Catalog items received from RabbitMq in same database. This will decouple the catalog and inventory service in case catalog service goes down
                .AddMassTransitWithRabbitMq();

AddCatalogClient(builder);

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

static void AddCatalogClient(WebApplicationBuilder builder)
{
    Random rand = new Random();

    builder.Services.AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7077");
    })
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(rand.Next(1, 1000))
     ))
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3,                      //3 request has to fail before circuit breakr uderstands that there is an error an close any other incomming connections
            TimeSpan.FromSeconds(15) //how much time the circuit will remain closed/broken before any new request can be allowed to go through
        ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}