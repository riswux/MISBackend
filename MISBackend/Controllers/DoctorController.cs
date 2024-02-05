using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MISBackend.Helpers;
using MISBackend.Middleware;
using Newtonsoft.Json.Linq;
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
        private readonly JwtTokenLifetimeManager _tokenLifetimeManager;

        public DoctorController(Repository.RepMisBack repMisBack,
                                ILogger<DoctorController> logger,
                                IConfiguration configuration,
                                JwtTokenLifetimeManager tokenLifetimeManager)
        {
            _repMisBack = repMisBack;
            _logger = logger;
            _configuration = configuration;
            _tokenLifetimeManager = tokenLifetimeManager;
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

            if (!Validation.IsValidEmail(doctor.Email))
            {
                _logger.LogCritical("Invalid Email",
                                    new { EndPoint = "Register", Data = doctor });
                return BadRequest(new { Errors = "Invalid Email" });
            }

            var saveData = _repMisBack.Register(doctor);
            if (saveData.Item1 == 200)
            {
                var token = Middleware.JWT.GenerateToken(_configuration, (saveData.Item3 == null ? Guid.NewGuid() : saveData.Item3.Id));
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

        [HttpPost("login", Name = "login")]
        [SwaggerResponse(200, "OK", typeof(MISBackend.Model.Response.TokenResponseModel))]
        [SwaggerResponse(400, "Invalid arguments")]
        [SwaggerResponse(500, "Internal Server Error", typeof(MISBackend.Model.Response.ResponseModel))]
        public IActionResult Login([FromBody] Model.Payload.LoginCredentialModel doctor)
        {
            if (doctor == null)
            {
                _logger.LogCritical("Invalid JSON data in the request body.", new { EndPoint = "Login", Data = doctor });
                return BadRequest(new { Error = "Invalid JSON data in the request body." });
            }

            // Melakukan validasi menggunakan ModelState
            var validationContext = new ValidationContext(doctor, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(doctor, validationContext, validationResults, validateAllProperties: true))
            {
                _logger.LogCritical(string.Join(',', validationResults.Select(v => v.ErrorMessage)),
                                    new { EndPoint = "Login", Data = doctor });
                return BadRequest(new { Errors = validationResults.Select(v => v.ErrorMessage) });
            }

            var saveData = _repMisBack.Login(doctor);
            if (saveData.Item1 == 200)
            {
                var token = Middleware.JWT.GenerateToken(_configuration, (saveData.Item3 == null ? Guid.NewGuid() : saveData.Item3.Id));
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

        [HttpPost("logout", Name = "logout")]
        [SwaggerResponse(200, "OK", typeof(MISBackend.Model.Response.ResponseModel))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal Server Error", typeof(MISBackend.Model.Response.ResponseModel))]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                //[FromHeader(Name = "Authorization")] string authorization
                Request.Headers.TryGetValue("Authorization", out var authorization);
                if (string.IsNullOrWhiteSpace(authorization)) return Ok();

                string bearerToken =
                    authorization.ToString().Replace("Bearer ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

                _tokenLifetimeManager.SignOut(new JwtSecurityToken(bearerToken));

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new MISBackend.Model.Response.ResponseModel { Status = ex.Message, Message = ex.Message });
            }
        }

        [HttpGet("profile", Name = "profile")]
        [SwaggerResponse(200, "OK", typeof(MISBackend.Model.Response.DoctorModel))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Not Found")]
        [SwaggerResponse(500, "Internal Server Error", typeof(MISBackend.Model.Response.ResponseModel))]
        [Authorize]
        public IActionResult Profile()
        {
            try
            {
                //[FromHeader(Name = "Authorization")] string authorization
                Request.Headers.TryGetValue("Authorization", out var authorization);
                if (string.IsNullOrWhiteSpace(authorization)) return Ok();

                string bearerToken =
                    authorization.ToString().Replace("Bearer ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

                // Mendekode token JWT
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(bearerToken) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    // Mendapatkan klaim (claim) dari token
                    var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "sub");

                    if (userIdClaim != null)
                    {
                        // Mendapatkan nilai klaim
                        Guid userId = Guid.Parse(userIdClaim.Value);

                        var data = _repMisBack.GetDoctor(userId);
                        if (data.Item1 == 200)
                        {
                            return Ok(data.Item3);
                        }
                        else if (data.Item1 == 400)
                        {
                            return NotFound();
                        }
                        else
                        {
                            return StatusCode(500, new MISBackend.Model.Response.ResponseModel { Status = data.Item2, Message = data.Item2 });
                        }
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new MISBackend.Model.Response.ResponseModel { Status = ex.Message, Message = ex.Message });
            }
        }

        [HttpPut("profile", Name = "profile")]
        [SwaggerResponse(200, "OK", typeof(MISBackend.Model.Response.DoctorModel))]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(404, "Not Found")]
        [SwaggerResponse(500, "Internal Server Error", typeof(MISBackend.Model.Response.ResponseModel))]
        [Authorize]
        public IActionResult Profile([FromBody] MISBackend.Model.Payload.DoctorEditModel doctor)
        {
            try
            {
                //[FromHeader(Name = "Authorization")] string authorization
                Request.Headers.TryGetValue("Authorization", out var authorization);
                if (string.IsNullOrWhiteSpace(authorization)) return Ok();

                string bearerToken =
                    authorization.ToString().Replace("Bearer ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

                // Mendekode token JWT
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(bearerToken) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    // Mendapatkan klaim (claim) dari token
                    var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "sub");

                    if (userIdClaim != null)
                    {
                        // Mendapatkan nilai klaim
                        Guid userId = Guid.Parse(userIdClaim.Value);

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

                        if (!Validation.IsValidEmail(doctor.Email))
                        {
                            _logger.LogCritical("Invalid Email",
                                                new { EndPoint = "Register", Data = doctor });
                            return BadRequest(new { Errors = "Invalid Email" });
                        }

                        var data = _repMisBack.Edit(userId, doctor);
                        if (data.Item1 == 200)
                        {
                            return Ok(data.Item3);
                        }
                        else if (data.Item1 == 400)
                        {
                            return NotFound();
                        }
                        else
                        {
                            return StatusCode(500, new MISBackend.Model.Response.ResponseModel { Status = data.Item2, Message = data.Item2 });
                        }
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new MISBackend.Model.Response.ResponseModel { Status = ex.Message, Message = ex.Message });
            }
        }
    }
}
