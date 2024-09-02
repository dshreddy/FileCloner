# **Project Specification Document**

### **Project Title**:  
File Cloner

### **Project Description**:
.NET Windows Desktop App for Online User Detection and File Version Control

### **Version**:  
1.0

### **Prepared By**:  
Sai Hemanth Reddy D

### **Date**:  
September 2, 2024

---

### **1. Introduction**

#### **1.1 Purpose**
This document outlines the specifications for a .NET Windows desktop application designed to:
- Detect online users within a network.
- Request and manage access to files within a specified folder on other users' systems.
- Perform version control operations on files based on the latest timestamps.
- Provide a user interface for selective file synchronization based on version control criteria.

#### **1.2 Scope**
The application will allow users to see who else is online, request access to specific folders on those systems, and synchronize files across systems based on version control rules. The scope includes:
- Online user detection.
- Folder access requests.
- Version control comparison and operations.
- User interface for file selection and synchronization.

#### **1.3 Definitions, Acronyms, and Abbreviations**
- **Client**: A user's instance of the desktop application.
- **Server**: A central component that manages online user detection and communication between clients.
- **Version Control**: A method of managing changes to files, particularly tracking which file version is the latest.

### **2. Functional Requirements**

#### **2.1 User Detection**
- **FR1.1**: The application shall detect and display a list of online users within the same network.
- **FR1.2**: The server shall maintain and broadcast the list of online users to all connected clients.

#### **2.2 Folder Access Requests**
- **FR2.1**: The application shall allow a user to send a request to access a specific folder on another online user's system.
- **FR2.2**: The target user shall receive the request and have the option to accept or reject it.
- **FR2.3**: Upon acceptance, the requesting user shall gain read-only access to the specified folder.

#### **2.3 Version Control Operations**
- **FR3.1**: The application shall compare the contents of the remote folder with the local folder.
- **FR3.2**: The application shall identify new files in the remote folder that are not present locally and check the corresponding checkboxes by default.
- **FR3.3**: For files present in both folders, the application shall compare their last modified timestamps.
- **FR3.4**: If the remote file has a newer timestamp, the corresponding checkbox shall be checked by default.

#### **2.4 User Interface for File Selection**
- **FR4.1**: The application shall display a UI with a list of files and checkboxes for each file.
- **FR4.2**: Users shall be able to manually check or uncheck the checkboxes.
- **FR4.3**: Upon confirmation, the application shall copy the selected files to the local folder, creating new files or replacing existing ones as needed.

### **3. Non-Functional Requirements**

#### **3.1 Performance**
- **NFR1.1**: The application shall handle file synchronization operations for folders containing up to 10,000 files.
- **NFR1.2**: The system shall respond to user actions within 2 seconds.

#### **3.2 Reliability**
- **NFR2.1**: The application shall ensure data integrity during file transfer operations.
- **NFR2.2**: The system shall provide error handling for file access issues such as permissions or file locks.

#### **3.3 Usability**
- **NFR3.1**: The UI shall be intuitive and easy to navigate for users with basic computer skills.
- **NFR3.2**: The application shall provide tooltips and documentation for all major features.

### **4. System Architecture**

#### **4.1 Client-Server Model**
- The application shall follow a client-server architecture, where each instance of the desktop app functions as a client, and a central server manages user detection and communication.

#### **4.2 Components**
- **Client Application**: Handles user interactions, displays the UI, and manages local file operations.
- **Server**: Manages online user detection, broadcasts the list of online users, and relays folder access requests.

### **5. Technical Specifications**

#### **5.1 Development Environment**
- **Programming Language**: C#
- **Framework**: .NET 6 or higher
- **IDE**: Visual Studio 2022

#### **5.2 Libraries and Tools**
- **Networking**: System.Net.Sockets for TCP/IP communication
- **UI**: Windows Forms or WPF
- **File Operations**: System.IO for file management
- **Version Control**: Custom implementation using file timestamps

#### **5.3 Data Storage**
- No persistent data storage is required. All operations are performed in-memory or through direct file access.

### **6. Testing and Validation**

#### **6.1 Unit Testing**
- **UT1**: Test individual components like file comparison, UI checkboxes, and file copying.

#### **6.2 Integration Testing**
- **IT1**: Test the interaction between the client and server, particularly for user detection and folder access requests.
- **IT2**: Test the end-to-end workflow from user detection to file synchronization.

#### **6.3 User Acceptance Testing**
- **UAT1**: Conduct testing with a sample group of users to ensure the application's usability and functionality.

### **7. Deployment**

#### **7.1 Deployment Environment**
- **Target OS**: Windows 10/11
- **Installation Package**: Create an installer using tools like Inno Setup or WiX.

### **8. Maintenance**

#### **8.1 Updates**
- Plan for periodic updates to address bugs, add features, and improve performance.
