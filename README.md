# GenesisBackEndChallenge

# Setting up the database
1. Create a database and name it GenesisChallenge
2. Find the 'CreateDBTables.sql' script in the root and run it against the database to create tables.
3. Modify the connection string in 'appsetting.json' file accordingly.

# Solution structure
The solution has four components as follows:
1. DALCore (Data Access Layer): it is a .NET CORE class library to access the data. It uses EF Core to do so.
2. BLLCore (Bussiness Logic Layer): it is a .NET CORE class library to do the bussiness logic e.g., generating password hash.
3. BackEndChallenge: it is an ASP.NET Web Api Core 2 project that provides the requested endpoints.
4. BackEndChallengeTest: it is a xUnit Test Core project to do unit testing for all the aforementioned projects above. It has three folders containing the unit tests for each of the projects.

# Running the application
The Swagger UI is shown in the root of web application in which the three requested endpoints could be tested.

# Loggin
The project is set to log any log with the 'Information' level or higher in 'Console' and 'Debug' by defult. You can change the setting in 'Program.cs' and 'appsettings.js', respectively.
