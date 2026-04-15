using System.ComponentModel.DataAnnotations;

namespace Földrengések2026.Models
{
    public class Telepules

    {
        public int ID { get; set; }

        [Required]

        [Display(Name = "Település név")]

        public string Nev { get; set; } = null!;

        [Required]

        [Display(Name = "Vármegye")]

        public string Varmegye { get; set; } = null!;

        public virtual ICollection<Naplo>? Naplok { get; set; } = new List<Naplo>();

    }
}
