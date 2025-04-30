namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedteacher : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreateTeacherViewModels",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 128),
                        ContactNumber = c.String(nullable: false),
                        ClassName = c.Int(nullable: false),
                        PhotoPath = c.String(),
                    })
                .PrimaryKey(t => t.Email);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CreateTeacherViewModels");
        }
    }
}
