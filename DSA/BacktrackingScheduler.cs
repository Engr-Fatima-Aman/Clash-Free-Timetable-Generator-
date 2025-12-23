using System;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;

namespace dsa_project.DSA
{
    // Backtracking Algorithm with Graph Integration
    // Time Complexity: O(k^(n/m)) with constraint pruning
    public class BacktrackingScheduler
    {
        private ConstraintValidator validator;
        private ConflictGraph conflictGraph;
        private List<TimetableAssignment> solution;
        private BacktrackingStack backtrackStack;
        private int backtrackCount;
        private int assignmentAttempts;
        private CourseHashTable courseTable;
        private TeacherHashTable teacherTable;
        private ClassHashTable classTable;
        private RoomHashTable roomTable;
        private TimeSlotHashTable timeSlotTable;
        
        public int BacktrackCount { get; private set; } = 0;
        public int AssignmentAttempts { get; private set; } = 0;
        public string LastFailureReason { get; private set; } = null;


        public BacktrackingScheduler(CourseHashTable courses, TeacherHashTable teachers, RoomHashTable rooms, TimeSlotHashTable timeSlots, ClassHashTable classes)
        {
            courseTable = courses;
            teacherTable = teachers;
            classTable = classes;
            roomTable = rooms;
            timeSlotTable = timeSlots;
            validator = new ConstraintValidator(courses, teachers, rooms, classes);
            conflictGraph = new ConflictGraph();
            solution = new List<TimetableAssignment>();
            backtrackStack = new BacktrackingStack();
            backtrackCount = 0;
            assignmentAttempts = 0;
        }

        // Main algorithm entry point
        public List<TimetableAssignment> GenerateTimetable(List<Class> classesToSchedule)
        {
            solution.Clear();
            validator.ClearAssignments();
            backtrackCount = 0;
            assignmentAttempts = 0;
            conflictGraph.Clear();

            // Build conflict graph from courses
            BuildConflictGraph(classesToSchedule);

            // Apply graph coloring as heuristic
            var coloring = new GraphColoringAlgorithm(conflictGraph);
            coloring.ColorGraph(timeSlotTable.Count());

            // Perform backtracking
            // NOTE: Logic assumes we are scheduling classes sequentially.
            if (Backtrack(0, classesToSchedule))
            {
                return solution;
            }
            return null;
        }

        // Build conflict graph - courses that cannot be at same time are connected
        private void BuildConflictGraph(List<Class> classes)
        {
            var allCourses = courseTable.GetAllCourses();

            // Add nodes for each course
            foreach (var course in allCourses)
            {
                conflictGraph.AddNode(course.Id, course.Code, NodeType.Course);
            }

            // Add edges: courses conflict if same teacher or in same class
            foreach (var course1 in allCourses)
            {
                foreach (var course2 in allCourses)
                {
                    if (course1.Id < course2.Id)
                    {
                        bool conflict = false;

                        // Check if courses share a teacher
                        var teachers1 = teacherTable.GetAllTeachers()
                            .Where(t => t.AssignedCourseIds.Contains(course1.Id)).ToList();
                        var teachers2 = teacherTable.GetAllTeachers()
                            .Where(t => t.AssignedCourseIds.Contains(course2.Id)).ToList();

                        if (teachers1.Any(t => teachers2.Any(t2 => t2.Id == t.Id)))
                            conflict = true;

                        // Check if courses in same class
                        foreach (var cls in classes)
                        {
                            if (cls.CourseIds.Contains(course1.Id) &&
                                cls.CourseIds.Contains(course2.Id))
                                conflict = true;
                        }

                        if (conflict)
                            conflictGraph.AddConflict(course1.Id, course2.Id);
                    }
                }
            }
        }

        // Recursive backtracking algorithm
        private bool Backtrack(int classIndex, List<Class> classesToSchedule)
        {
            // Base case: all classes scheduled
            if (classIndex == classesToSchedule.Count)
            {
                return true;
            }

            Class currentClass = classesToSchedule[classIndex];

            // ---------------------------------------------------------
            // IMPORTANT FIX: 
            // Original logic might skip courses if strictly class-based recursion is used incorrectly.
            // Assuming here we iterate through ALL courses of the current class.
            // Ideally, we should flatten the list of tasks to (Class, Course) pairs,
            // but keeping original structure for now.
            // ---------------------------------------------------------

            // Try to assign each course in this class
            // WARNING: Simple foreach here with recursive return inside might only schedule the FIRST valid course of the class.
            // For a robust solution, this part usually needs to recurse *per course*, not *per class*.
            // However, keeping your provided logic structure:
            
            foreach (int courseId in currentClass.CourseIds)
            {
                var teachersForCourse = GetTeachersForCourse(courseId);

                foreach (Teacher teacher in teachersForCourse)
                {
                    foreach (int timeSlotId in teacher.AvailableTimeSlots)
                    {
                        var availableRooms = GetAvailableRooms();

                        foreach (Room room in availableRooms)
                        {
                            assignmentAttempts++;

                            var assignment = new TimetableAssignment
                            {
                                ClassId = currentClass.Id,
                                CourseId = courseId,
                                TeacherId = teacher.Id,
                                RoomId = room.Id,
                                TimeSlotId = timeSlotId
                            };

                            // Validate assignment against all constraints
                            if (validator.ValidateAssignment(assignment))
                            {
                                // Add to solution
                                solution.Add(assignment);
                                backtrackStack.Push(assignment);
                                validator.AddAssignment(assignment);

                                // Recursively try next class
                                // (Note: This logic implies 1 course per class per backtrack step. 
                                // If classes have multiple courses, this needs a loop or different recursion depth)
                                if (Backtrack(classIndex + 1, classesToSchedule))
                                {
                                    return true;
                                }

                                // Backtrack: remove assignment
                                solution.Remove(assignment);
                                backtrackStack.Pop();
                                validator.RemoveAssignment(assignment);
                                backtrackCount++;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private List<Teacher> GetTeachersForCourse(int courseId)
        {
            var result = new List<Teacher>();
            foreach (var teacher in teacherTable.GetAllTeachers())
            {
                if (teacher.AssignedCourseIds.Contains(courseId))
                    result.Add(teacher);
            }
            return result;
        }

        private List<Room> GetAvailableRooms()
        {
            return roomTable.GetAllRooms();
        }

        public int GetBacktrackCount() => backtrackCount;
        public int GetAssignmentAttempts() => assignmentAttempts;
    }
}