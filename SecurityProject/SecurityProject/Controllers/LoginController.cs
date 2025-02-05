using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecurityProject.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SecurityProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        // GET: api/<LoginController>
        [HttpGet("/admin")]
        [Authorize(Roles ="administartor")]
        public IActionResult Get()
        {
            var currentUser = GetCurrentUser();
            return Ok($"Hi {currentUser.UserName} i am { currentUser.Role} ");
        }
        [HttpGet("/seller")]
        [Authorize(Roles = "seller")]
        public IActionResult GetSeller()
        {
            var currentUser = GetCurrentUser();
            return Ok($"Hi {currentUser.UserName} i am { currentUser.Role} ");
        }
        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var UserClaim = identity.Claims;
                return new UserModel()
                {
                    UserName = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Email = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    Role = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                    GivenName = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value,
                    SurName = UserClaim.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value
                };

            }
            return null;
        }
        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost]
        public IActionResult Post([FromBody]UserLogin login)
        {
            //אימות
            var user = Authenticate(login.UserName, login.Password);
            if (user != null)
            {
                //יצירת טוקן
                var token = Generate(user);
                return Ok(token);
            }
            return BadRequest("user not found");
        }

        private string Generate(UserModel user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier,user.UserName),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.Surname,user.SurName),
            new Claim(ClaimTypes.Role,user.Role),
            new Claim(ClaimTypes.GivenName,user.GivenName)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel Authenticate(string name, string password)
        {
          var user=  UserContacts.Db.FirstOrDefault(x => x.UserName == name && x.Password == password);
            if (user != null)
                return user;
            return null;

        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
