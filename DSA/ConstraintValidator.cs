using System;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;

namespace dsa_project.DSA
{
    // Validates all hard and soft constraints
    public class ConstraintValidator
    {
        private CourseHashTable courseTable;
        private TeacherHashTable teacherTable;
        private RoomHashTable roomTable;
        private ClassHashTable classTable;
        private List<TimetableAssignment> currentAssignments;

        public ConstraintValidator(CourseHashTable courses, TeacherHashTable teachers,
                RoomHashTable rooms, ClassHashTable classes)
        {
            courseTable = courses;
            teacherTable = teachers;
            roomTable = rooms;
            classTable = classes;
            currentAssignments = new List<TimetableAssignment>();
        }

        // Main validation method - checks all hard constraints
        public bool ValidateAssignment(TimetableAssignment assignment)
        {
            // HARD CONSTRAINTS (must be satisfied)

            // Constraint 1: No teacher teaches multiple courses at same time - O(n)
            if (!ValidateTeacherNoConflict(assignment))
                return false;

            // Constraint 2: No class has multiple courses at same time - O(n)
            if (!ValidateClassNoConflict(assignment))
                return false;

            // Constraint 3: No room is double-booked - O(n)
            if (!ValidateRoomNoConflict(assignment))
                return false;

            // Constraint 4: Room capacity sufficient - O(1)
            if (!ValidateRoomCapacity(assignment))
                return false;

            // Constraint 5: Teacher is available at time - O(1)
            if (!ValidateTeacherAvailability(assignment))
                return false;

            return true;
        }

        // Constraint 1: Teacher conflict check - O(n)
        private bool ValidateTeacherNoConflict(TimetableAssignment assignment)
        {
            foreach (var existing in currentAssignments)
            {
                if (existing.TeacherId == assignment.TeacherId &&
                    existing.TimeSlotId == assignment.TimeSlotId)
                {
                    return false; // Conflict found
                }
            }
            return true;
        }

        // Constraint 2: Class conflict check - O(n)
        private bool ValidateClassNoConflict(TimetableAssignment assignment)
        {
            foreach (var existing in currentAssignments)
            {
                if (existing.ClassId == assignment.ClassId &&
                    existing.TimeSlotId == assignment.TimeSlotId)
                {
                    return false; // Conflict found
                }
            }
            return true;
        }

        // Constraint 3: Room conflict check - O(n)
        private bool ValidateRoomNoConflict(TimetableAssignment assignment)
        {
            foreach (var existing in currentAssignments)
            {
                if (existing.RoomId == assignment.RoomId &&
                    existing.TimeSlotId == assignment.TimeSlotId)
                {
                    return false; // Conflict found
                }
            }
            return true;
        }

        // Constraint 4: Room capacity check - O(1)
        private bool ValidateRoomCapacity(TimetableAssignment assignment)
        {
            Room room = roomTable.GetRoomById(assignment.RoomId);
            Class cls = classTable.GetClassById(assignment.ClassId);

            if (room == null || cls == null)
                return false;

            return room.Capacity >= cls.TotalStudents;
        }

        // Constraint 5: Teacher availability check - O(1)
        private bool ValidateTeacherAvailability(TimetableAssignment assignment)
        {
            Teacher teacher = teacherTable.GetTeacherById(assignment.TeacherId);

            if (teacher == null || teacher.AvailableTimeSlots == null)
                return false;

            return teacher.AvailableTimeSlots.Contains(assignment.TimeSlotId);
        }

        // SOFT CONSTRAINTS (optional - for improvement)
        public int CalculateSoftConstraintViolations(TimetableAssignment assignment)
        {
            int violations = 0;

            // Soft constraint 1: Avoid Friday evening
            var slot = GetTimeSlot(assignment.TimeSlotId);
            if (slot != null && slot.Day == DayOfWeek.Friday && slot.StartTime.Hours >= 16)
                violations++;

            return violations;
        }

        private TimeSlot GetTimeSlot(int slotId)
        {
            // Note: Original code logic kept here.
            // Ideally, you should pass TimeSlotHashTable to this class constructor 
            // to fetch real time slots.
            return null;
        }

        public void AddAssignment(TimetableAssignment assignment)
        {
            currentAssignments.Add(assignment);
        }

        public void RemoveAssignment(TimetableAssignment assignment)
        {
            currentAssignments.Remove(assignment);
        }

        public void ClearAssignments()
        {
            currentAssignments.Clear();
        }
    }
}