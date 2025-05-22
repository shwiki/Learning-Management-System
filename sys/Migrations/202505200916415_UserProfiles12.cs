namespace sys.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UserProfiles12 : DbMigration
    {
        public override void Up()
        {
            // 1) Remove the existing foreign keys
            DropForeignKey("dbo.UserParentChild", "ChildId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserParentChild", "ParentId", "dbo.AspNetUsers");

            // 2) If there was any primary key on the join table, drop it
            DropPrimaryKey("dbo.UserParentChild");

            // 3) Add the composite primary key
            AddPrimaryKey(
                "dbo.UserParentChild",
                new[] { "ParentId", "ChildId" }
            );

            // 4) Re-create the FKs with cascadeDelete:false
            AddForeignKey(
                "dbo.UserParentChild",
                "ParentId",
                "dbo.AspNetUsers",
                "Id",
                cascadeDelete: false
            );
            AddForeignKey(
                "dbo.UserParentChild",
                "ChildId",
                "dbo.AspNetUsers",
                "Id",
                cascadeDelete: false
            );
        }

        public override void Down()
        {
            // Reverse operations
            DropForeignKey("dbo.UserParentChild", "ChildId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserParentChild", "ParentId", "dbo.AspNetUsers");
            DropPrimaryKey("dbo.UserParentChild");

            // Optionally re-create whatever key you had before (or leave it PK-less)
            // If your old implicit M2M had no PK, you can skip AddPrimaryKey here.
            // Otherwise re-create the old key:
            // AddPrimaryKey("dbo.UserParentChild", new[] { "ChildId", "ParentId" });

            AddForeignKey(
                "dbo.UserParentChild",
                "ParentId",
                "dbo.AspNetUsers",
                "Id",
                cascadeDelete: false
            );
            AddForeignKey(
                "dbo.UserParentChild",
                "ChildId",
                "dbo.AspNetUsers",
                "Id",
                cascadeDelete: false
            );
        }
    }
}
