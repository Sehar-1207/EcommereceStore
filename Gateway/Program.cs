using Gateway.Midleware;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SharedLibrary.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Load Ocelot configuration
builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);

// 2. Add Ocelot services and enable caching
builder.Services.AddOcelot()
    .AddCacheManager(settings => settings.WithDictionaryHandle());

// 3. Add your custom JWT Authentication services
JwtAuthentication.AddJwtAuthentication(builder.Services, builder.Configuration);

// 4. Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});


var app = builder.Build();

app.UseHttpsRedirection();

// 5. Use CORS - Must be called before authentication/authorization
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AttachedSignedHeader>();
app.MapGet("/", () => "API Gateway is running.");

await app.UseOcelot();

app.Run();