namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsubmissiongrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignmentGrades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubmissionId = c.Int(nullable: false),
                        Mark = c.Int(nullable: false),
                        Comments = c.String(),
                        GradedOn = c.DateTime(nullable: false),
                        TeacherId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssignmentSubmissions", t => t.SubmissionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.TeacherId)
                .Index(t => t.SubmissionId)
                .Index(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssignmentGrades", "TeacherId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AssignmentGrades", "SubmissionId", "dbo.AssignmentSubmissions");
            DropIndex("dbo.AssignmentGrades", new[] { "TeacherId" });
            DropIndex("dbo.AssignmentGrades", new[] { "SubmissionId" });
            DropTable("dbo.AssignmentGrades");
        }
    }
}
