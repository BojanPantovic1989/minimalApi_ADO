using minimalApi.Configuration;
using System.Data;
using System.Data.Common;

namespace minimalApi.Endpoints;

public static class GetFxRateEndpoint
{

    public static void AddGetAllFxRatesEndpoint(this WebApplication? app, WebApplicationBuilder builder)
    {
        if (app == null) return;        

        var appSettings = builder.Configuration.Get<AppSettings>();        
        app.MapGet("/fxRate", [Authorize] async (SqlConnection connection, HttpResponse response) =>
        {
            var result = new List<FxRate>();
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT currency_code, rate FROM currencies;", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new FxRate()
                {
                    CurrencyCode = reader.GetString(0),
                    Rate = reader.GetString(1),
                });
            }

            return result;
        })
        .Produces(StatusCodes.Status200OK);
    }    
}

