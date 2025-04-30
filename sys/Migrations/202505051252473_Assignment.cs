namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assignment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreateAssignments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        FilePath = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CreateAssignments");
        }
    }
}
