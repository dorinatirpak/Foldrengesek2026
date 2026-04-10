using System.ComponentModel.DataAnnotations;

namespace Földrengések2026.Models
{
    public class Naplo

    {

        public int ID { get; set; }

        [Required]

        [DataType(DataType.Date)]

        [Display(Name = "Dátum")]

        public DateTime Datum { get; set; }

        [Required]

        [DataType(DataType.Time)]

        [Display(Name = "Idő")]

        public TimeSpan Ido { get; set; }

        [Required]

        [Range(-2.0, 12.0, ErrorMessage = "A magnitúdó értéke -2.0 és 12.0 között kell legyen.")]

        [Display(Name = "Magnitúdó")]

        public double Magnitudo { get; set; }

        [Required]

        [Range(0.0, 12.0, ErrorMessage = "Az intenzitás értéke 0.0 és 12.0 között kell legyen.")]

        [Display(Name = "Intenzitás")]

        public double Intenzitas { get; set; }


        // FK + navigáció

        [Required]

        [DisplayFormat(NullDisplayText = "Nincs megadva település")]

        [Display(Name = "Település")]

        public int TelepulesID { get; set; }

        public virtual Telepules? Telepules { get; set; } = null!;

    }
}
