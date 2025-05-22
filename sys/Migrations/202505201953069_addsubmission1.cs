namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsubmission1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreateAssignments", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CreateAssignments", "Title");
        }
    }
}
