# **Project Specifications Document**

### **Project Name**:  
File Cloner

### **Project Description**:  
A .NET Windows Desktop Application for File Version Control and Synchronization between multiple systems, allowing users to clone, update, and manage files between folders on different machines. It enables seamless communication between systems, version tracking, and intelligent synchronization of files through an intuitive WPF interface.

### **Version**:  
1.0

### **Prepared By**:  
Sai Hemanth Reddy D

### **Created On**:  
September 2, 2024

### **Last Updated On**:  
September 23, 2024

---

### **Contents**
1. [Introduction](#1-introduction)
2. [Functional Requirements](#2-functional-requirements)
3. [Non-Functional Requirements](#3-non-functional-requirements)
4. [System Architecture](#4-system-architecture)
5. [Technical Specifications](#5-technical-specifications)
6. [Testing and Validation](#6-testing-and-validation)
7. [Deployment](#7-deployment)

---

### **1. Introduction**

The **File Cloner** application is designed to provide efficient file version control between systems using a .NET-based Windows desktop application. The app allows one user to request access to a folder on another system, synchronize files based on timestamps, and perform operations similar to version control. The app’s central feature is its ability to manage file updates between a **Requester** and **Responder**, using a central **Server** to coordinate requests and responses.

#### **1.1 Definitions, Acronyms, and Abbreviations**
- **Requester**: The system or user who initiates a request for file synchronization.
- **Responder**: The system or user who responds to a request by accepting or denying access to their files.
- **Client**: Any instance of the desktop application running on a user’s system.
- **Server**: The central component that detects online users, handles broadcast messages, and manages communication between the Requester and Responder.
- **Cloning**: The process of copying files from one system to another.

---

### **2. Functional Requirements**

#### **2.1 Client**

- **FR1**: Each client must have a `config.json` file that stores folder descriptions as key-value pairs.
  - **Key**: A user-friendly description of the folder (e.g., "Documents Folder").
  - **Value**: The full file path to the corresponding folder on the user's system.
- **FR2**: Upon receiving the address of the Responder from the server, the Requester and Responder should establish a connection for file synchronization.
- **FR3**: The client interface shall provide a visual display of the connection status between the Requester and Responder.

#### **2.2 Server**

- **FR1**: The server must be able to broadcast a request to all connected users when a Requester initiates a file cloning operation.
- **FR2**: The server must listen for responses from clients within a specified timeout period. If no response is received within this period, the request is considered rejected for that client.
- **FR3**: The server should return the first client that accepts the request as the Responder. If no clients accept the request, the server shall return `null` to the Requester.
- **FR4**: The server should log all requests and responses for auditing purposes.

#### **2.3 Requester**

- **FR1**: The Requester should be able to initiate the file cloning process by clicking the "Start" button.
- **FR2**: A dialog box should allow the Requester to choose a folder (key) from the `config.json` file, which will be included in the broadcast message.
- **FR3**: Upon connection with the Responder, the Requester should see a list of files and folders from the Responder, along with checkboxes for each.
  - **FR3.1**: The application should automatically check checkboxes for new files (i.e., files present in the Responder's folder but not in the Requester's folder).
  - **FR3.2**: The application should compare file timestamps for files present in both folders and check the checkbox if the Responder’s file has a newer timestamp.
  - **FR3.3**: Users should be able to manually check or uncheck any checkboxes.
- **FR4**: A "Start Cloning" button should initiate the file synchronization process, during which selected files and folders are either copied or replaced on the Requester’s system.
  - **FR4.1**: New files should be copied to the Requester.
  - **FR4.2**: Existing files should be updated based on timestamps (newer files from Responder should replace older files in Requester).

#### **2.4 Responder**

- **FR1**: Upon receiving a broadcast message from the server, the Responder should check whether it has the described folder in its `config.json`. If so, it should display a dialog box allowing the user to accept or decline the file cloning request.
  - **FR1.1**: The Responder must send its acceptance or rejection decision back to the server.
  - **FR1.2**: Until the request is either accepted or rejected, the Responder’s other functionalities must be disabled to ensure focus on the decision-making process.

#### **2.5 Folder Sync**

- **FR1**: Once the connection is established between the Requester and Responder, the app should perform version control operations between the folders specified in the `config.json` files of both the Requester and Responder.
  - **FR1.1**: If the Responder does not have the specified folder, a default temporary folder should be used for file synchronization.
  - **FR1.2**: Version control operations include copying new files, updating modified files, and replacing outdated files.
  - **FR1.3**: A log should be created to track all files that were synchronized during the operation.

---

### **3. Non-Functional Requirements**

#### **3.1 Performance**
- **NFR1.1**: The system must be capable of handling file synchronization for folders containing up to 10,000 files with an average size of 50 MB.
- **NFR1.2**: User actions, such as opening a folder or starting a file cloning request, must complete within 2 seconds.

#### **3.2 Reliability**
- **NFR2.1**: Data integrity must be ensured during file synchronization, with checks in place to prevent file corruption.
- **NFR2.2**: The system should handle network failures gracefully by retrying operations or alerting users to issues.

#### **3.3 Usability**
- **NFR3.1**: The user interface (UI) must be simple and intuitive, requiring minimal training for users to perform file cloning tasks.
- **NFR3.2**: Tooltips, error messages, and documentation should be provided to guide users through the process of file synchronization.

---

### **4. System Architecture**

- **Client-Server Model**: The system follows a client-server architecture, with a central server managing requests and connections between clients.
- **Modules**: 
  - **Server Module**: Handles user detection, request broadcasting, and connection management.
  - **Client Module**: Manages file synchronization, folder selection, and user interaction.
  - **Folder Sync Module**: Responsible for comparing files, checking timestamps, and performing version control operations.

---

### **5. Technical Specifications**

#### **5.1 Development Environment**
- **Programming Language**: C#
- **Framework**: .NET 6 or higher
- **IDE**: Visual Studio 2022

#### **5.2 Libraries and Tools**
- **WPF (Windows Presentation Foundation)**: For building the desktop application's UI.
- **System.IO**: For file handling operations.
- **SignalR**: For real-time communication between clients and server.

#### **5.3 Data Storage**
- No persistent data storage required. All file synchronization is done via direct access to file systems, with the application only temporarily holding information.

---

### **6. Testing and Validation**

#### **6.1 Unit Testing**
- **UT1**: Test individual functions like folder path retrieval, file timestamp comparison, and UI checkbox management.

#### **6.2 Integration Testing**
- **IT1**: Validate the communication between the server and clients, including request broadcasting and connection establishment.
- **IT2**: Test end-to-end workflows from initiating a file clone request to file synchronization completion.

#### **6.3 User Acceptance Testing**
- **UAT1**: Conduct testing with target users to ensure the application is intuitive, meets the functional requirements, and operates without errors.

---

### **7. Deployment**

#### **7.1 Deployment Environment**
- **Target OS**: Windows 10/11
- **Installation Package**: Create an installer using tools like Inno Setup or WiX for easy installation on user systems.
