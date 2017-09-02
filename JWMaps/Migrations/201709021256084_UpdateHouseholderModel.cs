namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateHouseholderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "PublisherId", c => c.Int(nullable: false));
            AddColumn("dbo.Publishers", "CongregationId", c => c.Int(nullable: false));
            DropColumn("dbo.Householders", "IsStudying");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Householders", "IsStudying", c => c.Boolean(nullable: false));
            DropColumn("dbo.Publishers", "CongregationId");
            DropColumn("dbo.Householders", "PublisherId");
        }
    }
}
