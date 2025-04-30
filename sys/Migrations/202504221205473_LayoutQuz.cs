namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LayoutQuz : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttemptAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuizAttemptId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        Selected = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizQAs", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("dbo.QuizAttempts", t => t.QuizAttemptId, cascadeDelete: true)
                .Index(t => t.QuizAttemptId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.QuizAttempts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.String(maxLength: 128),
                        QuizId = c.Int(nullable: false),
                        Started = c.DateTime(nullable: false),
                        Completed = c.DateTime(),
                        Score = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CreateQs", t => t.QuizId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.StudentId)
                .Index(t => t.StudentId)
                .Index(t => t.QuizId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuizAttempts", "StudentId", "dbo.AspNetUsers");
            DropForeignKey("dbo.QuizAttempts", "QuizId", "dbo.CreateQs");
            DropForeignKey("dbo.AttemptAnswers", "QuizAttemptId", "dbo.QuizAttempts");
            DropForeignKey("dbo.AttemptAnswers", "QuestionId", "dbo.QuizQAs");
            DropIndex("dbo.QuizAttempts", new[] { "QuizId" });
            DropIndex("dbo.QuizAttempts", new[] { "StudentId" });
            DropIndex("dbo.AttemptAnswers", new[] { "QuestionId" });
            DropIndex("dbo.AttemptAnswers", new[] { "QuizAttemptId" });
            DropTable("dbo.QuizAttempts");
            DropTable("dbo.AttemptAnswers");
        }
    }
}
