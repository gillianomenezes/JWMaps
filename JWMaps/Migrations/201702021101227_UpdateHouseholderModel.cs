namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateHouseholderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "Confirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Householders", "Category", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Householders", "Category");
            DropColumn("dbo.Householders", "Confirmed");
        }
    }
}
