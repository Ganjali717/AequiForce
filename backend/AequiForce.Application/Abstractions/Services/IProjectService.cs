using AequiForce.Application.Projects.Dto;

namespace AequiForce.Application.Abstractions.Services;

public interface IProjectService
{
    Task<IReadOnlyCollection<ProjectDto>> GetProjectsAsync(Guid organizationId, CancellationToken ct);
    Task<ProjectDto?> GetProjectAsync(Guid organizationId, Guid projectId, CancellationToken ct);
    Task<ProjectDto> CreateProjectAsync(Guid organizationId, CreateProjectRequest request, CancellationToken ct);
    Task<ProjectDto> UpdateProjectAsync(Guid organizationId, Guid projectId, UpdateProjectRequest request, CancellationToken ct);
}