namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notesupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notes", "ClassName", c => c.Int(nullable: false));
            DropColumn("dbo.Notes", "Level");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notes", "Level", c => c.Int(nullable: false));
            DropColumn("dbo.Notes", "ClassName");
        }
    }
}
