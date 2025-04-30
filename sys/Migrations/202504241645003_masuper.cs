namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class masuper : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ContactNumber", c => c.String());
            AddColumn("dbo.AspNetUsers", "ClassName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ClassName");
            DropColumn("dbo.AspNetUsers", "ContactNumber");
        }
    }
}
