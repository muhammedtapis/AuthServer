using AuthServer.Core.Configuration;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWorks;
using AuthServer.Repository;
using AuthServer.Repository.Repositories;
using AuthServer.Repository.UnitOfWorks;
using AuthServer.Service.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using SharedLibrary.Extensions;
using SharedLibrary.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//appsettingsteki TokenOptions ve CustomTOkenOptions sýnýfýný eþleþtirdik bu sayede appsettingsdeki verilere ulaþýcaz.
//CustomTokenOptions sýnýfý üzerinden,tabi ki DI olarka geçtikten sonra
builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));

//client appsettings ayarlarý ile client sýnýfý eþleme appsettingsten client listesi geliyor
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));

//DI REGISTER
//addScoped bir requestte bi defa oluþturur.
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); //tokenservisi kullanan servis
builder.Services.AddScoped<ITokenService, TokenService>(); //token oluþturma
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
    //options.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

//authentcation ekleme buapimiz hem token daðýtýyor hem de token doðrulama iþlemi gerçekleþtiriyor.
builder.Services.AddAuthentication(options =>
{
    //bu þema ne iþe yarýo ,ili tane ayrý üyelik sistemin olabilir farklý login kýsýmlarý kullanýcý giriþi-bayigiriþi gibi bunlara þema diyoruz.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //burasý authenticationdan gelen þema
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //iki þemayý birbiriyle konuþturan baðlayan yer
                                                                             //yani burdaki authentication JWT json web token kullanýcaðýný ve aþaðýdaki jwtScheme kullanýcaðýný bilsin
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
    //burasý bearerdan gelen þema,yukarýdaki þemayla konuþturmamýz lazým
    //custom token options eriþmemiz lazým nesne örneði oluþturmalýyýz ki appsettngsteki verilereeriþelim
    var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        //burasý token geçerlilik adýmlarý gibi düþün gelen token ile appsettingsteki alanlar karþýlaþtýrýlcak o belirtiliyor.
        //burada da validate edilcek alanlarý veriyoruz
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

        //validate yani kontrol kýsmý appsettingsteki alanlarla ayný ým onu kontrol et
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,

        //default 5 dk verir iki api serveri farklý zonelarda çalýþtýrabilirsin zaman farký olabilir o sebeple bunu yapýo
        //biz sýfýra çektik zaman farkýmýz yok
        ClockSkew = TimeSpan.Zero
    };
});
//fluentvaliaiton
builder.Services.AddControllers().AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<Program>();
});

//biz yazdýk bu ext.  metodu modelstate hatalarýný görebilmek için response gönderiyor.
builder.Services.UseCustomValidationResponse();

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
//custom middleware exception.
app.UseCustomException();

app.UseHttpsRedirection();

app.UseAuthentication();//ekledik
app.UseAuthorization();

app.MapControllers();

app.Run();