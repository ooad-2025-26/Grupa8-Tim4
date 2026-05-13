using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BentoLab.Models
{
    public class Korisnik
    {
        [Key]
        public int KorisnikID { get; set; }

        public string ImePrezime { get; set; }

        public string Email { get; set; }

        public string Lozinka { get; set; }

        public Uloga Uloga { get; set; }

        public List<Narudzba> Narudzbe { get; set; }

        public Korpa Korpa { get; set; }

        public List<Obavjestenje> Obavjestenja { get; set; }

        public Korisnik()
        {
        }
    }
}