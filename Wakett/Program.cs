using Microsoft.EntityFrameworkCore;
using RatesService.Data;
using RatesService.Messaging;
using RatesService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RatesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<CoinMarketCapClient>();
builder.Services.AddSingleton<RateChangePublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


app.MapGet("/fetch-rates", async (CoinMarketCapClient client, RateChangePublisher publisher) =>
{
    var rates = await client.GetCryptoRatesAsync();
    var oldRates = new Dictionary<string, decimal>();

    foreach (var rate in rates)
    {
        if (oldRates.ContainsKey(rate.Key))
        {
            var oldRate = oldRates[rate.Key];
            if (Math.Abs((rate.Value - oldRate) / oldRate) > 0.05m)
            {
                publisher.PublishRateChange(rate.Key, oldRate, rate.Value);
            }
        }
        oldRates[rate.Key] = rate.Value;
    }

    return Results.Ok("Rates processed successfully.");
});

app.Run();
