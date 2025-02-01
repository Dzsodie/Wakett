using Microsoft.EntityFrameworkCore;
using PositionsService.Data;
using PositionsService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PositionsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<PositionCsvLoader>();

// Add services to the container.
builder.Services.AddRazorPages();

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

var csvLoader = app.Services.GetRequiredService<PositionCsvLoader>();
await csvLoader.LoadPositionsFromCsv("positions.csv");

app.Run();

