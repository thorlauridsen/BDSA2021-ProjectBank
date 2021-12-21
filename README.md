# BDSA2021-ProjectBank

To run the program you must first navigate to the project directory.

Run the 'launch.ps1' script in powershell while making sure that port 1433 is not already exposed. 
If you are having trouble you can attempt to run the program manually using the following commands: 

Start database in docker:

    $password = New-Guid
    docker rm --force ProjectBank
    docker run --name ProjectBank -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest 
    $database = "ProjectBank"
    
Setup user secrets:
    
    $connectionString = "Server=localhost;Database=$database;User Id=sa;Password=$password"
    dotnet user-secrets set "ConnectionStrings:ProjectBank" "$connectionString" --project ./Server/
    
Run initial migration: (Only necessary if there is no existing InitialMigration folder)
    
    dotnet ef migrations add InitialMigration -p ./Infrastructure/ -s ./Server/
    
Update database:
    
    dotnet ef database update -p ./Infrastructure/ -s ./Server/ 
    
Run project:
    
    dotnet run --project ./Server/

If for any reason you are having trouble running the project. Make sure that there is no active docker container running on port 1433, delete the InitialMigration folder and run all the above commands in the given order from the project directory.

Run code coverage within a specific test directory:

    dotnet test /p:CollectCoverage=true
    
We recommend opening test users in incognito mode for testing the different user roles

Supervisor test user:
- Email: john@phlegetonoutlook.onmicrosoft.com
- Password: 82ss8zhC5Hxynfp9hDLMT4kU2aswAXD
- Do not setup two factor auth for this account

Student test user:
- Email: chris@phlegetonoutlook.onmicrosoft.com
- Password: NnqaGYX4P2RmDFQcBAfAYNfECuDk7ck
- Do not setup two factor auth for this account
 
ITU mail login:
- Login with your ITU mail (Only works for pate@itu.dk, rnie@itu.dk and the group)
