using Microsoft.EntityFrameworkCore;
using testingSite.Models;

namespace testingSite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<GroupAssignment> GroupAssignments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Log> Logs { get; set; }
        
    }
    
}
