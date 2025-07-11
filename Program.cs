using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TalkWithAyodeji.Data.DatabaseObject;
using TalkWithAyodeji.Hubs;
using TalkWithAyodeji.Repository.Data;
using TalkWithAyodeji.Repository.Seeder;
using TalkWithAyodeji.Repository.Seeder.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});
builder.Services.AddScoped<IAdminSeed,AdminSeed>();

//CLOUD DATABASE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

////LOCAL DATABASE
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Postgres")));

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
app.MapHub<ChatHub>("/chat");
app.MapControllers();

app.Run();
