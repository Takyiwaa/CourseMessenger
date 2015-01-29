namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Courses_AddedField_ShowCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "ShowCourse", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "ShowCourse");
        }
    }
}
