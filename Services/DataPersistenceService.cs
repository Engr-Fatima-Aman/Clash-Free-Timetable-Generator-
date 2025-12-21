using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dsa_project.Models;
using dsa_project.DSA;

namespace dsa_project.Services
{
    public class DataPersistenceService
    {
        private const string DATA_FOLDER = "TimetableData";
        private const string COURSES_FILE = "courses.csv";
        private const string TEACHERS_FILE = "teachers.csv";
        private const string ROOMS_FILE = "rooms.csv";
        private const string TIMESLOTS_FILE = "timeslots.csv";
        private const string CLASSES_FILE = "classes.csv";
        private const string ASSIGNMENTS_FILE = "assignments.csv";

        public DataPersistenceService()
        {
            if (!Directory.Exists(DATA_FOLDER))
                Directory.CreateDirectory(DATA_FOLDER);
        }

        // --- COURSES (Updated: Removed LabType) ---
        private void SaveCourses(CourseHashTable courses)
        {
            // Header updated
            var lines = new List<string> { "Id,Title,CreditHours,IsLab" };
            foreach (var c in courses.GetAllCourses())
            {
                lines.Add($"{c.Id},{c.Title},{c.CreditHours},{c.IsLab}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, COURSES_FILE), lines);
        }

        private void LoadCourses(CourseHashTable courses)
        {
            string path = Path.Combine(DATA_FOLDER, COURSES_FILE);
            if (!File.Exists(path)) return;
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) {
                var p = lines[i].Split(',');
                if (p.Length >= 4) { 
                    try {
                        courses.AddCourse(new Course { 
                            Id = int.Parse(p[0]), 
                            Title = p[1].Trim(), 
                            CreditHours = int.Parse(p[2]), 
                            IsLab = bool.Parse(p[3])
                        });
                    } catch { continue; }
                }
            }
        }

        // --- ROOMS (Updated: Removed LabType) ---
        private void SaveRooms(RoomHashTable rooms)
        {
            var lines = new List<string> { "Id,RoomNumber,Capacity,IsLabRoom" };
            foreach (var r in rooms.GetAllRooms())
            {
                lines.Add($"{r.Id},{r.RoomNumber},{r.Capacity},{r.IsLabRoom}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, ROOMS_FILE), lines);
        }

        private void LoadRooms(RoomHashTable rooms)
        {
            string path = Path.Combine(DATA_FOLDER, ROOMS_FILE);
            if (!File.Exists(path)) return;
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) {
                var p = lines[i].Split(',');
                if (p.Length >= 4) {
                    try {
                        rooms.AddRoom(new Room { 
                            Id = int.Parse(p[0]), 
                            RoomNumber = p[1].Trim(), 
                            Capacity = int.Parse(p[2]),
                            IsLabRoom = bool.Parse(p[3])
                        });
                    } catch { continue; }
                }
            }
        }

        // --- TEACHERS, CLASSES, TIMESLOTS & ASSIGNMENTS (Same as before) ---
        private void SaveTeachers(TeacherHashTable teachers)
        {
            var lines = new List<string> { "Id,Name,AssignedCourses" };
            foreach (var t in teachers.GetAllTeachers())
            {
                string cIds = string.Join(";", t.AssignedCourseIds);
                lines.Add($"{t.Id},{t.Name},{cIds}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, TEACHERS_FILE), lines);
        }

        private void LoadTeachers(TeacherHashTable teachers)
        {
            string path = Path.Combine(DATA_FOLDER, TEACHERS_FILE);
            if (!File.Exists(path)) return;
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) {
                var p = lines[i].Split(',');
                if (p.Length >= 2) {
                    try {
                        var t = new Teacher { Id = int.Parse(p[0]), Name = p[1].Trim() };
                        if (p.Length > 2 && !string.IsNullOrWhiteSpace(p[2])) 
                            t.AssignedCourseIds = p[2].Split(';').Select(s => int.Parse(s.Trim())).ToList();
                        teachers.AddTeacher(t);
                    } catch { continue; }
                }
            }
        }

        private void SaveClasses(ClassHashTable classes)
        {
            var lines = new List<string> { "Id,Name,Semester,Section,TotalStudents,CourseIds" };
            foreach (var c in classes.GetAllClasses())
            {
                string cIds = string.Join(";", c.CourseIds);
                lines.Add($"{c.Id},{c.Name},{c.Semester},{c.Section},{c.TotalStudents},{cIds}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, CLASSES_FILE), lines);
        }

        private void LoadClasses(ClassHashTable classes)
        {
            string path = Path.Combine(DATA_FOLDER, CLASSES_FILE);
            if (!File.Exists(path)) return;
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) {
                var p = lines[i].Split(',');
                if (p.Length >= 5) { 
                    try {
                        var c = new Class { 
                            Id = int.Parse(p[0]), 
                            Name = p[1].Trim(), 
                            Semester = int.Parse(p[2]), 
                            Section = p[3].Trim(), 
                            TotalStudents = int.Parse(p[4]) 
                        };
                        if (p.Length > 5 && !string.IsNullOrWhiteSpace(p[5])) 
                            c.CourseIds = p[5].Split(';').Select(s => int.Parse(s.Trim())).ToList();
                        classes.AddClass(c);
                    } catch { continue; }
                }
            }
        }

        private void SaveTimeSlots(TimeSlotHashTable timeSlots)
        {
            var lines = new List<string> { "Id,Day,StartTime,EndTime" };
            foreach (var s in timeSlots.GetAllTimeSlots())
                lines.Add($"{s.Id},{s.Day},{s.StartTime},{s.EndTime}");
            File.WriteAllLines(Path.Combine(DATA_FOLDER, TIMESLOTS_FILE), lines);
        }

        private void LoadTimeSlots(TimeSlotHashTable timeSlots)
        {
            string path = Path.Combine(DATA_FOLDER, TIMESLOTS_FILE);
            if (!File.Exists(path)) return;
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) {
                var p = lines[i].Split(',');
                if (p.Length >= 4) {
                    timeSlots.AddTimeSlot(new TimeSlot { 
                        Id = int.Parse(p[0]), 
                        Day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), p[1]), 
                        StartTime = TimeSpan.Parse(p[2]), 
                        EndTime = TimeSpan.Parse(p[3]) 
                    });
                }
            }
        }

        private void SaveAssignments(List<TimetableAssignment> assignments)
        {
            var lines = new List<string> { "ClassId,ClassName,Section,CourseId,TeacherId,RoomId,TimeSlotId" };
            foreach (var a in assignments)
                lines.Add($"{a.ClassId},{a.ClassName},{a.Section},{a.CourseId},{a.TeacherId},{a.RoomId},{a.TimeSlotId}");
            File.WriteAllLines(Path.Combine(DATA_FOLDER, ASSIGNMENTS_FILE), lines);
        }

        private void LoadAssignments(out List<TimetableAssignment> assignments)
        {
            assignments = new List<TimetableAssignment>();
            string path = Path.Combine(DATA_FOLDER, ASSIGNMENTS_FILE);
            if (!File.Exists(path)) return;
            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++) {
                var p = lines[i].Split(',');
                if (p.Length >= 7) {
                    assignments.Add(new TimetableAssignment { 
                        ClassId=int.Parse(p[0]), ClassName=p[1], Section=p[2], CourseId=int.Parse(p[3]), 
                        TeacherId=int.Parse(p[4]), RoomId=int.Parse(p[5]), TimeSlotId=int.Parse(p[6]) 
                    });
                }
            }
        }

        public bool SaveAllData(CourseHashTable courses, TeacherHashTable teachers,
            RoomHashTable rooms, TimeSlotHashTable timeSlots, ClassHashTable classes,
            List<TimetableAssignment> assignments)
        {
            try {
                SaveCourses(courses); SaveTeachers(teachers); SaveRooms(rooms);
                SaveTimeSlots(timeSlots); SaveClasses(classes); SaveAssignments(assignments);
                return true;
            } catch { return false; }
        }

        public bool LoadAllData(CourseHashTable courses, TeacherHashTable teachers,
            RoomHashTable rooms, TimeSlotHashTable timeSlots, ClassHashTable classes,
            out List<TimetableAssignment> assignments)
        {
            assignments = new List<TimetableAssignment>();
            try {
                LoadCourses(courses); LoadTeachers(teachers); LoadRooms(rooms);
                LoadTimeSlots(timeSlots); LoadClasses(classes); LoadAssignments(out assignments);
                return true;
            } catch { return false; }
        }
    }
}