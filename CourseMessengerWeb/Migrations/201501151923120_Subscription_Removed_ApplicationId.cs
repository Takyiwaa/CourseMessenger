namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Subscription_Removed_ApplicationId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Subscriptions", "ApplicationUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subscriptions", "ApplicationUserId", c => c.Int(nullable: false));
        }
    }
}
