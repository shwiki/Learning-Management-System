namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParentChildRelation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserParentChild",
                c => new
                    {
                        ParentId = c.String(nullable: false, maxLength: 128),
                        ChildId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ParentId, t.ChildId })
                .ForeignKey("dbo.AspNetUsers", t => t.ParentId)
                .ForeignKey("dbo.AspNetUsers", t => t.ChildId)
                .Index(t => t.ParentId)
                .Index(t => t.ChildId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserParentChild", "ChildId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserParentChild", "ParentId", "dbo.AspNetUsers");
            DropIndex("dbo.UserParentChild", new[] { "ChildId" });
            DropIndex("dbo.UserParentChild", new[] { "ParentId" });
            DropTable("dbo.UserParentChild");
        }
    }
}
