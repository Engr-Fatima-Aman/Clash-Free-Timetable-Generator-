using System;
using System.Collections.Generic;

namespace dsa_project.Models
{
    // LabCategory Enum ko poora delete kar diya gaya hai yahan se

    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CreditHours { get; set; }
        
        // Sirf ye flag kafi hai scheduler ke liye
        public bool IsLab { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public int Capacity { get; set; }
        
        // Sirf ye batana kafi hai ke ye Room hai ya Lab
        public bool IsLabRoom { get; set; }
    }

    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> AssignedCourseIds { get; set; } = new List<int>();
        public string TeacherType { get; set; } // "Permanent" or "Visiting"
    }

    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; } // BSE
        public int Semester { get; set; }
        public string Section { get; set; }
        public int TotalStudents { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
    }

    public class TimeSlot
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        
        // Calculated property
        public double DurationInHours => (EndTime - StartTime).TotalHours;
    }

    public class TimetableAssignment
    {
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public int RoomId { get; set; }
        public int TimeSlotId { get; set; }

        // Display properties
        public string ClassName { get; set; }
        public string Section { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public string RoomName { get; set; }
        public string TimeSlotInfo { get; set; }
    }
}