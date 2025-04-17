using LoadCoveredStripe.API.Data.AppDbContext;
using LoadCoveredStripe.API.Application.Interfaces.IRepositories;
using LoadCoveredStripe.API.Infrastructure.Data;
using LoadCoveredStripe.API.Infrastructure.Interfaces;
using LoadCoveredStripe.API.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure DbContext with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString: connectionString));

// Configure Stripe settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerBillingRepository, CustomerBillingRepository>();
builder.Services.AddScoped<IPriceCatalogRepository, PriceCatalogRepository>();

// Register services
builder.Services.AddScoped<IStripeService, StripeService>();


// Add hosted service to sync prices from Stripe (if needed)
// builder.Services.AddHostedService<StripePriceSyncService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(
        options =>
        {
            options.Title = "LoadCovered Billing API";
            options.WithTheme(ScalarTheme.Purple);
        });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();