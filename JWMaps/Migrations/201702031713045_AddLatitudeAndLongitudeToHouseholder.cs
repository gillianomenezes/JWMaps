namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLatitudeAndLongitudeToHouseholder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Householders", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Householders", "Longitude");
            DropColumn("dbo.Householders", "Latitude");
        }
    }
}
