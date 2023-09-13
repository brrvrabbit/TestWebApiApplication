using Microsoft.EntityFrameworkCore;
using System;
using WebApplication1.DBContext;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<AppDbContext>(options => 
                options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection"),
                ef => ef.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<AppDbContext>());

builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IVisitStatisticsService, VisitStatisticsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.MapPost("/report/user_statistics", (QueryParameters queryParameters, IApplicationDbContext db, IQueryService queryService) =>
{
    string guid = Guid.NewGuid().ToString();
    Query query = new()
    {
        QueryId = guid,
        QueryInfo = new()
        {
            QueryId = guid,
        },
        QueryParameters = queryParameters
    };
    queryService.ProcessQueryAsync(query);
    return Results.Json(guid);
});
app.MapGet("/report/info", async (string queryGuid, IQueryService queryService) =>
{
    var query = await queryService.GetQuery(queryGuid);
    if(query != null)
        return Results.Json(query.MakeInfoObject());
    else return Results.NotFound();
});

app.MapGet("/users/", async (IUserService userService) =>
{
    return await userService.GetUsersAsync();
});

//app.MapPost("/users/initialize", async (IApplicationDbContext db) =>
//{
//    int usersCount = 15,
//        minVisit = 1,
//        maxVisit = 100;
//    Random r = new Random();

//    User user;
//    for (int i = 0; i < usersCount; i++)
//    {
//        user = new()
//        {
//            Id = Guid.NewGuid().ToString(),
//            Username = "user" + r.Next().ToString(),
//        };
//        int visitCount = r.Next(1, 100);

//        for (int j = 0; j < visitCount; j++)
//        {
//            VisitStatistics visitStatistics = new()
//            {
//                UserId = user.Id,
//                Datetime = DateTime.Now.Subtract(TimeSpan.FromHours(r.Next(24)))
//            };
//            db.VisitStatistics.Add(visitStatistics.VisitStatisticsEntity);
//        }
        
//        db.Users.Add(user.UserEntity);
//        await db.SaveChanges();
//    }
//    return Results.Ok();
//});

app.Run();
