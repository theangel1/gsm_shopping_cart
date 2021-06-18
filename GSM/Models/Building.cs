using System;
using System.ComponentModel.DataAnnotations;

namespace GSM.Models
{
    public class Building
    {        
        public int Id { get; set; }
        public string Descripcion { get; set; }

        [Display(Name ="N° Dormitorios")]
        public int NumeroDormitorios { get; set; }

        [Display(Name = "N° Baños")]
        public int NumeroBano { get; set; }

        [Display(Name = "Living Comedor")]
        public bool HasLivingComedor { get; set; }

        [Display(Name = "Cocina Americana")]
        public bool HasCocinaAmericana { get; set; }

        [Display(Name = "Porche")]
        public bool HasPorche { get; set; }

        public string Imagen { get; set; }

        public double Precio { get; set; }

        public string ImagenPlano { get; set; }

    }
}
