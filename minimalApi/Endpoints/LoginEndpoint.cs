using minimalApi.Models;

namespace minimalApi.Endpoints;
public static class LoginEndpoint
{
    public static void AddLoginEndpoint(this WebApplication? app, WebApplicationBuilder builder)
    {
        if (app == null) return;

        app.MapPost("/login", [AllowAnonymous] async ([FromBodyAttribute] UserModel userModel, ITokenService tokenService, IUserService userService, HttpResponse response) =>
        {
            var userDto = userService.GetUser(userModel.UserName, userModel.Password);
            if (userDto == null)
            {
                response.StatusCode = 401;
                return;
            }

            var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Issuer"], builder.Configuration["Jwt:Audience"], userDto);
            await response.WriteAsJsonAsync(new { token = token });
            return;
        })
        .Produces(StatusCodes.Status200OK);
    }
}

