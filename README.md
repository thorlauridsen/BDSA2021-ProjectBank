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
    
Supervisor login:
Login with your own microsoft accounts

Student test user:
email: chris@phlegetonoutlook.onmicrosoft.com
password: Vava8688
