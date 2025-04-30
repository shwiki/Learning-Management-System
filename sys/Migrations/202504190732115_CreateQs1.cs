namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CreateQs1 : DbMigration
    {
        public override void Up()
        {
            // Create the quizzes table
            CreateTable(
                "dbo.CreateQs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    QuizTitle = c.String(nullable: false, maxLength: 100),
                    Subject = c.Int(nullable: false),
                    Instructions = c.String(maxLength: 500),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(nullable: false),
                    PublishGradeAfterSubmission = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            // Create the questions table
            CreateTable(
                "dbo.QuizQAs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    QuizId = c.Int(nullable: false),
                    QuestionText = c.String(nullable: false),
                    OptionA = c.String(nullable: false),
                    OptionB = c.String(nullable: false),
                    OptionC = c.String(nullable: false),
                    OptionD = c.String(nullable: false),
                    Answer = c.String(nullable: false),
                    Marks = c.Int(nullable: false),
                    Difficulty = c.Int(nullable: false),
                    Topic = c.String(nullable: false),
                    Explanation = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CreateQs", t => t.QuizId, cascadeDelete: true)
                .Index(t => t.QuizId);
        }

        public override void Down()
        {
            // Tear down in reverse order
            DropForeignKey("dbo.QuizQAs", "QuizId", "dbo.CreateQs");
            DropIndex("dbo.QuizQAs", new[] { "QuizId" });
            DropTable("dbo.QuizQAs");
            DropTable("dbo.CreateQs");
        }

    }
}