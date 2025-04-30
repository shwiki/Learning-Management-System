namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PendingUser2 : DbMigration
    {
        public override void Up()
        {
            AddColumn(
                "dbo.PendingUsers",
                "DateOfBirth",
                c => c.DateTime(nullable: false));

            // 2) Migrate data from Age → DateOfBirth if you have existing rows
            //    (optional—comment out if you don’t need to preserve old data)
            // Sql("UPDATE dbo.PendingUsers SET DateOfBirth = DATEADD(year, -Age, GETDATE())");

            // 3) Drop the old Age column
            DropColumn("dbo.PendingUsers", "Age");
        }
        
        public override void Down()
        {
            AddColumn(
                "dbo.PendingUsers",
                "Age",
                c => c.Int(nullable: true));

            // 2) (Optional) repopulate Age from DateOfBirth
            // Sql("UPDATE dbo.PendingUsers SET Age = DATEDIFF(year, DateOfBirth, GETDATE())");

            // 3) Drop the DateOfBirth column
            DropColumn("dbo.PendingUsers", "DateOfBirth");
        }
    }
}
