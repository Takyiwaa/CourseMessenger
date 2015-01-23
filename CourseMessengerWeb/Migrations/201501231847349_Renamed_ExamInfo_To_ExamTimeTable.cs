namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Renamed_ExamInfo_To_ExamTimeTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ExamInfoes", newName: "ExamTimeTables");
            RenameColumn(table: "dbo.Subscriptions", name: "ExamInfo_Id", newName: "ExamTimeTable_Id");
            RenameIndex(table: "dbo.Subscriptions", name: "IX_ExamInfo_Id", newName: "IX_ExamTimeTable_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Subscriptions", name: "IX_ExamTimeTable_Id", newName: "IX_ExamInfo_Id");
            RenameColumn(table: "dbo.Subscriptions", name: "ExamTimeTable_Id", newName: "ExamInfo_Id");
            RenameTable(name: "dbo.ExamTimeTables", newName: "ExamInfoes");
        }
    }
}
