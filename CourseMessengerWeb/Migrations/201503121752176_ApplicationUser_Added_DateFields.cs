namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUser_Added_DateFields : DbMigration
    {
        public override void Up()
        {
            //2015-01-29 19:32:00.000
            AddColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false, defaultValueSql: "getdate()"));
            AddColumn("dbo.AspNetUsers", "DateModified", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DateModified");
            DropColumn("dbo.AspNetUsers", "DateCreated");
        }
    }
}
