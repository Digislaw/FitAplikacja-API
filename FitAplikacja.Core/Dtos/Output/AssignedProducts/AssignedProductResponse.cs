using System;

namespace FitAplikacja.Core.Dtos.Output.AssignedProducts
{
    public class AssignedProductResponse
    {
        public int Id { get; set; }
        public DateTime Added { get; set; }
        public int? Weight { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }
    }
}
