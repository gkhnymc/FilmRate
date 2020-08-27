namespace FilmRate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrationv1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        ReleaseDate = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                        FilmID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Film", t => t.FilmID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID)
                .Index(t => t.FilmID);
            
            CreateTable(
                "dbo.Film",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FilmName = c.String(),
                        Genre = c.String(),
                        ReleaseDate = c.DateTime(nullable: false),
                        Director = c.String(),
                        Rate = c.Double(nullable: false),
                        ImagePath = c.String(),
                        FilmDescription = c.String(),
                        TrailerLink = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        UserPassword = c.String(),
                        IsAdminUser = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comment", "UserID", "dbo.User");
            DropForeignKey("dbo.Comment", "FilmID", "dbo.Film");
            DropIndex("dbo.Comment", new[] { "FilmID" });
            DropIndex("dbo.Comment", new[] { "UserID" });
            DropTable("dbo.User");
            DropTable("dbo.Film");
            DropTable("dbo.Comment");
        }
    }
}
