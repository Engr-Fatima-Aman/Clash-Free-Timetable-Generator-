using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dsa_project.DSA;
using dsa_project.Models;

namespace dsa_project.Services
{
    public class ExportService
    {
        public static void ExportTimetable(List<TimetableAssignment> assignments,
        ClassHashTable classTable, CourseHashTable courseTable,
        TeacherHashTable teacherTable, RoomHashTable roomTable,
        TimeSlotHashTable timeSlotTable, string filePath)
        {
            // Web environment mein path ensure karein
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string extension = Path.GetExtension(filePath).ToLower();
            if (extension == ".csv")
                ExportToCSV(assignments, classTable, courseTable, teacherTable, roomTable, timeSlotTable, filePath);
            else
                ExportToText(assignments, classTable, courseTable, teacherTable, roomTable, timeSlotTable, filePath);
        }

        private static void ExportToText(List<TimetableAssignment> assignments,
            ClassHashTable classTable, CourseHashTable courseTable,
            TeacherHashTable teacherTable, RoomHashTable roomTable,
            TimeSlotHashTable timeSlotTable, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("    DEPARTMENT TIMETABLE REPORT");
            sb.AppendLine("========================================");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Total Assignments: {assignments.Count}");
            sb.AppendLine("========================================\n");

            // Class timetables
            sb.AppendLine("CLASS TIMETABLES");
            sb.AppendLine("----------------------------------------");

            var groupedByClass = assignments.GroupBy(a => a.ClassId);
            foreach (var classGroup in groupedByClass)
            {
                var cls = classTable.GetClassById(classGroup.Key);
                sb.AppendLine($"\nClass: {cls.Name} (Students: {cls.TotalStudents})");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine(string.Format("{0,-20} {1,-15} {2,-20} {3,-15}",
                    "Time Slot", "Course", "Teacher", "Room"));
                sb.AppendLine(new string('-', 70));

                foreach (var assignment in classGroup.OrderBy(a => a.TimeSlotId))
                {
                    var course = courseTable.GetCourseById(assignment.CourseId);
                    var teacher = teacherTable.GetTeacherById(assignment.TeacherId);
                    var room = roomTable.GetRoomById(assignment.RoomId);
                    var timeSlot = timeSlotTable.GetTimeSlotById(assignment.TimeSlotId);

                    string timeStr = timeSlot != null ?
                        $"{timeSlot.Day} {timeSlot.StartTime:HH:mm}-{timeSlot.EndTime:HH:mm}" : "N/A";
                    string courseStr = course?.Code ?? "N/A";
                    string teacherStr = teacher?.Name ?? "N/A";
                    string roomStr = room?.RoomNumber ?? "N/A";

                    sb.AppendLine(string.Format("{0,-20} {1,-15} {2,-20} {3,-15}",
                        timeStr, courseStr, teacherStr, roomStr));
                }
            }

            // Teacher timetables
            sb.AppendLine("\n========================================");
            sb.AppendLine("TEACHER TIMETABLES");
            sb.AppendLine("----------------------------------------");

            var groupedByTeacher = assignments.GroupBy(a => a.TeacherId);
            foreach (var teacherGroup in groupedByTeacher)
            {
                var teacher = teacherTable.GetTeacherById(teacherGroup.Key);
                sb.AppendLine($"\nTeacher: {teacher.Name} ({teacher.EmployeeId})");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine(string.Format("{0,-20} {1,-15} {2,-15} {3,-15}",
                    "Time Slot", "Class", "Course", "Room"));
                sb.AppendLine(new string('-', 65));

                foreach (var assignment in teacherGroup.OrderBy(a => a.TimeSlotId))
                {
                    var cls = classTable.GetClassById(assignment.ClassId);
                    var course = courseTable.GetCourseById(assignment.CourseId);
                    var room = roomTable.GetRoomById(assignment.RoomId);
                    var timeSlot = timeSlotTable.GetTimeSlotById(assignment.TimeSlotId);

                    string timeStr = timeSlot != null ?
                        $"{timeSlot.Day} {timeSlot.StartTime:HH:mm}-{timeSlot.EndTime:HH:mm}" : "N/A";
                    string classStr = cls?.Name ?? "N/A";
                    string courseStr = course?.Code ?? "N/A";
                    string roomStr = room?.RoomNumber ?? "N/A";

                    sb.AppendLine(string.Format("{0,-20} {1,-15} {2,-15} {3,-15}",
                        timeStr, classStr, courseStr, roomStr));
                }
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        private static void ExportToCSV(List<TimetableAssignment> assignments,
            ClassHashTable classTable, CourseHashTable courseTable,
            TeacherHashTable teacherTable, RoomHashTable roomTable,
            TimeSlotHashTable timeSlotTable, string filePath)
        {
            var lines = new List<string>();
            lines.Add("Class,Course,Teacher,Room,Day,TimeSlot");

            foreach (var assignment in assignments.OrderBy(a => a.ClassId).ThenBy(a => a.TimeSlotId))
            {
                var cls = classTable.GetClassById(assignment.ClassId);
                var course = courseTable.GetCourseById(assignment.CourseId);
                var teacher = teacherTable.GetTeacherById(assignment.TeacherId);
                var room = roomTable.GetRoomById(assignment.RoomId);
                var timeSlot = timeSlotTable.GetTimeSlotById(assignment.TimeSlotId);

                string classStr = cls?.Name ?? "N/A";
                string courseStr = course?.Code ?? "N/A";
                string teacherStr = teacher?.Name ?? "N/A";
                string roomStr = room?.RoomNumber ?? "N/A";
                string dayStr = timeSlot?.Day.ToString() ?? "N/A";
                string timeStr = timeSlot != null ?
                    $"{timeSlot.StartTime:HH:mm}-{timeSlot.EndTime:HH:mm}" : "N/A";

                lines.Add($"{classStr},{courseStr},{teacherStr},{roomStr},{dayStr},{timeStr}");
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}