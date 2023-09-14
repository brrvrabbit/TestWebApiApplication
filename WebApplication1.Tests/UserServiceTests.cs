using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.DBEntities;
using WebApplication1.Interfaces;
using WebApplication1.Services;
using Xunit;

namespace WebApplication1.Tests
{
    public class UserServiceTests
    {

        [Fact]
        public async void GetUsersAsyncReturnsUsersList()
        {
            //Arrange
            var factory = new ConnectionFactory();
            var context = factory.CreateContextForSQLite();

            int usersCount = 10;
            var testUsers = GetTestUserEntities(usersCount);         
            context.AddRange(testUsers);
            await context.SaveChanges();

            var service = new UserService(context);
            //Act
            var result = await service.GetUsersListAsync();

            //Assert 
            Assert.Equal(testUsers.Count, result.Count);
            Assert.NotEmpty(result);
        }
        public void GetUsersAsyncReturnsListsOfUsers()
        {
            //var service = new UserService();
        }

        private List<UserEntity> GetTestUserEntities(int count)
        {
            Random r = new Random();

            List<UserEntity> list = new();
            for (int i = 0; i < count; i++)
            {
                var user = new UserEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "user" + r.Next().ToString(),
                };
                list.Add(user);
            }
            return list;
        }
    }
}
