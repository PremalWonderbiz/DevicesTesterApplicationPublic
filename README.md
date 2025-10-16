# DevicesTester Desktop Application

A modular **.NET 8 WPF application** for managing, validating, and testing devices.
This project follows a **layered architecture** with clear separation of concerns: Core models, business services, WPF UI, and automated tests.

---

## 🚀 Features

* **Device Model & Validation**

  * Strongly typed `Device` model
  * Custom attributes (e.g., `IPAddressAttribute`) for input validation
  * Annotation-driven validation (supports required fields, regex, ranges)

* **Data Providers & Repository Pattern**

  * `IDeviceDataProvider` for pluggable data sources
  * `IDeviceRepository` for persistence abstraction
  * JSON-based implementation included (`JsonDeviceDataProvider`)

* **Device Operations & Flow**
  
  * **Add / Create** – register new devices with validation checks
  * **Update** – modify existing device metadata and credentials
  * * **Authentication** – secure login before accessing device operations
  * **Delete** – safely remove devices from the system
  * **View Static Data** – inspect device configuration, metadata, and fixed attributes (ID, IP, ports, etc.)
  * **View Dynamic Data** – monitor live values such as status, health, or runtime metrics
  * **Validation Flow** – user input validated instantly in UI (required, regex, ranges)
  * **Repository Flow** – operations routed through a consistent repository/service layer

* **Static & Dynamic Data**

  * **Static Data:** constant information such as Device ID, IP, port, and configuration.
  * **Dynamic Data:** runtime attributes (e.g., CPU, RAM usage, status) that **change over time**.
  * Dynamic data is **dummy/simulated** and stored in **five JSON files**, which are rotated to mimic real-world changes.

* **WPF User Interface**

  * MVVM-friendly structure
  * XAML views with data validation and error notifications
  * Form-based device input and validation feedback

* **Testing**

  * Unit tests for repositories and data providers
  * JSON-based mock device data for testing scenarios

---

## 📂 Project Structure

```
DevicesTester.sln
├── DeviceTesterCore              # Core models and shared logic
│   ├── Dependencies
│   ├── Models
│   ├── Interfaces
│   └── CustomAttributes
│
├── DeviceTesterServices          # Repository and service implementations
│   ├── Dependencies
│   ├── Repositories
│   └── Services
│
├── DeviceTesterTests             # Backend unit tests
│   ├── Dependencies
│   ├── RepositoryTests
│   └── ServicesTests
│
├── DeviceTesterUI                # WPF UI (MVVM architecture)
│   ├── Dependencies
│   ├── Properties
│   ├── Animations
│   ├── Commands
│   ├── Converters
│   ├── DummyData
│   ├── Helpers
│   ├── ViewModels
│   ├── Views
│   ├── Windows
│   ├── App.xaml
│   ├── App.xaml.cs
│   └── AssemblyInfo.cs
│
├── DeviceTesterUITests           # UI-level unit tests (MVVM)
│   ├── Dependencies
│   ├── Converters
│   ├── Helpers
│   └── ViewModelTests
│
└── .gitignore
```

---

## 🛠️ Tech Stack

* **Framework**: .NET 8
* **UI**: WPF (Windows Presentation Foundation)
* **Architecture**: Layered (Core → Services → UI → Tests)
* **Testing**: MSTest/xUnit/NUnit (depending on setup)
* **Language**: C#

---

## ⚙️ Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Visual Studio 2022 (or JetBrains Rider)

### Build & Run

```bash
# Clone the repo
git clone https://github.com/your-username/DevicesTester.git
cd DevicesTester

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run WPF UI
cd DeviceTesterUI
dotnet run
```

---

## 📖 Example Workflow

1. **Add a Device**

   * Fill in Device ID, IP address, port, username, password, etc.
   * Validation runs automatically (required, regex, ranges).
2. **Authenticate Device**

   * Authenticate the device to access the data.

3. **View Device Data**

   * **Static Data:** fixed info like Device ID, IP, ports, hardware metadata.
   * **Dynamic Data:** runtime metrics that **change between executions**, pulled from one of **five rotating dummy JSON files**.

4. **Update Device**

   * Modify device properties (e.g., credentials or network details).

5. **Delete Device**

   * Remove devices safely through repository layer.

---

## 🤝 Contributing

Contributions are welcome!

* Fork the repo
* Create a feature branch
* Submit a pull request
