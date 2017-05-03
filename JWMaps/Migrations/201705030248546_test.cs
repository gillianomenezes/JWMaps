namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Householders", "TerritoryMapId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Householders", "TerritoryMapId", c => c.Int());
        }
    }
}
