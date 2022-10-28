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
using NuGet.Common;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;

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
            if (!Check_JWT_table())
            {
                return BadRequest();
            }
            return Ok($"Ваш логин: {User.Identity.Name}");
        }
        [Authorize]
        [Route("getrole")]
        public IActionResult GetRole()
        {
            if (!Check_JWT_table())
            {
                return BadRequest();
            }
            Users person = _db.Users.FirstOrDefault(x => x.Name == User.Identity.Name);

            return Ok("Ваша роль: "+Convert.ToString(person.Role));
        }
        [Authorize]
        [Route("getsession")]
        public IActionResult GetSessin()
        {
            if (!Check_JWT_table())
            {
                return BadRequest();
            }
            if (HttpContext.Session.GetString(User.Identity.Name) is not null)
            {

                return Ok(Convert.ToString(User.Identity.Name));
            }
            return BadRequest();
        }
        [Authorize]
        [Route("deletesession")]
        public IActionResult deletesession()
        {
           
            var token = Convert.ToString(Request.Headers["Authorization"]);
            token=token.Remove(0, 7);
            JWT_A_T jwt =_db.JWT_A_T.Find(token);
            _db.JWT_A_T.Remove(jwt);
            _db.SaveChanges(); 
            return Ok();
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
        public IActionResult admin()
        {
            if (!Check_JWT_table())
            {
                return BadRequest();
            }
            Users person = _db.Users.FirstOrDefault(x => x.Name == User.Identity.Name);
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
            Users person = _db.Users.FirstOrDefault(x => x.Name == User.Identity.Name);

            if (person.Role == "admin")
            {
                return await _db.Users.ToListAsync();
            }
            else
            {
                throw new Exception("No root");
            }
           
        }
        [Authorize]
        private bool Check_JWT_table()
        {
            var token = Convert.ToString(Request.Headers["Authorization"]);
            token = token.Remove(0, 7);
            JWT_A_T jwt = _db.JWT_A_T.Find(token);
            if(jwt is not null)
            {
                return true;
            }
            return false;
        } 

     

    }
}