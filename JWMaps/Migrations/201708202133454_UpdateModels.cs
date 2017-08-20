namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Householders", "IsStudying", c => c.Boolean(nullable: false));
            DropColumn("dbo.Householders", "PublisherId");
            DropColumn("dbo.Householders", "Confirmed");
            DropColumn("dbo.Householders", "TerritoryMapId");
            DropTable("dbo.Publishers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Publishers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Householders", "TerritoryMapId", c => c.Int());
            AddColumn("dbo.Householders", "Confirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Householders", "PublisherId", c => c.Int());
            DropColumn("dbo.Householders", "IsStudying");
        }
    }
}
