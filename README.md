# capstone-project-gameslayer 🎮


## Elevator Pitch 🛗📣
Comment or rate your favorite game! Also, find useful information/ tips and play some trivia! 


## Video Description 📺



## Installation 🚧

### Prerequisites 🧑‍💻

**Before you begin, ensure you have the following software installed:**
 - **Git**: 
   - Download and install Git from [https://git-scm.com/](https://git-scm.com/).
   - Follow the instructions on the Git website to install it on your operating system.
     
- **Visual Studio**:
   - Download and install Visual Studio Community 2022 (Version 17.9.6) from [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/).
     
- **MySQL Workbench**:
   - Download and install MySQL workbench (Version 8.0.30) from [https://dev.mysql.com/downloads/workbench/](https://dev.mysql.com/downloads/workbench/).

### Step 1: Clone the Repository
**Then, you'll need to download the project files to your computer:**

1. Clone the repository from GitHub.
   - https://github.com/it-sd-capstone/capstone-project-gameslayer
     
2. Open Git and navigate to the directory where you want to store the project.
   - You can use the 'cd' command to change directories.  For example: cd path/to/your/directory
     
4. Run the following command to clone the repository
   - git clone git@github.com:it-sd-capstone/capstone-project-gameslayer.git

### Step 2: Install Extensions in Visual Studio

**Make sure to have these extensions installed in Visual Studio:** 
  - xUnit.net.TestGenerator2022
  - SQL Server Data Tools - SQL Editor
  - HLSL Tools for Visual Studio
  - Live Share 2022
  - Web Live Preview
  - ML.Net Model Builder 2022
  - Azure Data Lake and Stream Analytics Tools
  - Microsoft Library Manager

### Step 3: Install NuGet Packages 

**Make sure to install these packages using Nuget package manager on both solutions in Visual Studio:**
   - MySql.Data
   - MySql.Data.EntityFramework
   - xunit
   - xunit.analyzers
   - Azure.Core
   - Azure.Storage.common
   - Azure.Storage.Blobs
   - Dapper
   - BCrypt.Net-Next
   - SendGrid
   - Microsoft.EntityFrameworkCore
   - Microsoft.VisualStudio.Web.CodeGeneration.Design

### Step 4: Connect MySQL Workbench

**To connect with MySQL workbench client, follow the steps below:**
  - First, obtain an SSL Certificate by following the steps at [Microsoft Build](https://learn.microsoft.com/en-us/azure/mysql/single-server/how-to-configure-ssl).
  - Then open MySQL workbench and click the + symbol in the MySQL Connections tab to add a new connection.
  - Enter a name for the connection in the Connection name field.
  - Select Standard (TCP/IP) as the Connection Type.
  - Enter videogamegrade.mysql.database.azure.com in hostname field.
  - Enter gamegradeadmin as username and then enter your Password.
  - Go to the SSL tab and update the Use SSL field to Require. 
  - In the SSL CA File field, enter the file location of the DigiCertGlobalRootCA.crt.pem file.
  - Click Test Connection to test the connection.
  - *If the connection is successful, click OK to save the connection.*
    
  - **To connect to the database from Visual Studio you will need to create a connection string:**
    - conString = "Server=videogamegrade.mysql.database.azure.com;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;"
## Testing ⚡
- **To run the tests:**
  - Open xUnitTest.cs
  -  Run xUnitTest
    - Go to test -> run all. That should open Test Explorer with four tests running two that pass, one that fails, and one that is skipped. 

   

## Run/Access 🚀🌐

**You have two options to run or access the project:**

  - *Option 1:* **Run the solution in Visual Studio.**
     - Open the 'VideoGameGrade' solution in Visual Studio.
     - Click the Play button to start the website
  
  - *Option 2:* **Open the website directly.**
     - Open your web browser and navigate to https://videogamegrade.azurewebsites.net/
  
## Authors ✍️ 
<a href="https://github.com/it-sd-capstone/capstone-project-gameslayer/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=it-sd-capstone/capstone-project-gameslayer" />
</a>

🧑‍🎓 *Joshua Werlein*
🧑‍🎓 *Thomas Paulson*
👩‍🎓 *Alyshia Kreher*
🧑‍🎓 *Khizar Buck*
