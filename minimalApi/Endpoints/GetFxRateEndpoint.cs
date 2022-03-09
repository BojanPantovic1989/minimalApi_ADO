using Dapper;
using minimalApi.Configuration;
using minimalApi.Database;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace minimalApi.Endpoints;


public class DapperContext
{
    private readonly AppSettings appSettings;

    public DapperContext(AppSettings appSettings)
    {
        this.appSettings = appSettings;
    }
    public IDbConnection GetConnection() => new MySqlConnection(appSettings.ConnectionString);
}

public static class GetFxRateEndpoint
{

    public static void AddGetAllFxRatesEndpoint(this WebApplication? app)
    {
        if (app == null) return;        

        app.MapGet("/fxRate", [Authorize] async ([FromServices]DapperContext context, HttpResponse response,CancellationToken cancellationToken) =>
        {
            using var conn = context.GetConnection();
            var result = await conn.QueryAsync<FxRate>(new CommandDefinition(
                "SELECT currency_code, rate FROM v_currencies",
                cancellationToken: cancellationToken
                ));
            return result;
        })
        .Produces(StatusCodes.Status200OK);
    }    

}

