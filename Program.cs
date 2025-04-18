using System.Configuration;
using System.Reflection;
using LoadCoveredStripe.API.Application.Interfaces.IRepositories;
using LoadCoveredStripe.API.Application.Interfaces.IServices;
using LoadCoveredStripe.API.Application.Services;
using LoadCoveredStripe.API.Infrastructure.Data;
using LoadCoveredStripe.API.Infrastructure.Interfaces;
using LoadCoveredStripe.API.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Stripe;
using static LoadCoveredStripe.API.Application.Utilities.Constants;

var builder = WebApplication.CreateBuilder(args);

// Get the configuration 
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

// Get connection string
var sqlConnectionString = config[SQL_DB_CONN_STRING_NAME] ??
                          throw new ConfigurationErrorsException(
                              "Database Connection String not found in configuration.");

// Get stripe secret key
var stripeSecretKey = config[STRIPE_SECRET_KEY_NAME] ??
                       throw new ConfigurationErrorsException(
                           "Stripe Secret Key not found in configuration.");

// Get Stripe webhook secret
var stripeWebhookSecret = config[STRIPE_WEBHOOK_SECRET_NAME] ??
                           throw new ConfigurationErrorsException(
                               "Stripe Webhook Secret not found in configuration.");

// Configure DbContext with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString: sqlConnectionString));

// Configure Stripe settings
builder.Services.Configure<StripeSettings>(options => {
    options.SecretKey = stripeSecretKey;
    options.WebhookSecret = stripeWebhookSecret;
});
StripeConfiguration.ApiKey = stripeSecretKey;

// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerBillingRepository, CustomerBillingRepository>();
builder.Services.AddScoped<IPriceCatalogRepository, PriceCatalogRepository>();

// Register application services
builder.Services.AddScoped<ICustomerService, AppCustomerService>();
builder.Services.AddScoped<IStripeService, StripeService>();


// Add hosted service to sync prices from Stripe (if needed)
builder.Services.AddHostedService<StripePriceSyncService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LoadCovered Stripe API",
        Version = "v1",
        Description = "A comprehensive API for managing Stripe payment integrations, providing endpoints for subscription management, payment methods, and webhook handling. This API enables businesses to handle the complete payment lifecycle with Stripe, including creating setup intents, managing subscriptions (subscribe, change plans, pause, cancel, reactivate), updating payment methods, and processing Stripe webhook events.",
        Contact = new OpenApiContact
        {
            Name = "LoadCovered Support",
            Email = "support@loadcovered.com",
            Url = new Uri("https://www.loadcovered.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Proprietary",
            Url = new Uri("https://www.loadcovered.com/app-privacy-policy")
        },
        TermsOfService = new Uri("https://www.loadcovered.com/app-privacy-policy")
    });
    
    // Group endpoints by controller
    options.TagActionsBy(api => [api.GroupName ?? api.ActionDescriptor.RouteValues["controller"]]);
    options.DocInclusionPredicate((_, _) => true);
    
    // Add security definition for future authentication implementation
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // Enable XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi().CacheOutput();
    app.UseSwagger(options => { options.RouteTemplate = "openapi/{documentName}.json"; });
    app.MapScalarApiReference(
        "/docs",
        options =>
        {
            options.Title = "LoadCovered Stripe API";
            options.WithTheme(ScalarTheme.Purple);
        });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();