using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection; 

namespace BentoLab.Controllers
{
    public class HomeController : Controller
    {
        // VRAĆAMO STARI KONSTRUKTOR (Bez parametara) da sistem ne baca grešku pri pokretanju
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        //public IActionResult AdminPanel()
        //{
        //    // Ručno izvlačimo UserManager iz servisa na siguran način koji ne ruši aplikaciju
        //    var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();

        //    // Povlačimo korisnike i sortiramo ih hronološki
        //    var users = userManager.Users.ToList().OrderByDescending(u => u.Id).ToList();

        //    return View(users);
        //}

        //ukloniti ovaj komentar kada spojimo i zakomentirati lažne korisnike ili gornji kod
        // [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        { 
            var lažniKorisnici = new List<BentoLab.Models.Korisnik>
    {
        new BentoLab.Models.Korisnik { Id = 3, UserName = "ilma.zubovic", Email = "ilma@bentolab.com" },
        new BentoLab.Models.Korisnik { Id = 2, UserName = "amina.begic", Email = "amina@bentolab.com" },
        new BentoLab.Models.Korisnik { Id = 1, UserName = "test.korisnik", Email = "test@gmail.com" }
    };

            return View(lažniKorisnici);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}