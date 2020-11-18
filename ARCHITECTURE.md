# ARCHITECTURE

## Modules
  1. .dll for developers of custom antivirus modules (File Context)
  2. Analysis module (custom module)
  3. File scanner - the main module. is responsible for connecting and running custom analyzers
  4. Reporter -  еhe module allows you to compose and return a detailed analysis report
  
### File Context 
  - Library providing functionality for parsing a PE file, obtaining the necessary information about the file structure

### Analysis module
  -  A user-written module to analyze a file for viruses. The module must implement the interface specified by the developers.

### File scanner 
  - The main module is responsible for connecting, running custom analyzers and returning detailed report to user 

### Reporter
  - Functionality that allows you to collect the details of the file analysis for further use or return to the user