using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;
using dsa_project.DSA;
using dsa_project.Services;

namespace dsa_project.Controllers
{
    public class TimetableController : Controller
    {
        private readonly DataPersistenceService _persistenceService;

        public TimetableController()
        {
            _persistenceService = new DataPersistenceService();
        }

        public IActionResult Index()
        {
            var courses = new CourseHashTable();
            var teachers = new TeacherHashTable();
            var rooms = new RoomHashTable();
            var timeSlots = new TimeSlotHashTable();
            var classes = new ClassHashTable();
            
            _persistenceService.LoadAllData(courses, teachers, rooms, timeSlots, classes, out var assignments);

            if (assignments == null) assignments = new List<TimetableAssignment>();

            // FIX: Grouping by ClassName only (Clean View)
            var groupedResult = assignments
                .GroupBy(a => new { a.ClassName })
                .Cast<IGrouping<dynamic, TimetableAssignment>>()
                .ToList();

            return View(groupedResult);
        }

        [HttpGet]
        public IActionResult Generate()
        {
            var courses = new CourseHashTable();
            var teachers = new TeacherHashTable();
            var rooms = new RoomHashTable();
            var timeSlots = new TimeSlotHashTable();
            var classes = new ClassHashTable();

            _persistenceService.LoadAllData(courses, teachers, rooms, timeSlots, classes, out _);

            // Check if slots exist, otherwise create them
            if (timeSlots.GetAllTimeSlots().Count == 0)
            {
                int slotId = 1;
                DayOfWeek[] days = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
                
                foreach (var day in days)
                {
                    // --- THEORY SLOTS ---
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(10, 30, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(10, 30, 0), EndTime = new TimeSpan(12, 30, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(12, 30, 0), EndTime = new TimeSpan(13, 30, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(13, 30, 0), EndTime = new TimeSpan(15, 30, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(16, 30, 0) });

                    // --- LAB SLOTS (FIXED) ---
                    // These exact hours match the 'BacktrackingScheduler' logic (8, 11, 14)
                    
                    // 1. Morning Lab (8:30 - 11:30)
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(11, 30, 0) });
                    
                    // 2. Mid-Day Lab (11:30 - 2:30)
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(11, 30, 0), EndTime = new TimeSpan(14, 30, 0) });

                    // 3. Afternoon Lab (2:30 - 5:30)
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(14, 30, 0), EndTime = new TimeSpan(17, 30, 0) });
                }
            }

            var scheduler = new BacktrackingScheduler(courses, teachers, rooms, timeSlots, classes);
            var result = scheduler.GenerateTimetable(classes.GetAllClasses());

            if (result != null && result.Any())
            {
                _persistenceService.SaveAllData(courses, teachers, rooms, timeSlots, classes, result);
                
                var groupedResult = result
                    .GroupBy(a => new { a.ClassName })
                    .Cast<IGrouping<dynamic, TimetableAssignment>>()
                    .ToList();

                return View("Index", groupedResult); 
            }

            TempData["Error"] = "Scheduling Failed! Check console for attempts.";
            return RedirectToAction("Index", "Home");
        }
    }
}