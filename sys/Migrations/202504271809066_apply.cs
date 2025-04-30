namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apply : DbMigration
    {
        public override void Up()
        {
            AlterColumn(
                  "dbo.AspNetUsers",
                  "ClassName",
                  c => c.Int(nullable: true)
                  );
        }
        
        public override void Down()
        {
            AlterColumn(
                "dbo.AspNetUsers",
                "ClassName",
                c => c.Int(nullable: false));
        }
    }
}
