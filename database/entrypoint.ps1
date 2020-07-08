$sa_password="jHvogy9@M%2c"
$host_port=1435
$container_name="fsharp-fullstack-concept-sqlserver-instance"

docker build --build-arg SA_PASSWORD=$sa_password -t fsharp-fullstack-concept-sqlserver .

try {
    Invoke-Expression "docker run -p "${host_port}:1433" -d --name $container_name fsharp-fullstack-concept-sqlserver"
}
catch {
    Write-Host "Seems like container with name '$container_name' already exists. Trying to start existing one."
    docker container start $container_name
}

Write-Host "==========================================="
Write-Host "Hurray! Now you can connect to the database"
Write-Host "NOTE that this sql server is not persistent. Since you shut down the container all the data will be lost."
Write-Host "server name = 'localhost,$host_port'"
Write-Host "login = 'sa', password='$sa_password'"
Write-Host "If you want to stop sql server instance execute 'docker container stop $container_name'  in the powershell"
Write-Host "==========================================="