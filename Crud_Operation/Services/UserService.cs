using Crud_Operation.Model;
using Crud_Operation.Repository.Interface;
using Crud_Operation.Services.Interface;

namespace Crud_Operation.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task<bool> Delete(int Id)
        {
            return _userRepository.Delete(Id);
        }

        public Task<List<User>> GetAll()
        {
            return _userRepository.GetAll();
        }

        public async Task<IEnumerable<User>> GetAllPaged(int pageNumber, int pageSize)
        {
            return await _userRepository.GetAllPaged(pageNumber,pageSize);
        }

        public Task<User> GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public async Task<int> GetTotalCount()
        {
            return await _userRepository.GetTotalCount();
        }

        public async Task<User> Update(User entity)
        {
            var user = await _userRepository.Update(entity);
            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password

            };
        }
    }
}