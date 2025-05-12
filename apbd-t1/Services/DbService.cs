using apbd_t1.Exceptions;
using apbd_t1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd_t1.Services;

public class DbService : IDbService
{
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";
    
    public async Task<VisitResponseDto> GetVisitById(int id)
    {
        VisitResponseDto? visit = null;

        await using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        // Check that visit exists
        string command = "SELECT COUNT(*) FROM Visit WHERE visit_id = @visit_id";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@visit_id", id);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (reader.GetInt32(0) == 0) throw new NotFoundException("Visit not found");
                }
            }
        }
        
        // Fetch visit data
        command = @"
            SELECT V.visit_id, V.date, C.first_name, C.last_name, C.date_of_birth, M.mechanic_id, M.licence_number, S.name, J.service_fee
            FROM Visit V
            JOIN Client C ON V.client_id = C.client_id
            JOIN Mechanic M ON V.mechanic_id = M.mechanic_id
            JOIN Visit_Service J ON V.visit_id = J.visit_id
            JOIN Service S ON J.service_id = S.service_id
            WHERE V.visit_id = @visit_id";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@visit_id", id);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idTemp = reader.GetInt32(0);
                    if (visit == null)
                    {
                        visit = new VisitResponseDto()
                        {
                            Date = reader.GetDateTime(1),
                            Client = new VisitResponseClientDto()
                            {
                                FirstName = reader.GetString(2),
                                LastName = reader.GetString(3),
                                DateOfBirth = reader.GetDateTime(4),
                            },
                            Mechanic = new VisitResponseMechanicDto(){
                                MechanicId = reader.GetInt32(5),
                                LicenceNumber = reader.GetString(6),
                            },
                        };
                    }
                    visit.VisitServices.Add(new VisitResponseServiceDto()
                    {
                        Name = reader.GetString(7),
                        ServiceFee = reader.GetDecimal(8),
                    });
                }
            }
        }
        
        return visit;
    }

    public async Task AddVisitByRequestDto(VisitRequestDto requestDto)
    {
        int mechanicId = 0;
        
        await using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        // Check validity
        if (requestDto.VisitId < 1) throw new BadRequestException("Invalid VisitId");
        if (requestDto.ClientId < 1) throw new BadRequestException("Invalid ClientId");
        
        // Check that visit does not exist
        string command = "SELECT COUNT(*) FROM Visit WHERE visit_id = @visit_id";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@visit_id", requestDto.VisitId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (reader.GetInt32(0) != 0) throw new ConflictException("Visit already exists");
                }
            }
        }
        
        // Check that client exists
        command = "SELECT COUNT(*) FROM Client WHERE client_id = @client_id";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@client_id", requestDto.ClientId);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (reader.GetInt32(0) == 0) throw new NotFoundException("Client not found");
                }
            }
        }
        
        // Check that mechanic exists
        command = "SELECT COUNT(*) FROM Mechanic WHERE licence_number = @licence_number";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@licence_number", requestDto.MechanicLicenceNumber);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (reader.GetInt32(0) == 0) throw new NotFoundException("Mechanic not found");
                }
            }
        }
        
        // Check that services exist
        foreach (var service in requestDto.Services)
        {
            command = "SELECT COUNT(*) FROM Service WHERE name = @name";
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@name", service.ServiceName);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(0) == 0) throw new NotFoundException("Service not found");
                    }
                }
            }
        }
        
        // Get Mechanic.mechanic_id -> mechanicId
        command = "SELECT mechanic_id FROM Mechanic WHERE licence_number = @licence_number";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@licence_number", requestDto.MechanicLicenceNumber);
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    mechanicId = reader.GetInt32(0);
                }
            }
        } 
        
        // Insert Visit
        command = "INSERT INTO Visit (visit_id, client_id, mechanic_id) VALUES (@visit_id, @client_id, @mechanic_id)";
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@visit_id", requestDto.VisitId);
            cmd.Parameters.AddWithValue("@client_id", requestDto.ClientId);
            cmd.Parameters.AddWithValue("@mechanic_id", mechanicId);
            await cmd.ExecuteNonQueryAsync();
        }
        
        // Insert Visit_Services
        foreach (var service in requestDto.Services)
        {
            int serviceId = 0;
            
            command = "SELECT service_id FROM Service WHERE name = @name";
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@name", service.ServiceName);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        serviceId = reader.GetInt32(0);
                    }
                }
            }
            
            command = "INSERT INTO Visit_Service (visit_id, service_id, service_fee) VALUES (@visit_id, @service_id, @service_fee)";
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@visit_id", requestDto.VisitId);
                cmd.Parameters.AddWithValue("@service_id", serviceId);
                cmd.Parameters.AddWithValue("@service_fee", service.ServiceFee);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(0) == 0) throw new NotFoundException("Service not found");
                    }
                }
            }
        }
    }
}