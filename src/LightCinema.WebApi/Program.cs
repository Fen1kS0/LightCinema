using System.Text.Json;
using System.Text.Json.Serialization;
using Habr.WebApi.Filters;
using LightCinema.Data;
using LightCinema.WebApi.Application.Auth;
using LightCinema.WebApi.Application.Filters;
using LightCinema.WebApi.Application.Services;
using LightCinema.WebApi.Application.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(configure =>
    {
        configure.Filters.Add<ExceptionFilter>();
        configure.Filters.Add<ValidationFilter>();
    })
    .AddJsonOptions(option =>
    {
        option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddCustomAuthorization();
builder.Services.AddCustomAuthentication(builder.Configuration);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddTransient<JwtService>();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOpt => sqlOpt.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var dbContext = new ApplicationDbContext();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    await SeedData.Seed(dbContext);
}

app.UseCors(configurePolicy => configurePolicy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();