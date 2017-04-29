namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixHouseholderAttributeName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "LastTimeIncludedInTerritoryMap", c => c.DateTime());
            DropColumn("dbo.Householders", "LasTimeIncludedInTerritoryMap");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Householders", "LasTimeIncludedInTerritoryMap", c => c.DateTime());
            DropColumn("dbo.Householders", "LastTimeIncludedInTerritoryMap");
        }
    }
}
