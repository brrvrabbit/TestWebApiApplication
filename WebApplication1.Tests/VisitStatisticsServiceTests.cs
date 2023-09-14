using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Services;
using Xunit;

namespace WebApplication1.Tests
{
    public class VisitStatisticsServiceTests
    {
        [Fact]
        public async void GetVisitStatisticsTest()
        {
            //Arrange
            var factory = new ConnectionFactory();
            var context = factory.CreateContextForInMemory();
            var dummyData = context.GenerateDummyData();
            context.Users.AddRange(dummyData.Item1);
            context.VisitStatistics.AddRange(dummyData.Item2);
            await context.SaveChanges();

            var service = new VisitStatisticsService(context);
            //Act
            var result = await service.GetVisitStatisticsAsync();
            
            //Assert
            Assert.NotNull(result);
            Assert.Equal(context.VisitStatistics.Count(), result.Count);
        }
    }
}
