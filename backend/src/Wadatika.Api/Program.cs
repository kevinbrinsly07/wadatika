using Microsoft.Extensions.Options;

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


