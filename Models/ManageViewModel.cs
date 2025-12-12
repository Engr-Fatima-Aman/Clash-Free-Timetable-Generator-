using System.Collections.Generic;

namespace dsa_project.Models
{
    public class ManageViewModel
    {
        public List<Course> Courses { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Room> Rooms { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
        public List<Class> Classes { get; set; }
    }
}