using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;
using ZooCare.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Додай цей блок для підключення БД
builder.Services.AddDbContext<ZooContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// CORS для дозволу запитів від Wokwi та інших клієнтів
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ZooCare API",
        Version = "v1",
        Description = "API для управління зоопарком"
    });

    // Додаємо підтримку Bearer авторизації
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Реєстрація репозиторіїв
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Реєстрація сервісів
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IEnclosureService, EnclosureService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IIoTService, IoTService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();

// Ініціалізація ролей при старті
try
{
    using (var scope = app.Services.CreateScope())
    {
        var adminService = scope.ServiceProvider.GetRequiredService<ZooCare.API.Interfaces.IAdminService>();
        try
        {
            await adminService.InitializeRolesAsync();
            Console.WriteLine("✓ Ролі успішно ініціалізовано");
        }
        catch (Exception ex)
        {
            // Ролі вже ініціалізовані або помилка БД - ігноруємо
            Console.WriteLine($"⚠️ Помилка ініціалізації ролей (можливо вже ініціалізовано): {ex.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Помилка при створенні scope для ініціалізації ролей: {ex.Message}");
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// CORS має бути на початку pipeline, перед UseHttpsRedirection
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

