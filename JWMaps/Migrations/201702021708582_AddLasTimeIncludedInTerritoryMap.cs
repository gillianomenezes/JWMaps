namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLasTimeIncludedInTerritoryMap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "LasTimeIncludedInTerritoryMap", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Householders", "LasTimeIncludedInTerritoryMap");
        }
    }
}
