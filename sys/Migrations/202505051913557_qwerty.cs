namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qwerty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreateAssignments", "Subject", c => c.Int(nullable: false));
            AddColumn("dbo.CreateAssignments", "ClassName", c => c.Int(nullable: false));
            AddColumn("dbo.CreateQs", "ClassName", c => c.Int(nullable: false));
            AlterColumn("dbo.CreateAssignments", "FilePath", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CreateAssignments", "FilePath", c => c.String(maxLength: 255));
            DropColumn("dbo.CreateQs", "ClassName");
            DropColumn("dbo.CreateAssignments", "ClassName");
            DropColumn("dbo.CreateAssignments", "Subject");
        }
    }
}
