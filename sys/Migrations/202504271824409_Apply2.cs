namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Apply2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "ClassName", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "ClassName", c => c.Int(nullable: false));
        }
    }
}
