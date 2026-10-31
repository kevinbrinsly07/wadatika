using Wadatika.Application;
using Wadatika.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var frontendOrigin = builder.Configuration["Frontend:Origin"]
    ?? "http://localhost:3000";

builder.Services.AddCors(options =>
{
     options.AddPolicy("Frontend", policy =>
     {
          policy.WithOrigins(frontendOrigin)
             .AllowAnyHeader()
             .AllowAnyMethod();
     });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
     app.MapOpenApi();
}

app.UseCors("Frontend");
app.MapControllers();

app.Run();