using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using TrackItApp.Infrastructure.Implementations.Persistence;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

//Database
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
