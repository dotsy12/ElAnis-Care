using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models.Auth.Identity;
using Microsoft.EntityFrameworkCore;


namespace ElAnis.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AuthContext context) : base(context) { }

        public async Task<User?> FindByEmailAsync(string email)=> await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
     

        public async Task<User?> FindByPhoneAsync(string phone)=> await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
     

        public async Task<User?> FindByEmailOrPhoneAsync(string email, string phone)
        {
            if (!string.IsNullOrEmpty(email))
                return await FindByEmailAsync(email);
            if (!string.IsNullOrEmpty(phone))
                return await FindByPhoneAsync(phone);
            return null;
        }

        public async Task<bool> IsEmailExistsAsync(string email) => await _dbSet.AnyAsync(u => u.Email == email);
      

        public async Task<bool> IsPhoneExistsAsync(string phone) => await _dbSet.AnyAsync(u => u.PhoneNumber == phone);
       
    }
}
