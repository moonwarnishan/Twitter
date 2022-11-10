
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:4200")
            .AllowCredentials()
    )
);
builder.Services.Configure<DatabaseSetting>(
    builder.Configuration.GetSection("DatabaseSettings"));
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis-17516.c301.ap-south-1-1.ec2.cloud.redislabs.com:17516,password=r4CglWMh8yDjLs3LWYA7evwkFWTReC6n");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IFollowBlockServices, FollowBlockServices>();
builder.Services.AddSingleton<IFollowBlockIndividualServices, FollowBlockIndividualServices>();
builder.Services.AddSingleton<IRabbitMqPublish, RabbitMqPublish>();
builder.Services.AddSingleton<ITweetServices,TweetServices>();
builder.Services.AddSingleton<ILikeCommentRetweetServices, LikeCommentRetweetServices>();
builder.Services.AddSingleton<INotificationServices, NotificationServices>();
builder.Services.AddSingleton<IGetTweetServices,GetTweetServices > ();
builder.Services.AddSingleton<IRabbitMqDeleteService, RabbitMQDeleteServices>();
builder.Services.AddSingleton<IRedisServices, RedisServices>();
builder.Services.AddSingleton<IRabbitMqNotification, RabbitMqNotification>();
builder.Services.AddControllers();
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JWTKey").ToString())),
        ValidateIssuer = false,
        ValidateAudience = false
    };

});
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting web host");

// Full setup of serilog. We read log settings from appsettings.json
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSerilogRequestLogging(configure =>
{
    configure.MessageTemplate = "HTTP {RequestMethod} {RequestPath} {userName} responded {StatusCode} in {Elapsed:0.0000}ms";
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
if (!app.Environment.IsDevelopment())
{
    // Do not add exception handler for dev environment. In dev,
    // we get the developer exception page with detailed error info.
    app.UseExceptionHandler(errorApp =>
    {
        // Logs unhandled exceptions. For more information about all the
        // different possibilities for how to handle errors see
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-5.0
        errorApp.Run(async context =>
        {
            // Return machine-readable problem details. See RFC 7807 for details.
            // https://datatracker.ietf.org/doc/html/rfc7807#page-6
            var pd = new ProblemDetails
            {
                Type = "https://demo.api.com/errors/internal-server-error",
                Title = "An unrecoverable error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "This is a demo error used to demonstrate problem details",
            };
            pd.Extensions.Add("RequestId", context.TraceIdentifier);
            await context.Response.WriteAsJsonAsync(pd, pd.GetType(), null, contentType: "application/problem+json");
        });
    });
}
app.MapGet("/", (IDiagnosticContext diagnosticContext) =>
{
    // You can enrich the diagnostic context with custom properties.
    // They will be logged with the HTTP request.
    diagnosticContext.Set("UserId", "someone");
});
app.MapControllers();
app.UseCors("CorsPolicy");
IHostApplicationLifetime lifetime = app.Lifetime;
// IServiceProvider serviceProvider = app.Services.GetRequiredService<IServiceProvider>();
// lifetime.ApplicationStarted.Register(
//     () =>
//     {
//         var rabbitMqservices = (IRabbitMQConsume)serviceProvider.GetService(typeof(IRabbitMQConsume))!;
//         rabbitMqservices.Connect();
//     });
app.Run();
