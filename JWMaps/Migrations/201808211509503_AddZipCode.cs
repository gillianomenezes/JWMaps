namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddZipCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "ZipCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Householders", "ZipCode");
        }
    }
}
