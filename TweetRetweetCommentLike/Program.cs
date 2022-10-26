
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
var multiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton<IFollowBlockServices, FollowBlockServices>();
builder.Services.AddSingleton<IFollowBlockIndividualServices, FollowBlockIndividualServices>();
builder.Services.AddSingleton<IRabbitMqPublish, RabbitMqPublish>();
builder.Services.AddSingleton<ITweetServices,TweetServices>();
builder.Services.AddSingleton<ILikeCommentRetweetServices, LikeCommentRetweetServices>();
builder.Services.AddSingleton<INotificationServices, NotificationServices>();
builder.Services.AddSingleton<IGetTweetServices,GetTweetServices > ();
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
IHostApplicationLifetime lifetime = app.Lifetime;
// IServiceProvider serviceProvider = app.Services.GetRequiredService<IServiceProvider>();
// lifetime.ApplicationStarted.Register(
//     () =>
//     {
//         var rabbitMqservices = (IRabbitMQConsume)serviceProvider.GetService(typeof(IRabbitMQConsume))!;
//         rabbitMqservices.Connect();
//     });
app.Run();
