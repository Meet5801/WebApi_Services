using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Crud_Operation.Model;
using Crud_Operation.Repository.Interface;
using System;
using System.Threading.Tasks;
using Crud_Operation.Model.data;
using Microsoft.Data.SqlClient;

namespace Crud_Operation.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserDbContext _context;

        public AuthRepository(UserDbContext context)
        {
            _context = context;
        }

            public async Task<LoginReponseView> Login(LoginViewModel model)
            {
                var user = await _context.Users.Where(u => u.PhoneNumber == model.PhoneNumber && u.Password == model.Password)
                    .Select(u => new LoginReponseView
                    {
                        Id = u.Id,
                        Firstname = u.FirstName,
                        Lastname = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber
                    })
                    .SingleOrDefaultAsync();

                return user;
            }

        public async Task<User> Register(User entity)
        {
            if (await IsPhoneNumberExists(entity.PhoneNumber) || await IsEmailExists(entity.Email))
            {
                throw new InvalidOperationException("A user with this phone number or email already exists.");
            }

            var parameters = new[]
            {
        new SqlParameter("@FirstName", entity.FirstName),
        new SqlParameter("@LastName", entity.LastName),
        new SqlParameter("@Email", entity.Email),
        new SqlParameter("@PhoneNumber", entity.PhoneNumber),
        new SqlParameter("@Password", entity.Password) // Ensure to hash the password before storing
    };

            var userId = await _context.Database.ExecuteSqlRawAsync("EXEC AddUser @FirstName, @LastName, @Email, @PhoneNumber, @Password", parameters);

            entity.Id = userId;

            return entity;
        }

        private async Task<bool> IsPhoneNumberExists(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        private async Task<bool> IsEmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> RefreshToken(string token)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == token);
        }

        public async Task UpdateRefreshToken(int userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
