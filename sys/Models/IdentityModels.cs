using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using sys.Models.Approval;
using sys.Models.Assignment;
using sys.Models.Quiz;
using sys.Models.Student;
using static sys.Models.Student.PendingUser;

namespace sys.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string ContactNumber { get; set; }
        public GradeNo ClassName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            // e.g., userIdentity.AddClaim(new Claim("ContactNumber", this.ContactNumber ?? ""));

            return userIdentity;
        }
        // A parent can have many children…
        [InverseProperty("Parents")]
        public virtual ICollection<ApplicationUser> Children { get; set; }

        // …and a student (child) can have many parents
        [InverseProperty("Children")]
        public virtual ICollection<ApplicationUser> Parents { get; set; }
        public string PhotoPath { get; internal set; }

        public ApplicationUser()
        {
            Children = new HashSet<ApplicationUser>();
            Parents = new HashSet<ApplicationUser>();
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<sys.Models.Notes> Notes { get; set; }
        public DbSet<sys.Models.Quiz.CreateQ> CreateQs { get; set; }
        public DbSet<sys.Models.Quiz.QuizQA> QuizQAs { get; set; }
        public DbSet<sys.Models.Quiz.AttemptAnswer> AttemptAnswers { get; set; }
        public DbSet<sys.Models.CreateTeacherViewModel> CreateTeacher { get; set; }
        public DbSet<CreateAssignment> Assignments { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<PendingUser> PendingUsers { get; set; }
        public DbSet<ApprovedStudent> ApprovedStudents { get; set; }
        public DbSet<ApprovedParent> ApprovedParents { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PendingUser's ClassName to be stored as an integer instead of a string.
            // This requires updating your database schema to change the type of [ClassName] from NVARCHAR(100) to INT.
            modelBuilder.Entity<PendingUser>()
                .Property(p => p.ClassName)
                .HasColumnType("int");
            modelBuilder.Entity<PendingUser>()
                .Property(p => p.RequestedRole)
                .HasColumnType("int");

            modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Children)
            .WithMany(u => u.Parents)
            .Map(m =>
            {
                m.ToTable("UserParentChild");      // name of junction table
                m.MapLeftKey("ParentId");          // FK to the parent user
                m.MapRightKey("ChildId");          // FK to the child user
            });
        }

    }
}