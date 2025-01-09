using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILinksRepository, LinksRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<ITopTracksRepository, TopTrackRepository>();
builder.Services.AddScoped<ITopArtistsRepository, TopArtistsRepository>();
builder.Services.AddScoped<ITrackArtistRepository, TrackArtisRepository>();


// TODO: Возможно, стоит сделать разделение базы данных, если мы будем реализовывать автоматическое подтягивание кода
// из гитхаба на сервере
builder.Services.AddControllers();
builder.Services.AddDbContext<ServerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// Настройка портов
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5160);
    serverOptions.ListenAnyIP(7199, listenOptions => listenOptions.UseHttps(options => options.ServerCertificate =
        new System.Security.Cryptography.X509Certificates.X509Certificate2(
            "/home/syncly/server.pfx",
            "Syncly1488@")));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();