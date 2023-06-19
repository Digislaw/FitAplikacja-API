using FitAplikacja.Core.Models;

namespace FitAplikacja.Core.Interfaces
{
    public interface IUserRelatedEntity : IEntity
    {
        string ApplicationUserId { get; set; }
        ApplicationUser User { get; set; }
    }
}
