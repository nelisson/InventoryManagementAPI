# Inventory Management API

## Overview

The **Inventory Management API** is a RESTful web service developed using .NET 8 and C#. It provides endpoints for managing products and inventory levels in an inventory system. Users can perform basic CRUD (Create, Read, Update, Delete) operations on products and adjust inventory stock levels.

**Key Features:**

- **Product Management**: Create, retrieve, update, and delete products.
- **Inventory Management**: Add or remove stock for products.
- **API Documentation**: Integrated Swagger UI for easy testing and exploration of the API.
- **SQLite Database**: Uses an SQLite database for simplicity and ease of setup.
- **SOLID Principles**: The codebase adheres to SOLID principles and uses design patterns like Repository Pattern and Dependency Injection.
- **Unit Testing**: Includes unit tests for business logic using xUnit and code coverage analysis with Coverlet.

---

## Setup and Running the Application Locally

### Prerequisites

- **.NET 8 SDK**: Download and install from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet/8.0).
- **Visual Studio 2022** or **Visual Studio Code**: For development and running the application.

### Steps

1. **Clone the Repository**

   ```bash
   git clone https://github.com/nelisson/InventoryManagementAPI.git
   cd InventoryManagementAPI
   ```

2. **Navigate to the Project Directory**

   ```bash
   cd InventoryManagementAPI
   ```

3. **Restore Dependencies**

   ```bash
   dotnet restore
   ```

4. **Build the Application**

   ```bash
   dotnet build
   ```

5. **Run the Application**

   ```bash
   dotnet run
   ```

6. **Access the API Documentation**

   - Open your web browser and navigate to `http://localhost:5178/swagger`.
   - You should see the Swagger UI where you can test the API endpoints.

### Running Unit Tests

1. **Navigate to the Test Project Directory**

   ```bash
   cd InventoryManagementAPI.Tests
   ```

2. **Run Tests with Code Coverage**

   ```bash
   dotnet test --settings coverlet.runsettings /p:Exclude="[InventoryManagementAPI]InventoryManagementAPI.Migrations.*"
   ```

3. **View Code Coverage Report**

   - The code coverage report will be generated in the `CoverageReport` directory.
   - Open `index.html` in the `CoverageReport` folder to view the detailed code coverage report.

---

## Design Decisions and Assumptions

### Design Principles

- **SOLID Principles**: The application follows SOLID principles to create a maintainable and scalable codebase.
  - **Single Responsibility Principle**: Each class and method has a single responsibility.
  - **Open/Closed Principle**: The code is open for extension but closed for modification.
  - **Liskov Substitution Principle**: Interfaces and base classes are correctly implemented.
  - **Interface Segregation Principle**: Interfaces are specific to what clients need.
  - **Dependency Inversion Principle**: Dependencies are injected rather than hard-coded.

### Design Patterns

- **Repository Pattern**: Abstracts data access logic, making it easier to manage and test.
- **Dependency Injection**: Used to inject dependencies, enhancing testability and modularity.

### Use of SQLite File-Based Database

- The application uses a file-based SQLite database for simplicity and ease of setup. This eliminates the need for a full database server while providing persistent storage across application restarts, making it suitable for development and testing purposes.
- **Assumption**: The application does not require a high-performance or scalable database solution in the development environment, as SQLite's file-based approach provides sufficient functionality.

### Testing Frameworks

- **xUnit**: Chosen for unit testing due to its modern approach and ease of integration with .NET.
- **Coverlet**: Used for code coverage analysis, integrated with xUnit and works seamlessly with .NET CLI.
- **ReportGenerator**: Generates detailed code coverage reports in HTML format.

### API Documentation

- **Swagger**: Integrated using Swashbuckle.AspNetCore package to provide interactive API documentation and testing interface.

### Error Handling and Validation

- **Data Annotations**: Used for model validation to ensure data integrity.
- **Global Exception Handling**: Implemented to handle exceptions gracefully and return appropriate HTTP status codes.

### Assumptions

- The API is intended for development and testing purposes.
- Security and authentication mechanisms are not implemented as they were not specified in the requirements.
- Concurrency is handled optimistically using row versioning with a `[Timestamp]` attribute in the model.

---

## Additional Notes

- **Code Coverage**: A high code coverage percentage is aimed for, especially in critical components.
- **Logging**: Basic logging is assumed to be handled by default configurations.
- **Extensibility**: The application is designed to be easily extendable for additional features such as authentication, authorization, and database persistence.

---

## Contact

For any questions or suggestions, please contact Nélisson Cavalheiro at [nelisson.ad@gmail.com].

---
