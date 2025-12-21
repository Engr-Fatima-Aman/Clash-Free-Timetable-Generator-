using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Added for File operations
using dsa_project.Models;
using dsa_project.DSA;
using dsa_project.Services;

namespace dsa_project.Controllers
{
    public class SetupController : Controller
    {
        private readonly DataPersistenceService _dataService;

        public SetupController()
        {
            _dataService = new DataPersistenceService();
        }

        private void LoadAllData(out CourseHashTable c, out TeacherHashTable t, out RoomHashTable r, out TimeSlotHashTable ts, out ClassHashTable cl)
        {
            c = new CourseHashTable(); 
            t = new TeacherHashTable(); 
            r = new RoomHashTable(); 
            ts = new TimeSlotHashTable(); 
            cl = new ClassHashTable();
            _dataService.LoadAllData(c, t, r, ts, cl, out _);
        }

        private void SaveAllData(CourseHashTable c, TeacherHashTable t, RoomHashTable r, TimeSlotHashTable ts, ClassHashTable cl)
        {
            _dataService.SaveAllData(c, t, r, ts, cl, new List<TimetableAssignment>());
        }

        // --- 1. COURSES ---
        [HttpGet]
        public IActionResult AddCourse() => View();
        
        [HttpPost]
        public IActionResult AddCourse(Course course)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var all = c.GetAllCourses();
            course.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1;
            
            c.AddCourse(course);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Course '{course.Title}' Added Successfully!";
            return View();
        }

        // --- 2. ROOMS ---
        [HttpGet]
        public IActionResult AddRoom() => View();
        
        [HttpPost]
        public IActionResult AddRoom(Room room)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var all = r.GetAllRooms();
            room.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1;

            r.AddRoom(room);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Room '{room.RoomNumber}' Added Successfully!";
            return View();
        }

        // --- 3. TEACHERS ---
        // FIXED: Added GET method so the button can open the page
        [HttpGet]
        public IActionResult AddTeacher() => View();

        [HttpPost]
        public IActionResult AddTeacher(Teacher teacher, string AssignedCoursesStr)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var all = t.GetAllTeachers();
            teacher.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1;
            
            if (!string.IsNullOrEmpty(AssignedCoursesStr))
                teacher.AssignedCourseIds = AssignedCoursesStr.Split(',').Select(s => int.Parse(s.Trim())).ToList();
            
            t.AddTeacher(teacher);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Teacher '{teacher.Name}' Added Successfully!";
            return View();
        }

        // --- 4. CLASSES ---
        [HttpGet]
        public IActionResult AddClass()
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            ViewBag.Courses = c.GetAllCourses();
            return View();
        }

        [HttpPost]
        public IActionResult AddClass(Class cls, string courseIdsInput)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var all = cl.GetAllClasses();
            cls.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1;

            if (!string.IsNullOrEmpty(courseIdsInput))
                cls.CourseIds = courseIdsInput.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s.Trim())).ToList();

            cl.AddClass(cls);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // --- 5. DATA MANAGEMENT ---
        public IActionResult Manage()
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var model = new ManageViewModel {
                Courses = c.GetAllCourses().OrderBy(x => x.Id).ToList(),
                Teachers = t.GetAllTeachers().OrderBy(x => x.Id).ToList(),
                Rooms = r.GetAllRooms().OrderBy(x => x.Id).ToList(),
                TimeSlots = ts.GetAllTimeSlots().OrderBy(x => x.Id).ToList(),
                Classes = cl.GetAllClasses().OrderBy(x => x.Id).ToList()
            };
            return View(model);
        }

        // FIXED: Added ResetData logic
        [HttpGet]
        public IActionResult ResetData()
        {
            try {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "TimetableData");
                if (Directory.Exists(folderPath)) {
                    foreach (string file in Directory.GetFiles(folderPath)) {
                        System.IO.File.Delete(file);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) {
                TempData["Error"] = "Reset failed: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult DeleteCourse(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); c.RemoveCourse(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }
        public IActionResult DeleteRoom(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); r.RemoveRoom(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }
        public IActionResult DeleteTeacher(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); t.RemoveTeacher(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }
        public IActionResult DeleteClass(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); cl.RemoveClass(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }

        // --- 6. SLOT GENERATION ---
        public IActionResult GenerateDefaultSlots()
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            ts = new TimeSlotHashTable(); 
            int id = 1;
            DayOfWeek[] days = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            foreach (var day in days)
            {
                for (int hour = 8; hour <= 15; hour++)
                {
                    TimeSpan start = new TimeSpan(hour, 30, 0);
                    // 1h Slot
                    ts.AddTimeSlot(new TimeSlot { Id = id++, Day = day, StartTime = start, EndTime = start.Add(TimeSpan.FromHours(1)) });
                    // 2h Slot
                    if (hour + 2 <= 17)
                        ts.AddTimeSlot(new TimeSlot { Id = id++, Day = day, StartTime = start, EndTime = start.Add(TimeSpan.FromHours(2)) });
                    // 3h Slot (Lab)
                    if (hour + 3 <= 17)
                        ts.AddTimeSlot(new TimeSlot { Id = id++, Day = day, StartTime = start, EndTime = start.Add(TimeSpan.FromHours(3)) });
                }
            }
            SaveAllData(c, t, r, ts, cl);
            TempData["Message"] = "Time slots generated successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}