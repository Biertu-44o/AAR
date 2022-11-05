using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using static System.Collections.Specialized.BitVector32;

namespace TokenApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ApplicationContext _db;

        public ValuesController(ApplicationContext db)
        {
            _db = db;
        }
        [Authorize]
        [Route("getlogin")]
        public IActionResult GetLogin()
        {
            return Ok($"Ваш логин: {User.Identity.Name}");
        }
        [Authorize]
        [Route("getrole")]
        public async Task<IActionResult> GetRole()
        {
            Users person = await _db.Users.FirstOrDefaultAsync(x => x.Name == User.Identity.Name);

            return Ok("Ваша роль: "+Convert.ToString(person.Role));
        }
        [Authorize]
        [Route("getsession")]
        public IActionResult GetSessin()
        {
            if (HttpContext.Session.GetString(User.Identity.Name) is not null)
            {

                return Ok(Convert.ToString(User.Identity.Name));
            }
            return BadRequest();
        }
        [Authorize]
        [Route("createsession")]
        public IActionResult CreateSessin()
        { 
            HttpContext.Session.Set<string>(Convert.ToString(User.Identity.Name), User.Identity.Name);
            return Ok();
        }
        [Authorize]
        [Route("admin")]
        public async Task<IActionResult> admin()
        {
            Users person = await _db.Users.FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
            try
            {
                if (person.Role == "admin")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch { throw new Exception("No root"); } 
        }
        [Authorize]
        [HttpGet("table")]
        public async Task<List<Users>> table()
        {
            Users person = await _db.Users.FirstOrDefaultAsync(x => x.Name == User.Identity.Name);

            if (person.Role == "admin")
            {
                return await _db.Users.ToListAsync();
            }
            else
            {
                throw new Exception("No root");
            }
           
        }



     

    }
}