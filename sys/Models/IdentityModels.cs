using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using sys.Models.Approval;
using sys.Models.Assignment;
using sys.Models.Chat;
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
        public string PhotoPath { get; internal set; }

        // ← NEW: which messages this user has read
        public virtual ICollection<MessageRead> MessageReads { get; set; }

        // existing parent/child
        public virtual ICollection<UserParentChild> AsParentLinks { get; set; }

        // links *where this user is the child*
        public virtual ICollection<UserParentChild> AsChildLinks { get; set; }
        public ApplicationUser()
        {
            AsParentLinks = new HashSet<UserParentChild>();
            AsChildLinks = new HashSet<UserParentChild>();
            MessageReads = new HashSet<MessageRead>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager
              .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
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
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageRead> MessageReads { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<sys.Models.Notes> Notes { get; set; }
        public DbSet<sys.Models.Quiz.CreateQ> CreateQs { get; set; }
        public DbSet<sys.Models.Quiz.QuizQA> QuizQAs { get; set; }
        public DbSet<sys.Models.Quiz.AttemptAnswer> AttemptAnswers { get; set; }
        public DbSet<sys.Models.Assignment.CreateAssignment> CreateAssignments { get; set; }
        public DbSet<sys.Models.CreateTeacherViewModel> CreateTeacherViewModel { get; set; }
        public DbSet<UserParentChild> UserParentChild { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<PendingUser> PendingUsers { get; set; }
        public DbSet<AssignmentGrade> AssignmentGrades { get; set; }
        public DbSet<ApprovedStudent> ApprovedStudents { get; set; }
        public DbSet<ApprovedParent> ApprovedParents { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ────────────────────────────────────────────────────────────
            // PendingUser config (unchanged)
            modelBuilder.Entity<PendingUser>()
                .Property(p => p.ClassName)
                .HasColumnType("int");
            modelBuilder.Entity<PendingUser>()
                .Property(p => p.RequestedRole)
                .HasColumnType("int");

            modelBuilder.Entity<UserParentChild>()
  .HasKey(upc => new { upc.ParentId, upc.ChildId });

            modelBuilder.Entity<UserParentChild>()
              .HasRequired(upc => upc.Parent)
              .WithMany(u => u.AsParentLinks)
              .HasForeignKey(upc => upc.ParentId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserParentChild>()
              .HasRequired(upc => upc.Child)
              .WithMany(u => u.AsChildLinks)
              .HasForeignKey(upc => upc.ChildId)
              .WillCascadeOnDelete(false);
            // ────────────────────────────────────────────────────────────
            // Message → Sender / Recipient (both ApplicationUser)
            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Sender)
                .WithMany()       // no nav prop on ApplicationUser for sent messages
                .HasForeignKey(m => m.SenderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                .HasOptional(m => m.Recipient)
                .WithMany()       // no nav prop for received
                .HasForeignKey(m => m.RecipientId)
                .WillCascadeOnDelete(false);

            // ────────────────────────────────────────────────────────────
            // MessageRead composite PK + relations
            modelBuilder.Entity<MessageRead>()
                .HasKey(mr => new { mr.MessageId, mr.UserId });

            modelBuilder.Entity<MessageRead>()
                .HasRequired(mr => mr.Message)
                .WithMany(m => m.ReadReceipts)   // nav prop on Message
                .HasForeignKey(mr => mr.MessageId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<MessageRead>()
                .HasRequired(mr => mr.User)
                .WithMany(u => u.MessageReads)   // nav prop on ApplicationUser
                .HasForeignKey(mr => mr.UserId)
                .WillCascadeOnDelete(true);
        }        
    }
}