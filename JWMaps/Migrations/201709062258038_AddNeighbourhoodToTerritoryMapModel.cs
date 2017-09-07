namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNeighbourhoodToTerritoryMapModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TerritoryMaps", "Neighbourhood", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TerritoryMaps", "Neighbourhood");
        }
    }
}
