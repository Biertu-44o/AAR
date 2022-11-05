using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TokenApp.Controllers
{

    public class AccountController : Controller
    {
        private readonly ApplicationContext _db;

        public AccountController(ApplicationContext db)
        {
            _db = db;
        }
        [Authorize]
        [HttpPost("/del")]
        public async Task<IActionResult> del(string username, string password)
        {
            try
            {
                Users person = await _db.Users.SingleOrDefaultAsync(x => x.Name == User.Identity.Name);

                if (person.Role == "admin")
                {
                    Users user = await _db.Users.SingleOrDefaultAsync(x => x.Name == username);
                    if (user == null) throw new Exception("Пользователь не найден");

                    // если пользователь найден, удаляем его
                    _db.Users.Remove(user);
                    await _db.SaveChangesAsync();
                    return Json(user);
                }
                throw new Exception("No root");
            }

            catch
            {
                throw new Exception("No root");
            }
        }

        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }
        private ClaimsIdentity GetIdentity(string username, string password)
        {
           
            Users person = _db.Users.FirstOrDefault(x => x.Name == username);
            if (person is null)
            {
                return null;
            }
            Users CheckPassword=new Users {Password = password};
           
            if (!BCrypt.Net.BCrypt.Verify(CheckPassword.Password,person.Password))
            {
                return null;
            }
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Name)
                    // роль удалена из токена
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }



            // если пользователя не найдено
            return null;
        }
    }
}