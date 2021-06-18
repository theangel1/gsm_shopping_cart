using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSM.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required, Display(Name ="R.U.T.")]
        public string Rut { get; set; }

        [Required, Display(Name ="Razón Social")]
        public string RazonSocial { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        public string Ciudad { get; set; }

        [NotMapped]
        public bool IsAdmin { get; set; }

    }
}
