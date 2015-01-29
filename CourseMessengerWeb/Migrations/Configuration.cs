using System.Collections.Generic;
using CourseMessengerWeb.Components;
using CourseMessengerWeb.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CourseMessengerWeb.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CourseMessengerWeb.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CourseMessengerWeb.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);


            foreach (var role in ApplicationRoles.GetAllRoles())
            {
                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }    
            }



            context.Departments.AddOrUpdate(d => d.Name, new Department
                                                         {
                                                             Name ="Computer Engineering",
                                                             
                                                         });


            context.Courses.AddOrUpdate(c=>c.Code,new []{new Course
            {
                Id=1,
                DepartmentId=1,
                Code="CPEN 403",
                Description= "Control Systems",
                Name= "Control Systems",
                ShowCourse =true,
            },new Course
            {
                Id=2,
                DepartmentId=1,
                Code = "CPEN 401",
                Name = "Artificial Intelligence",
                Description = "Artificial Intelligence",
                ShowCourse=true,
            }});

            //context.ExamTimeTables.AddOrUpdate(c=>c.CourseId, new []
            //{
               
            //     new ExamTimeTable
            //    {
            //        Id=1,
            //        CourseId = 1,
            //        StartTime = DateTime.Now.AddDays(7),
            //        EndTime = DateTime.Now.AddDays(7).AddHours(2),
            //        ReminderType = 1,
            //        Status = 1
            //    },  new ExamTimeTable
            //    {
            //        Id=2,
            //        CourseId = 2,
            //        StartTime = DateTime.Now.AddDays(6),
            //        EndTime = DateTime.Now.AddDays(6).AddHours(2),
            //        ReminderType = 1,
            //        Status = 1
            //    }, 
                
            //});

            //context.ReminderMessages.AddOrUpdate(r=>r.Message,new []
            //                                                  {
            //                                                      new ReminderMessage
            //                                                      {
            //                                                          Id=1,
            //                                                          Message = "Hi [Firstname], [Course] starts at [StartTime] and ends at [EndTime]",
            //                                                          ReminderType = (int)SubscriptionType.ExamTimeTable,
            //                                                      }
            //                                                  });

        }
    }
}
