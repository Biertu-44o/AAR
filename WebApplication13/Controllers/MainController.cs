
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

//Перехватчик глоб. ошибок
[ApiController]
[Route("api/[controller]")]
public class MainController : ControllerBase
{
    #region Base content
    private readonly ApplicationContext _db;

    public MainController(ApplicationContext db)
    {
        _db = db;
    }
    #endregion

    #region Unsafe Registration
    [HttpPost("unregistration")]
    public async Task<Users> UnRegistration(Users user)
    {
        try
        {
            var ifExist = await _db.Users.SingleOrDefaultAsync(x => x.Name == user.Name);
            if (ifExist is not null || user.Password is null)
            {
                throw new Exception("Ошибка");
            }

            string reg = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
            if (!Regex.IsMatch(user.Password, reg))
            {
                throw new Exception("Ошибка");
            }
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }
        catch
        {
            throw new Exception("Упс, что-то пошло не так");
        }
    }
    #endregion
}
