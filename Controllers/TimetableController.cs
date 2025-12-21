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

            var groupedResult = assignments
                .GroupBy(a => new { a.ClassName, a.Section })
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

            if (timeSlots.GetAllTimeSlots().Count == 0)
            {
                int slotId = 1;
                DayOfWeek[] days = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
                
                foreach (var day in days)
                {
                    // Theory: 2 Hours (Calculated automatically by model)
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(10, 30, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(10, 30, 0), EndTime = new TimeSpan(12, 30, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(13, 30, 0), EndTime = new TimeSpan(15, 30, 0) });

                    // Theory: 1 Hour
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(16, 30, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(12, 30, 0), EndTime = new TimeSpan(13, 30, 0) });

                    // Lab: 3 Hours
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(12, 0, 0) });
                    timeSlots.AddTimeSlot(new TimeSlot { Id = slotId++, Day = day, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(17, 0, 0) });
                }
            }

            var scheduler = new BacktrackingScheduler(courses, teachers, rooms, timeSlots, classes);
            var result = scheduler.GenerateTimetable(classes.GetAllClasses());

            if (result != null && result.Any())
            {
                _persistenceService.SaveAllData(courses, teachers, rooms, timeSlots, classes, result);
                
                var groupedResult = result
                    .GroupBy(a => new { a.ClassName, a.Section })
                    .Cast<IGrouping<dynamic, TimetableAssignment>>()
                    .ToList();

                return View("Index", groupedResult); 
            }

            TempData["Error"] = "Scheduling Failed! Check console for attempts.";
            return RedirectToAction("Index", "Home");
        }
    }
}