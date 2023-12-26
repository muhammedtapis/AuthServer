using AuthServer.Core.Configuration;
using SharedLibrary.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//appsettingsteki TokenOptions ve CustomTOkenOptions sýnýfýný eþleþtirdik bu sayede appsettingsdeki verilere ulaþýcaz.
//CustomTokenOptions sýnýfý üzerinden,tabi ki DI olarka geçtikten sonra
builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));

//client appsettings ayarlarý ile client sýnýfý eþleme appsettingsten client listesi geliyor
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));

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

app.UseAuthorization();

app.MapControllers();

app.Run();