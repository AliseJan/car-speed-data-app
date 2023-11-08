## Solution by Alise Jankevica

This solution includes the source code, scripts for attaching the database to LocalDB, screenshots of the working app, and the speed.txt file for testing the upload/display 
data functionalities. Please read this guide thoroughly and ensure that your device meets the prerequisites for successful application execution. 

This `README.md` file provides the necessary instructions to run the application, as well as basic troubleshooting steps to help new users get started.

# CarStatsApp Setup Guide

## Prerequisites

To run this application, you'll need:
- SQL Server LocalDB 2022
- .NET 6 SDK
- Visual Studio 2022 or another compatible IDE, such as VS Code

## Running the Application

1. Rebuild the solution and restore NuGet packages:
	```shell
	dotnet restore
	dotnet build

2. Start the application from IDE by clicking the run button or by executing:
	```shell
	dotnet run --project path/to/CarStatsApp/CarStatsApp.csproj

3. If the application is executed on Visual Studio, the application page will open by itself, otherwise access the application through a web browser at https://localhost:5177 
where 5177 is the port number the API is running on. 
It will open a spa proxy landing page, but when the client application is compiled succesfully, it will automatically redirect to the APP.

## TroubleShooting

If you run into issues:

- Verify that SQL Server LocalDB is running.
- Check that the .mdf file is correctly attached to your LocalDB instance.
- Confirm that the connection string in appsettings.json is accurate.
- Ensure the .NET 6 SDK is properly installed on your machine.
- Review any application logs for detailed error messages.

If you continue to encounter problems, consider the following:

- Restart SQL Server LocalDB.
- Confirm there are no network issues if connecting to a remote SQL Server instance.
- If your LocalDb version is earlier than 16.0 (2022), then consider creating a new database on your SQL server using Migration script. 



## Possible improvements

- Implemented suite of unit and integration tests using testing frameworks such as Jest and React Testing Library. 
This will help ensure that components render correctly and the application logic works as expected.
- Improved design and user interface by integrating a UI library like Ant Design, ensuring the design is responsive and provides a seamless experience on various devices and incorporating accessibility best practices.