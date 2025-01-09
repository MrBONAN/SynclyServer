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
// Настройка https://ru.stackoverflow.com/questions/1426836/%D0%9D%D0%B5-%D0%BC%D0%BE%D0%B3%D1%83-%D0%BF%D0%BE%D0%B4%D0%BA%D0%BB%D1%8E%D1%87%D0%B8%D1%82%D1%81%D1%8F-%D0%BA-%D1%81%D0%B5%D1%80%D0%B2%D0%B5%D1%80%D1%83-%D0%BD%D0%B0-asp-net-core-web-api
// + chatgpt)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5160); // Привязка ко всем IP-адресам на порту 5160
    serverOptions.ListenAnyIP(7199, listenOptions => listenOptions.UseHttps()); // Привязка для HTTPS
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