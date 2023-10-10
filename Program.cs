using Microsoft.EntityFrameworkCore;
using MinhaWebAPI.data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connerctionString = builder.Configuration.GetConnectionString("EstoqueConnection");

builder.Services.AddDbContext<FolhaContext>(options => options.UseMySql(connerctionString, ServerVersion.AutoDetect(connerctionString)));

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
