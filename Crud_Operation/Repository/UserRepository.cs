using Crud_Operation.Model;
using Crud_Operation.Model.data;
using Crud_Operation.Repository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Crud_Operation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetById(int id)
        {
            var idParameter = new SqlParameter("@Id", id);

            var user = await _context.Users.FromSqlRaw("EXEC GetByIdUsers @Id", idParameter).ToListAsync();

            return user.SingleOrDefault();
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.FromSqlRaw("EXEC GetAllUsers").ToListAsync();
        }

        public async Task<User> Update(User entity)
        {
            var idParameter = new SqlParameter("@Id", entity.Id);
            var firstNameParameter = new SqlParameter("@FirstName", entity.FirstName ?? (object)DBNull.Value);
            var lastNameParameter = new SqlParameter("@LastName", entity.LastName ?? (object)DBNull.Value);
            var emailParameter = new SqlParameter("@Email", entity.Email ?? (object)DBNull.Value);
            var phoneNumberParameter = new SqlParameter("@PhoneNumber", entity.PhoneNumber ?? (object)DBNull.Value);
            var passwordParameter = new SqlParameter("@Password", entity.Password ?? (object)DBNull.Value);

            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpdateUser @Id, @FirstName, @LastName, @Email, @PhoneNumber, @Password",
                idParameter, firstNameParameter, lastNameParameter, emailParameter, phoneNumberParameter, passwordParameter
            );

            if (rowsAffected < 0)
            {
                return entity;
            }

            return null;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            var idParameter = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteUser @Id", idParameter);
            return true;
        }

        public async Task<IEnumerable<User>> GetAllPaged(int pageNumber, int pageSize)
        {
            var users = new List<User>();

            var connectionString = _context.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("GetUsersPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }


        public async Task<int> GetTotalCount()
        {
            return await _context.Users.CountAsync();
        }
    }
}
