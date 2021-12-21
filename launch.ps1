Write-Host -ForegroundColor Green "Starting SQL server in docker container..."
$password = New-Guid
docker rm --force ProjectBank
docker run --name ProjectBank -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest 
$database = "ProjectBank"
$connectionString = "Server=localhost;Database=$database;User Id=sa;Password=$password"
Write-Host ""

Write-Host -ForegroundColor Green "Configuring User Secrets..."
Write-Host "Configuring Connection String"
dotnet user-secrets set "ConnectionStrings:ProjectBank" "$connectionString" --project ./Server/
Write-Host ""

Write-Host -ForegroundColor Green "Updating database..."
dotnet ef database update -p ./Infrastructure/ -s ./Server/ 
Write-Host ""

Write-Host -ForegroundColor Green "Starting application..."
dotnet run --project ./Server/
Write-Host ""