namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeschema : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PendingUsers", "ClassName", c => c.Int(nullable: false));
        }

        public override void Down()
        {
        }
    }
}
