using kol1.Models;
using kol1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kol1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitsController : ControllerBase
    {
        private readonly IVisitService _visitsService;

        public VisitsController(IVisitService visitsService)
        {
            _visitsService = visitsService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisit(int id)
        {
            var visit = await _visitsService.getVisit(id);
            if (visit is null) return NotFound();
            return Ok(visit);
        }

        [HttpPost]
        public async Task<IActionResult> PostVisit([FromBody] PostVisitDTO postVisitDTO)
        {
            var res = await _visitsService.PostVisit(postVisitDTO);
            if (res == 0) return Created();
            if (res == 1) return BadRequest("Istnieje wizyta o podanym indentyfikatorze");
            if (res == 2) return BadRequest("Nie istnieje klient o podanym indentyfikatorze");
            if (res == 3) return BadRequest("Nie istnieje mechanik o podanym numerze licencji");
            if (res == 4) return BadRequest("Nie istnieje serwis o podanej nazwie");
            return Conflict();
        }
    }
}
