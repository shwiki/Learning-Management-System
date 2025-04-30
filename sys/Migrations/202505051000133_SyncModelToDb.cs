namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncModelToDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PhotoPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "PhotoPath");
        }
    }
}
