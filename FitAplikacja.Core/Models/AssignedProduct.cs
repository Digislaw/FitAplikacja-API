using FitAplikacja.Core.Interfaces;
using System;

namespace FitAplikacja.Core.Models
{
    public class AssignedProduct : IUserRelatedEntity
    {
        public int Id { get; set; }
        public DateTime Added { get; set; }
        public int? Weight { get; set; }
        public int Count { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
