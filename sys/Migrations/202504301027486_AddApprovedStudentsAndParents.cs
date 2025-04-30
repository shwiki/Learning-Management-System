namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApprovedStudentsAndParents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApprovedParents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        PhotoPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApprovedStudents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        ClassName = c.Int(nullable: false),
                        PhotoPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApprovedStudents");
            DropTable("dbo.ApprovedParents");
        }
    }
}
