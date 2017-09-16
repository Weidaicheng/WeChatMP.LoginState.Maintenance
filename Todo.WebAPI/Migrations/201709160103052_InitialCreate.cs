namespace Todo.WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Todoes",
                c => new
                    {
                        TodoId = c.Guid(nullable: false),
                        Title = c.String(unicode: false),
                        Content = c.String(unicode: false),
                        UseAlert = c.Boolean(nullable: false),
                        AlertTime = c.DateTime(precision: 0),
                        AlertBeforeMinutes = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false, precision: 0),
                        UpdateTime = c.DateTime(precision: 0),
                        FormId = c.String(unicode: false),
                        TodoUser_TodoUserId = c.Guid(),
                    })
                .PrimaryKey(t => t.TodoId)
                .ForeignKey("dbo.TodoUsers", t => t.TodoUser_TodoUserId)
                .Index(t => t.TodoUser_TodoUserId);
            
            CreateTable(
                "dbo.TodoUsers",
                c => new
                    {
                        TodoUserId = c.Guid(nullable: false),
                        OpenId = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.TodoUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Todoes", "TodoUser_TodoUserId", "dbo.TodoUsers");
            DropIndex("dbo.Todoes", new[] { "TodoUser_TodoUserId" });
            DropTable("dbo.TodoUsers");
            DropTable("dbo.Todoes");
        }
    }
}
