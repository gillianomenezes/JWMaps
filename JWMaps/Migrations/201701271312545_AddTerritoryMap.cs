namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTerritoryMap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "TerritoryMapId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Householders", "TerritoryMapId");
        }
    }
}
