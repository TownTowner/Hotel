using Hotel.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }

        [HttpGet]
        [Route("Reservations")]
        public IActionResult GetReservations()
        {
            var reservations = _userService.GetReservations(User.Identity.Name);
            return Ok(reservations);
        }
    }
}
