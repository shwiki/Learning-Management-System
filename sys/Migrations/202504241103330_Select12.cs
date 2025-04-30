 namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Select12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notes", "Subject", c => c.Int(nullable: false));
            DropColumn("dbo.Notes", "CourseCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notes", "CourseCode", c => c.String());
            DropColumn("dbo.Notes", "Subject");
        }
    }
}
