using System.Collections.Generic;

namespace dsa_project.Models
{
    public class ManageViewModel
    {
        // Initializing with empty lists to avoid null errors
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public List<Class> Classes { get; set; } = new List<Class>();
    }
}