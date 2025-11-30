using AequiForce.Application.Abstractions.Services;
using AequiForce.Application.Projects.Dto;
using AequiForce.Domain;
using AequiForce.Domain.Entities;
using AequiForce.Domain.Enums;
using AequiForce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AequiForce.Application.Projects.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<ProjectDto>> GetProjectsAsync(
        Guid organizationId,
        CancellationToken ct)
    {
        var projects = await _db.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return projects
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProjectDto?> GetProjectAsync(
        Guid organizationId,
        Guid projectId,
        CancellationToken ct)
    {
        var project = await _db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.OrganizationId == organizationId && p.Id == projectId,
                ct);

        return project is null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateProjectAsync(
        Guid organizationId,
        CreateProjectRequest request,
        CancellationToken ct)
    {
        var entity = new Project
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Name = request.Name,
            Description = request.Description,
            BusinessUnit = request.BusinessUnit,
            Currency = request.Currency,

            BaselineAnnualCost = request.BaselineAnnualCost,
            BaselineFte = request.BaselineFte,
            PostAiAnnualCost = request.PostAiAnnualCost,
            PostAiFte = request.PostAiFte,

            ImplementationOneOffCost = request.ImplementationOneOffCost,
            DisplacementRiskPercent = request.DisplacementRiskPercent,

            PlannedStartDate = null,
            PlannedEndDate = null,

            Status = ProjectStatus.Draft
        };

        _db.Projects.Add(entity);
        await _db.SaveChangesAsync(ct);

        return MapToDto(entity);
    }

    public async Task<ProjectDto> UpdateProjectAsync(
        Guid organizationId,
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken ct)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(
                p => p.OrganizationId == organizationId && p.Id == projectId,
                ct);

        if (project is null)
            throw new InvalidOperationException($"Project {projectId} not found for organization {organizationId}.");

        project.Name = request.Name;
        project.Description = request.Description;
        project.BusinessUnit = request.BusinessUnit;

        project.BaselineAnnualCost = request.BaselineAnnualCost;
        project.BaselineFte = request.BaselineFte;
        project.PostAiAnnualCost = request.PostAiAnnualCost;
        project.PostAiFte = request.PostAiFte;

        project.ImplementationOneOffCost = request.ImplementationOneOffCost;
        project.DisplacementRiskPercent = request.DisplacementRiskPercent;
        project.Status = request.Status;

        await _db.SaveChangesAsync(ct);

        return MapToDto(project);
    }

    private static ProjectDto MapToDto(Project project)
        => new()
        {
            Id = project.Id,
            OrganizationId = project.OrganizationId,
            Name = project.Name,
            Description = project.Description,
            BusinessUnit = project.BusinessUnit,
            Currency = project.Currency,
            BaselineAnnualCost = project.BaselineAnnualCost,
            BaselineFte = project.BaselineFte,
            PostAiAnnualCost = project.PostAiAnnualCost,
            PostAiFte = project.PostAiFte,
            ImplementationOneOffCost = project.ImplementationOneOffCost,
            DisplacementRiskPercent = project.DisplacementRiskPercent,
            Status = project.Status
        };
}
