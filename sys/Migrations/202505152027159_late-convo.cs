namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lateconvo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Userprofiles", "ClassName", c => c.Int(nullable: false));
            DropColumn("dbo.Userprofiles", "Grade");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Userprofiles", "Grade", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.Userprofiles", "ClassName");
        }
    }
}
