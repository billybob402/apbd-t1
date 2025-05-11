using apbd_t1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd_t1.Services;

public class TempService : ITempService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;";
    //private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";
    //private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";

    public async Task<List<TempDTO>> GetTemps()
    {
        var tempsDict = new Dictionary<int, TempDTO>();

        await using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        string command = "SELECT * FROM Temps";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idTemp = reader.GetInt32(0);
                    if (!tempsDict.TryGetValue(idTemp, out TempDTO temp))
                    {
                        temp = new TempDTO()
                        {
                            IdTemp = idTemp,
                            A = reader.GetString(1),
                        };
                        tempsDict[idTemp] = temp;
                    }
                    temp.Things.Add(new ThingDTO()
                    {
                        IdThing = reader.GetInt32(2),
                        B = reader.GetString(3),
                    });
                }
            }
        }
        
        return tempsDict.Values.ToList();
    }
}