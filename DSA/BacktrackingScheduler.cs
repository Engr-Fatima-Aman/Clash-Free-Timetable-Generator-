using System;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;

namespace dsa_project.DSA
{
    public class SchedulingTask
    {
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public int RequiredHours { get; set; }
        public bool IsLab { get; set; }
    }

    public class BacktrackingScheduler
    {
        private List<TimetableAssignment> solution;
        private CourseHashTable courseTable;
        private TeacherHashTable teacherTable;
        private ClassHashTable classTable;
        private RoomHashTable roomTable;
        private TimeSlotHashTable timeSlotTable;
        
        private int assignmentAttempts = 0;
        private const int MAX_ATTEMPTS = 2000000; 

        public BacktrackingScheduler(CourseHashTable courses, TeacherHashTable teachers,
                RoomHashTable rooms, TimeSlotHashTable timeSlots, ClassHashTable classes)
        {
            courseTable = courses;
            teacherTable = teachers;
            classTable = classes;
            roomTable = rooms;
            timeSlotTable = timeSlots;
            solution = new List<TimetableAssignment>();
        }

        public List<TimetableAssignment> GenerateTimetable(List<Class> classesToSchedule)
        {
            solution.Clear();
            assignmentAttempts = 0;
            Console.WriteLine("--- Starting Robust Scheduler ---");

            var tasks = CreateSchedulingTasks(classesToSchedule)
                        .OrderByDescending(t => t.IsLab) 
                        .ThenByDescending(t => t.RequiredHours) 
                        .ToList();

            if (Backtrack(0, tasks))
            {
                Console.WriteLine($"+++ SUCCESS: Timetable Generated in {assignmentAttempts} attempts!");
                return solution;
            }
            
            Console.WriteLine("--- FAILURE: Optimization failed. Check Teacher/Room availability. ---");
            return new List<TimetableAssignment>(); 
        }

        private bool Backtrack(int taskIndex, List<SchedulingTask> tasks)
        {
            if (assignmentAttempts > MAX_ATTEMPTS) return false;
            if (taskIndex == tasks.Count) return true;

            var currentTask = tasks[taskIndex];

            if (assignmentAttempts % 1000 == 0)
            {
                Console.WriteLine($"Attempts: {assignmentAttempts} | Task: {taskIndex}/{tasks.Count} | CourseID: {currentTask.CourseId}");
            }

            var teachers = GetTeachersForCourse(currentTask.CourseId);
            if (!teachers.Any()) return false;

            var rooms = roomTable.GetAllRooms()
                                 .Where(r => r.IsLabRoom == currentTask.IsLab)
                                 .OrderBy(r => Guid.NewGuid()).ToList();
            
            if (!rooms.Any()) rooms = roomTable.GetAllRooms().ToList(); 

            var allCandidateSlots = timeSlotTable.GetAllTimeSlots().ToList();

            foreach (var teacher in teachers)
            {
                foreach (var slot in allCandidateSlots)
                {
                    if (currentTask.IsLab)
                    {
                        bool isLongSlot = slot.DurationInHours >= 2.8;
                        bool isUniLabTime = (slot.StartTime.Hours == 8 || slot.StartTime.Hours == 11 || slot.StartTime.Hours == 14);

                        if (!isLongSlot || !isUniLabTime) continue; 
                    }
                    else 
                    {
                        bool isCorrectDuration = Math.Abs(slot.DurationInHours - currentTask.RequiredHours) < 0.2;
                        if (!isCorrectDuration || slot.DurationInHours >= 2.8) continue;
                    }

                    if (!currentTask.IsLab && IsCourseAlreadyScheduledOnDay(currentTask.CourseId, currentTask.ClassId, slot.Day))
                        continue;

                    foreach (var room in rooms)
                    {
                        assignmentAttempts++;
                        
                        if (IsSafe(currentTask, teacher.Id, room.Id, slot))
                        {
                            var currentClass = classTable.GetClassById(currentTask.ClassId);
                            var currentCourse = courseTable.GetCourseById(currentTask.CourseId);

                            var assignment = new TimetableAssignment {
                                ClassId = currentTask.ClassId,
                                CourseId = currentTask.CourseId,
                                TeacherId = teacher.Id,
                                RoomId = room.Id,
                                TimeSlotId = slot.Id,
                                
                                // FIX: Using 'Semester' and 'Section' exactly as defined in your Class model
                                ClassName = $"{currentClass.Semester}-{currentClass.Section}", 
                                Section = currentClass.Section,
                                
                                CourseName = currentCourse.Title,
                                TeacherName = teacher.Name,
                                RoomName = room.RoomNumber,
                                TimeSlotInfo = $"{slot.Day} ({slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm})"
                            };

                            solution.Add(assignment);
                            if (Backtrack(taskIndex + 1, tasks)) return true;
                            solution.Remove(assignment); 
                        }
                    }
                }
            }
            return false;
        }

        private bool IsSafe(SchedulingTask task, int teacherId, int roomId, TimeSlot newSlot)
        {
            foreach (var existing in solution)
            {
                var existingSlot = timeSlotTable.GetTimeSlotById(existing.TimeSlotId);
                if (existingSlot.Day == newSlot.Day)
                {
                    bool timeOverlaps = newSlot.StartTime < existingSlot.EndTime && 
                                        newSlot.EndTime > existingSlot.StartTime;

                    if (timeOverlaps)
                    {
                        if (existing.TeacherId == teacherId) return false;
                        if (existing.RoomId == roomId) return false;
                        if (existing.ClassId == task.ClassId) return false;
                    }
                }
            }
            return true;
        }

        private List<SchedulingTask> CreateSchedulingTasks(List<Class> classes)
        {
            var tasks = new List<SchedulingTask>();
            foreach (var cls in classes)
            {
                foreach (var courseId in cls.CourseIds)
                {
                    var course = courseTable.GetCourseById(courseId);
                    if (course == null) continue;

                    if (course.IsLab || course.Title.ToLower().Contains("lab"))
                    {
                        tasks.Add(new SchedulingTask { ClassId = cls.Id, CourseId = courseId, RequiredHours = 3, IsLab = true });
                    }
                    else if (course.CreditHours == 3)
                    {
                        tasks.Add(new SchedulingTask { ClassId = cls.Id, CourseId = courseId, RequiredHours = 2, IsLab = false });
                        tasks.Add(new SchedulingTask { ClassId = cls.Id, CourseId = courseId, RequiredHours = 1, IsLab = false });
                    }
                    else
                    {
                        tasks.Add(new SchedulingTask { ClassId = cls.Id, CourseId = courseId, RequiredHours = (int)course.CreditHours, IsLab = false });
                    }
                }
            }
            return tasks;
        }

        private bool IsCourseAlreadyScheduledOnDay(int courseId, int classId, DayOfWeek day)
        {
            return solution.Any(a => a.CourseId == courseId && 
                                     a.ClassId == classId && 
                                     timeSlotTable.GetTimeSlotById(a.TimeSlotId).Day == day);
        }

        private List<Teacher> GetTeachersForCourse(int courseId)
        {
            return teacherTable.GetAllTeachers().Where(t => t.AssignedCourseIds.Contains(courseId)).ToList();
        }
    }
}