using BitcoinPriceAggregator.Data.Aggregators;
using BitcoinPriceAggregator.Data.Interfaces;
using BitcoinPriceAggregator.Data.Scrapers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IPriceScraper, BitfinexPriceScraper>();
builder.Services.AddHttpClient<IPriceScraper, BitstampPriceScraper>();
builder.Services.AddTransient<IPriceAggregator, MeanPriceAggregator>();
// Add services to the container.

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

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();