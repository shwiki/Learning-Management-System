namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PendingUser1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PendingUsers",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Email = c.String(nullable: false, maxLength: 256),
                    FirstName = c.String(nullable: false, maxLength: 100),
                    LastName = c.String(nullable: false, maxLength: 100),
                    ClassName = c.String(nullable: false, maxLength: 100),
                    Age = c.Int(nullable: false),
                    PhotoPath = c.String(),                  // store the relative path/filename
                    RequestedRole = c.String(nullable: false, maxLength: 50),
                    AppliedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.PendingUsers");
        }
    }
}