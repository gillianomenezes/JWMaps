namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateHouseholderLasTimeVisitedControl : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Householders", "LastTimeVisited");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Householders", "LastTimeVisited", c => c.DateTime());
        }
    }
}
