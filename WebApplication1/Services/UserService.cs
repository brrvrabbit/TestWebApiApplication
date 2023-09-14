using WebApplication1.DBEntities;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        readonly IApplicationDbContext _applicationDbContext;
        public UserService(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<User>> GetUsersListAsync()
        {
            return await AdaptUser(_applicationDbContext.Users.ToList());
        }
        private static async Task<List<User>> AdaptUser(List<UserEntity> UserEntities)
        {
            List<User> userList = new();
            User? user;
            foreach (UserEntity UserEntity in UserEntities)
            {
                user = new()
                {
                    Id = UserEntity.Id,
                    Username = UserEntity.Username,
                };
                userList.Add(user);
            }
            return userList;
        }
    }
}
