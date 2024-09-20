using Crud_Operation.Model;

namespace Crud_Operation.Repository.Interface
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<IEnumerable<User>> GetAllPaged(int pageNumber, int pageSize); // New method
        Task<int> GetTotalCount();
        Task<User> GetById(int Id);
        Task<User> Update(User entity);
        Task<bool> Delete(int Id);

    }
}
