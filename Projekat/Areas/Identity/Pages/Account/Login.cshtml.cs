using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BentoLab.Models;

namespace BentoLab.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly UserManager<Korisnik> _userManager;

        public LoginModel(SignInManager<Korisnik> signInManager, UserManager<Korisnik> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email je obavezan.")]
            [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Lozinka je obavezna.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Provjeravamo da li korisnik uopšte postoji u bazi
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Korisnik sa ovim emailom ne postoji.");
                    return Page();
                }

                // Provjeravamo da li je potvrdio mail
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Račun nije verifikovan. Molimo potvrdite vaš email.");
                    return Page();
                }

                // Pokušaj prijave
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                { 

                    // 1. ime u 'adminKorisnik' da se ne sudara sa postojećim 'user'
                    var adminKorisnik = await _userManager.FindByEmailAsync(Input.Email);

                    // 2. Provjera uloge koristeći novo ime varijable
                    if (adminKorisnik != null && await _userManager.IsInRoleAsync(adminKorisnik, "Admin"))
                    {
                        // Ako je admin, šalji ga direktno na AdminPanel u HomeControlleru
                        return RedirectToAction("AdminPanel", "Home");
                    }

                    // Ako nije admin, šalji ga na početnu stranicu za klijente
                    return LocalRedirect("~/");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Ovaj račun je zaključan.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Pogrešna lozinka. Pokušajte ponovo.");
                }
            }
            return Page();
        }
    }
}