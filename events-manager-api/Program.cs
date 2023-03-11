using events_manager_api.Application.Dtos;
using events_manager_api.Application.Services;
using events_manager_api.Application.Services.Impl;
using events_manager_api.Common.ErrorHandler;
using events_manager_api.Domain.Repositories;
using events_manager_api.Domain.UnitOfWork;
using events_manager_api.Domain.UnitOfWork.Impl;
using events_manager_api.Infrastructure.Clients;
using events_manager_api.Infrastructure.Clients.Impl;
using events_manager_api.Util;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;

builder.Services.AddHttpClient<IWeatherstackClient, WeatherstackClient>(client =>
{
    client.BaseAddress = new Uri($"{configuration["Weatherstack:BaseUrl"]}"); // Pass the base url string to the client constructor in order to omit pass IConfiguration to the client constructor
});

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();  // Which is the correct way to register the UnitOfWork?
builder.Services.AddScoped<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IInviteService, InviteService>();

builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
