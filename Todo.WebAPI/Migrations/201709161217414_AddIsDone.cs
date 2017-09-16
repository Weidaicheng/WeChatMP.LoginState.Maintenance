namespace Todo.WebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsDone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Todoes", "IsDone", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Todoes", "IsDone");
        }
    }
}
