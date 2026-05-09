using Microsoft.EntityFrameworkCore;
using VolunteerCenter.Data;
using VolunteerCenter.Models;
using VolunteerCenter.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена в appsettings.json");

builder.Services.AddDbContext<VolunteerCenterContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IVolunteerService, VolunteerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Волонтёрский Центр РТУ МИРЭА API",
        Version = "1.0",
        Description = "API для управления волонтёрами, специалистами и бенефициарами РТУ МИРЭА"
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Волонтёрский Центр API v1");
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VolunteerCenterContext>();
    
    db.Database.Migrate();
    Console.WriteLine("Миграции успешно применены. База данных готова.");

    if (!db.Specialists.Any())
    {
        var specialist1 = new Specialist
        {
            FullName = "Наумов Александр Андреевич",
            Position = "Председатель",
            Email = "naumov@mirea.ru",
            Phone = "+7 (958) 801-24-02"
        };

        var specialist2 = new Specialist
        {
            FullName = "Петров Дмитрий Александрович",
            Position = "Заместитель председателя",
            Email = "petrov@mirea.ru",
            Phone = "+7 (495) 765-43-21"
        };

        db.Specialists.AddRange(specialist1, specialist2);
        db.SaveChanges();
        Console.WriteLine("✅ Специалисты добавлены.");

        var volunteer1 = new Volunteer
        {
            FullName = "Сидорова Мария Александровна",
            Email = "sidorova.ma@student.mirea.ru",
            Phone = "+7 (800) 555-35-35",
            BirthDate = new DateTime(2005, 3, 15),
            CuratorId = specialist1.Id
        };

        var volunteer2 = new Volunteer
        {
            FullName = "Козлов Артём Дмитриевич",
            Email = "kozlov.ad@student.mirea.ru",
            Phone = "+7 (928) 322-53-38",
            BirthDate = new DateTime(2004, 11, 8),
            CuratorId = specialist1.Id
        };

        var volunteer3 = new Volunteer
        {
            FullName = "Морозова Екатерина Павловна",
            Email = "morozova.ep@student.mirea.ru",
            Phone = "+7 (255) 352-25-55",
            BirthDate = new DateTime(2006, 1, 22),
            CuratorId = specialist2.Id
        };

        db.Volunteers.AddRange(volunteer1, volunteer2, volunteer3);
        db.SaveChanges();
        Console.WriteLine("Волонтёры добавлены.");

        var beneficiary1 = new Beneficiary
        {
            FullName = "Пожилой центр «Забота»",
            Address = "г. Москва, ул. Профсоюзная, д. 45",
            Description = "Помощь пожилым людям: прогулки, покупки, общение",
            Phone = "+7 (495) 565-89-92"
        };

        var beneficiary2 = new Beneficiary
        {
            FullName = "Детский дом № 17",
            Address = "г. Москва, ул. Зеленоградская, д. 12",
            Description = "Помощь в организации досуга детей",
            Phone = "+7 (495) 672-32-22"
        };

        db.Beneficiaries.AddRange(beneficiary1, beneficiary2);
        db.SaveChanges();
        Console.WriteLine("Бенефициары добавлены.");

        db.VolunteerBeneficiaries.AddRange(
            new VolunteerBeneficiary
            {
                VolunteerId = volunteer1.Id,
                BeneficiaryId = beneficiary1.Id,
                AssignmentDate = DateTime.UtcNow
            },
            new VolunteerBeneficiary
            {
                VolunteerId = volunteer2.Id,
                BeneficiaryId = beneficiary1.Id,
                AssignmentDate = DateTime.UtcNow
            },
            new VolunteerBeneficiary
            {
                VolunteerId = volunteer3.Id,
                BeneficiaryId = beneficiary2.Id,
                AssignmentDate = DateTime.UtcNow
            }
        );

        db.SaveChanges();
        Console.WriteLine("Связи волонтёров с бенефициарами добавлены.");
    }
    else
    {
        Console.WriteLine("Тестовые данные уже существуют.");
    }
}


