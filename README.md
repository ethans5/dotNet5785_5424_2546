# Volunteer Management System (VMS)

[![Language](https://img.shields.io/badge/Language-C%23-blue.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Framework](https://img.shields.io/badge/Framework-.NET-purple.svg)](https://dotnet.microsoft.com/)
[![UI](https://img.shields.io/badge/UI-WPF-lightgrey.svg)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Architecture](https://img.shields.io/badge/Architecture-N--Tier-green.svg)]()

## 📌 Overview
The Volunteer Management System (VMS) is a comprehensive enterprise-grade desktop application built with C# and .NET. It is designed to orchestrate and streamline the operations of volunteer organizations by managing service requests (Calls) and efficiently dispatching them to available Volunteers. 

By leveraging geolocation data and intelligent matching algorithms, the system ensures that volunteers are assigned tasks based on their availability, skills, proximity, and transport capabilities.

## 👥 Authors
* **Ethan Sarfati**
* **Ruben Bensimon**

## 🏗️ System Architecture
This project rigorously adheres to a **3-Tier Architecture** pattern, ensuring strong separation of concerns, scalability, and maintainability:

### 1. Data Access Layer (DAL)
The DAL encapsulates data storage and retrieval mechanisms.
* **`DalFacade`**: Exposes the data access contracts (Interfaces) and defines the core Data Objects (DO) such as `Call`, `Volunteer`, and `Assignment`.
* **`DalList`**: Provides an in-memory data store using standard collections, primarily used for rapid prototyping and mock testing.
* **`DalXml`**: The primary persistence implementation. It utilizes LINQ to XML to securely serialize and deserialize data across structured XML files (`calls.xml`, `volunteers.xml`, `assignments.xml`).
* **`DalTest`**: A dedicated console application for integration testing of CRUD operations against the database layer.

### 2. Business Logic Layer (BL)
The BL serves as the brain of the application, isolating business rules from data access and presentation.
* **`BL`**: Manages the Business Objects (BO) and orchestrates complex workflows. Key responsibilities include:
  * Dynamic calculation of geographical distances (Aerial, Walking, Driving) between tasks and volunteers.
  * Validation of assignment constraints (e.g., matching volunteer transport capabilities with task distance requirements).
  * State machine management for the lifecycle of Calls (Open, In Progress, Treated, Expired).
* **`BlTest`**: A suite for validating business rules independently of the user interface.

### 3. Presentation Layer (PL)
The PL provides an intuitive user experience.
* **`PL`**: A robust Graphical User Interface (GUI) developed using **Windows Presentation Foundation (WPF)**. Features include secure login gateways, dynamic dashboards, and detailed management views tailored for both administrators and volunteers.

## ✨ Key Features
* **Volunteer Profiling:** Comprehensive management of volunteer records, including operational radius, status tracking, and skill sets.
* **Task Geolocation:** Creation of service requests mapped with precise geographic coordinates (Latitude/Longitude) and time-bound constraints.
* **Smart Dispatching Engine:** An algorithmic approach to assigning the most suitable volunteer to a task based on real-time location and capabilities.
* **Lifecycle Tracking:** Granular traceability of task assignments, from initiation to resolution or cancellation.
* **Robust Persistence:** Reliable state saving using XML configuration and data files.

## 💻 Technology Stack
* **Language:** C# 10+ (Extensive use of `record` types and advanced LINQ)
* **Framework:** .NET 
* **User Interface:** WPF (XAML & Code-Behind)
* **Data Persistence:** XML (LINQ to XML)
* **Design Patterns:** N-Tier Architecture, Factory Pattern, Singleton Pattern

---
<div align="center">
  <i>"Always Pull Before Push"</i>
</div>