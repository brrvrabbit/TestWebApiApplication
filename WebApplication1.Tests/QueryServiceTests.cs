using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Services;
using WebApplication1.Interfaces;
using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;
using System.Collections;
using WebApplication1.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Tests
{
    public class QueryServiceTests
    {
        [Fact]
        public async void GetQueryAsyncTest()
        {
            //Arrange
            int timeToProccess = 10000;

            var serviceScopeMock = new Mock<IServiceScopeFactory>();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Query:ProcessingTime"]).Returns(timeToProccess.ToString());

            var factory = new ConnectionFactory();
            var context = factory.CreateContextForInMemory();
            var dummyData = context.GenerateDummyData();
            context.Users.AddRange(dummyData.Item1);
            context.VisitStatistics.AddRange(dummyData.Item2);
            await context.SaveChanges();

            var query = GenerateQuery();
            var queryParameters = query.QueryParameters;

            var visitStatisticsEntities = context.VisitStatistics
                .Where(v => (v.UserId == queryParameters.UserId) && (v.Datetime >= queryParameters.RangeBegin) && (v.Datetime <= queryParameters.RangeEnd))
                .ToList();

            var visitStatisticsServiceMock = new Mock<IVisitStatisticsService>(); 
            visitStatisticsServiceMock.Setup(vs => vs //Настройка метода FindInRange
                .FindInRangeAsync(queryParameters.RangeBegin, queryParameters.RangeEnd, queryParameters.UserId))
                .ReturnsAsync(AdaptVisitStatistics(visitStatisticsEntities));

            var queryService = new QueryService(
                context, serviceScopeMock.Object, configurationMock.Object, visitStatisticsServiceMock.Object);

            Random r = new Random();

            var process = queryService.ProcessQueryAsync(query);

            //Act
            var correctInputQueryResult = await queryService.GetQueryAsync(query.QueryId) ; //Подаем правильный guid
            var incorrectInputQueryResult = await queryService.GetQueryAsync(Guid.NewGuid().ToString()); // Подаем рандомный guid
            var incorrectInputFormatQueryResult = await queryService.GetQueryAsync(r.Next().ToString()); // Подаем id не в формате guid
            var nullInputQueryResult = await queryService.GetQueryAsync(null); // Подаем null

            await process;
            //Assert
            Assert.Equal(correctInputQueryResult.QueryId, query.QueryId);

            Assert.Null(incorrectInputQueryResult);

            Assert.Null(incorrectInputFormatQueryResult);

            Assert.Null(nullInputQueryResult);
        }
        [Fact]
        public async void GetQueryListAsyncTest()
        {
            //Arrange
            int timeToProccess = 10000;

            var serviceScopeMock = new Mock<IServiceScopeFactory>();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Query:ProcessingTime"]).Returns(timeToProccess.ToString());

            var factory = new ConnectionFactory();
            var context = factory.CreateContextForInMemory();
            var dummyData = context.GenerateDummyData();
            context.Users.AddRange(dummyData.Item1);
            context.VisitStatistics.AddRange(dummyData.Item2);
            await context.SaveChanges();

            var query = GenerateQuery();
            var queryParameters = query.QueryParameters;

            var visitStatisticsEntities = context.VisitStatistics
                .Where(v => (v.UserId == queryParameters.UserId) && (v.Datetime >= queryParameters.RangeBegin) && (v.Datetime <= queryParameters.RangeEnd))
                .ToList();

            var visitStatisticsServiceMock = new Mock<IVisitStatisticsService>();
            visitStatisticsServiceMock.Setup(vs => vs //Настройка метода FindInRange
                .FindInRangeAsync(queryParameters.RangeBegin, queryParameters.RangeEnd, queryParameters.UserId))
                .ReturnsAsync(AdaptVisitStatistics(visitStatisticsEntities));

            var queryService = new QueryService(
                context, serviceScopeMock.Object, configurationMock.Object, visitStatisticsServiceMock.Object);

            Random r = new Random();

            var process = queryService.ProcessQueryAsync(query);

            //Act
            var queryResultDuringProcess = await queryService.GetQueriesListAsync(); //Подаем правильный guid

            await process;

            var queryResultAfterProcess = await queryService.GetQueriesListAsync();
            //Assert
            Assert.NotNull(queryResultDuringProcess);
            Assert.NotNull(queryResultAfterProcess);
        }

        private Query GenerateQuery()
        {
            string queryGuid = Guid.NewGuid().ToString();

            var query = new Query()
            {
                QueryId = queryGuid,
                QueryInfo = new()
                {
                    QueryId = queryGuid
                },
                QueryParameters = new()
                {
                    RangeBegin = DateTime.Now.Subtract(TimeSpan.FromHours(24)),
                    RangeEnd = DateTime.Now,
                    UserId = Guid.NewGuid().ToString()
                }
            };

            return query;
        }
        private static List<VisitStatistics> AdaptVisitStatistics(List<VisitStatisticsEntity> visitStatisticsEntities)
        {
            List<VisitStatistics> visitStatisticsList = new();
            VisitStatistics? visitStatistics;
            foreach (VisitStatisticsEntity visitStatisticsEntity in visitStatisticsEntities)
            {
                visitStatistics = new()
                {
                    Id = visitStatisticsEntity.Id,
                    Datetime = visitStatisticsEntity.Datetime,
                    UserId = visitStatisticsEntity.UserId,
                };
                visitStatisticsList.Add(visitStatistics);
            }
            return visitStatisticsList;
        }
    }
}
