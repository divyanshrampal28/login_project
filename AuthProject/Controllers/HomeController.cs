using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthProject.Models;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace AuthProject.Controllers
{
    public class HomeController : Controller
    {
        private SampleDbContext dbContext;

        public HomeController(SampleDbContext db_context)
        {
            dbContext = db_context;
        }

        private string CreatePasswordHash(string password)
        {
            
            return BCrypt.Net.BCrypt.HashPassword(password); ;

        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword);
        }


        [HttpPost]
        public IActionResult Register(UserTable req)
        {
            if (dbContext.UserTable.Any(u => u.Email == req.Email))
            {
                return BadRequest("this user already exists");
            }

            if (req.Password.Length < 8)
            {
                return BadRequest("password less than 8 letters");
            }

            String encryptedPassword = CreatePasswordHash(req.Password);

            var user = new UserTable
            {
                Email = req.Email,
                FName = req.FName,
                LName = req.LName,
                Password = encryptedPassword,
                IsAdmin = false
            };

            dbContext.UserTable.Add(user);
            dbContext.SaveChanges();

            return RedirectToAction("Login", "Home");


        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserTable req)
        {
            var user = dbContext.UserTable.FirstOrDefault(d => d.Email == req.Email);
            if (user == null)
            {
                return BadRequest("Invalid Email Address");
            }

            string storedPassword = user.Password;

            string enteredPassword = req.Password;

            bool isPasswordValid = VerifyPassword(enteredPassword, storedPassword);

            if (isPasswordValid == false)
            {
                return BadRequest("Incorrect Password");
            }

            
            var securityKey = "This_is_our_security_key";
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email) 
            };

            var token = new JwtSecurityToken(
                issuer: "smesk.in",
                audience: "readers",
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials,
                claims: claims
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            HttpContext.Session.SetString("JWTToken", tokenString);

            //return Ok(tokenString);
            return RedirectToAction("", "Home");

         }


        public IActionResult Index()
        {

            var tokenString = HttpContext.Session.GetString("JWTToken");

            if (!string.IsNullOrEmpty(tokenString))
            {
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(tokenString);

                
                var emailClaim = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

                var user = dbContext.UserTable.FirstOrDefault(u => u.Email == emailClaim.Value);

                
                ViewBag.FName = user.FName;

            }

            return View();
            
        }

        public IActionResult Register()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
