using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;
using dsa_project.DSA;
using dsa_project.Services;

namespace dsa_project.Controllers
{
    public class TimetableController : Controller
    {
        private readonly DataPersistenceService _dataService;

        public TimetableController()
        {
            _dataService = new DataPersistenceService();
        }

        // 1. Home Page (Jahan "Generate" button hoga)
        public IActionResult Index()
        {
            return View();
        }

        // 2. Button dabane par ye chalega
        [HttpPost]
        public IActionResult Generate()
        {
            // --- Step A: Data Load ---
            var courseTable = new CourseHashTable();
            var teacherTable = new TeacherHashTable();
            var roomTable = new RoomHashTable();
            var timeSlotTable = new TimeSlotHashTable();
            var classTable = new ClassHashTable();
            List<TimetableAssignment> existingAssignments;

            // File se real data load karne ki koshish
            _dataService.LoadAllData(courseTable, teacherTable, roomTable, timeSlotTable, classTable, out existingAssignments);

            // --- CHECK: Agar data empty hai to error dikhaye ---
            // Ab hum dummy data nahi banayenge, user ko bolenge input de
            if (courseTable.Count() == 0 || teacherTable.Count() == 0 || roomTable.Count() == 0 || classTable.Count() == 0)
            {
                ViewBag.Error = "No Data Found! Please add Courses, Teachers, Rooms, and Classes first using the Input buttons.";
                return View("Index");
            }

            // --- Step B: Algorithm Run ---
            var scheduler = new BacktrackingScheduler(courseTable, teacherTable, roomTable, timeSlotTable, classTable);
            var result = scheduler.GenerateTimetable(classTable.GetAllClasses());

            if (result != null && result.Count > 0)
            {
                // Result ko View ke liye tayar karna (Names add karna)
                foreach (var item in result)
                {
                    var cls = classTable.GetClassById(item.ClassId);
                    var course = courseTable.GetCourseById(item.CourseId);
                    var teacher = teacherTable.GetTeacherById(item.TeacherId);
                    var room = roomTable.GetRoomById(item.RoomId);
                    var slot = timeSlotTable.GetTimeSlotById(item.TimeSlotId);

                    item.ClassName = cls?.Name ?? "Unknown";
                    item.CourseName = course?.Code ?? "Unknown";
                    item.TeacherName = teacher?.Name ?? "Unknown";
                    item.RoomName = room?.RoomNumber ?? "Unknown";
                    item.TimeSlotInfo = slot != null ? $"{slot.Day} {slot.StartTime:hh\\:mm}" : "Unknown";
                }

                // --- Step C: Save Result (Optional) ---
                _dataService.SaveAllData(courseTable, teacherTable, roomTable, timeSlotTable, classTable, result);

                return View("Result", result);
            }
            else
            {
                ViewBag.Error = "Conflict! Could not generate timetable. Please check constraints (e.g., teacher availability vs class time).";
                return View("Index");
            }
        }
    }
}