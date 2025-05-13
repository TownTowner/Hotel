using Hotel.Core;
using Hotel.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Hotel.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation($"{DateTime.UtcNow}, User Get - this is a nice message to test the logs");
            var users = _userService.GetUsers();
            return Ok(CommonResponse.Create(users));
        }

        [HttpGet]
        [Route("Reservations")]
        public IActionResult GetReservations()
        {
            _logger.LogInformation($"{DateTime.UtcNow}, User GetReservations - this is a nice message to test the logs");
            var user = HttpContext.User;
            var reservations = _userService.GetReservations();
            return Ok(CommonResponse.Create(reservations));
        }

        [HttpGet]
        [Route("test")]
        [AllowAnonymous]
        public IActionResult GetTest()
        {
            return Ok(new { now = DateTime.Now.ToString() });
        }
    }
}
