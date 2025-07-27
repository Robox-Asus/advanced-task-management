using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<ProjectTeamMember> ProjectTeamMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // 1. Call the base implementation first
        base.OnModelCreating(builder);

        // 2. Configure the many-to-many relationship for ProjectTeamMember
        //    This entity represents the "junction table" between Projects and ApplicationUsers.
        builder.Entity<ProjectTeamMember>()
            .HasKey(ptm => new { ptm.ProjectId, ptm.UserId }); // composite primary key
        // Explanation:
        // - `builder.Entity<ProjectTeamMember>()`: We are starting to configure the 'ProjectTeamMember' entity.
        // - `.HasKey(ptm => new { ptm.ProjectId, ptm.UserId })`: This is crucial! It tells EF Core that the primary key
        //   for the `ProjectTeamMember` table is *not* a single column, but a combination of `ProjectId` and `UserId`.
        //   This ensures that a specific user can only be a team member of a specific project once, and it forms the
        //   unique identifier for each entry in this join table. This cannot be done with simple data annotations.

        builder.Entity<ProjectTeamMember>()
            .HasOne(ptm => ptm.Project) // A ProjectTeamMember has one Project
            .WithMany(p => p.ProjectTeamMembers) // A Project can have many ProjectTeamMembers
            .HasForeignKey(ptm => ptm.ProjectId); // The foreign key in ProjectTeamMember pointing to Project is ProjectId
          // Explanation:
          // - This block defines the relationship from `ProjectTeamMember` back to `Project`.
          // - `HasOne(ptm => ptm.Project)`: Specifies that each `ProjectTeamMember` record is associated with exactly one `Project`.
          // - `WithMany(p => p.ProjectTeamMembers)`: Specifies that a `Project` can be associated with many `ProjectTeamMember` records.
          // - `HasForeignKey(ptm => ptm.ProjectId)`: Explicitly states that `ProjectId` in the `ProjectTeamMember` table is the foreign key
          //   that links to the `Project` table's primary key.

        builder.Entity<ProjectTeamMember>() 
            .HasOne(ptm => ptm.User) // A ProjectTeamMember has one User
            .WithMany() // A User can have many ProjectTeamMembers, but ApplicationUser doesn't have a direct navigation property back to ProjectTeamMember
            .HasForeignKey(ptm => ptm.UserId); // The foreign key in ProjectTeamMember pointing to User is UserId
       // Explanation:
       // - This block defines the relationship from `ProjectTeamMember` back to `ApplicationUser` (referred to as `User` here).
       // - `HasOne(ptm => ptm.User)`: Specifies that each `ProjectTeamMember` record is associated with exactly one `ApplicationUser`.
       // - `.WithMany()`: This is used when the "many" side of the relationship (in this case, `ApplicationUser` having many `ProjectTeamMember` records)
       //   does *not* have a navigation property back to `ProjectTeamMember`. If `ApplicationUser` had `public ICollection<ProjectTeamMember> ProjectsJoined { get; set; }`,
       //   you would put `WithMany(u => u.ProjectsJoined)`. Since it doesn't, we use `WithMany()` to tell EF Core there's no navigation property on the other side.
       // - `HasForeignKey(ptm => ptm.UserId)`: Explicitly states that `UserId` in the `ProjectTeamMember` table is the foreign key
       //   that links to the `ApplicationUser` table's primary key.

        // 3. Configure relationships for TaskItem
        builder.Entity<TaskItem>()
            .HasOne(ti => ti.Project) // A TaskItem has one Project
            .WithMany(p => p.Tasks) // A Project can have many TaskItems
            .HasForeignKey(ti => ti.ProjectId) // The foreign key in TaskItem pointing to Project is ProjectId
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of project if tasks exist
        // Explanation:
        // - This defines the one-to-many relationship between `Project` and `TaskItem`.
        // - `HasOne(ti => ti.Project)`: A `TaskItem` belongs to one `Project`.
        // - `WithMany(p => p.Tasks)`: A `Project` can have many `TaskItem`s (via its `Tasks` collection).
        // - `HasForeignKey(ti => ti.ProjectId)`: `ProjectId` in `TaskItem` is the foreign key.
        // - `.OnDelete(DeleteBehavior.Restrict)`: This is a very important setting for data integrity.
        //   It tells the database: "If you try to delete a `Project` that still has `TaskItem`s associated with it,
        //   **prevent that deletion**." This prevents orphaned tasks and ensures you don't accidentally delete a
        //   project that still has active work. Without this, EF Core might default to `Cascade` (delete tasks too)
        //   or `NoAction` (which might still allow deletion depending on DB settings, potentially leading to errors).

        builder.Entity<TaskItem>()
            .HasOne(ti => ti.AssignedTo) // A TaskItem is assigned to one ApplicationUser (or null)
            .WithMany(au => au.AssignedTasks) // An ApplicationUser can have many assigned tasks
            .HasForeignKey(ti => ti.AssignedToId) // The foreign key in TaskItem pointing to ApplicationUser is AssignedToId
            .OnDelete(DeleteBehavior.SetNull); // Set AssignedToId to null if user is deleted
       // Explanation:
       // - This defines the relationship between `TaskItem` and the `ApplicationUser` it's assigned to.
       // - `HasOne(ti => ti.AssignedTo)`: A `TaskItem` is assigned to one `ApplicationUser`.
       // - `WithMany(au => au.AssignedTasks)`: An `ApplicationUser` can have many `TaskItem`s assigned to them (via their `AssignedTasks` collection).
       // - `HasForeignKey(ti => ti.AssignedToId)`: `AssignedToId` in `TaskItem` is the foreign key.
       // - `.OnDelete(DeleteBehavior.SetNull)`: This tells the database: "If an `ApplicationUser` is deleted,
       //   any `TaskItem`s that were assigned to that user should have their `AssignedToId` foreign key
       //   set to `NULL` (meaning they become unassigned)." This is a soft way to handle user deletion without
       //   deleting all their assigned tasks.

        // 4. Configure relationships for TaskComment
        builder.Entity<TaskComment>()
            .HasOne(tc => tc.TaskItem) // A TaskComment belongs to one TaskItem
            .WithMany(ti => ti.Comments) // A TaskItem can have many TaskComments
            .HasForeignKey(tc => tc.TaskItemId) // The foreign key in TaskComment pointing to TaskItem is TaskItemId
            .OnDelete(DeleteBehavior.Cascade); // Delete comments if task is deleted
       // Explanation:
       // - This defines the one-to-many relationship between `TaskItem` and `TaskComment`.
       // - `HasOne(tc => tc.TaskItem)`: A `TaskComment` belongs to one `TaskItem`.
       // - `WithMany(ti => ti.Comments)`: A `TaskItem` can have many `TaskComment`s (via its `Comments` collection).
       // - `HasForeignKey(tc => tc.TaskItemId)`: `TaskItemId` in `TaskComment` is the foreign key.
       // - `.OnDelete(DeleteBehavior.Cascade)`: This tells the database: "If a `TaskItem` is deleted,
       //   **automatically delete all `TaskComment`s** that belong to that task." This is a common and logical
       //   behavior for child records that wouldn't make sense without their parent.

        builder.Entity<TaskComment>()
            .HasOne(tc => tc.User) // A TaskComment was made by one ApplicationUser
            .WithMany() // An ApplicationUser can make many comments, but ApplicationUser doesn't have a direct navigation property back to TaskComment
            .HasForeignKey(tc => tc.UserId) // The foreign key in TaskComment pointing to ApplicationUser is UserId
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of user if comments exist
        // Explanation:
        // - This defines the relationship between `TaskComment` and the `ApplicationUser` who made the comment.
        // - `HasOne(tc => tc.User)`: A `TaskComment` was written by one `ApplicationUser`.
        // - `WithMany()`: Similar to `ProjectTeamMember` to `User`, `ApplicationUser` doesn't have a direct navigation property back to `TaskComment`.
        // - `HasForeignKey(tc => tc.UserId)`: `UserId` in `TaskComment` is the foreign key.
        // - `.OnDelete(DeleteBehavior.Restrict)`: This tells the database: "If an `ApplicationUser` is deleted,
        //   **prevent that deletion** if they still have `TaskComment`s associated with them." This is a strong
        //   data integrity constraint, ensuring you don't lose the history of who made which comment. You would
        //   typically need to delete the comments first before deleting the user.

        // 5. Seed roles
        // Seed roles with static GUIDs to prevent PendingModelChangesWarning
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "a18be9c0-aa65-4af8-bd17-0021544243a3", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "a18be9c0-aa65-4af8-bd17-0021544243a3" },
                new IdentityRole { Id = "c18be9c0-aa65-4af8-bd17-0021544243c3", Name = "ProjectManager", NormalizedName = "PROJECTMANAGER", ConcurrencyStamp = "c18be9c0-aa65-4af8-bd17-0021544243c3" },
                new IdentityRole { Id = "e18be9c0-aa65-4af8-bd17-0021544243e3", Name = "TeamMember", NormalizedName = "TEAMMEMBER", ConcurrencyStamp = "e18be9c0-aa65-4af8-bd17-0021544243e3" }
            );
        // Explanation:
        // - `builder.Entity<IdentityRole>()`: We are configuring the `IdentityRole` entity, which is part of ASP.NET Core Identity
        //   and represents user roles in your application.
        // - `.HasData(...)`: This is a seeding feature. It tells EF Core to insert these specific `IdentityRole` records
        //   into the `AspNetRoles` table (the default table for `IdentityRole`) when you run `dotnet ef database update`.
        //   This ensures that your application starts with predefined roles like "Admin", "ProjectManager", and "TeamMember",
        //   which are essential for your authorization setup. `NormalizedName` is typically the uppercase version of the `Name`
        //   and is used internally by Identity for consistency.
    }
}