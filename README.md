# BDSA2021-ProjectBank

    $password = New-Guid
    docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 -d mcr.microsoft.com/azure-sql-edge:latest
    $database = "ProjectBank"
    $connectionString = "Server=localhost;Database=$database;User Id=sa;Password=$password"
    dotnet user-secrets set "ConnectionStrings:ProjectBank" "$connectionString" --project .\Server\
    cd .\Infrastructure\
    dotnet ef migrations add InitialMigration -s ..\Server\
    dotnet ef database update -s ..\Server\
    cd ..
    dotnet run --project .\Server\


CODE COVERAGE

    dotnet add package coverlet.collector
    dotnet add package coverlet.msbuild

    dotnet test /p:CollectCoverage=true
    
Supervisor test user:
- Email: john@phlegetonoutlook.onmicrosoft.com
- Password: 82ss8zhC5Hxynfp9hDLMT4kU2aswAXD

Student test user:
- Email: chris@phlegetonoutlook.onmicrosoft.com
- Password: NnqaGYX4P2RmDFQcBAfAYNfECuDk7ck
 
ITU mail login:
- Login with your ITU mail (Only works for pate@itu.dk, rnie@itu.dk and the group)
