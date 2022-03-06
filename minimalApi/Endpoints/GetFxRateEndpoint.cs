using minimalApi.Configuration;
using minimalApi.Database;
using System.Data;
using System.Data.Common;

namespace minimalApi.Endpoints;

public static class GetFxRateEndpoint
{

    public static void AddGetAllFxRatesEndpoint(this WebApplication? app, WebApplicationBuilder builder)
    {
        if (app == null) return;        

        var appSettings = builder.Configuration.Get<AppSettings>();
        app.MapGet("/fxRate", [Authorize] async (DbProviderFactory dbProviderFactory, HttpResponse response) =>
        {
            using var db = dbProviderFactory.CreateConnection();
            db.ConnectionString = appSettings.ConnectionString;
            await db.OpenAsync();

            using var cmd = CreateReadCommand(db);

            var result = ReadResult(cmd);
            await response.WriteAsJsonAsync(result);
            return;
        })
        .Produces(StatusCodes.Status200OK);
    }    

    static DbCommand CreateReadCommand(DbConnection connection)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT currency_code, rate FROM v_currencies";

        (cmd as MySqlConnector.MySqlCommand)?.Prepare();

        return cmd;
    }

    static async Task<List<FxRate>> ReadResult(DbCommand cmd)
    {
        var result = new List<FxRate>();

        using (var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
        {
            await rdr.ReadAsync();
            while (await rdr.ReadAsync())
            {
                result.Add(new FxRate()
                {
                    CurrencyCode = rdr.GetString(0),
                    Rate = rdr.GetString(1),
                });
            }
        }
        return result;
    }
}

