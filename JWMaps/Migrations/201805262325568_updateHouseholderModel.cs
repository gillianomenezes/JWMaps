namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateHouseholderModel : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Householders", "PublisherId");
            AddForeignKey("dbo.Householders", "PublisherId", "dbo.Publishers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Householders", "PublisherId", "dbo.Publishers");
            DropIndex("dbo.Householders", new[] { "PublisherId" });
        }
    }
}
