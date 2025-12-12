using System;
using System.Collections.Generic;
using dsa_project.Models;

namespace dsa_project.DSA
{
    // Backtracking ke liye custom stack (LIFO) [cite: 17, 167]
    public class BacktrackingStack
    {
        private Stack<TimetableAssignment> assignmentStack;

        public BacktrackingStack()
        {
            assignmentStack = new Stack<TimetableAssignment>();
        }

        public void Push(TimetableAssignment assignment) => assignmentStack.Push(assignment);
        public TimetableAssignment Pop() => assignmentStack.Count > 0 ? assignmentStack.Pop() : null;
        public bool IsEmpty() => assignmentStack.Count == 0;
        public int GetDepth() => assignmentStack.Count;
        public void Clear() => assignmentStack.Clear();
    }

    // Resources (Rooms/Teachers) ko track karne ke liye O(1) lookup [cite: 194]
    public class ResourceTracker
    {
        private HashSet<string> usedResources;

        public ResourceTracker()
        {
            usedResources = new HashSet<string>();
        }

        public bool IsResourceAvailable(string resourceType, int resourceId, int timeSlotId)
        {
            string key = $"{resourceType}_{resourceId}_{timeSlotId}";
            return !usedResources.Contains(key);
        }

        public void MarkResourceUsed(string resourceType, int resourceId, int timeSlotId)
        {
            string key = $"{resourceType}_{resourceId}_{timeSlotId}";
            usedResources.Add(key);
        }

        public void UnmarkResourceUsed(string resourceType, int resourceId, int timeSlotId)
        {
            string key = $"{resourceType}_{resourceId}_{timeSlotId}";
            usedResources.Remove(key);
        }

        public void Clear() => usedResources.Clear();
    }

    // Time Slots ki availability track karne ke liye
    public class TimeSlotTracker
    {
        private HashSet<int> usedTimeSlots;
        private HashSet<int> availableTimeSlots;

        public TimeSlotTracker(int totalTimeSlots)
        {
            usedTimeSlots = new HashSet<int>();
            availableTimeSlots = new HashSet<int>();

            for (int i = 0; i < totalTimeSlots; i++)
            {
                availableTimeSlots.Add(i);
            }
        }

        public bool IsAvailable(int timeSlotId) => availableTimeSlots.Contains(timeSlotId);

        public void MarkUsed(int timeSlotId)
        {
            if (availableTimeSlots.Contains(timeSlotId))
            {
                availableTimeSlots.Remove(timeSlotId);
                usedTimeSlots.Add(timeSlotId);
            }
        }

        public void MarkAvailable(int timeSlotId)
        {
            if (usedTimeSlots.Contains(timeSlotId))
            {
                usedTimeSlots.Remove(timeSlotId);
                availableTimeSlots.Add(timeSlotId);
            }
        }

        public int GetAvailableCount() => availableTimeSlots.Count;
        public List<int> GetAllAvailable() => new List<int>(availableTimeSlots);
        public void Clear()
        {
            usedTimeSlots.Clear();
            availableTimeSlots.Clear();
        }
    }
}