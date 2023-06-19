namespace FitAplikacja.Core.Dtos.Output.Products
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Calories { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }
        public double? Protein { get; set; }
        public string Barcode { get; set; }
        public int? Weight { get; set; }
    }
}
