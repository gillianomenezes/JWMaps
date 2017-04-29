namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustTerritoryMapModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "TerritoryMap_Id", c => c.Int());
            AddColumn("dbo.TerritoryMaps", "CreationDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Householders", "TerritoryMap_Id");
            AddForeignKey("dbo.Householders", "TerritoryMap_Id", "dbo.TerritoryMaps", "Id");
            DropColumn("dbo.Householders", "TerritoryMapId");
            DropColumn("dbo.Householders", "LastTimeIncludedInTerritoryMap");
            DropColumn("dbo.TerritoryMaps", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TerritoryMaps", "Name", c => c.String());
            AddColumn("dbo.Householders", "LastTimeIncludedInTerritoryMap", c => c.DateTime());
            AddColumn("dbo.Householders", "TerritoryMapId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Householders", "TerritoryMap_Id", "dbo.TerritoryMaps");
            DropIndex("dbo.Householders", new[] { "TerritoryMap_Id" });
            DropColumn("dbo.TerritoryMaps", "CreationDate");
            DropColumn("dbo.Householders", "TerritoryMap_Id");
        }
    }
}
