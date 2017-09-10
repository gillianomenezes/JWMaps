namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVisitModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PublisherName = c.String(),
                        DateOfVisit = c.DateTime(nullable: false),
                        Description = c.String(),
                        Householder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Householders", t => t.Householder_Id)
                .Index(t => t.Householder_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visits", "Householder_Id", "dbo.Householders");
            DropIndex("dbo.Visits", new[] { "Householder_Id" });
            DropTable("dbo.Visits");
        }
    }
}
