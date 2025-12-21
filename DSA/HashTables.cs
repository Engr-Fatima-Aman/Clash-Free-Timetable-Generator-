using System;
using System.Collections.Generic;
using System.Linq;
using dsa_project.Models;

namespace dsa_project.DSA
{
    public class CourseHashTable
    {
        private Dictionary<int, Course> courseById;
        private Dictionary<string, Course> courseByTitle; 

        public CourseHashTable()
        {
            courseById = new Dictionary<int, Course>();
            courseByTitle = new Dictionary<string, Course>();
        }

        public void AddCourse(Course course)
        {
            courseById[course.Id] = course;
            if (!string.IsNullOrEmpty(course.Title))
                courseByTitle[course.Title] = course;
        }

        public Course GetCourseById(int id) => courseById.ContainsKey(id) ? courseById[id] : null;
        public Course GetCourseByTitle(string title) => courseByTitle.ContainsKey(title) ? courseByTitle[title] : null;
        public bool Contains(int id) => courseById.ContainsKey(id);

        public void RemoveCourse(int id)
        {
            if (courseById.ContainsKey(id))
            {
                var title = courseById[id].Title;
                if (!string.IsNullOrEmpty(title) && courseByTitle.ContainsKey(title)) courseByTitle.Remove(title);
                courseById.Remove(id);
            }
        }

        public List<Course> GetAllCourses() => new List<Course>(courseById.Values);
        public int Count() => courseById.Count;
    }

    public class TeacherHashTable
    {
        private Dictionary<int, Teacher> teacherById;
        private Dictionary<string, Teacher> teacherByName;

        public TeacherHashTable()
        {
            teacherById = new Dictionary<int, Teacher>();
            teacherByName = new Dictionary<string, Teacher>();
        }

        public void AddTeacher(Teacher teacher)
        {
            teacherById[teacher.Id] = teacher;
            if (!string.IsNullOrEmpty(teacher.Name))
                teacherByName[teacher.Name] = teacher;
        }

        public Teacher GetTeacherById(int id) => teacherById.ContainsKey(id) ? teacherById[id] : null;
        public Teacher GetTeacherByName(string name) => teacherByName.ContainsKey(name) ? teacherByName[name] : null;
        public bool Contains(int id) => teacherById.ContainsKey(id);

        public void RemoveTeacher(int id)
        {
            if (teacherById.ContainsKey(id))
            {
                var name = teacherById[id].Name;
                if (!string.IsNullOrEmpty(name) && teacherByName.ContainsKey(name)) teacherByName.Remove(name);
                teacherById.Remove(id);
            }
        }

        public List<Teacher> GetAllTeachers() => new List<Teacher>(teacherById.Values);
        public int Count() => teacherById.Count;
    }

    public class RoomHashTable
    {
        private Dictionary<int, Room> roomById;
        private Dictionary<string, Room> roomByNumber;

        public RoomHashTable()
        {
            roomById = new Dictionary<int, Room>();
            roomByNumber = new Dictionary<string, Room>();
        }

        public void AddRoom(Room room)
        {
            roomById[room.Id] = room;
            if (!string.IsNullOrEmpty(room.RoomNumber))
                roomByNumber[room.RoomNumber] = room;
        }

        public Room GetRoomById(int id) => roomById.ContainsKey(id) ? roomById[id] : null;
        public Room GetRoomByNumber(string number) => roomByNumber.ContainsKey(number) ? roomByNumber[number] : null;
        public bool Contains(int id) => roomById.ContainsKey(id);

        public void RemoveRoom(int id)
        {
            if (roomById.ContainsKey(id))
            {
                var number = roomById[id].RoomNumber;
                if (!string.IsNullOrEmpty(number) && roomByNumber.ContainsKey(number)) roomByNumber.Remove(number);
                roomById.Remove(id);
            }
        }

        public List<Room> GetAllRooms() => new List<Room>(roomById.Values);
        public int Count() => roomById.Count;
    }

    public class TimeSlotHashTable
    {
        private Dictionary<int, TimeSlot> timeSlotById;
        public TimeSlotHashTable() => timeSlotById = new Dictionary<int, TimeSlot>();
        public void AddTimeSlot(TimeSlot slot) => timeSlotById[slot.Id] = slot;
        public TimeSlot GetTimeSlotById(int id) => timeSlotById.ContainsKey(id) ? timeSlotById[id] : null;
        public bool Contains(int id) => timeSlotById.ContainsKey(id);
        public void RemoveTimeSlot(int id) => timeSlotById.Remove(id);
        public List<TimeSlot> GetAllTimeSlots() => new List<TimeSlot>(timeSlotById.Values);
        public int Count() => timeSlotById.Count;
    }

    public class ClassHashTable
    {
        private Dictionary<int, Class> classById;
        private Dictionary<string, Class> classByName;

        public ClassHashTable()
        {
            classById = new Dictionary<int, Class>();
            classByName = new Dictionary<string, Class>();
        }

        public void AddClass(Class cls)
        {
            classById[cls.Id] = cls;
            if (!string.IsNullOrEmpty(cls.Name))
                classByName[cls.Name + "-" + cls.Section] = cls; 
        }

        // FIX: Added RemoveClass method to solve Controller errors
        public void RemoveClass(int id)
        {
            if (classById.ContainsKey(id))
            {
                var nameKey = classById[id].Name + "-" + classById[id].Section;
                if (classByName.ContainsKey(nameKey)) classByName.Remove(nameKey);
                classById.Remove(id);
            }
        }

        public Class GetClassById(int id) => classById.ContainsKey(id) ? classById[id] : null;
        public Class GetClassByName(string name) => classByName.ContainsKey(name) ? classByName[name] : null;
        public bool Contains(int id) => classById.ContainsKey(id);
        public List<Class> GetAllClasses() => new List<Class>(classById.Values);
        public int Count() => classById.Count;
    }
}