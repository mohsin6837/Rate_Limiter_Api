using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-Client-Id";
    options.GeneralRules = new List<RateLimitRule>
    {
        new() {
           Endpoint= "*",
           Period="60s",
           Limit=4
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore,MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore,MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration,RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseIpRateLimiting();
app.MapControllers();

app.Run();
