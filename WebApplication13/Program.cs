using Microsoft.EntityFrameworkCore;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TokenApp;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder();

string connection = "Server=(localdb)\\mssqllocaldb;Database=SecondTestDB;Trusted_Connection=True;";

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MyApp.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.IsEssential = true;
}); 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // ��������, ����� �� �������������� �������� ��� ��������� ������
                            ValidateIssuer = true,
                            // ������, �������������� ��������
                            ValidIssuer = AuthOptions.ISSUER,

                            // ����� �� �������������� ����������� ������
                            ValidateAudience = true,
                            // ��������� ����������� ������
                            ValidAudience = AuthOptions.AUDIENCE,
                            // ����� �� �������������� ����� �������������
                            ValidateLifetime = true,

                            // ��������� ����� ������������
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            // ��������� ����� ������������
                            ValidateIssuerSigningKey = true,
                        };
                    });
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptchaV3"));
var app = builder.Build();
app.MapRazorPages();
app.UseDeveloperExceptionPage();

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.MapControllers();
app.Run();