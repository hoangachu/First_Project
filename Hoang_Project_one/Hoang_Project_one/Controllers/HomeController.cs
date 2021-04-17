
using Hoang_Project_one.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Hoang_Project_one.Models.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Hoang_Project_one.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            return Redirect("/Home/Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            string qrcode = "Lan Bò Bề";
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(qRCodeData);
                using (Bitmap bitmap = qRCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            int i = 0;
            User user = new User();
            using (SqlConnection con = new SqlConnection(Startup.connectionString))
            {

                if (ModelState.IsValid)
                {
                    try
                    {
                        user = new UserController().GetUserByUserName(Username);
                        var password = "";
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            password = Multis.Multis.Decrypt(user.Password).ToLower();
                        }

                        if (password.ToLower().Equals(Password.ToLower()))
                        {
                            i = 1;
                        }
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim("Name",Username),
                            new Claim("Password",password)
                        };
                        // Create identity
                        ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");
                        //create principle
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                        //sign-in
                        await HttpContext.SignInAsync(
                           scheme: "DemoSecurityScheme",
                           principal: claimsPrincipal,
                           properties: new AuthenticationProperties()
                           {
                               IsPersistent = true,
                               ExpiresUtc = DateTime.UtcNow.AddMinutes(100)

                           }

                       ); ;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }

                return Ok(new { data = i, url = "/ChatBot/ChatPreview?UserID=" + user.UserID + "&&ReceiverID=0" });
            }

        }

    }
}

