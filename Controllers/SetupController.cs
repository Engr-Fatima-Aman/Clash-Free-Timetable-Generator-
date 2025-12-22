using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; 
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

        // ==========================================
        // 1. COURSES EDIT & ADD
        // ==========================================
        [HttpGet] public IActionResult AddCourse() => View();
        
        [HttpPost]
        public IActionResult AddCourse(Course course)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var all = c.GetAllCourses();
            course.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1;
            c.AddCourse(course);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            LoadAllData(out var c, out _, out _, out _, out _);
            var course = c.GetAllCourses().FirstOrDefault(x => x.Id == id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        public IActionResult EditCourse(Course course)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            c.RemoveCourse(course.Id); // Purana remove kiya
            c.AddCourse(course);       // Naya add kiya (Update)
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // ==========================================
        // 2. ROOMS EDIT & ADD
        // ==========================================
        [HttpGet] public IActionResult AddRoom() => View();
        
        [HttpPost]
        public IActionResult AddRoom(Room room)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var all = r.GetAllRooms();
            room.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1;
            r.AddRoom(room);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public IActionResult EditRoom(int id)
        {
            LoadAllData(out _, out _, out var r, out _, out _);
            var room = r.GetAllRooms().FirstOrDefault(x => x.Id == id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost]
        public IActionResult EditRoom(Room room)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            r.RemoveRoom(room.Id);
            r.AddRoom(room);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // ==========================================
        // 3. TEACHERS EDIT & ADD
        // ==========================================
        [HttpGet] 
        public IActionResult AddTeacher() 
        {
            LoadAllData(out var c, out _, out _, out _, out _);
            ViewBag.Courses = c.GetAllCourses();
            return View();
        }

        [HttpPost]
        public IActionResult AddTeacher(Teacher teacher, string AssignedCoursesStr)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var all = t.GetAllTeachers();
            teacher.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1;
            ProcessTeacherCourses(teacher, AssignedCoursesStr);
            t.AddTeacher(teacher);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public IActionResult EditTeacher(int id)
        {
            LoadAllData(out var c, out var t, out _, out _, out _);
            var teacher = t.GetAllTeachers().FirstOrDefault(x => x.Id == id);
            if (teacher == null) return NotFound();
            ViewBag.Courses = c.GetAllCourses();
            // Assigned IDs ko string mein convert kar rahe hain form ke liye
            ViewBag.AssignedCoursesStr = teacher.AssignedCourseIds != null ? string.Join(",", teacher.AssignedCourseIds) : "";
            return View(teacher);
        }

        [HttpPost]
        public IActionResult EditTeacher(Teacher teacher, string AssignedCoursesStr)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            ProcessTeacherCourses(teacher, AssignedCoursesStr);
            t.RemoveTeacher(teacher.Id);
            t.AddTeacher(teacher);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        private void ProcessTeacherCourses(Teacher teacher, string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                teacher.AssignedCourseIds = input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(s => int.Parse(s.Trim())).ToList();
            }
        }

        // ==========================================
        // 4. CLASSES EDIT & ADD
        // ==========================================
        [HttpGet]
        public IActionResult AddClass()
        {
            LoadAllData(out var c, out _, out _, out _, out _);
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

        [HttpGet]
        public IActionResult EditClass(int id)
        {
            LoadAllData(out var c, out _, out _, out _, out var cl);
            var cls = cl.GetAllClasses().FirstOrDefault(x => x.Id == id);
            if (cls == null) return NotFound();
            ViewBag.Courses = c.GetAllCourses();
            ViewBag.CourseIdsInput = cls.CourseIds != null ? string.Join(",", cls.CourseIds) : "";
            return View(cls);
        }

        [HttpPost]
        public IActionResult EditClass(Class cls, string courseIdsInput)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            if (!string.IsNullOrEmpty(courseIdsInput))
                cls.CourseIds = courseIdsInput.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s.Trim())).ToList();
            
            cl.RemoveClass(cls.Id);
            cl.AddClass(cls);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // --- Rest of the Methods (Manage, Delete, Reset, Generate) ---
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

        public IActionResult DeleteCourse(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); c.RemoveCourse(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }
        public IActionResult DeleteRoom(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); r.RemoveRoom(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }
        public IActionResult DeleteTeacher(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); t.RemoveTeacher(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }
        public IActionResult DeleteClass(int id) { LoadAllData(out var c, out var t, out var r, out var ts, out var cl); cl.RemoveClass(id); SaveAllData(c, t, r, ts, cl); return RedirectToAction("Manage"); }

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
                TempData["Message"] = "All data has been reset.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex) {
                TempData["Error"] = "Reset failed: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult GenerateDefaultSlots()
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            ts = new TimeSlotHashTable(); 
            int id = 1;
            DayOfWeek[] days = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            foreach (var day in days)
            {
                TimeSpan[] labStarts = { new TimeSpan(8, 30, 0), new TimeSpan(11, 30, 0), new TimeSpan(14, 30, 0) };
                foreach (var start in labStarts)
                {
                    ts.AddTimeSlot(new TimeSlot { 
                        Id = id++, Day = day, StartTime = start, EndTime = start.Add(TimeSpan.FromHours(3)) 
                    });
                }

                for (int hour = 8; hour <= 16; hour++)
                {
                    TimeSpan theoryStart = new TimeSpan(hour, 30, 0);
                    ts.AddTimeSlot(new TimeSlot { Id = id++, Day = day, StartTime = theoryStart, EndTime = theoryStart.Add(TimeSpan.FromHours(1)) });
                    if (theoryStart.Add(TimeSpan.FromHours(2)) <= new TimeSpan(17, 30, 0))
                    {
                        ts.AddTimeSlot(new TimeSlot { Id = id++, Day = day, StartTime = theoryStart, EndTime = theoryStart.Add(TimeSpan.FromHours(2)) });
                    }
                }
            }
            SaveAllData(c, t, r, ts, cl);
            TempData["Message"] = "Standard Slots generated!";
            return RedirectToAction("Index", "Home");
        }
    }
}