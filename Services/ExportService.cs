using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using dsa_project.Models;
using dsa_project.DSA;

namespace dsa_project.Services
{
    public class ExportService
    {
        public byte[] ExportToCsv(List<TimetableAssignment> assignments, CourseHashTable courses)
        {
            var csv = new StringBuilder();
            // Header ko thora clear rakhte hain
            csv.AppendLine("Class,Section,Course,Teacher,Room,ScheduleInfo");

            foreach (var a in assignments)
            {
                // Course ki check pehle hi assignments mein Name property ki surat mein moojood hai
                // Lekin safety ke liye hum assignments wali properties use karenge jo controller ne populate ki thin
                string className = a.ClassName ?? "N/A";
                string section = a.Section ?? "N/A";
                string courseName = a.CourseName ?? "N/A";
                string teacherName = a.TeacherName ?? "N/A";
                string roomName = a.RoomName ?? "N/A";
                string slotInfo = a.TimeSlotInfo?.Replace(",", "-") ?? "N/A"; // CSV mein comma error na de isliye replace kiya

                csv.AppendLine($"{className},{section},{courseName},{teacherName},{roomName},{slotInfo}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}