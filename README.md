# Automatic Timetable Generator (SchedulePro)

## Description
[cite_start]The **Automatic Timetable Generator** is a web-based application developed to automate the complex process of timetable scheduling for university departments[cite: 14]. [cite_start]Manually managing courses, teachers, classrooms, and time slots often leads to conflicts and errors[cite: 15].

[cite_start]This project solves that problem by using advanced algorithms to generate **conflict-free** schedules, reducing manual workload and improving accuracy[cite: 19, 24].

## Key Features
* [cite_start]**Conflict-Free Scheduling:** Automatically detects and prevents overlaps between teachers, rooms, and student classes[cite: 24].
* [cite_start]**Algorithm-Driven:** Uses a combination of **Backtracking** and **Greedy Graph Coloring** algorithms to ensure valid assignments[cite: 33, 34].
* [cite_start]**Web-Based Interface:** User-friendly dashboard developed with HTML, CSS, and JavaScript for easy data management[cite: 18].
* [cite_start]**Resource Management:** Manage Courses, Teachers, Rooms, and Classes from a centralized dashboard[cite: 416].
* [cite_start]**Export Options:** Ability to view generated timetables and export them as PDF files[cite: 362].

## Algorithms Used
[cite_start]This project integrates multiple paradigms to achieve efficiency[cite: 26]:

1.  **Backtracking Scheduling Algorithm:** The core logic that recursively assigns teachers, rooms, and time slots. [cite_start]If a conflict arises, it backtracks to find an alternative solution[cite: 34, 46].
2.  [cite_start]**Greedy Graph Coloring Algorithm:** Used to estimate the minimum number of time slots required by representing constraints as a "Conflict Graph"[cite: 52, 56].
3.  [cite_start]**Heuristics:** Sorts tasks by difficulty (e.g., Lab sessions first) to optimize processing time[cite: 112].

## Technologies
* [cite_start]**Backend:** C# (.NET) using ASP.NET MVC[cite: 16].
* [cite_start]**Frontend:** HTML, CSS, JavaScript[cite: 18].
* [cite_start]**Data Structures:** Hash Tables, Stacks, and Conflict Graphs for efficient processing[cite: 36, 63].

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
    Press `F5` to build the solution and launch the application in your browser (default: `localhost:5081`)[cite: 311].

## Usage
1.  [cite_start]**Manage Data:** Use the dashboard to add your Courses, Rooms, Teachers, and Classes[cite: 330].
2.  [cite_start]**Generate:** Click the "Generate Schedule" button to run the backtracking algorithm[cite: 351].
3.  [cite_start]**View/Export:** Review the color-coded timetable and export it if needed[cite: 370].

## Authors
* [cite_start]**Fatima Aman** (02-131242-060) [cite: 6]
* [cite_start]**Nukhba Tehreem** (02-131242-042) [cite: 6]
* [cite_start]**Mariah Akber** (02-131242-103) [cite: 6]

*Supervised by: Engr. Majid / Engr. Saniya Sarim* [cite: 7]

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
**Please credit the original authors if you use this code.**
