using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:4200")
            .AllowCredentials()
    )
);

// Add services to the container.
builder.Services.Configure<DatabaseSetting>(
    builder.Configuration.GetSection("DatabaseSetting"));

builder.Services.AddSingleton<UserServices>();
builder.Services.AddSingleton<ISearchServiceMongo, SearchServiceMongo>();
builder.Services.AddSingleton<JwtServices>();
var multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton<PasswordResetServices>();
builder.Services.AddSingleton<IRabbitMQConsume, RabbitMqConsume>();
builder.Services.AddSingleton<IRabbitMqDeleteService, RabbitMqDeleteService>();
builder.Services.AddSingleton<IRabbitMQNotification,RabbitMQNotification>();
builder.Services.AddSingleton<IRedisServices,RedisServices>();
builder.Services.AddSingleton<NotificationHub>();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.Configure<IdentityOptions>(options =>
    options.ClaimsIdentity.UserNameClaimType = ClaimTypes.NameIdentifier);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JWTKey").ToString() ?? string.Empty)),
        ValidateIssuer = false,
        ValidateAudience = false
    };

});
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();


builder.Services.AddSingleton(emailConfig);

builder.Services.Configure<FormOptions>(o => {
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

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

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors("CorsPolicy");
app.MapHub<NotificationHub>("/livenotification");
IHostApplicationLifetime lifetime = app.Lifetime;
IServiceProvider serviceProvider = app.Services.GetRequiredService<IServiceProvider>();
lifetime.ApplicationStarted.Register(
    () =>
    {
        var rabbitMqservices = (IRabbitMQNotification)serviceProvider.GetService(typeof(IRabbitMQNotification))!;
        rabbitMqservices.Connect();
    });

app.Run();
