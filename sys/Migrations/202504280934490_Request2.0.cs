namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Request20 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PendingUsers", "RequestedRole", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.PendingUsers", "RequestedRole", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
