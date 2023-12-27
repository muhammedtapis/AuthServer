using Microsoft.AspNetCore.Authorization;
using MiniApp1.API.Requirements;
using SharedLibrary.Configuration;
using SharedLibrary.Extensions;
using static MiniApp1.API.Requirements.BirthDateRequirement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));
//aşağıda tokenoptionstan nesne örneği alıyoruz AddCustomAuth extension metodunda kullancaz.
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();

builder.Services.AddCustomTokenAuth(tokenOptions);

//uygulama ayağa kalkarken sadece bi defa oluşturcak bu nesne örneğini singleton ile. kendi oluşturduğunu handler AgePolicy için
builder.Services.AddSingleton<IAuthorizationHandler, BirthDateRequirementHandler>();

//policy oluşturma  claim bazlı yetkilendirme için

builder.Services.AddAuthorization(options => //auth ayarları yapıcaz
{
    options.AddPolicy("AdanaPolicy", policy => //AdanaPolicy policy ekliyoruz isim verdik
    {
        policy.RequireClaim("city", "adana");  //bu policy ait claimtype ve claim değeri ne olacak ?
    });

    options.AddPolicy("AgePolicy", policy =>
    {
        policy.Requirements.Add(new BirthDateRequirement(18));
    });
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

app.Run();