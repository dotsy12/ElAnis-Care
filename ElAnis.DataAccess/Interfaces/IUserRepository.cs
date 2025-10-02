using ElAnis.Entities.Models.Auth.Identity;


namespace ElAnis.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByPhoneAsync(string phone);
        Task<User?> FindByEmailOrPhoneAsync(string email, string phone);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneExistsAsync(string phone);
    }
}