namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LectureHours_added_status : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LectureHours", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LectureHours", "Status");
        }
    }
}
