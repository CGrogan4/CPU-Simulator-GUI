# CPU Simulator GUI Setup Guide

The steps I used to set up and run the CPU Simulator GUI project.

## Prerequisites

* **Operating System:** Windows (This application is built using Windows Forms, which is specific to the Windows platform.)
* **Visual Studio 2022 (Community Edition):** Required for building and running the project.
* **.NET Framework 4.8 Developer Pack:** Necessary for compiling the application.

## Installation Steps

1.  **Install Visual Studio 2022 (Community Edition)**
    * Download the Visual Studio 2022 Community Edition.
    * During the installation process, you check the box for the ".NET desktop development" workload.. This component includes support for Windows Forms and the .NET Framework, which makes the process easier.

2.  **Install .NET Framework 4.8 Developer Pack**
    * Download the developer pack from the following link:
    * Run the downloaded installer and follow the on-screen instructions. This will allow your system to compile applications targeting .NET Framework 4.8.

3.  **Open the Project in Visual Studio**
    * Launch Visual Studio 2022.
    * Go to **File > Open > Project/Solution**.
    * Browse to the location where you cloned the repository and select the `CPU-Simulator-GUI.sln` file. Click **Open**.

4.  **Set the Startup Project**
    * In the **Solution Explorer** panel (usually located on the right side of the Visual Studio window), find the project named **CpuSchedulingWinForms**.
    * **Right-click** on the **CpuSchedulingWinForms** project.
    * Select **"Set as Startup Project"** from the context menu.
    * The **CpuSchedulingWinForms** project should now appear in **bold text**, indicating that it is the project that will run when you start the application.

5.  **Build and Run**
    * Press **F5** on your keyboard or click the **green(Start)** button in the Visual Studio toolbar.
    * If the build is successful, a windowed GUI application will appear on your screen – this is the CPU scheduling simulator!

**To work with this project, you must use Visual Studio 2022 on a Windows operating system following the steps outlined above.**

## Notable Changes in This Repository:

* **Two new scheduling algorithm methods have been added:**
    * Highest Response Ratio Next (HRRN) scheduling support.
    * Multilevel Feedback Queue Scheduling (MLFQ) scheduling support.
* **GUI Enhancements:**
    * Appropriate buttons and user interface components have been added to support the usability of the new algorithms.
    * These new elements are colored light blue to visually distinguish them from existing components.
* **Enhanced Performance Metric Tracking:** Support has been added to test and display the following key performance metrics for all implemented scheduling algorithms:
    * **Average Waiting Time (AWT)**
    * **Average Turnaround Time (ATT)**
    * **CPU Utilization (%)**
    * **Throughput (Processes per Second)**
    * **Response Time (RT) [Optional]**


##**Original Read Me Notes:**

## CPU-Simulator GUI
A grahical user interface application with these components: CPU Simulator, QR code and Barcode Generator.


## Usage

```
Please install the font: "IDAutomationHC39M Free Version". You can find it in the project root folder.
```

## License
This project is licensed under the terms of the [MIT license](https://choosealicense.com/licenses/mit/).
