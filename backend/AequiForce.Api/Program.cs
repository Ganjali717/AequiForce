using AequiForce.Application.Abstractions.Services;
using AequiForce.Application.Calculations;
using AequiForce.Application.Policies.Services;
using AequiForce.Application.Projects.Services;
using AequiForce.Infrastructure.Persistence;
using AequiForce.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IPolicyEvaluationEngine, DefaultPolicyEvaluationEngine>();
builder.Services.AddScoped<IComplianceCalculationService, ComplianceCalculationService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();