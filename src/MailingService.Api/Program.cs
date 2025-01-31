using MailingService.Application.Services;
using MailingService.Domain.Interfaces;
using MailingService.Domain.Settings;
using MailingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure settings
builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<RateLimitSettings>(settings =>
{
    settings.DailyEmailLimit = builder.Configuration.GetSection("MailSettings:DailyEmailLimit").Get<int>();
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddSingleton<IRateLimiterService, RateLimiterService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();