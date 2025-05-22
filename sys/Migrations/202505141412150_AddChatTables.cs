namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChatTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageReads",
                c => new
                    {
                        MessageId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ReadAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MessageId, t.UserId })
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.MessageId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassId = c.Int(nullable: false),
                        SenderId = c.String(nullable: false, maxLength: 128),
                        RecipientId = c.String(maxLength: 128),
                        Content = c.String(nullable: false),
                        SentAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.RecipientId)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderId)
                .Index(t => t.SenderId)
                .Index(t => t.RecipientId);
            
            CreateTable(
                "dbo.Userprofiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Grade = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Userprofiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageReads", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageReads", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.Messages", "SenderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "RecipientId", "dbo.AspNetUsers");
            DropIndex("dbo.Userprofiles", new[] { "UserId" });
            DropIndex("dbo.Messages", new[] { "RecipientId" });
            DropIndex("dbo.Messages", new[] { "SenderId" });
            DropIndex("dbo.MessageReads", new[] { "UserId" });
            DropIndex("dbo.MessageReads", new[] { "MessageId" });
            DropTable("dbo.Userprofiles");
            DropTable("dbo.Messages");
            DropTable("dbo.MessageReads");
        }
    }
}
