namespace JWMaps.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateHouseholderModel1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Householders", name: "PublisherId", newName: "Publisher_Id");
            RenameIndex(table: "dbo.Householders", name: "IX_PublisherId", newName: "IX_Publisher_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Householders", name: "IX_Publisher_Id", newName: "IX_PublisherId");
            RenameColumn(table: "dbo.Householders", name: "Publisher_Id", newName: "PublisherId");
        }
    }
}
