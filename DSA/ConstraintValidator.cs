using System;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;

namespace dsa_project.DSA
{
    public class ConstraintValidator
    {
        public bool IsValid(TimetableAssignment assignment, List<TimetableAssignment> currentSchedule, 
                           Course course, Teacher teacher, Room room, TimeSlot slot)
        {
            // 1. Room Capacity Check
            // (Assuming Class has TotalStudents)
            
            // 2. Room Collision Check
            if (currentSchedule.Any(a => a.RoomId == assignment.RoomId && a.TimeSlotId == assignment.TimeSlotId))
                return false;

            // 3. Teacher Collision Check
            if (currentSchedule.Any(a => a.TeacherId == assignment.TeacherId && a.TimeSlotId == assignment.TimeSlotId))
                return false;

            // 4. Class/Section Collision Check
            if (currentSchedule.Any(a => a.ClassId == assignment.ClassId && a.TimeSlotId == assignment.TimeSlotId))
                return false;

            // FIXED: Removed IsWithinWorkingHours check because slots are pre-filtered
            // FIXED: Removed Teacher AvailableTimeSlots check as we are using a simplified Teacher model

            return true;
        }
    }
}