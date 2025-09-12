using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Qdrant.Client;
using TalkWithAyodeji.Data.DatabaseObject;
using TalkWithAyodeji.Hubs;
using TalkWithAyodeji.Repository.Data;
using TalkWithAyodeji.Repository.Seeder;
using TalkWithAyodeji.Repository.Seeder.Seed;
using TalkWithAyodeji.Service.Helpers;
using TalkWithAyodeji.Service.Implementation;
using TalkWithAyodeji.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IQdrantService, QdrantService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddKernel();
builder.Services.AddSingleton<ChatHistory>();
builder.Services.AddSingleton<HttpClient>();
//builder.Services.AddSingleton<QdrantClient>();

//builder.Services.AddStackExchangeRedisCache(option =>
//{
//    option.Configuration = builder.Configuration.GetConnectionString("Redis");
//    option.InstanceName = "TalkToAyodejiRedis";

//});
var options = StackExchange.Redis.ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
options.AbortOnConnectFail = false;
options.ConnectRetry = 5;
options.EndPoints.Add(builder.Configuration["Redis:Host"], 6379);
options.ConnectTimeout = 10000;
builder.Services.AddStackExchangeRedisCache(option =>
{
    option.ConfigurationOptions = options;
    option.InstanceName = "TalkToAyodejiRedis";

});
var origins = builder.Configuration.GetSection("Origins")
                                   .GetChildren()
                                   .ToArray()
                                   .Select(x => x.Value)
                                   .ToArray();



builder.Services.AddCors(c =>
{
    c.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins(origins)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
//SignalR configuration
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;  // Useful for debugging
    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Ping interval to keep connection alive
    options.MaximumReceiveMessageSize = 1024 * 1024;  // 1 MB limit
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // Client disconnects if no activity for 30 seconds
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);  // Time allowed to complete the 	initial handshake
    options.MaximumParallelInvocationsPerClient = 5;  // Limit parallel client invocations
});
builder.Services.AddSingleton<QdrantClient>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IOptions<QdrantConfig>>().Value;
    
    var qdrantClient = new QdrantClient(
              host: builder.Configuration["Qdrant:HostName"],
              https: true,
              apiKey: builder.Configuration["Qdrant:ApiKey"]
            );

    return qdrantClient;
});
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});
builder.Services.AddOpenAIChatCompletion("gpt-4.1-nano-2025-04-14", builder.Configuration["OpenAI:APIKey"]);

builder.Services.AddScoped<IAdminSeed,AdminSeed>();

//CLOUD DATABASE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));



////LOCAL DATABASE
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                   Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
            )
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var userSeeder = services.GetRequiredService<IAdminSeed>();
    await userSeeder.AddDefaultAdmin();
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapHub<ChatHub>("/chat");
app.MapControllers();
app.UseCors("CorsPolicy");
app.Run();
