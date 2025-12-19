using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // --- Helper Methods ---
        private void LoadAllData(out CourseHashTable c, out TeacherHashTable t, out RoomHashTable r, out TimeSlotHashTable ts, out ClassHashTable cl)
        {
            c = new CourseHashTable(); t = new TeacherHashTable(); r = new RoomHashTable(); ts = new TimeSlotHashTable(); cl = new ClassHashTable();
            _dataService.LoadAllData(c, t, r, ts, cl, out _);
        }

        private void SaveAllData(CourseHashTable c, TeacherHashTable t, RoomHashTable r, TimeSlotHashTable ts, ClassHashTable cl)
        {
            _dataService.SaveAllData(c, t, r, ts, cl, new List<TimetableAssignment>());
        }

        // ==========================================
        //  ⚡ NEW FEATURE: AUTO GENERATE TIME SLOTS
        // ==========================================
        public IActionResult AutoGenerateSlots()
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);

            var days = new List<DayOfWeek> { 
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, 
                DayOfWeek.Thursday, DayOfWeek.Friday 
            };

            int startHour = 8; // 8:00 AM
            int endHour = 16;  // 4:00 PM (16:00)
            int idCounter = ts.Count() + 1;

            foreach (var day in days)
            {
                for (int h = startHour; h < endHour; h++)
                {
                    // Check: Agar ye slot pehle se hai to duplicate na banaye
                    if (ts.GetAllTimeSlots().Any(s => s.Day == day && s.StartTime.Hours == h)) 
                        continue;

                    var slot = new TimeSlot
                    {
                        Id = idCounter++,
                        Day = day,
                        StartTime = new TimeSpan(h, 0, 0),    // Example: 8:00
                        EndTime = new TimeSpan(h + 1, 0, 0)   // Example: 9:00
                    };
                    ts.AddTimeSlot(slot);
                }
            }

            SaveAllData(c, t, r, ts, cl);
            // Wapis Manage page par bhej denge taake user dekh sake
            return RedirectToAction("Manage");
        }

        // ============================
        // 1. ADD DATA ACTIONS (Create)
        // ============================

        public IActionResult AddCourse() => View();
        [HttpPost]
        public IActionResult AddCourse(Course course)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            course.Id = c.Count() + 1; 
            c.AddCourse(course);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Course '{course.Code}' Added!";
            return View();
        }

        public IActionResult AddTeacher() => View();
        [HttpPost]
        public IActionResult AddTeacher(Teacher teacher, string AvailableSlotsStr, string AssignedCoursesStr)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            teacher.Id = t.Count() + 1;
            
            if (!string.IsNullOrEmpty(AvailableSlotsStr))
                teacher.AvailableTimeSlots = AvailableSlotsStr.Split(',').Select(int.Parse).ToList();
            if (!string.IsNullOrEmpty(AssignedCoursesStr))
                teacher.AssignedCourseIds = AssignedCoursesStr.Split(',').Select(int.Parse).ToList();

            t.AddTeacher(teacher);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Teacher '{teacher.Name}' Added!";
            return View();
        }

        public IActionResult AddRoom() => View();
        [HttpPost]
        public IActionResult AddRoom(Room room)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            room.Id = r.Count() + 1;
            r.AddRoom(room);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Room '{room.RoomNumber}' Added!";
            return View();
        }

        public IActionResult AddTimeSlot() => View();
        [HttpPost]
        public IActionResult AddTimeSlot(TimeSlot slot)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            slot.Id = ts.Count() + 1;
            ts.AddTimeSlot(slot);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Time Slot Added!";
            return View();
        }

        public IActionResult AddClass() => View();
        [HttpPost]
        public IActionResult AddClass(Class cls, string CourseIdsStr)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            cls.Id = cl.Count() + 1;
            
            if (!string.IsNullOrEmpty(CourseIdsStr))
                cls.CourseIds = CourseIdsStr.Split(',').Select(int.Parse).ToList();

            cl.AddClass(cls);
            SaveAllData(c, t, r, ts, cl);
            ViewBag.Message = $"Class '{cls.Name}' Added!";
            return View();
        }

        // ============================
        // 2. MANAGE DATA (Read/Delete)
        // ============================

        public IActionResult Manage()
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            var model = new ManageViewModel
            {
                Courses = c.GetAllCourses(),
                Teachers = t.GetAllTeachers(),
                Rooms = r.GetAllRooms(),
                TimeSlots = ts.GetAllTimeSlots(),
                Classes = cl.GetAllClasses()
            };
            return View(model);
        }

        // --- Delete Actions ---
        public IActionResult DeleteCourse(int id)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            c.RemoveCourse(id);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        public IActionResult DeleteTeacher(int id)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            t.RemoveTeacher(id);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        public IActionResult DeleteRoom(int id)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            r.RemoveRoom(id);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        public IActionResult DeleteTimeSlot(int id)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            ts.RemoveTimeSlot(id);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        public IActionResult DeleteClass(int id)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            cl.RemoveClass(id);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // ============================
        // 3. EDIT ACTIONS (Update)
        // ============================

        // --- Edit Course ---
        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            LoadAllData(out var c, out _, out _, out _, out _);
            var course = c.GetCourseById(id);
            return course == null ? RedirectToAction("Manage") : View(course);
        }

        [HttpPost]
        public IActionResult EditCourse(Course updatedCourse)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            c.RemoveCourse(updatedCourse.Id); // Purana delete
            c.AddCourse(updatedCourse);       // Naya add (Simple Update)
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // --- Edit Teacher ---
        [HttpGet]
        public IActionResult EditTeacher(int id)
        {
            LoadAllData(out _, out var t, out _, out _, out _);
            var teacher = t.GetTeacherById(id);
            if (teacher == null) return RedirectToAction("Manage");

            ViewBag.AvailableSlotsStr = string.Join(",", teacher.AvailableTimeSlots);
            ViewBag.AssignedCoursesStr = string.Join(",", teacher.AssignedCourseIds);

            return View(teacher);
        }

        [HttpPost]
        public IActionResult EditTeacher(Teacher updatedTeacher, string AvailableSlotsStr, string AssignedCoursesStr)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            
            if (!string.IsNullOrEmpty(AvailableSlotsStr))
                updatedTeacher.AvailableTimeSlots = AvailableSlotsStr.Split(',').Select(int.Parse).ToList();
            
            if (!string.IsNullOrEmpty(AssignedCoursesStr))
                updatedTeacher.AssignedCourseIds = AssignedCoursesStr.Split(',').Select(int.Parse).ToList();

            t.RemoveTeacher(updatedTeacher.Id);
            t.AddTeacher(updatedTeacher);
            
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // --- Edit Room ---
        [HttpGet]
        public IActionResult EditRoom(int id)
        {
            LoadAllData(out _, out _, out var r, out _, out _);
            var room = r.GetRoomById(id);
            return room == null ? RedirectToAction("Manage") : View(room);
        }

        [HttpPost]
        public IActionResult EditRoom(Room updatedRoom)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            r.RemoveRoom(updatedRoom.Id);
            r.AddRoom(updatedRoom);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // --- Edit TimeSlot ---
        [HttpGet]
        public IActionResult EditTimeSlot(int id)
        {
            LoadAllData(out _, out _, out _, out var ts, out _);
            var slot = ts.GetTimeSlotById(id);
            return slot == null ? RedirectToAction("Manage") : View(slot);
        }

        [HttpPost]
        public IActionResult EditTimeSlot(TimeSlot updatedSlot)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);
            ts.RemoveTimeSlot(updatedSlot.Id);
            ts.AddTimeSlot(updatedSlot);
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // --- Edit Class ---
        [HttpGet]
        public IActionResult EditClass(int id)
        {
            LoadAllData(out _, out _, out _, out _, out var cl);
            var cls = cl.GetClassById(id);
            if (cls == null) return RedirectToAction("Manage");

            ViewBag.CourseIdsStr = string.Join(",", cls.CourseIds);
            return View(cls);
        }

        [HttpPost]
        public IActionResult EditClass(Class updatedClass, string CourseIdsStr)
        {
            LoadAllData(out var c, out var t, out var r, out var ts, out var cl);

            if (!string.IsNullOrEmpty(CourseIdsStr))
                updatedClass.CourseIds = CourseIdsStr.Split(',').Select(int.Parse).ToList();

            cl.RemoveClass(updatedClass.Id);
            cl.AddClass(updatedClass);
            
            SaveAllData(c, t, r, ts, cl);
            return RedirectToAction("Manage");
        }

        // ============================
        // 4. RESET DATA (Danger Zone)
        // ============================
        public IActionResult ResetData()
        {
            string dataFolder = "TimetableData";
            if (System.IO.Directory.Exists(dataFolder))
            {
                System.IO.Directory.Delete(dataFolder, true);
            }
            System.IO.Directory.CreateDirectory(dataFolder);
            
            return RedirectToAction("Index", "Timetable");
        }
    }
}