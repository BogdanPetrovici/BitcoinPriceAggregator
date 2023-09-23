using BitcoinPriceAggregator.BL;
using BitcoinPriceAggregator.Data;
using BitcoinPriceAggregator.Web.Interfaces;
using BitcoinPriceAggregator.Web.Scrapers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<PriceAggregatorContext>();
builder.Services.AddTransient<IPriceRepository, InMemoryRepository>();
builder.Services.AddHttpClient<IPriceScraper, BitfinexPriceScraper>();
builder.Services.AddHttpClient<IPriceScraper, BitstampPriceScraper>();
builder.Services.AddTransient<IPriceAggregator, MeanPriceAggregator>();
builder.Services.AddTransient<IPriceCache, AggregatedPriceCache>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

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
