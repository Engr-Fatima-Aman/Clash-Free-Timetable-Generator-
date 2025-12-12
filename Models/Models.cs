using System;
using System.Collections.Generic;

namespace dsa_project.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int CreditHours { get; set; }
        public int PrerequisiteId { get; set; }

        public override string ToString() => $"{Code} - {Title} ({CreditHours}h)";
    }

    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeId { get; set; }
        public string Type { get; set; } // "Permanent" or "Visiting"
        public List<int> AvailableTimeSlots { get; set; } = new List<int>();
        public List<int> AssignedCourseIds { get; set; } = new List<int>();

        public override string ToString() => $"{Name} ({EmployeeId}) - {Type}";
    }

    public class TimeSlot
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public override string ToString() => $"{Day} {StartTime:HH:mm}-{EndTime:HH:mm}";
    }

    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public int Capacity { get; set; }

        public override string ToString() => $"{RoomNumber} (Cap: {Capacity})";
    }

    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; } // "BSE 1(A)"
        public int Semester { get; set; }
        public char Section { get; set; }
        public int TotalStudents { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();

        public override string ToString() => Name;
    }

    public class TimetableAssignment
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public int RoomId { get; set; }
        public int TimeSlotId { get; set; }

        // Web View ke liye extra properties (Optional helpers)
        // Note: Ye database/CSV mein save nahi honge, sirf dikhane ke liye hain
        public string ClassName { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public string RoomName { get; set; }
        public string TimeSlotInfo { get; set; }

        public override string ToString() => $"Class {ClassId}: Course {CourseId} at Slot {TimeSlotId}";
    }

    public class TeacherTimetableAssignment
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int RoomId { get; set; }
        public int TimeSlotId { get; set; }
    }
}