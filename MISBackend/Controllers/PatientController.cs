using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISBackend.Middleware;
using System.ComponentModel;
using static MISBackend.DAL.Enum.DataEnum;

namespace MISBackend.Controllers
{
    [Route("api/patient")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly BLL.Services.RepMisBack _repMisBack;
        private readonly ILogger<DoctorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly JwtTokenLifetimeManager _tokenLifetimeManager;

        public PatientController(BLL.Services.RepMisBack repMisBack,
                                ILogger<DoctorController> logger,
                                IConfiguration configuration,
                                JwtTokenLifetimeManager tokenLifetimeManager)
        {
            _repMisBack = repMisBack;
            _logger = logger;
            _configuration = configuration;
            _tokenLifetimeManager = tokenLifetimeManager;
        }

        [HttpGet]
        public IActionResult GetList(
            string Name,
            [FromQuery, DefaultValue(new string[] { "Disease", "Recovery", "Death" })] string[] Conclusions, 
            PatientSorting Sorting, 
            bool ScheduledVisits,
            bool OnlyMine,
            int Page,
            int Size)
        {
            return Ok();
        }
    }
}
