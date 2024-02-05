using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MISBackend.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly Repository.RepMisBack _repMisBack;
        private readonly ILogger<DoctorController> _logger;
        private readonly IConfiguration _configuration;

        public DoctorController(Repository.RepMisBack repMisBack,
                                ILogger<DoctorController> logger,
                                IConfiguration configuration)
        {
            _repMisBack = repMisBack;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("register", Name = "register")]
        [SwaggerResponse(200, "OK", typeof(MISBackend.Model.Response.TokenResponseModel))]
        [SwaggerResponse(400, "Invalid arguments")]
        [SwaggerResponse(500, "Internal Server Error", typeof(MISBackend.Model.Response.ResponseModel))]
        public IActionResult Register([FromBody] Model.Payload.DoctorRegisterModel doctor)
        {
            if (doctor == null)
            {
                _logger.LogCritical("Invalid JSON data in the request body.", new { EndPoint = "Register", Data = doctor });
                return BadRequest(new { Error = "Invalid JSON data in the request body." });
            }

            // Melakukan validasi menggunakan ModelState
            var validationContext = new ValidationContext(doctor, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(doctor, validationContext, validationResults, validateAllProperties: true))
            {
                _logger.LogCritical(string.Join(',', validationResults.Select(v => v.ErrorMessage)),
                                    new { EndPoint = "Register", Data = doctor });
                return BadRequest(new { Errors = validationResults.Select(v => v.ErrorMessage) });
            }

            var saveData = _repMisBack.Register(doctor);
            if (saveData.Item1 == 200)
            {
                var token = Middleware.JWT.GenerateToken(_configuration);
                return Ok(new MISBackend.Model.Response.TokenResponseModel { Token = token });
            }
            else if (saveData.Item1 == 500)
            {
                return StatusCode(500, new MISBackend.Model.Response.ResponseModel { Status = saveData.Item1.ToString(), Message = saveData.Item2 });
            }
            else 
            {
                return BadRequest(new { Errors = saveData.Item2 });
            }
        }
    }
}
