namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSharedDateInTerritoryMapToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TerritoryMaps", "SharedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TerritoryMaps", "SharedDate", c => c.DateTime(nullable: false));
        }
    }
}
