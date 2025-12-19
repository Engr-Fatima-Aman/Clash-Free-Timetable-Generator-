using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;

namespace dsa_project.DSA
{
    public class ConstraintValidator
    {
        private readonly TeacherHashTable _teacherTable;

        // Constructor: Hum sirf TeachersTable le rahe hain kyunki Availability check karni hai
        public ConstraintValidator(CourseHashTable c, TeacherHashTable t, RoomHashTable r, ClassHashTable cl)
        {
            _teacherTable = t;
        }

        // Main Validation Function
        public bool ValidateAssignment(TimetableAssignment newAssign, List<TimetableAssignment> currentAssignments)
        {
            // 1. CLASH CHECKS (Existing Timetable ke sath)
            foreach (var existing in currentAssignments)
            {
                // Agar Time Slot Same hai (Clash ho sakta hai)
                if (existing.TimeSlotId == newAssign.TimeSlotId)
                {
                    // A. Room Clash: Ek room mein 2 classes nahi ho sakti
                    if (existing.RoomId == newAssign.RoomId) return false;

                    // B. Teacher Clash: Ek teacher 2 jagah nahi parha sakta
                    if (existing.TeacherId == newAssign.TeacherId) return false;

                    // C. Class Clash: Ek class 2 subjects nahi parh sakti same time pe
                    if (existing.ClassId == newAssign.ClassId) return false;
                }
            }

            // 2. TEACHER AVAILABILITY CHECK
            var teacher = _teacherTable.GetTeacherById(newAssign.TeacherId);
            if (teacher != null && teacher.AvailableTimeSlots != null && teacher.AvailableTimeSlots.Count > 0)
            {
                // Agar teacher ne specific slots diye hain, aur ye slot unmein nahi hai -> Invalid
                if (!teacher.AvailableTimeSlots.Contains(newAssign.TimeSlotId))
                {
                    return false;
                }
            }

            // Sab clear hai
            return true;
        }
    }
}