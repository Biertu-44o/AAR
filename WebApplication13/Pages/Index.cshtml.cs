using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace WebApplication13.Pages
{
    [CustomExceptionFilterAttribute]
    [ValidateReCaptcha("index")]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Login { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        private readonly ApplicationContext _db;


        public IndexModel(ApplicationContext db)
        {
            _db = db;
        }

        public async Task OnPost()
        { 
                if (!ModelState.IsValid)
                {
                    throw new Exception ();
                }

                Users user = new Users();
                user.Name = this.Login;
                user.Password = this.Password;
                user.Second = this.Name;

               

                var ifExist = await _db.Users.SingleOrDefaultAsync(x => x.Name == user.Name);
                if (ifExist is not null || user.Password is null)
                {
                    throw new Exception("incorrect data entered");
                }

                string reg = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
                string log = @"[\w-[\d]]\w{1,9}";
                if (!Regex.IsMatch(user.Password, reg) || !Regex.IsMatch(user.Name, log))
                {
                    throw new Exception("incorrect data entered");
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
            
            
        }
    }
}
