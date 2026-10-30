using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using WadAtika.Infrastructure.Persistence;
using Wadatika.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

//Register services with the dependency injection container.
builder.Services.AddControllers();

//Register OpenAPI service
builder.Services.AddOpenApi();

//Configure frontend access
var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:3000";

builder.Services.AddCors(Options =>
{
     Options.AddPolicy("Frontend", policy =>
     {
          policy
          .WithOrigins(frontendOrigin)
          .AllowAnyHeader()
          .AllowAnyMethod();
     });
});

var connectionString =
     builder.Configuration.GetConnectionString("DefaultConnection")
     ?? throw new InvalidOperationException(
          "Database connection string is not configured. ");

builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseNpgsql(connectionString));

var app = builder.Build();

//Expose OpenAPI documentation in development
if (app.Environment.IsDevelopment())
{
     app.MapOpenApi();
}

//Configure the HTTP request pipeline.
app.UseCors("Frontend");

app.MapControllers();

app.Run();


