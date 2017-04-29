namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTerritoryMapIdToHouseholderModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Householders", "TerritoryMap_Id", "dbo.TerritoryMaps");
            DropIndex("dbo.Householders", new[] { "TerritoryMap_Id" });
            AddColumn("dbo.Householders", "TerritoryMapId", c => c.Int());
            AddColumn("dbo.TerritoryMaps", "SharedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Householders", "TerritoryMap_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Householders", "TerritoryMap_Id", c => c.Int());
            DropColumn("dbo.TerritoryMaps", "SharedDate");
            DropColumn("dbo.Householders", "TerritoryMapId");
            CreateIndex("dbo.Householders", "TerritoryMap_Id");
            AddForeignKey("dbo.Householders", "TerritoryMap_Id", "dbo.TerritoryMaps", "Id");
        }
    }
}
