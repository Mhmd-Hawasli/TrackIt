using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Repositories;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Application.Mapping;
using TrackItApp.Application.Services;
using TrackItApp.Domain.Repositories;
using TrackItApp.Infrastructure.Implementations.Persistence;
using TrackItApp.Infrastructure.Implementations.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database
#region Database
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
#endregion 

//DI
#region DI
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
#endregion 

//Auto Mapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(UserProfile).Assembly);
});

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

app.Run();
