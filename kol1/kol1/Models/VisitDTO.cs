using kol1.Controllers;

namespace kol1.Models;

public class VisitDTO
{
    public DateTime date { get; set; }
    public ClientDTO client {get; set;}
    public MechanicDTO mechanic {get; set;}
    public List<ServiceDTO> visitServices {get; set;}
}

public class ClientDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class MechanicDTO
{
    public int mechanicId { get; set; }
    public string licenceNumber { get; set; }
}
