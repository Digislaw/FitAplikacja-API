using FitAplikacja.Core.Interfaces;
using System.Collections.Generic;

namespace FitAplikacja.Core.Models
{
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Calories { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }
        public double? Protein { get; set; }
        public string Barcode { get; set; }
        public int? Weight { get; set; }

        public virtual ICollection<AssignedProduct> AssignedProducts { get; set; }
    }
}
