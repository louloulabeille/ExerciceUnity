using System;
using System.Collections.Generic;

#nullable disable

namespace ExerciceUnity.Models
{
    public partial class VendeurPublipostage2009
    {
        public int? Annee { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal? CaProduit { get; set; }
    }
}
