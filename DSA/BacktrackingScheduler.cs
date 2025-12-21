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

            // Tasks ko priority dena: Pehle Labs phir lambay slots (3h, 2h, 1h)
            var tasks = CreateSchedulingTasks(classesToSchedule)
                        .OrderByDescending(t => t.IsLab) 
                        .ThenByDescending(t => t.RequiredHours) 
                        .ToList();

            if (Backtrack(0, tasks))
            {
                return solution;
            }
            
            return null;
        }

        private bool Backtrack(int taskIndex, List<SchedulingTask> tasks)
        {
            if (assignmentAttempts > MAX_ATTEMPTS) return false;
            if (taskIndex == tasks.Count) return true;

            var currentTask = tasks[taskIndex];
            var teachers = GetTeachersForCourse(currentTask.CourseId);
            
            // Rooms filter: Lab course hai toh Lab room, warna normal room
            var rooms = roomTable.GetAllRooms()
                         .Where(r => r.IsLabRoom == currentTask.IsLab)
                         .OrderBy(r => Guid.NewGuid()).ToList();
            
            // Slots filter: Duration aur EndTime check
            var allSlots = timeSlotTable.GetAllTimeSlots()
                            .Where(s => Math.Abs(s.DurationInHours - currentTask.RequiredHours) < 0.1)
                            .Where(s => {
                                if (currentTask.IsLab) 
                                    return s.EndTime <= new TimeSpan(17, 30, 0);
                                else 
                                    return s.EndTime <= new TimeSpan(16, 30, 0);
                            })
                            .OrderBy(s => Guid.NewGuid()).ToList();

            foreach (var teacher in teachers)
            {
                foreach (var slot in allSlots)
                {
                    // Aik hi din mein aik hi course do baar nahi hona chahiye (Except Labs)
                    if (!currentTask.IsLab && IsCourseAlreadyScheduledOnDay(currentTask.CourseId, currentTask.ClassId, slot.Day))
                        continue;

                    foreach (var room in rooms)
                    {
                        assignmentAttempts++;
                        
                        if (IsSafe(currentTask, teacher.Id, room.Id, slot))
                        {
                            var assignment = new TimetableAssignment {
                                ClassId = currentTask.ClassId,
                                CourseId = currentTask.CourseId,
                                TeacherId = teacher.Id,
                                RoomId = room.Id,
                                TimeSlotId = slot.Id,
                                ClassName = classTable.GetClassById(currentTask.ClassId).Name,
                                Section = classTable.GetClassById(currentTask.ClassId).Section,
                                CourseName = courseTable.GetCourseById(currentTask.CourseId).Title,
                                TeacherName = teacher.Name,
                                RoomName = room.RoomNumber,
                                TimeSlotInfo = $"{slot.Day} ({slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm})"
                            };

                            solution.Add(assignment);
                            if (Backtrack(taskIndex + 1, tasks)) return true;
                            solution.Remove(assignment); // Backtrack step
                        }
                    }
                }
            }
            return false;
        }

        private bool IsSafe(SchedulingTask task, int teacherId, int roomId, TimeSlot newSlot)
        {
            // O(N) check for collisions
            foreach (var existing in solution)
            {
                var existingSlot = timeSlotTable.GetTimeSlotById(existing.TimeSlotId);

                if (existingSlot.Day == newSlot.Day)
                {
                    // Standard collision check logic
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

                    if (course.IsLab)
                        tasks.Add(new SchedulingTask { ClassId = cls.Id, CourseId = courseId, RequiredHours = 3, IsLab = true });
                    else if (course.CreditHours == 3)
                    {
                        // 3 credit hours = 2h slot + 1h slot
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