// AequiForce.Api/Controllers/v1/ProjectsController.cs

using AequiForce.Api.Models.Calculations;
using AequiForce.Application.Abstractions.Services;
using AequiForce.Application.Calculations;
using AequiForce.Application.Projects.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AequiForce.Api.Controllers.v1;

[ApiController]
[Route("api/organizations/{organizationId:guid}/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProjectDto>>> GetProjects(
        Guid organizationId,
        CancellationToken ct)
    {
        var projects = await _projectService.GetProjectsAsync(organizationId, ct);
        return Ok(projects);
    }

    [HttpGet("{projectId:guid}")]
    public async Task<ActionResult<ProjectDto>> GetProject(
        Guid organizationId,
        Guid projectId,
        CancellationToken ct)
    {
        var project = await _projectService.GetProjectAsync(organizationId, projectId, ct);
        if (project is null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(
        Guid organizationId,
        [FromBody] CreateProjectRequest request,
        CancellationToken ct)
    {
        var project = await _projectService.CreateProjectAsync(organizationId, request, ct);
        return CreatedAtAction(nameof(GetProject),
            new { organizationId, projectId = project.Id, version = "1.0" }, project);
    }

    [HttpPut("{projectId:guid}")]
    public async Task<ActionResult<ProjectDto>> UpdateProject(
        Guid organizationId,
        Guid projectId,
        [FromBody] UpdateProjectRequest request,
        CancellationToken ct)
    {
        var project = await _projectService.UpdateProjectAsync(organizationId, projectId, request, ct);
        return Ok(project);
    }


    [HttpPost("{projectId:guid}/calculate/preview")]
    public async Task<ActionResult<CalculationResponse>> PreviewCalculation(
        Guid organizationId,
        Guid projectId,
        [FromQuery] Guid? policyId,
        [FromServices] IComplianceCalculationService calculationService,
        CancellationToken ct)
    {
        var result = await calculationService.CalculatePreviewAsync(projectId, policyId, ct);
        var response = MapToResponse(result);
        return Ok(response);
    }

    [HttpPost("{projectId:guid}/calculate/commit")]
    public async Task<ActionResult<CalculationResponse>> CommitCalculation(
        Guid organizationId,
        Guid projectId,
        [FromQuery] Guid? policyId,
        [FromServices] IComplianceCalculationService calculationService,
        CancellationToken ct)
    {
        // здесь можно достать userId из токена
        Guid? userId = null;
        var result = await calculationService.CalculateAndPersistAsync(projectId, policyId, userId, ct);
        var response = MapToResponse(result);
        return Ok(response);
    }

    private static CalculationResponse MapToResponse(CalculationResult result)
    {
        return new CalculationResponse
        {
            ProjectId = result.ProjectId,
            PolicyId = result.PolicyId,
            AnnualSavings = result.Impact.AnnualSavings,
            DisplacedFte = result.Impact.DisplacedFte,
            Currency = result.Impact.Currency,
            AppliedReinvestmentPercent = result.AppliedReinvestmentPercent,
            RequiredReinvestmentAmount = result.RequiredReinvestmentAmount,
            Allocations = result.Allocations.Select(a => new CalculationBucketResponse
            {
                Bucket = a.Bucket,
                BucketPercent = a.BucketPercent,
                Amount = a.Amount
            }).ToList()
        };
    }
}
