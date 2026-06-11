using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BentoLab.Controllers
{
    public class KorpasController : Controller
    {
        // 1. PRIKAZ KORPE SA ARTIKLIMA
        public IActionResult Index()
        {
            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");
            return View();
        }

        // 2. EKRAN ZA ODABIR DOSTAVE
        public IActionResult Create()
        {
            string[] cijene = TempData["CijenaKreiraneTorte"] as string[];
            double medjuzbir = 0;
            if (cijene != null)
            {
                foreach (var c in cijene)
                {
                    medjuzbir += double.Parse(c, CultureInfo.InvariantCulture);
                }
            }

            ViewBag.Medjuzbir = medjuzbir.ToString("F2", CultureInfo.InvariantCulture);

            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");
            return View();
        }

        [HttpPost]
        public IActionResult ProcesirajPreuzimanje(string nacinPreuzimanja, string emailKupca)
        {
            if (nacinPreuzimanja == "Dostava")
            {
                // Ako ide na dostavu, možemo proslijediti mail dalje preko TempData da ga ne kuca dvaput
                TempData["EmailKupcaDostava"] = emailKupca;
                return RedirectToAction(nameof(Dostava));
            }

            // Ako je lično preuzimanje, odmah šaljemo mail jer imamo string emailKupca!
            var stavke = PreuzmiIocistiKorpu(TempData);
            string brojNarudzbe = "CK-" + new Random().Next(1000, 9999);

            if (!string.IsNullOrEmpty(emailKupca))
            {
                EmailService.PosaljiObavjestenje(emailKupca, brojNarudzbe, "Zaprimljeno (Lično preuzimanje)");
            }

            return RedirectToAction(nameof(Uspjeh));
        }

        // 3. EKRAN ZA POPUNJAVANJE PODATAKA O DOSTAVI
        public IActionResult Dostava()
        {
            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");
            return View();
        }
        [HttpPost]
        public IActionResult PotvrdiDostavu(string ime, string adresa, string grad, string telefon, string napomena, string emailKupca)
        {
            // Tvoj postojeći kod koji spašava narudžbu...
            var stavke = PreuzmiIocistiKorpu(TempData);

            // OVDJE OKIDAMO MAILTRAP:
            if (!string.IsNullOrEmpty(emailKupca))
            {
                EmailService.PosaljiObavjestenje(emailKupca, "CK-0847", "Zaprimljeno");
            }

            return RedirectToAction("Uspjeh");
        }

        // 4. EKRAN USPJEŠNE NARUDŽBE
        public IActionResult Uspjeh()
        {
            return View();
        }

        // ČISTA STATIČKA METODA ZA UPIS U BAZU I ČIŠĆENJE
        public static List<dynamic> PreuzmiIocistiKorpu(ITempDataDictionary tempData)
        {
            var listaStavki = new List<dynamic>();

            if (tempData != null)
            {
                string[] nazivi = tempData["NazivKreiraneTorte"] as string[];
                string[] cijene = tempData["CijenaKreiraneTorte"] as string[];

                if (nazivi != null && cijene != null)
                {
                    for (int i = 0; i < nazivi.Length; i++)
                    {
                        listaStavki.Add(new
                        {
                            NazivZaBazu = nazivi[i],
                            CijenaZaBazu = decimal.Parse(cijene[i], CultureInfo.InvariantCulture)
                        });
                    }
                }

                // Brišemo TempData tek kad je narudžba uspješno završena i poslana bazi
                tempData.Remove("NazivKreiraneTorte");
                tempData.Remove("CijenaKreiraneTorte");
            }

            return listaStavki;
        }
    }
}