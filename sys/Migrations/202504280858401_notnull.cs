namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notnull : DbMigration
    {
        public override void Up()
        {
            // 1) Give every existing user a default ClassName (e.g. Grade_1 = 0)
            Sql("UPDATE dbo.AspNetUsers SET ClassName = 0 WHERE ClassName IS NULL");

            // 2) Now safely alter to NOT NULL
            AlterColumn("dbo.AspNetUsers", "ClassName", c => c.Int(nullable: false));
        }


        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "ClassName", c => c.Int());
        }
    }
}
