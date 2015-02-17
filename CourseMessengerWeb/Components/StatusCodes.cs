using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseMessengerWeb.Components
{
    public class StatusCodes
    {
        public class ReminderStatusCodes
        {

            public static Dictionary<int, string> All
            {
                get
                {
                    return new Dictionary<int, string>()
                           {
                               {Active,"Active"},
                               {Inactive,"Inactive"}
                           };
                }
            }

            public static int Active
            {
                get { return 1; }
            }

            public static int Inactive
            {
                get { return 0; }
            }

        }

        public class ReminderTypes
        {
            public static int ExamTimeTable
            {
                get { return 1; }
            }

            public static Dictionary<int, string> All
            {
                get
                {
                    return new Dictionary<int, string>()
                           {
                               {ExamTimeTable,"Exam Time Table"},
                               {LectureHours,"Lecture Hours"},
                               {NewsTips,"News Tips"},
                           };
                }
            }

            public static int LectureHours
            {
                get { return 2; }
            }

            public static int NewsTips
            {
                get { return 3; }
            }
        }
    }
}