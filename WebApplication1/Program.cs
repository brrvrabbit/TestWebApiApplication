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

builder.Services.AddTransient<IQueryService>();
builder.Services.AddTransient<IUserService>();
builder.Services.AddTransient<IVisitStatisticsService>();

builder.Services.AddDbContext<AppDbContext>(options => 
                options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection"),
                ef => ef.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<AppDbContext>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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
    return Results.Json(query.MakeInfoObject());
});


app.Run();
