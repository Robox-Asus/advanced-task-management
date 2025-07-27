using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.DTOs;

namespace TaskManagement.Api.Services
{
    public class ProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    DueDate = p.DueDate,
                    ProjectManagerId = p.ProjectManagerId,
                    ProjectManagerName = p.ProjectManager != null ? $"{p.ProjectManager.FirstName} {p.ProjectManager.LastName}" : "N/A"
                })
                .ToListAsync();
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Tasks)
                .Include(p => p.ProjectTeamMembers)
                    .ThenInclude(ptm => ptm.User)
                .Where(p => p.Id == id)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate,
                    DueDate = p.DueDate,
                    ProjectManagerId = p.ProjectManagerId,
                    ProjectManagerName = p.ProjectManager != null ? $"{p.ProjectManager.FirstName} {p.ProjectManager.LastName}" : "N/A",
                    Tasks = p.Tasks.Select(t => new TaskDto // Simplified TaskDto for nested use
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status.ToString(),
                        Priority = t.Priority.ToString(),
                        DueDate = t.DueDate,
                        AssignedToId = t.AssignedToId,
                        AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned"
                    }).ToList(),
                    TeamMembers = p.ProjectTeamMembers.Select(ptm => new UserDto // Simplified UserDto for nested use
                    {
                        Id = ptm.User.Id,
                        Email = ptm.User.Email??"",
                        FirstName = ptm.User.FirstName,
                        LastName = ptm.User.LastName
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Project> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            var project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                DueDate = createProjectDto.DueDate,
                ProjectManagerId = createProjectDto.ProjectManagerId,
                CreatedDate = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return false;
            }

            project.Name = updateProjectDto.Name;
            project.Description = updateProjectDto.Description;
            project.DueDate = updateProjectDto.DueDate;
            project.ProjectManagerId = updateProjectDto.ProjectManagerId;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return false;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddTeamMemberToProjectAsync(int projectId, string userId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            var user = await _context.Users.FindAsync(userId);

            if (project == null || user == null)
            {
                return false;
            }

            var existingMember = await _context.ProjectTeamMembers
                .AnyAsync(ptm => ptm.ProjectId == projectId && ptm.UserId == userId);

            if (existingMember)
            {
                return false; // Already a member
            }

            _context.ProjectTeamMembers.Add(new ProjectTeamMember { ProjectId = projectId, UserId = userId });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTeamMemberFromProjectAsync(int projectId, string userId)
        {
            var projectTeamMember = await _context.ProjectTeamMembers
                .FirstOrDefaultAsync(ptm => ptm.ProjectId == projectId && ptm.UserId == userId);

            if (projectTeamMember == null)
            {
                return false;
            }

            _context.ProjectTeamMembers.Remove(projectTeamMember);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}