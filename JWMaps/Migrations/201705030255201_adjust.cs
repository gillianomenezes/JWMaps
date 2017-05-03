namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adjust : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "TerritoryMapId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Householders", "TerritoryMapId");
        }
    }
}
