namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTerritoryMap1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TerritoryMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TerritoryMaps");
        }
    }
}
