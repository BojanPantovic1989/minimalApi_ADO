
using minimalApi.Configuration;
using minimalApi.Database;
using minimalApi.Endpoints;
using MySqlConnector;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);
var appsettings = builder.Configuration.Get<AppSettings>();

builder.Services.AddSingleton(appsettings);

builder.Services.AddSingleton<DbProviderFactory>(MySqlConnectorFactory.Instance);

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<DapperContext>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (BadHttpRequestException ex)
    {
        ctx.Response.StatusCode = ex.StatusCode;
        await ctx.Response.WriteAsync(ex.Message);
    }
});

app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.AddLoginEndpoint(builder);
app.AddGetAllFxRatesEndpoint();

app.Run();