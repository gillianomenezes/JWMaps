namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateAddedForHouseholder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "DateAdded", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Householders", "DateAdded");
        }
    }
}
