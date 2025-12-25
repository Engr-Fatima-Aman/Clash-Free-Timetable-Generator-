# Automatic Timetable Generator (SchedulePro)

## Description
The **Automatic Timetable Generator** is a web-based application developed to automate the complex process of timetable scheduling for university departments. Manually managing courses, teachers, classrooms, and time slots often leads to conflicts and errors.

This project solves that problem by using advanced algorithms to generate **conflict-free** schedules, reducing manual workload and improving accuracy.

## Key Features
* **Conflict-Free Scheduling:** Automatically detects and prevents overlaps between teachers, rooms, and student classes.
* **Algorithm-Driven:** Uses a combination of **Backtracking** and **Greedy Graph Coloring** algorithms to ensure valid assignments.
* **Web-Based Interface:** User-friendly dashboard developed with HTML, CSS, and JavaScript for easy data management.
* **Resource Management:** Manage Courses, Teachers, Rooms, and Classes from a centralized dashboard.
* **Export Options:** Ability to view generated timetables and export them as PDF files.

## Algorithms Used
This project integrates multiple paradigms to achieve efficiency:

1.  **Backtracking Scheduling Algorithm:** The core logic that recursively assigns teachers, rooms, and time slots. If a conflict arises, it backtracks to find an alternative solution.
2.  **Greedy Graph Coloring Algorithm:** Used to estimate the minimum number of time slots required by representing constraints as a "Conflict Graph".
3.  **Heuristics:** Sorts tasks by difficulty (e.g., Lab sessions first) to optimize processing time.

## Technologies
* **Backend:** C# (.NET) using ASP.NET MVC.
* **Frontend:** HTML, CSS, JavaScript.
* **Data Structures:** Hash Tables, Stacks, and Conflict Graphs for efficient processing.

## Getting Started

### Prerequisites
To run this project, you will need:
* Visual Studio (2019 or later) with ASP.NET web development workload.
* .NET SDK.

### Installation
1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/Engr-Fatima-Aman/Clash-Free-Timetable-Generator.git](https://github.com/Engr-Fatima-Aman/Clash-Free-Timetable-Generator.git)
    ```
2.  **Open the Solution:**
    Open the `.sln` file in Visual Studio.
3.  **Build and Run:**
    Press `F5` to build the solution and launch the application in your browser (default: `localhost:5081`).

## Usage
1.  **Manage Data:** Use the dashboard to add your Courses, Rooms, Teachers, and Classes.
2.  **Generate:** Click the "Generate Schedule" button to run the backtracking algorithm.
3.  **View/Export:** Review the color-coded timetable and export it if needed.

## Authors
* **Fatima Aman** 
* **Nukhba Tehreem**
* **Mariah Akber** 


## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
**Please credit the original authors if you use this code.**
