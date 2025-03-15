using Auth.DataAccess;
using Auth.Entities;
using Auth.JWT;
using Common.Config;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// todo throw a variable in docker-compose to set the config path
var configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH");
builder.Configuration.AddJsonFile(configPath!, optional: false, reloadOnChange: true);
builder.Services.Configure<ApiConfig>(builder.Configuration.GetSection(nameof(ApiConfig)));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NpgSQL"))); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(authBuilder =>
    {
        authBuilder.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        authBuilder.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        authBuilder.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwtOptions =>
    {
        var apiConfig = builder.Configuration.GetSection(nameof(ApiConfig)).Get<ApiConfig>()!;
        jwtOptions.MetadataAddress = apiConfig.MetadataAddress;
        jwtOptions.Authority = apiConfig.Authority;
        jwtOptions.Audience = apiConfig.Audience;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidAudiences = apiConfig.ValidAudiences.Split(';'),
            ValidIssuers = apiConfig.ValidIssuers.Split(';')
        };

        jwtOptions.MapInboundClaims = false;
    })
    .AddGoogleOpenIdConnect(googleOptions =>
    {
        var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        googleOptions.ClientId = googleAuthNSection["ClientId"];
        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
    });

builder.Services.AddAuthorization();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<ITokenService, TokenService>();

var requireAuthPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(requireAuthPolicy);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.Run();

app.MapGet("/hello", [Authorize] () => "Hi");

