using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();
    }
}
