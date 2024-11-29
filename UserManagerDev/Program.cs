using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using UserManagerDev.Database;
using UserManagerDev.Database.Entities;
using UserManagerDev.Endpoints;
using UserManagerDev.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(t => t.FullName?.Replace('+', '.')));

builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(builder.Configuration["ConnectionString"]));
builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


builder.Services.AddSingleton<TokenProvider>();

builder.Services.AddHealthChecks();

var smtp = new SmtpClient
{
    Host = "smtp.gmail.com",
    Port = 587,
    EnableSsl = true,
    UseDefaultCredentials = false,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    Credentials = new NetworkCredential("vovan990028@gmail.com", "emkx zlpq usgg tokk")
};

builder.Services.AddFluentEmail("vovan990028@gmail.com")
    .AddSmtpSender(smtp);

builder.Services.AddEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.MapEndpoints();

app.Run();