namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Models_Renamed_Reminders_To_ExamTimeTable_Others : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ExamTimeTables", newName: "ExamTimeTables");
            DropForeignKey("dbo.Subscriptions", "ReminderId", "dbo.ExamTimeTables");
            DropIndex("dbo.Subscriptions", new[] { "ReminderId" });
            RenameColumn(table: "dbo.Subscriptions", name: "ReminderId", newName: "ExamInfo_Id");
            AddColumn("dbo.Subscriptions", "EntityId", c => c.Int(nullable: false));
            AddColumn("dbo.Subscriptions", "SubscriptionType", c => c.Int(nullable: false));
            AlterColumn("dbo.Subscriptions", "ExamInfo_Id", c => c.Int());
            CreateIndex("dbo.Subscriptions", "ExamInfo_Id");
            AddForeignKey("dbo.Subscriptions", "ExamInfo_Id", "dbo.ExamTimeTables", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "ExamInfo_Id", "dbo.ExamTimeTables");
            DropIndex("dbo.Subscriptions", new[] { "ExamInfo_Id" });
            AlterColumn("dbo.Subscriptions", "ExamInfo_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Subscriptions", "SubscriptionType");
            DropColumn("dbo.Subscriptions", "EntityId");
            RenameColumn(table: "dbo.Subscriptions", name: "ExamInfo_Id", newName: "ReminderId");
            CreateIndex("dbo.Subscriptions", "ReminderId");
            AddForeignKey("dbo.Subscriptions", "ReminderId", "dbo.ExamTimeTables", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.ExamTimeTables", newName: "ExamTimeTables");
        }
    }
}
