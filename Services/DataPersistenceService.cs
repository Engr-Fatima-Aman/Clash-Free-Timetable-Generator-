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
        // Web app mein ye folder project ki root directory mein banega
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

        public bool SaveAllData(CourseHashTable courses, TeacherHashTable teachers,
            RoomHashTable rooms, TimeSlotHashTable timeSlots, ClassHashTable classes,
            List<TimetableAssignment> assignments)
        {
            try
            {
                SaveCourses(courses);
                SaveTeachers(teachers);
                SaveRooms(rooms);
                SaveTimeSlots(timeSlots);
                SaveClasses(classes);
                SaveAssignments(assignments);
                return true;
            }
            catch (Exception ex)
            {
                // Web app mein MessageBox nahi chalta, isliye Console pe log karenge
                Console.WriteLine($"Save failed: {ex.Message}");
                return false;
            }
        }

        public bool LoadAllData(CourseHashTable courses, TeacherHashTable teachers,
            RoomHashTable rooms, TimeSlotHashTable timeSlots, ClassHashTable classes,
            out List<TimetableAssignment> assignments)
        {
            assignments = new List<TimetableAssignment>();
            try
            {
                LoadCourses(courses);
                LoadTeachers(teachers);
                LoadRooms(rooms);
                LoadTimeSlots(timeSlots);
                LoadClasses(classes);
                LoadAssignments(out assignments);
                return true;
            }
            catch (Exception ex)
            {
                // Error ko server terminal par dikhayenge
                Console.WriteLine($"Load failed: {ex.Message}");
                return false;
            }
        }

        private void SaveCourses(CourseHashTable courses)
        {
            var lines = new List<string> { "Id,Code,Title,CreditHours,PrerequisiteId" };
            foreach (var course in courses.GetAllCourses())
            {
                lines.Add($"{course.Id},{course.Code},{course.Title},{course.CreditHours},{course.PrerequisiteId}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, COURSES_FILE), lines);
        }

        private void LoadCourses(CourseHashTable courses)
        {
            string path = Path.Combine(DATA_FOLDER, COURSES_FILE);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                courses.AddCourse(new Course
                {
                    Id = int.Parse(parts[0]),
                    Code = parts[1],
                    Title = parts[2],
                    CreditHours = int.Parse(parts[3]),
                    PrerequisiteId = int.Parse(parts[4])
                });
            }
        }

        private void SaveTeachers(TeacherHashTable teachers)
        {
            var lines = new List<string> { "Id,Name,EmployeeId,Type,AvailableSlots,AssignedCourses" };
            foreach (var teacher in teachers.GetAllTeachers())
            {
                string slots = string.Join(";", teacher.AvailableTimeSlots);
                string courses = string.Join(";", teacher.AssignedCourseIds);
                lines.Add($"{teacher.Id},{teacher.Name},{teacher.EmployeeId},{teacher.Type},{slots},{courses}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, TEACHERS_FILE), lines);
        }

        private void LoadTeachers(TeacherHashTable teachers)
        {
            string path = Path.Combine(DATA_FOLDER, TEACHERS_FILE);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                var teacher = new Teacher
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    EmployeeId = parts[2],
                    Type = parts[3]
                };

                if (!string.IsNullOrEmpty(parts[4]))
                    teacher.AvailableTimeSlots = parts[4].Split(';').Select(int.Parse).ToList();

                if (parts.Length > 5 && !string.IsNullOrEmpty(parts[5]))
                    teacher.AssignedCourseIds = parts[5].Split(';').Select(int.Parse).ToList();

                teachers.AddTeacher(teacher);
            }
        }

        private void SaveRooms(RoomHashTable rooms)
        {
            var lines = new List<string> { "Id,RoomNumber,Capacity" };
            foreach (var room in rooms.GetAllRooms())
            {
                lines.Add($"{room.Id},{room.RoomNumber},{room.Capacity}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, ROOMS_FILE), lines);
        }

        private void LoadRooms(RoomHashTable rooms)
        {
            string path = Path.Combine(DATA_FOLDER, ROOMS_FILE);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                rooms.AddRoom(new Room
                {
                    Id = int.Parse(parts[0]),
                    RoomNumber = parts[1],
                    Capacity = int.Parse(parts[2])
                });
            }
        }

        private void SaveTimeSlots(TimeSlotHashTable timeSlots)
        {
            var lines = new List<string> { "Id,Day,StartTime,EndTime" };
            foreach (var slot in timeSlots.GetAllTimeSlots())
            {
                lines.Add($"{slot.Id},{slot.Day},{slot.StartTime},{slot.EndTime}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, TIMESLOTS_FILE), lines);
        }

        private void LoadTimeSlots(TimeSlotHashTable timeSlots)
        {
            string path = Path.Combine(DATA_FOLDER, TIMESLOTS_FILE);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                timeSlots.AddTimeSlot(new TimeSlot
                {
                    Id = int.Parse(parts[0]),
                    Day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[1]),
                    StartTime = TimeSpan.Parse(parts[2]),
                    EndTime = TimeSpan.Parse(parts[3])
                });
            }
        }

        private void SaveClasses(ClassHashTable classes)
        {
            var lines = new List<string> { "Id,Name,Semester,Section,TotalStudents,CourseIds" };
            foreach (var cls in classes.GetAllClasses())
            {
                string courseIds = string.Join(";", cls.CourseIds);
                lines.Add($"{cls.Id},{cls.Name},{cls.Semester},{cls.Section},{cls.TotalStudents},{courseIds}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, CLASSES_FILE), lines);
        }

        private void LoadClasses(ClassHashTable classes)
        {
            string path = Path.Combine(DATA_FOLDER, CLASSES_FILE);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                var cls = new Class
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    Semester = int.Parse(parts[2]),
                    Section = char.Parse(parts[3]),
                    TotalStudents = int.Parse(parts[4])
                };

                if (parts.Length > 5 && !string.IsNullOrEmpty(parts[5]))
                    cls.CourseIds = parts[5].Split(';').Select(int.Parse).ToList();

                classes.AddClass(cls);
            }
        }

        private void SaveAssignments(List<TimetableAssignment> assignments)
        {
            var lines = new List<string> { "Id,ClassId,CourseId,TeacherId,RoomId,TimeSlotId" };
            foreach (var assignment in assignments)
            {
                lines.Add($"{assignment.Id},{assignment.ClassId},{assignment.CourseId},{assignment.TeacherId},{assignment.RoomId},{assignment.TimeSlotId}");
            }
            File.WriteAllLines(Path.Combine(DATA_FOLDER, ASSIGNMENTS_FILE), lines);
        }

        private void LoadAssignments(out List<TimetableAssignment> assignments)
        {
            assignments = new List<TimetableAssignment>();
            string path = Path.Combine(DATA_FOLDER, ASSIGNMENTS_FILE);
            if (!File.Exists(path)) return;

            var lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                assignments.Add(new TimetableAssignment
                {
                    Id = int.Parse(parts[0]),
                    ClassId = int.Parse(parts[1]),
                    CourseId = int.Parse(parts[2]),
                    TeacherId = int.Parse(parts[3]),
                    RoomId = int.Parse(parts[4]),
                    TimeSlotId = int.Parse(parts[5])
                });
            }
        }
    }
}