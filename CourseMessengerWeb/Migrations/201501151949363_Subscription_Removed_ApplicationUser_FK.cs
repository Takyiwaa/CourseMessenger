namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Subscription_Removed_ApplicationUser_FK : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "IndexNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscriptions", "IndexNumber");
        }
    }
}
