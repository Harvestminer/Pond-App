# ASP.NET PondApp Project

## Overview
This project is an ASP.NET application being developed by Tyler Wesley. The goal of this project is to leverage the ASP.NET framework to build a robust and scalable web application.

## Table of Contents
- [Project Structure](#project-structure)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Project Structure
The project follows the standard ASP.NET project structure, including folders for Controllers, Views, and Models.

```
/ProjectRoot
|-- /Controllers
|-- /Data
|-- /Domain
|-- /Migrations
|-- /Models
|-- /Properties
|-- /Services
|-- /Views
|-- /wwwroot
|-- Program.cs
|-- appsettings.json
```

## Features
- **MVC Architecture**: The application follows the Model-View-Controller (MVC) design pattern.
- **User Authentication**: Built-in user authentication for secure login and registration.
- **Responsive Design**: Mobile-friendly design to ensure accessibility across all devices.
- **Database Integration**: Integration with SQL Server for data storage and retrieval.

## Installation
To get started with the project, clone the repository and set up the development environment.

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Steps
1. Clone the repository:
    ```sh
    git clone https://github.com/Harvestminer/PondApp.git
    ```
2. Navigate to the project directory:
    ```sh
    cd PondApp
    ```
3. Restore the dependencies:
    ```sh
    dotnet restore
    ```
4. Update the `appsettings.json` file with your SQL Server connection string.

## Usage
To run the application, use the following command:
```sh
dotnet run
```

Navigate to https://localhost:5001 in your web browser to access the application.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue to discuss your ideas for enhancing the project.

## Contributing
This project is licensed under the MIT [License](./LICENSE). See the LICENSE file for more details.

---

Developed by Tyler Wesley