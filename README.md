# FileCloner

## Terminology
`requester` - The system which raised the request for cloning
`responder` - The system which received the request & reverted back with it's response
`input file` - The file sent by the `responder` to `requester` as a response to the request received. This includes the folder description for which is configured for cloning
`summary file` - The file sent by the `requester` to `responder` which contains the list of files the it wants from the `responder` system

## Overview
File Cloner is a group project written in C# using WPF. It is developed to be utlised as a module in our Software Engineering (Aug - Dec 2024) course project taught by Mr. Ramasawmy Krishnan Chittur. The features of this module include

1. All users can raise a request & become a `responder`, and also respond to request & act as a `responder`
2. The `responder` of the request will be revert back to sender by sharing a file which includes the description of the folder in the receivers system
3. Upon receiving multiple such responses from  `responder`s, the `requester` can click a button to summarise the responses, which will generate a summary of the list of files that are latest in the `responder`s system.
4. The `requester`s UI will then be updated with this summary. Files which are not there in `requester` system will be shown in green color & files which are there but one of the `responder` has the latest one will be shown in red color, both type of files will be checked by default
5. The `requester` can then choose which files he wants to pull in before clicking on Start Clonig button.
6. After clicking on Start Cloning button the files will be pulled from the `responder` system into `requester` system


## Design


## Class Diagram
![class and module diagram](./module_and_class_diagram.png "Class & Module Diagram")

## Environment

## References


## Contributors
Views & ViewModels - Sai Hemanth Reddy & Sarath A 
<br>
NetworkService - Sai Hemanth Reddy & Neeraj Krishna N 
<br>
FileExplorerServiceProvider, Summary Generator - Sai Hemanth Reddy
<br>
DiffGenerator - Evans Samuel Biju