app.MapGet("/", (IConfiguration config) =>
{
    return Results.Ok(new
    {
        application = config["AppSettings:AppName"] ?? "Волонтёрский Центр РТУ МИРЭА",
        version = config["AppSettings:Version"] ?? "1.0",
        university = config["AppSettings:University"],
        message = "Добро пожаловать в API Волонтёрского Центра!",
        endpoints = new[]
        {
            "GET  /                               — Информация об API",
            "GET  /api/volunteers                 — Список всех волонтёров",
            "GET  /api/volunteers/with-curator    — Волонтёры с кураторами",
            "GET  /api/beneficiaries              — Список бенефициаров",
            "GET  /api/config                     — Конфигурация приложения",
            "GET  /swagger                        — Swagger UI",
            "POST /api/volunteers                 — Добавить волонтёра",
            "POST /api/specialist                 — Добавить куратора",
            "POST /api/beneficiares               — Добавить бенефициара"
        }
    });
})
.WithName("Home")
.WithSummary("Главная страница API");

app.MapGet("/api/volunteers", async (IVolunteerService service, IConfiguration config) =>
{
    var maxItems = config.GetValue<int>("AppSettings:MaxItems", 50);
    var volunteers = await service.GetAllAsync(maxItems);
    
    return Results.Ok(new 
    { 
        count = volunteers.Count(),
        maxItems,
        data = volunteers 
    });
})
.WithName("GetAllVolunteers")
.WithSummary("Получить список всех волонтёров");

app.MapGet("/api/volunteers/with-curator", async (IVolunteerService service, IConfiguration config) =>
{
    var maxItems = config.GetValue<int>("AppSettings:MaxItems", 50);
    var volunteers = await service.GetVolunteersWithCuratorAsync(maxItems);
    
    return Results.Ok(new 
    { 
        count = volunteers.Count(),
        data = volunteers 
    });
})
.WithName("GetVolunteersWithCurator")
.WithSummary("Получить волонтёров вместе с информацией о кураторе");

app.MapGet("/api/beneficiaries", async (IVolunteerService service, IConfiguration config) =>
{
    var maxItems = config.GetValue<int>("AppSettings:MaxItems", 50);
    var beneficiaries = await service.GetBeneficiariesAsync(maxItems);
    
    return Results.Ok(new 
    { 
        count = beneficiaries.Count(),
        data = beneficiaries 
    });
})
.WithName("GetBeneficiaries")
.WithSummary("Получить список бенефициаров");

app.MapGet("/api/config", (IConfiguration config) =>
{
    return Results.Ok(new
    {
        appName = config["AppSettings:AppName"],
        version = config["AppSettings:Version"],
        university = config["AppSettings:University"],
        maxItems = config.GetValue<int>("AppSettings:MaxItems"),
        connectionString = "[hidden]"
    });
})
.WithName("GetConfig")
.WithSummary("Получить конфигурацию приложения");


app.MapPost("/api/specialists", async (VolunteerCenterContext db, Specialist specialist) =>
{
    db.Specialists.Add(specialist);
    await db.SaveChangesAsync();
    return Results.Created($"/api/specialists/{specialist.Id}", specialist);
})
.WithName("CreateSpecialist")
.WithSummary("Добавить специалиста");


app.MapPost("/api/volunteers", async (VolunteerCenterContext db, Volunteer volunteer) =>
{
    db.Volunteers.Add(volunteer);
    await db.SaveChangesAsync();
    return Results.Created($"/api/volunteers/{volunteer.Id}", volunteer);
})
.WithName("CreateVolunteer")
.WithSummary("Добавить волонтёра");


app.MapPost("/api/beneficiaries", async (VolunteerCenterContext db, Beneficiary beneficiary) =>
{
    db.Beneficiaries.Add(beneficiary);
    await db.SaveChangesAsync();
    return Results.Created($"/api/beneficiaries/{beneficiary.Id}", beneficiary);
})
.WithName("CreateBeneficiary")
.WithSummary("Добавить бенефициара");


app.MapPost("/api/volunteer-beneficiaries", async (VolunteerCenterContext db, VolunteerBeneficiary vb) =>
{
    db.VolunteerBeneficiaries.Add(vb);
    await db.SaveChangesAsync();
    return Results.Created($"/api/volunteer-beneficiaries", vb);
})
.WithName("CreateVolunteerBeneficiary")
.WithSummary("Назначить волонтёра бенефициару");

app.Run();