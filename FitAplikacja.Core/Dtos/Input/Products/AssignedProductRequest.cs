using System;

namespace FitAplikacja.Core.Dtos.Input.Products
{
    public class AssignedProductRequest
    {
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public int? Weight { get; set; }
        public int Count { get; set; }
    }
}
