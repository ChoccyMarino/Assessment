using Microsoft.EntityFrameworkCore;
using Assessment.Data; 
using MediatR;
using FluentValidation;
using Assessment.Behaviors;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options=>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


//register MediatR, finds all handlers in the assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


// <,> means it registers any IPipelineBehavior, regardless of what types. eg:
// IPipelineBehavior<RegisterCommand, RegisterResult>
// IPipelineBehavior<LoginCommand, LoginResult>
// it will use the ValidationBehavior for all of them
// so we tell ASP.NET , whenever it sees ANY IPipelineBehavior, use ValidationBehavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// registers FluentValidation validators from the assembly
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
   


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

