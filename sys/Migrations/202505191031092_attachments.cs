namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class attachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        FilePath = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .Index(t => t.MessageId);
            
            AddColumn("dbo.Messages", "TextContent", c => c.String());
            DropColumn("dbo.Messages", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "Content", c => c.String(nullable: false));
            DropForeignKey("dbo.MessageAttachments", "MessageId", "dbo.Messages");
            DropIndex("dbo.MessageAttachments", new[] { "MessageId" });
            DropColumn("dbo.Messages", "TextContent");
            DropTable("dbo.MessageAttachments");
        }
    }
}
