using kol1.Controllers;
using kol1.Models;
using Microsoft.AspNetCore.Mvc;

namespace kol1.Services;

public interface IVisitService
{
    public Task<VisitDTO?> getVisit(int id);
    
    public Task<int> PostVisit(PostVisitDTO visitDTO);
}