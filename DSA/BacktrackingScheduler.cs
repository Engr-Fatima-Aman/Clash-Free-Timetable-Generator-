using System;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;

namespace dsa_project.DSA
{
    public class BacktrackingScheduler
    {
        private readonly ConstraintValidator _validator;
        private readonly CourseHashTable _courseTable;
        private readonly TeacherHashTable _teacherTable;
        private readonly RoomHashTable _roomTable;
        
        // Sorted Slots list (zaroori hai consecutive check ke liye)
        private readonly List<TimeSlot> _sortedSlots; 

        public BacktrackingScheduler(CourseHashTable courses, TeacherHashTable teachers, RoomHashTable rooms, TimeSlotHashTable timeSlots, ClassHashTable classes)
        {
            _courseTable = courses;
            _teacherTable = teachers;
            _roomTable = rooms;
            
            // Validator ko sahi parameters pass kar rahe hain
            _validator = new ConstraintValidator(courses, teachers, rooms, classes);
            
            // Slots ko Day aur Time ke hisaab se sort kar rahe hain
            _sortedSlots = timeSlots.GetAllTimeSlots()
                            .OrderBy(s => s.Day)
                            .ThenBy(s => s.StartTime)
                            .ToList();
        }

        public List<TimetableAssignment> GenerateTimetable(List<Class> classes)
        {
            var allTasks = new List<LectureTask>();

            // 1. DATA PREPARATION: Class -> Courses -> Tasks
            foreach (var cls in classes)
            {
                if (cls.CourseIds == null) continue;

                foreach (var courseId in cls.CourseIds)
                {
                    var course = _courseTable.GetCourseById(courseId);
                    // Find Teacher
                    var teacher = _teacherTable.GetAllTeachers()
                                    .FirstOrDefault(t => t.AssignedCourseIds.Contains(courseId));

                    if (course != null && teacher != null)
                    {
                        // === SMART LOGIC: Credit Hours Handling ===
                        
                        // Check if Lab (Name mein 'Lab' ho ya 1 Credit Hour ho)
                        bool isLab = course.Title.ToLower().Contains("lab") || 
                                     (course.CreditHours == 1 && course.Title.ToLower().Contains("lab"));
                        
                        if (isLab)
                        {
                            // Lab: 3 Hours Straight
                            allTasks.Add(new LectureTask { Class = cls, Course = course, Teacher = teacher, Duration = 3 });
                        }
                        else if (course.CreditHours == 3)
                        {
                            // Theory 3 CH: Split -> 2 Hours + 1 Hour
                            allTasks.Add(new LectureTask { Class = cls, Course = course, Teacher = teacher, Duration = 2 });
                            allTasks.Add(new LectureTask { Class = cls, Course = course, Teacher = teacher, Duration = 1 });
                        }
                        else if (course.CreditHours == 2)
                        {
                            // Theory 2 CH: 2 Hours Straight
                            allTasks.Add(new LectureTask { Class = cls, Course = course, Teacher = teacher, Duration = 2 });
                        }
                        else
                        {
                            // Default: 1 Hour blocks (jitne credit hours utne blocks)
                            for(int k=0; k < course.CreditHours; k++)
                                allTasks.Add(new LectureTask { Class = cls, Course = course, Teacher = teacher, Duration = 1 });
                        }
                    }
                }
            }

            // Optimization: Bari duration wale tasks pehle schedule karein
            allTasks = allTasks.OrderByDescending(t => t.Duration)
                               .ThenByDescending(t => t.Class.TotalStudents)
                               .ToList();

            var assignments = new List<TimetableAssignment>();
            
            // Start Backtracking Algorithm
            if (Solve(0, allTasks, assignments))
            {
                return assignments;
            }

            return null; // Failure
        }

        // --- RECURSIVE FUNCTION ---
        private bool Solve(int index, List<LectureTask> tasks, List<TimetableAssignment> assignments)
        {
            // Base Case: Agar saare tasks schedule ho gaye
            if (index >= tasks.Count) return true; 

            var currentTask = tasks[index];
            var rooms = _roomTable.GetAllRooms();

            // Try Every Room
            foreach (var room in rooms)
            {
                // Constraint: Room Capacity Check
                if (room.Capacity < currentTask.Class.TotalStudents) continue; 

                // Try Every Time Slot (Sorted List mein loop)
                for (int i = 0; i < _sortedSlots.Count; i++)
                {
                    // Check: Kya yahan se aage 'Duration' amount ke slots valid hain?
                    var proposedSlots = new List<TimeSlot>();
                    bool sequencePossible = true;

                    for (int d = 0; d < currentTask.Duration; d++)
                    {
                        // Bounds check
                        if (i + d >= _sortedSlots.Count) { sequencePossible = false; break; }

                        var s1 = _sortedSlots[i + d];
                        
                        // Check Continuity:
                        if (d > 0)
                        {
                            var prev = _sortedSlots[i + d - 1];
                            // Agar Din badal gaya (Mon -> Tue)
                            if (s1.Day != prev.Day) { sequencePossible = false; break; } 
                            // Agar Gap hai (> 10 mins)
                            if ((s1.StartTime - prev.EndTime).TotalMinutes > 10) { sequencePossible = false; break; } 
                        }
                        proposedSlots.Add(s1);
                    }

                    if (!sequencePossible) continue; // Ye start point sahi nahi hai

                    // VALIDATION: Kya Room/Teacher in sab slots par free hain?
                    bool allValid = true;
                    var tempAssignments = new List<TimetableAssignment>();

                    foreach (var slot in proposedSlots)
                    {
                        var assign = new TimetableAssignment
                        {
                            ClassId = currentTask.Class.Id,
                            CourseId = currentTask.Course.Id,
                            TeacherId = currentTask.Teacher.Id,
                            RoomId = room.Id,
                            TimeSlotId = slot.Id
                        };

                        if (!_validator.ValidateAssignment(assign, assignments))
                        {
                            allValid = false; 
                            break;
                        }
                        tempAssignments.Add(assign);
                    }

                    if (allValid)
                    {
                        // 1. ASSIGN (Temporary add)
                        assignments.AddRange(tempAssignments);

                        // 2. RECURSE (Next task try karein)
                        if (Solve(index + 1, tasks, assignments)) return true;

                        // 3. BACKTRACK (Fail hua to wapis hatayen)
                        foreach (var a in tempAssignments) assignments.Remove(a);
                    }
                }
            }

            return false; // Koi solution nahi mila
        }

        // Helper Class (Is file ke andar hi rahega)
        private class LectureTask
        {
            public Class Class { get; set; }
            public Course Course { get; set; }
            public Teacher Teacher { get; set; }
            public int Duration { get; set; } // 1, 2, or 3 hours
        }
    }
}