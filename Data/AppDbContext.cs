using Microsoft.EntityFrameworkCore;
using testingSite.Models;
using System.Linq.Expressions;

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

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserAnswer>().HasIndex(ua => new { ua.AttemptId, ua.QuestionId, ua.AnswerId }).IsUnique();
            modelBuilder.Entity<Answer>().HasIndex(a => a.QuestionId);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                    var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)),parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }    
}
