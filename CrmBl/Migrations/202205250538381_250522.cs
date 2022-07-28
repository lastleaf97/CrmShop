namespace CrmBl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _250522 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sellers", "Login", c => c.String());
            AddColumn("dbo.Sellers", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sellers", "Password");
            DropColumn("dbo.Sellers", "Login");
        }
    }
}
