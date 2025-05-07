using kol1.Models;

namespace kol1.Controllers;

public class PostVisitDTO
{
    public int visitId { get; set; }
    public int clientId { get; set; }
    public string mechanicLicenceNumber { get; set; }
    public List<PostServiceDTO> services { get; set; }
}