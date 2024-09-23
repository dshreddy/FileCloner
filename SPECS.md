# **Project Specifications Document**

### **Project Name**:  
File Cloner

### **Project Description**: 
A .NET Windows Desktop App for managing file version control and synchronization across multiple systems. The app allows one user (Requester) to initiate a file request from another user (Responder), providing a seamless mechanism to access, compare, and update files from a shared folder based on versioning.

### **Version**:  
1.0

### **Prepared By**:  
Sai Hemanth Reddy D

### **Created On**:  
September 2, 2024

### **Last Updated On**:  
September 23, 2024

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
The File Cloner application is designed as a desktop utility that facilitates file version control between users on different systems. By establishing a secure connection, one user (Requester) can request access to folders from another user (Responder) and sync files between systems based on the latest file modifications. The app supports automated detection of file versions and allows selective file transfer based on user preferences, using checkboxes.

#### **1.1 Definitions, Acronyms, and Abbreviations**
- **Requester**: The user/system that initiates the file cloning request.
- **Responder**: The user/system that accepts the request and provides access to files.
- **Client**: The desktop instance running on a user's system.
- **Server**: The central component managing online user detection, communication, and request routing between clients.
- **Folder Sync**: The process of comparing and updating files between the Requester and Responder based on version timestamps and file differences.

---

### **2. Functional Requirements**

#### **2.1 Client**
- **FR1**: Each client must contain a `config.json` file, acting as a key-value store for local folder paths.
  - The **key** represents folder descriptions or names.
  - The **value** is the absolute path to the folder on the client system.
- **FR2**: Once the server provides the `responder` to the `requester`, the application must establish a peer-to-peer connection to begin file sharing and synchronization.

#### **2.2 Server**
- **FR1**: The server must broadcast messages to all connected users when a request for file sharing is initiated by any Requester.
- **FR2**: The server must listen for responses from clients within a predefined timeout window. If a client does not respond within this time frame, the request is considered rejected by that client.
- **FR3**: The server should return the first Responder who accepts the request to the Requester. If no Responder accepts the request, the server returns `null` to the Requester.

#### **2.3 Requester**
- **FR1**: The Requester should be able to initiate a file cloning request by clicking the "Start" button.
- **FR2**: Upon initiating the request, a dialog box should appear, allowing the Requester to select one of the keys from the `config.json` file, representing the folder they wish to synchronize.
- **FR3**: Once a connection between the Requester and Responder is established, the following file synchronization process must occur:
  - **FR3.1**: The system should automatically check new files in the Responder's folder that are missing from the Requester's folder, marking their corresponding checkboxes by default.
  - **FR3.2**: For files present in both folders, the system should compare their last modified timestamps.
  - **FR3.3**: If a file in the Responder's folder is newer, the corresponding checkbox should be checked by default.
  - **FR3.4**: Users should be able to manually check/uncheck checkboxes for files/folders they wish to sync.
- **FR4**: A "Start Cloning" button should trigger the file synchronization process, copying or replacing all checked files/folders from the Responder's folder to the Requester's folder.

#### **2.4 Responder**
- **FR1**: Upon receiving a broadcast message from the server, the Responder should display a dialog box allowing the user to accept or decline the file cloning request.
  - **FR1.1**: The Responder must send their acceptance/rejection decision back to the server.
  - **FR1.2**: Until the request is either accepted or rejected, the Responder's other functionalities must be disabled.

---

### **3. Non-Functional Requirements**

#### **3.1 Performance**
- **NFR1.1**: The application must handle synchronization operations for folders containing up to 10,000 files with minimal performance impact.
- **NFR1.2**: The system should respond to user actions, such as button clicks and dialog displays, within 2 seconds to maintain a responsive UI.

#### **3.2 Reliability**
- **NFR2.1**: The application must ensure data integrity during file transfers, preventing corruption or incomplete file writes.
- **NFR2.2**: The system must provide robust error handling for scenarios such as file permission issues or locked files during synchronization.

#### **3.3 Usability**
- **NFR3.1**: The user interface must be intuitive and accessible to users with minimal technical knowledge.
- **NFR3.2**: Tooltips and help documentation must be provided for major features to guide users through the synchronization process.

---

### **4. System Architecture**
The application consists of three main components:
1. **Client Application**: Running on each user's system, handling file requests and synchronization.
2. **Server Component**: A centralized service responsible for managing communication between clients, broadcasting requests, and routing responses.
3. **File Synchronization Engine**: A module responsible for detecting file differences, comparing timestamps, and managing file transfer operations.

---

### **5. Technical Specifications**

#### **5.1 Development Environment**
- **Programming Language**: C#
- **Framework**: .NET 6 or higher
- **IDE**: Visual Studio 2022

#### **5.2 Libraries and Tools**
- **JSON Parsing**: Newtonsoft.Json for handling the `config.json` files.
- **File Transfer**: System.IO for local file management and System.Net.Sockets for client-server communication.
- **UI Framework**: WPF for building the graphical user interface.

#### **5.3 Data Storage**
- No persistent data storage is required. All data is stored in memory or retrieved from the file system during runtime.

---

### **6. Testing and Validation**

#### **6.1 Unit Testing**
- **UT1**: Test core functionalities like file comparison (by timestamp), checkbox state, and file copy operations.

#### **6.2 Integration Testing**
- **IT1**: Ensure seamless interaction between the client and server for user detection, request broadcasting, and response handling.
- **IT2**: Validate the full request/response workflow, from request initiation to file synchronization.

#### **6.3 User Acceptance Testing**
- **UAT1**: Conduct testing with a sample group of users to validate ease of use and accuracy of the file version control system.

---

### **7. Deployment**

#### **7.1 Deployment Environment**
- **Target OS**: Windows 10/11
- **Installation Package**: Use tools like Inno Setup or WiX to package the app for easy installation and distribution on Windows systems.
