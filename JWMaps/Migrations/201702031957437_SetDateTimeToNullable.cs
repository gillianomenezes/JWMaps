namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetDateTimeToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Householders", "LasTimeIncludedInTerritoryMap", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Householders", "LasTimeIncludedInTerritoryMap", c => c.DateTime(nullable: false));
        }
    }
}
