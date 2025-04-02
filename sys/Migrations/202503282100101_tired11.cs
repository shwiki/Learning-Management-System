namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tired11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notes",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CourseCode = c.String(),
                    Level = c.Int(nullable: false),
                    Topic = c.String(),
                    KeyContent = c.String(),
                    Description = c.String(),
                    Image = c.String(),
                })
                .PrimaryKey(t => t.id);
        }
        
        public override void Down()
        {
           DropTable("dbo.Notes");
        }
    }
}
