using Crud_Operation.Model;

namespace Crud_Operation.Services.Interface
{
    public interface IUserService
    {
        Task<List<User>> GetAll();
        Task<IEnumerable<User>> GetAllPaged(int pageNumber, int pageSize); // New method
        Task<int> GetTotalCount();
        Task<User> GetById(int id);
        Task<User> Update(User entity);
        Task<bool> Delete(int Id);
    }
}
