using System.Data.Common;
using kol1.Controllers;
using kol1.Models;
using Microsoft.Data.SqlClient;

namespace kol1.Services;

public class VisitService : IVisitService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    public async Task<VisitDTO?> getVisit(int id)
    {
        VisitDTO? visit = null;

        string command = "select v.date,c.first_name,c.last_name,c.date_of_birth,m.mechanic_id,m.licence_number,s.name,vs.service_fee from Visit v join Client c on v.client_id = c.client_id join Mechanic m on v.mechanic_id = m.mechanic_id join Visit_Service vs on v.visit_id = vs.visit_id join Service s on vs.service_id = s.service_id where v.visit_id = @visitId";
            
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command,conn))
        {
            await conn.OpenAsync();
            
            cmd.Parameters.AddWithValue("@visitId", id);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (visit is null)
                    {
                        visit = new VisitDTO()
                        {
                            date = reader.GetDateTime(0),
                            client = new ClientDTO()
                            {
                                firstName = reader.GetString(1),
                                lastName = reader.GetString(2),
                                dateOfBirth = reader.GetDateTime(3),
                            },
                            mechanic = new MechanicDTO()
                            {
                                mechanicId = reader.GetInt32(4),
                                licenceNumber = reader.GetString(5),
                            },
                            visitServices = new List<ServiceDTO>()
                            {
                                new ServiceDTO()
                                {
                                    name = reader.GetString(6),
                                    serviceFee = (double)reader.GetDecimal(7)
                                }
                            }
                        };
                    }
                    else
                    {
                        visit.visitServices.Add(new ServiceDTO()
                        {
                            name = reader.GetString(6),
                            serviceFee = (double)reader.GetDecimal(7)
                        });
                    }
                }
            }
        }
        Console.Out.WriteLine();
        return visit;
    }

    public async Task<int> PostVisit(PostVisitDTO visitDTO)
    {
        string command1 = "Select count(*) from Visit where visit_id = @visitId";
        string command2 = "Select count(*) from Client where client_id = @clientId";
        string command3 = "Select mechanic_id from Mechanic where licence_number = @licenceNumber";
        int? resCom3;
        string command4 = "Insert into Visit Values (@visitId,@clientId,@mechanicId,@date)";
        string command5 = "Select service_id from Service where name = @serviceName";
        int? resCom5;
        string command6 = "Insert into Visit_Service Values (@visitId,@serviceId,@serviceFee)";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd1 = new SqlCommand(command1, conn))
            {
                cmd1.Parameters.AddWithValue("@visitId", visitDTO.visitId);
                int rowsAffected = await cmd1.ExecuteNonQueryAsync();
                if (rowsAffected > 0) return 1;
            }

            using (SqlCommand cmd2 = new SqlCommand(command2,conn))
            {
                cmd2.Parameters.AddWithValue("@clientId", visitDTO.clientId);
                int rowsAffected = await cmd2.ExecuteNonQueryAsync();
                if (rowsAffected == 0) return 2;
            }

            using (SqlCommand cmd3 = new SqlCommand(command3,conn))
            {
                cmd3.Parameters.AddWithValue("@licenceNumber", visitDTO.mechanicLicenceNumber);
                resCom3 = (int?)await cmd3.ExecuteScalarAsync();
                if (resCom3 is null) return 3;
            }
            
            using (SqlCommand cmd4 = new SqlCommand(command4, conn))
            {
                DbTransaction transaction = await conn.BeginTransactionAsync();
                cmd4.Transaction = transaction as SqlTransaction;
                
                try
                {
                    cmd4.Parameters.AddWithValue("@visitId", visitDTO.visitId);
                    cmd4.Parameters.AddWithValue("@clientId", visitDTO.clientId);
                    cmd4.Parameters.AddWithValue("@mechanicId", resCom3);
                    cmd4.Parameters.AddWithValue("@date",DateTime.Now);

                    await cmd4.ExecuteNonQueryAsync();
                    
                    foreach (var service in visitDTO.services)
                    {
                        using (SqlCommand cmd5 = new SqlCommand(command5, conn))
                        {
                            cmd5.Parameters.AddWithValue("@serviceName", service.serviceName);
                            resCom5 = (int?)await cmd5.ExecuteScalarAsync();
                            if (resCom5 is null) return 4;
                        }
                        
                        using (SqlCommand cmd6 = new SqlCommand(command6, conn))
                        {
                            cmd6.Parameters.AddWithValue("@visitId", visitDTO.visitId);
                            cmd6.Parameters.AddWithValue("@serviceId", resCom5);
                            cmd6.Parameters.AddWithValue("@serviceFee", service.serviceFee);
                            
                            await cmd6.ExecuteNonQueryAsync();
                        }
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return 5;
                }
            }
        }

        return 0;
    }
}