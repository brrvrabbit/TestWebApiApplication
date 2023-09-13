using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using WebApplication1.DBContext;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options => 
                options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection"),
                ef => ef.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

var options = new DbContextOptionsBuilder<AppDbContext>();
options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                ef => ef.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<AppDbContext>());

builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IVisitStatisticsService, VisitStatisticsService>();

using (AppDbContext db = new AppDbContext(options.Options))
{
    //db.Database.EnsureDeleted();
}



    var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


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

app.Run();
