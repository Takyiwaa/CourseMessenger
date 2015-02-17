namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class created_NewsTips_model_cont_view : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NewsTips",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReminderType = c.Int(nullable: false),
                        Name = c.String(),
                        StartTime = c.Time(nullable: false, precision: 7),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Subscriptions", "NewsTips_Id", c => c.Int());
            CreateIndex("dbo.Subscriptions", "NewsTips_Id");
            AddForeignKey("dbo.Subscriptions", "NewsTips_Id", "dbo.NewsTips", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "NewsTips_Id", "dbo.NewsTips");
            DropIndex("dbo.Subscriptions", new[] { "NewsTips_Id" });
            DropColumn("dbo.Subscriptions", "NewsTips_Id");
            DropTable("dbo.NewsTips");
        }
    }
}
