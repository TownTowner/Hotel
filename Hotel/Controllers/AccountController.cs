using Hotel.Configuration;
using Hotel.Core;
using Hotel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        //private readonly UserManager<IdentityUser> _userManager;
        private readonly UserService _userService;
        private readonly JwtConfig _jwtConfig;

        public AccountController(UserService userService, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userService = userService;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration user)
        {
            // 检查传入请求是否有效
            if (ModelState.IsValid)
            {
                var existingUser = _userService.FindByEmail(user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() { "Email already in use" },
                        Success = false
                    });
                }

                var newUser = new User() { Email = user.Email, Name = user.Username, CreatedTime = DateTime.Now };
                //var isCreated = _userService.CreateUser(newUser, user.Password);
                var uId = await _userService.CreateUserAsync(newUser, user.Password);
                if (uId > 0)
                {
                    //var jwtToken = GenerateJwtToken(newUser);
                    var jwtToken = BuildToken(newUser);

                    return Ok(new RegistrationResponse()
                    {
                        Success = true,
                        Token = jwtToken
                    });
                }
                else
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string> { "Register error" },
                        Success = false
                    });
                }
            }

            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>() { "Invalid payload" },
                Success = false
            });
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] UserLogin user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _userService.FindByEmail(user.Email);
                if (existingUser == null)
                {
                    // 出于安全原因，我们不想透露太多关于请求失败的信息
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() { "Invalid login request" },
                        Success = false
                    });
                }

                // 现在我们需要检查用户是否输入了正确的密码
                //var isCorrect = await _userService.CheckPasswordAsync(existingUser, user.Password);
                var isCorrect = existingUser.Password == user.Password;
                if (!isCorrect)
                {
                    // 出于安全原因，我们不想透露太多关于请求失败的信息
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() { "Invalid login request" },
                        Success = false
                    });
                }

                //var jwtToken = GenerateJwtToken(existingUser);
                var jwtToken = BuildToken(existingUser);

                return Ok(new RegistrationResponse()
                {
                    Success = true,
                    Token = jwtToken
                });
            }

            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>() { "Invalid payload" },
                Success = false
            });
        }


        public string BuildToken(User user)
        {
            TimeSpan ExpiryDuration = TimeSpan.FromSeconds(_jwtConfig.ExpireSeconds);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Name??user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var tokenDescriptor = new JwtSecurityToken(claims: claims, expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            //现在，是时候定义 jwt token 了，它将负责创建我们的 tokens
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // 从 appsettings 中获得我们的 secret 
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            // 定义我们的 token descriptor
            // 我们需要使用 claims （token 中的属性）给出关于 token 的信息，它们属于特定的用户，
            // 因此，可以包含用户的 Id、名字、邮箱等。
            // 好消息是，这些信息由我们的服务器和 Identity framework 生成，它们是有效且可信的。
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                // Jti 用于刷新 token，我们将在下一篇中讲到
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                // token 的过期时间需要缩短，并利用 refresh token 来保持用户的登录状态，
                // 不过由于这只是一个演示应用，我们可以对其进行延长以适应我们当前的需求
                Expires = DateTime.UtcNow.AddHours(6),
                // 这里我们添加了加密算法信息，用于加密我们的 token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
