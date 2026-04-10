using System.ComponentModel.DataAnnotations;

namespace Földrengések2026.Models
{
    public class Telepules

    {
        public int ID { get; set; }

        [Required]

        [StringLength(200, ErrorMessage = "A település neve legfeljebb 200 karakter lehet.")]

        [Display(Name = "Település név")]

        public string Nev { get; set; } = null!;

        [Required]

        [StringLength(200, ErrorMessage = "A vármegye neve legfeljebb 200 karakter lehet.")]

        [Display(Name = "Vármegye")]

        public string Varmegye { get; set; } = null!;


        // Navigáció: településhez tartozó naplóbejegyzések lekéréséhez:

        public virtual ICollection<Naplo>? Naplok { get; set; } = new List<Naplo>();

    }
}
