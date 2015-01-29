using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseMessengerWeb.Components
{
    public class ApplicationRoles
    {
        public const string Administrator = "Administrator";
        public const string Student = "Student";

        public static string[] GetAllRoles()
        {
            return new[] {Administrator, Student};
        }

        
    }

    
     
 
}