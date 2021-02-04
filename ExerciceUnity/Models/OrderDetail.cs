using System;
using System.Collections.Generic;

#nullable disable

namespace ExerciceUnity.Models
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public byte[] Ts { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
