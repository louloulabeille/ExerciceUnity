using System;
using System.Collections.Generic;

#nullable disable

namespace ExerciceUnity.Models
{
    public partial class VenteProduit
    {
        public int? Annee { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double? CaProduit { get; set; }
    }
}
