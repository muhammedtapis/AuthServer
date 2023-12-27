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

//appsettingsteki TokenOptions ve CustomTOkenOptions s�n�f�n� e�le�tirdik bu sayede appsettingsdeki verilere ula��caz.
//CustomTokenOptions s�n�f� �zerinden,tabi ki DI olarka ge�tikten sonra
builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));

//client appsettings ayarlar� ile client s�n�f� e�leme appsettingsten client listesi geliyor
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));

//DI REGISTER
//addScoped bir requestte bi defa olu�turur.
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); //tokenservisi kullanan servis
builder.Services.AddScoped<ITokenService, TokenService>(); //token olu�turma
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

//authentcation ekleme buapimiz hem token da��t�yor hem de token do�rulama i�lemi ger�ekle�tiriyor.
builder.Services.AddAuthentication(options =>
{
    //bu �ema ne i�e yar�o ,ili tane ayr� �yelik sistemin olabilir farkl� login k�s�mlar� kullan�c� giri�i-bayigiri�i gibi bunlara �ema diyoruz.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //buras� authenticationdan gelen �ema
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //iki �emay� birbiriyle konu�turan ba�layan yer
                                                                             //yani burdaki authentication JWT json web token kullan�ca��n� ve a�a��daki jwtScheme kullan�ca��n� bilsin
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
    //buras� bearerdan gelen �ema,yukar�daki �emayla konu�turmam�z laz�m
    //custom token options eri�memiz laz�m nesne �rne�i olu�turmal�y�z ki appsettngsteki verilereeri�elim
    var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>();
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        //buras� token ge�erlilik ad�mlar� gibi d���n gelen token ile appsettingsteki alanlar kar��la�t�r�lcak o belirtiliyor.
        //burada da validate edilcek alanlar� veriyoruz
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

        //validate yani kontrol k�sm� appsettingsteki alanlarla ayn� �m onu kontrol et
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,

        //default 5 dk verir iki api serveri farkl� zonelarda �al��t�rabilirsin zaman fark� olabilir o sebeple bunu yap�o
        //biz s�f�ra �ektik zaman fark�m�z yok
        ClockSkew = TimeSpan.Zero
    };
});
//fluentvaliaiton
builder.Services.AddControllers().AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssemblyContaining<Program>();
});

//biz yazd�k bu ext.  metodu modelstate hatalar�n� g�rebilmek i�in response g�nderiyor.
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