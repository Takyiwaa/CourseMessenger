namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class created_LectureHour_entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LectureHours",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        Duration = c.Single(nullable: false),
                        DayOfWeek = c.Int(nullable: false),
                        ReminderType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
            AddColumn("dbo.Subscriptions", "LectureHour_Id", c => c.Int());
            CreateIndex("dbo.Subscriptions", "LectureHour_Id");
            AddForeignKey("dbo.Subscriptions", "LectureHour_Id", "dbo.LectureHours", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "LectureHour_Id", "dbo.LectureHours");
            DropForeignKey("dbo.LectureHours", "CourseId", "dbo.Courses");
            DropIndex("dbo.LectureHours", new[] { "CourseId" });
            DropIndex("dbo.Subscriptions", new[] { "LectureHour_Id" });
            DropColumn("dbo.Subscriptions", "LectureHour_Id");
            DropTable("dbo.LectureHours");
        }
    }
}
